using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Services
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CategoryRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<CategoryViewModel> GetCategoriesListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<CategoryViewModel> list = new List<CategoryViewModel>();
            int totalCount = 0;
            try
            {
                if (string.IsNullOrEmpty(start))
                {
                    start = "0";
                }
                if (string.IsNullOrEmpty(length))
                {
                    length = "0";
                }
                int offset = (Convert.ToInt32(start) / Convert.ToInt32(length)) * Convert.ToInt32(length);
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    // Get Count
                    string query = "SELECT Count(s.CategoryId) as MyRowCount FROM tblCategory s LEFT JOIN tblCategory cc ON s.ParentCategoryId = cc.CategoryId WHERE " + whereCondition + " ";
                    // Get List
                    
                    query += "SELECT s.CategoryId, " +
          "ISNULL(s.CategoryName, '') AS CategoryName, " +
          "ISNULL(cc.CategoryName, '') AS ParentCategoryName, " +
          "(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString'," +
          "(case when ISNULL(dc.DocumentId , 0) = 0 then  '/Areas/Admin/Content/noimage.png' else  '/Areas/Admin/FormsDocuments/Category/' + CAST(ISNULL(dc.DocumentId , 0) AS VARCHAR) + '.' + ISNULL(dc.DocumentExtension, '') end ) as ImagePath," +
        "ISNULL(s.CreatedDate, '') AS CreatedDate, " +
          "ISNULL(s.CreatedBy, '') AS CreatedBy, " +
          "ISNULL(s.UpdatedDate, '') AS UpdatedDate, " +
          "ISNULL(s.UpdatedBy, '') AS UpdatedBy " +
          "FROM tblCategory s " +
          "LEFT JOIN tblDocument dc ON s.CategoryId = dc.TypeId " +
          "AND dc.DocumentType = 'Category' " +
          "AND dc.Remarks = 'ProfilePicture' " +
           "LEFT JOIN tblCategory cc ON s.ParentCategoryId = cc.CategoryId " +
          "WHERE " + whereCondition + " " +
          sorting + " " +
          "OFFSET " + offset + " ROWS " +
          "FETCH NEXT " + length + " ROWS ONLY";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                totalCount = Convert.ToInt32(reader["MyRowCount"]);
                            }

                            reader.NextResult();

                            while (reader.Read())
                            {
                                list.Add(new CategoryViewModel()
                                {
                                    CategoryId = Convert.ToInt32(reader["CategoryId"]),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    ParentCategoryName = reader["ParentCategoryName"].ToString(),
                                    StatusString = reader["StatusString"].ToString(),
                                    ImagePath = reader["ImagePath"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString()),
                                    CreatedByString = reader["CreatedBy"].ToString(),
                                    UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"].ToString()),
                                    UpdatedByString = reader["UpdatedBy"].ToString(),
                                    TotalRecords = totalCount
                                });
                            }
                        }
                        cmd.Dispose();
                    }
                    con.Close();
                    con.Dispose();
                    SqlConnection.ClearPool(con);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "CategoryRepository", "GetCategoriesListAndCount");
            }
            return list;
        }
        public CategoryViewModel GetCategoryById(int id)
        {
            CategoryViewModel model = new CategoryViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblCategory>()
                         where f.CategoryId == id
                         select new CategoryViewModel
                         {
                             CategoryId = f.CategoryId,
                             CategoryName = f.CategoryName ?? "",
                             ParentCategoryId = f.ParentCategoryId ?? 0,
                             Status = f.Status ?? false,
                             ParentCategoryName = _unitOfWork.Db.Set<tblCategory>().Where(x=>x.CategoryId == f.ParentCategoryId).Select(x=>x.CategoryName).FirstOrDefault() ?? "",
                             ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                          .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                          .Select(x => "/Areas/Admin/FormsDocuments/Category/" + x.DocumentId + "." + x.DocumentExtension)
                                          .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                             DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                          .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                          .Select(x => x.DocumentId)
                                          .FirstOrDefault(),
                         }).FirstOrDefault();
            }
            else
            {
                model = new CategoryViewModel
                {
                    CategoryId = 0,
                    CategoryName = "",
                    ParentCategoryId = 0,
                    ParentCategoryName = "",
                    Status = false,
                    ImagePath = "/Areas/Admin/Content/noimage.png",
                    DocumentId = 0,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateCategory(CategoryViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.CategoryId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblCategory>().Where(x => x.CategoryId == model.CategoryId).Any(x => x.CategoryName.ToLower().Trim() == model.CategoryName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblCategory>().Any(x => x.CategoryName.ToLower().Trim() == model.CategoryName.ToLower().Trim() && x.ParentCategoryId == model.ParentCategoryId);
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Category Name already exists.";
                            return response;
                        }
                    }
                    if (model.StatusString == "Yes")
                    {
                        model.Status = true;
                    }
                    else
                    {
                        model.Status = false;
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblCategory>().Any(x => x.CategoryName.ToLower().Trim() == model.CategoryName.ToLower().Trim() && x.ParentCategoryId == model.ParentCategoryId);
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Category Name already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateCategoryDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Category Saved Successfully";
                    response.Id = result.Id;
                }
                else
                {
                    response.Status = false;
                    response.Message = result.Message;
                    response.Id = result.Id;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message.ToString();
                response.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message.ToString(), "Web Application",
                                "CategoryRepository", "InsertUpdateCategory");
            }
            return response;
        }
        private ResultViewModel InsertUpdateCategoryDb(CategoryViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateCategory", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = st.CategoryId;
                        cmd.Parameters.Add("@CategoryName", SqlDbType.NVarChar).Value = st.CategoryName;
                        cmd.Parameters.Add("@ParentCategoryId", SqlDbType.Int).Value = st.ParentCategoryId;
                        cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = st.Status;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = loggedInUserId;
                        cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = insertUpdateStatus;
                        cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@CheckReturn2", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        result.Message = cmd.Parameters["@CheckReturn"].Value.ToString();
                        result.Id = Convert.ToInt32(cmd.Parameters["@CheckReturn2"].Value.ToString());
                        cmd.Dispose();
                    }
                    con.Close();
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message.ToString();
                result.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message.ToString(), "Web Application",
                                "CategoryRepository", "InsertUpdateCategoryDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteCategory(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            CategoryViewModel model = new CategoryViewModel();
            model.CategoryId = id;
            ResultViewModel result = InsertUpdateCategoryDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Unit Deleted Successfully";
                response.Id = result.Id;
            }
            //else if (result.Message == "Fail")
            //{
            //    response.Status = false;

            //    if (result.Id == -1)
            //    {
            //        response.Message = "Error! Please delete all data from Technician Accounts against this Designation first.";
            //    }
            //    else
            //    {
            //        response.Message = "Error! Please delete all data against this Designation first.";
            //    }
            //    response.Id = result.Id;
            //}
            else
            {
                response.Status = false;
                response.Message = result.Message;
                response.Id = result.Id;
            }
            return response;
        }
        public List<CategoryViewModel> GetBrowseCategories()
        {
            return (from f in _unitOfWork.Db.Set<tblCategory>()
                    where f.Status == true && f.ParentCategoryId != 0
                    select new CategoryViewModel
                    {
                        CategoryId = f.CategoryId,
                        CategoryName = f.CategoryName ?? "",
                        ParentCategoryId = f.ParentCategoryId ?? 0,
                        Status = f.Status ?? false,
                        ParentCategoryName = _unitOfWork.Db.Set<tblCategory>().Where(x => x.CategoryId == f.ParentCategoryId && f.ParentCategoryId != 0).Select(x => x.CategoryName).FirstOrDefault() ?? "",
                       ImagePath = _unitOfWork.Db.Set<tblDocument>()
                            .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                            .Select(x => "/Areas/Admin/FormsDocuments/Category/" + x.DocumentId + "." + x.DocumentExtension)
                            .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                            .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                            .Select(x => x.DocumentId)
                            .FirstOrDefault(),

                    }).ToList();
        }
        public List<CategoryViewModel> GetMasterCategories()
        {
            return (from f in _unitOfWork.Db.Set<tblCategory>()
                    where f.Status == true && f.ParentCategoryId == 0
                    select new CategoryViewModel
                    {
                        CategoryId = f.CategoryId,
                        CategoryName = f.CategoryName ?? "",
                        ParentCategoryId = f.ParentCategoryId ?? 0,
                        Status = f.Status ?? false,
                        ParentCategoryName = _unitOfWork.Db.Set<tblCategory>().Where(x => x.CategoryId == f.ParentCategoryId && f.ParentCategoryId == 0).Select(x => x.CategoryName).FirstOrDefault() ?? "",
                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                            .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                            .Select(x => "/Areas/Admin/FormsDocuments/Category/" + x.DocumentId + "." + x.DocumentExtension)
                            .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                            .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                            .Select(x => x.DocumentId)
                            .FirstOrDefault(),

                    }).ToList();
        }
        public List<HeaderViewModel> GetCategoriesForHeader()
        {
            var masterCategories = (from f in _unitOfWork.Db.Set<tblCategory>()
                                    where f.Status == true && f.ParentCategoryId == 0
                                    select new CategoryViewModel
                                    {
                                        CategoryId = f.CategoryId,
                                        CategoryName = f.CategoryName ?? "",
                                        Status = f.Status ?? false,
                                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                            .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                            .Select(x => "/Areas/Admin/FormsDocuments/Category/" + x.DocumentId + "." + x.DocumentExtension)
                                            .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                            .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                            .Select(x => x.DocumentId)
                                            .FirstOrDefault(),
                                    }).ToList();

            var childCategories = (from f in _unitOfWork.Db.Set<tblCategory>()
                                   where f.Status == true && f.ParentCategoryId != 0
                                   select new CategoryViewModel
                                   {
                                       CategoryId = f.CategoryId,
                                       CategoryName = f.CategoryName ?? "",
                                       ParentCategoryId = f.ParentCategoryId ?? 0,
                                       Status = f.Status ?? false,
                                       ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                           .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                           .Select(x => "/Areas/Admin/FormsDocuments/Category/" + x.DocumentId + "." + x.DocumentExtension)
                                           .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                                       DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                           .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                           .Select(x => x.DocumentId)
                                           .FirstOrDefault(),
                                   }).ToList();

            return new List<HeaderViewModel>
            {
             new HeaderViewModel
             {
              MasterCategory = masterCategories,
              ChildCategory = childCategories
             }
            };
        }

        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //public void Dispose()
        //{
        //}
    }
}
