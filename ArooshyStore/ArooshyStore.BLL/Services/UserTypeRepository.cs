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
    public class UserTypeRepository : IUserTypeRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserTypeRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<UserTypeViewModel> GetUserTypesListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<UserTypeViewModel> list = new List<UserTypeViewModel>();
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
                    string query = "SELECT Count(s.UserTypeId) as MyRowCount FROM tblUserType s where " + whereCondition + " ";
                    //Get List
                    query += " select s.UserTypeId,isnull(s.TypeName,'') as TypeName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblUserType s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new UserTypeViewModel()
                                {
                                    UserTypeId = Convert.ToInt32(reader["UserTypeId"]),
                                    TypeName = reader["TypeName"].ToString(),
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
                                "UserTypeRepository", "GetUserTypesListAndCount");
            }
            return list;
        }
        public UserTypeViewModel GetUserTypeById(int id)
        {
            UserTypeViewModel model = new UserTypeViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblUserType>()
                         where f.UserTypeId == id
                         select new UserTypeViewModel
                         {
                             UserTypeId = f.UserTypeId,
                             TypeName = f.TypeName,
                         }).FirstOrDefault();
            }
            else
            {
                model = new UserTypeViewModel
                {
                    UserTypeId = 0,
                    TypeName = "",
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateUserType(UserTypeViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.UserTypeId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblUserType>().Where(x => x.UserTypeId == model.UserTypeId).Any(x => x.TypeName.ToLower().Trim() == model.TypeName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblUserType>().Any(x => x.TypeName.ToLower().Trim() == model.TypeName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Type Name already exists.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblUserType>().Any(x => x.TypeName.ToLower().Trim() == model.TypeName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Type Name already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateUserTypeDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "User Designation Saved Successfully";
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
                                "UserTypeRepository", "InsertUpdateUserType");
            }
            return response;
        }
        private ResultViewModel InsertUpdateUserTypeDb(UserTypeViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateUserType", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@UserTypeId", SqlDbType.Int).Value = st.UserTypeId;
                        cmd.Parameters.Add("@TypeName", SqlDbType.NVarChar).Value = st.TypeName;
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
                                "UserTypeRepository", "InsertUpdateUserTypeDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteUserType(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            UserTypeViewModel model = new UserTypeViewModel();
            model.UserTypeId = id;
            ResultViewModel result = InsertUpdateUserTypeDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "User Designation Deleted Successfully";
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
