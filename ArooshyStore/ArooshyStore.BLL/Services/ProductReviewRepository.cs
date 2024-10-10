using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Services
{
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductReviewRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<ProductReviewViewModel> GetProductReviewsListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<ProductReviewViewModel> list = new List<ProductReviewViewModel>();
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
                    //Get Count
                    string query = "SELECT Count(s.ReviewId) as MyRowCount FROM tblProductReview s where " + whereCondition + " ";
                    //Get List
                    //query += " select s.ReviewId,isnull(s.ReviewByName,'') as ReviewByName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblProductReview s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += " select s.ReviewId,isnull(s.ReviewByName,'') as ReviewByName,isnull(s.ReviewDetail,'') as ReviewDetail,isnull(s.ReviewByEmail,'') as ReviewByEmail, ISNULL(s.ReviewByCustomerId, 0) AS ReviewByCustomerId, ISNULL(s.ProductId, 0) AS ProductId, ISNULL(s.Rating, 0) AS Rating ,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblProductReview s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";

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

                            // this advances to the next resultset 
                            reader.NextResult();

                            while (reader.Read())
                            {
                                list.Add(new ProductReviewViewModel()
                                {
                                    ReviewId = Convert.ToInt32(reader["ReviewId"]),
                                    ReviewByCustomerId = Convert.ToInt32(reader["ReviewByCustomerId"]),
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    Rating = Convert.ToInt32(reader["Rating"]),
                                    ReviewByName = reader["ReviewByName"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString()),
                                    CreatedByString = reader["CreatedBy"].ToString(),
                                    UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"].ToString()),
                                    UpdatedByString = reader["UpdatedBy"].ToString(),
                                    TotalRecords = totalCount
                                }) ;
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
                                "ProductReviewRepository", "GetProductReviewsListAndCount");
            }
            return list;
        }
        public ProductReviewViewModel GetProductReviewById(int id)
        {
            ProductReviewViewModel model = new ProductReviewViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblProductReview>()
                         where f.ReviewId == id
                         select new ProductReviewViewModel
                         {
                             ReviewId = f.ReviewId,
                             ReviewByCustomerId = f.ReviewByCustomerId,
                             ProductId = f.ProductId,
                             Rating = f.Rating,
                             ReviewByName = f.ReviewByName,
                             ReviewDetail = f.ReviewDetail,
                             ReviewByEmail = f.ReviewByEmail,
                         }).FirstOrDefault();
            }
            else
            {
                model = new ProductReviewViewModel
                {
                    ReviewId = 0,
                    ReviewByCustomerId = 0,
                    ProductId = 0,
                    Rating = 0,
                    ReviewByName = "",
                    ReviewByEmail = "",
                    ReviewDetail = "",
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateProductReview(ProductReviewViewModel model)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.ReviewId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblProductReview>().Where(x => x.ReviewId == model.ReviewId).Any(x => x.ReviewByName.ToLower().Trim() == model.ReviewByName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblProductReview>().Any(x => x.ReviewByName.ToLower().Trim() == model.ReviewByName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Product Review Name already exists.";
                            return response;
                        }
                    }
                    
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblProductReview>().Any(x => x.ReviewByName.ToLower().Trim() == model.ReviewByName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Product Review Name already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateProductReviewDb(model, insertUpdateStatus);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Product Review Saved Successfully";
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
                error.InsertError(0, ex.Message.ToString(), "Web Application", "CustomerSupplierRepository", "InsertUpdateCustomerSupplier"); // Logging without user ID
            
        }
            return response;
        }
        private ResultViewModel InsertUpdateProductReviewDb(ProductReviewViewModel st, string insertUpdateStatus)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateProductReview", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ReviewId", SqlDbType.Int).Value = st.ReviewId;
                        cmd.Parameters.Add("@ReviewByCustomerId", SqlDbType.Int).Value = st.ReviewByCustomerId;
                        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = st.ProductId;
                        cmd.Parameters.Add("@Rating", SqlDbType.Int).Value = st.Rating;
                        cmd.Parameters.Add("@ReviewByName", SqlDbType.NVarChar).Value = st.ReviewByName;
                        cmd.Parameters.Add("@ReviewByEmail", SqlDbType.NVarChar).Value = st.ReviewByEmail;
                        cmd.Parameters.Add("@ReviewDetail", SqlDbType.NVarChar).Value = st.ReviewDetail;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = st.ReviewId;
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
                error.InsertError(0, ex.Message.ToString(), "Web Application", "CustomerSupplierRepository", "InsertUpdateCustomerSupplier"); // Logging without user ID
            
        }
            return result;
        }
        public StatusMessageViewModel DeleteProductReview(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            ProductReviewViewModel model = new ProductReviewViewModel();
            model.ReviewId = id;
            ResultViewModel result = InsertUpdateProductReviewDb(model, "Delete");
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Product Review Deleted Successfully";
                response.Id = result.Id;
            }
            else
            {
                response.Status = false;
                response.Message = result.Message;
                response.Id = result.Id;
            }
            return response;
        }

        public List<ProductReviewViewModel> GetProductReviews(int productId)
        {
            return (from r in _unitOfWork.Db.Set<tblProductReview>()
                    where r.ProductId == productId
                    orderby r.ReviewId descending
                    select new ProductReviewViewModel
                    {
                        ReviewId = r.ReviewId,
                        ProductId = r.ProductId,
                        ReviewByName = r.ReviewByName,
                        ReviewByEmail = r.ReviewByEmail,
                        Rating = r.Rating,
                        ReviewDetail = r.ReviewDetail,
                        CreatedDate = r.CreatedDate,
                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == r.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                           .Select(x => "/Areas/Admin/Content/noimage.png/" + x.DocumentId + "." + x.DocumentExtension)
                                                           .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == r.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                           .Select(x => x.DocumentId)
                                                           .FirstOrDefault(),
                    }).Take(3).ToList();
        }
        public List<ProductReviewViewModel> GetAllReviews()
        {
            return (from r in _unitOfWork.Db.Set<tblProductReview>()
                    select new ProductReviewViewModel
                    {
                        ReviewId = r.ReviewId,
                        ProductId = r.ProductId,
                        ReviewByName = r.ReviewByName,
                        ReviewByEmail = r.ReviewByEmail,
                        Rating = r.Rating,
                        ReviewDetail = r.ReviewDetail,
                        CreatedDate = r.CreatedDate,
                        ProductName = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == r.ProductId).Select(x => x.ProductName).FirstOrDefault() ?? "",

                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                   .Where(x => x.TypeId == r.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                    .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                                   .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                    .Where(x => x.TypeId == r.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                   .Select(x => x.DocumentId)
                                                   .FirstOrDefault(),
                    }).ToList();
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
