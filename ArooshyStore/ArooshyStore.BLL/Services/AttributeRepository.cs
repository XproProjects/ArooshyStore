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
    public class AttributeRepository : IAttributeRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public AttributeRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<AttributeViewModel> GetAttributesListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<AttributeViewModel> list = new List<AttributeViewModel>();
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
                    string query = "SELECT Count(s.AttributeId) as MyRowCount FROM tblAttribute s where " + whereCondition + " ";
                    //Get List
                    //query += " select s.AttributeId,isnull(s.AttributeName,'') as AttributeName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblAttribute s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += " select s.AttributeId,isnull(s.AttributeName,'') as AttributeName,(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblAttribute s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";

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
                                list.Add(new AttributeViewModel()
                                {
                                    AttributeId = Convert.ToInt32(reader["AttributeId"]),
                                    AttributeName = reader["AttributeName"].ToString(),
                                    StatusString = reader["StatusString"].ToString(),
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
                                "AttributeRepository", "GetAttributesListAndCount");
            }
            return list;
        }
        public AttributeViewModel GetAttributeById(int id)
        {
            AttributeViewModel model = new AttributeViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblAttribute>()
                         where f.AttributeId == id
                         select new AttributeViewModel
                         {
                             AttributeId = f.AttributeId,
                             AttributeName = f.AttributeName,
                             Status = f.Status,
                         }).FirstOrDefault();
            }
            else
            {
                model = new AttributeViewModel
                {
                    AttributeId = 0,
                    AttributeName = "",
                    Status = false,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateAttribute(AttributeViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.AttributeId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblAttribute>().Where(x => x.AttributeId == model.AttributeId).Any(x => x.AttributeName.ToLower().Trim() == model.AttributeName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblAttribute>().Any(x => x.AttributeName.ToLower().Trim() == model.AttributeName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Attribute already exists.";
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
                    bool check2 = _unitOfWork.Db.Set<tblAttribute>().Any(x => x.AttributeName.ToLower().Trim() == model.AttributeName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Attribute already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateAttributeDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Attribute Saved Successfully";
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
                                "AttributeRepository", "InsertUpdateAttribute");
            }
            return response;
        }
        private ResultViewModel InsertUpdateAttributeDb(AttributeViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateAttribute", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@AttributeId", SqlDbType.Int).Value = st.AttributeId;
                        cmd.Parameters.Add("@AttributeName", SqlDbType.NVarChar).Value = st.AttributeName;
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
                                "AttributeRepository", "InsertUpdateAttributeDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteAttribute(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            AttributeViewModel model = new AttributeViewModel();
            model.AttributeId = id;
            ResultViewModel result = InsertUpdateAttributeDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Attribute Deleted Successfully";
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


        #region AttributeDetail
        public AttributeViewModel GetAttributeDetailById(int id,int attributeId)
        {
            AttributeViewModel model = new AttributeViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblAttributeDetail>()
                         where f.AttributeDetailId == id
                         select new AttributeViewModel
                         {
                             AttributeDetailId = f.AttributeDetailId,
                             AttributeDetailName = f.AttributeDetailName,
                             Status = f.Status,
                             AttributeId = attributeId
                         }).FirstOrDefault();
            }
            else
            {
                model = new AttributeViewModel
                {
                    AttributeDetailId = 0,
                    AttributeDetailName = "",
                    Status = false,
                    AttributeId = attributeId,
                };
            }
            return model;
        }

        public List<AttributeViewModel> GetAttributeDetails(int attributeId)
        {
            List<AttributeViewModel> details = new List<AttributeViewModel>();

            details = (from a in _unitOfWork.Db.Set<tblAttributeDetail>()
                       where a.AttributeId == attributeId
                       orderby a.AttributeDetailName
                       select new AttributeViewModel
                       {
                           AttributeDetailId = a.AttributeDetailId,
                           AttributeDetailName = a.AttributeDetailName ?? "",
                           StatusString = a.Status == true ? "Active" : "In-Active",
                           AttributeId = attributeId,
                           //CreatedDate = a.CreatedDate,
                           //CreatedBy = a.CreatedBy ?? 0, 
                           //CreatedByString = (from u in _unitOfWork.Db.Set<tblUser>()
                           //                   join i in _unitOfWork.Db.Set<tblInfo>() on u.InfoId equals i.InfoId
                           //                   where u.UserId == a.CreatedBy
                           //                   select i.FullName).FirstOrDefault() ?? "Record Deleted",
                           //UpdatedDate = a.UpdatedDate,
                           //UpdatedBy = a.UpdatedBy ?? 0, 
                           //UpdatedByString = (from u in _unitOfWork.Db.Set<tblUser>()
                           //                   join i in _unitOfWork.Db.Set<tblInfo>() on u.InfoId equals i.InfoId
                           //                   where u.UserId == a.UpdatedBy
                           //                   select i.FullName).FirstOrDefault() ?? "Record Deleted"
                       }).ToList();

            return details;
        }
        public StatusMessageViewModel InsertUpdateAttributeDetail(AttributeViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.AttributeDetailId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblAttributeDetail>().Where(x => x.AttributeDetailId == model.AttributeDetailId).Any(x => x.AttributeDetailName.ToLower().Trim() == model.AttributeDetailName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblAttributeDetail>().Any(x => x.AttributeDetailName.ToLower().Trim() == model.AttributeDetailName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Attribute Detail already exists.";
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
                    bool check2 = _unitOfWork.Db.Set<tblAttributeDetail>().Any(x => x.AttributeDetailName.ToLower().Trim() == model.AttributeDetailName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Attribute Detail already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateAttributeDetailDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Attribute Detail Saved Successfully";
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
                                "AttributeDetailRepository", "InsertUpdateAttributeDetail");
            }
            return response;
        }
        private ResultViewModel InsertUpdateAttributeDetailDb(AttributeViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateAttributeDetail", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@AttributeDetailId", SqlDbType.Int).Value = st.AttributeDetailId;
                        cmd.Parameters.Add("@AttributeDetailName", SqlDbType.NVarChar).Value = st.AttributeDetailName;
                        cmd.Parameters.Add("@AttributeId", SqlDbType.Int).Value = st.AttributeId;
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
                                "AttributeDetailRepository", "InsertUpdateAttributeDetailDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteAttributeDetail(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            AttributeViewModel model = new AttributeViewModel();
            model.AttributeDetailId = id;
            ResultViewModel result = InsertUpdateAttributeDetailDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Attribute Detail Deleted Successfully";
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

        #endregion

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
