using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.PasswordEncrypt;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;
using Newtonsoft.Json;

namespace ArooshyStore.BLL.Services
{
    public class UserRepository : IUserRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        //private string documentSiteName = ConfigurationManager.AppSettings["DocumentSiteName"].ToString();
        public UserRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #region User Registration
        public List<UserViewModel> GetUsersListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<UserViewModel> list = new List<UserViewModel>();
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
                    string query = "SELECT Count(s.UserId) as MyRowCount from tblUser s inner join tblInfo c on s.InfoId = c.InfoId left join tblUserType t on s.UserTypeId = t.UserTypeId where " + whereCondition + " ";
                    //Get List
                    query += " select s.UserId,isnull(c.FullName,'') as 'FullName',isnull(t.TypeName,'') as 'TypeName',isnull(c.Contact1,'') as 'Contact1',isnull(c.Address1,'') as 'Address1',isnull(c.Address2,'') as 'Address2',isnull(c.Contact2,'') as 'Contact2',isnull(c.Email,'') as 'Email',isnull(c.Cnic,'') as 'Cnic',isnull(c.Gender,'') as 'Gender',isnull(c.DOB,'') as 'DOB',isnull((select '/Areas/Admin/FormsDocuments/User/' + cast(isnull(dc.DocumentId,0) as varchar) + '.' +  isnull(dc.DocumentExtension,'')  from tblDocument dc where dc.TypeId = CAST(s.UserId as varchar)  and dc.DocumentType = 'User' and dc.Remarks = 'ProfilePicture' ),'/Areas/Admin/Content/dummy.png') as 'ImagePath',(case when isnull(s.IsActive,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull(s.CreatedDate,'') as 'CreatedDate', (case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy'    from tblUser s   inner join tblInfo c on s.InfoId = c.InfoId   left join tblUserType t on s.UserTypeId = t.UserTypeId  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new UserViewModel()
                                {
                                    UserId = Convert.ToInt32(reader["UserId"]),
                                    FullName = reader["FullName"].ToString(),
                                    TypeName = reader["TypeName"].ToString(),
                                    UserIdWithTypeName = reader["UserId"].ToString() + "|" + reader["TypeName"].ToString(),
                                    Contact1 = reader["Contact1"].ToString(),
                                    Contact2 = reader["Contact2"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Cnic = reader["Cnic"].ToString(),
                                    Gender = reader["Gender"].ToString(),
                                    DOB = Convert.ToDateTime(reader["DOB"]),
                                    StatusString = reader["StatusString"].ToString(),
                                    Address1 = reader["Address1"].ToString(),
                                    Address2 = reader["Address2"].ToString(),
                                    ImagePath = reader["ImagePath"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString()),
                                    CreatedByString = reader["CreatedBy"].ToString(),
                                    UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"].ToString()),
                                    UpdatedByString = reader["UpdatedBy"].ToString(),
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
                                "UserRepository", "GetUsersListAndCount");
            }
            return list;
        }
        public UserViewModel GetUserById(int id)
        {
            UserViewModel model = new UserViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblUser>()
                         join i in _unitOfWork.Db.Set<tblInfo>() on f.InfoId equals i.InfoId
                         join up in _unitOfWork.Db.Set<tblUserType>() on f.UserTypeId equals up.UserTypeId
                         where f.UserId == id
                         select new UserViewModel
                         {
                             UserId = f.UserId,
                             InfoId = f.InfoId,
                             FullName = i.FullName,
                             UserTypeId = f.UserTypeId,
                             Gender = i.Gender,
                             Cnic = i.Cnic,
                             Contact1 = i.Contact1,
                             Contact2 = i.Contact2,
                             DOB = i.DOB,
                             Email = i.Email,
                             Password = f.Password,
                             Address1 = i.Address1 ?? "",
                             Address2 = i.Address2 ?? "",
                             IsActive = f.IsActive,
                             TypeName = up.TypeName ?? "",
                             ImagePath = _unitOfWork.Db.Set<tblDocument>().Where(x => x.TypeId == f.UserId.ToString() && x.DocumentType == "User" && x.Remarks == "ProfilePicture").Select(x => "/Areas/Admin/FormsDocuments/User/" + x.DocumentId + "." + x.DocumentExtension).FirstOrDefault() ?? "/Areas/Admin/Content/dummy.png",
                             DocumentId = _unitOfWork.Db.Set<tblDocument>().Where(x => x.TypeId == f.UserId.ToString() && x.DocumentType == "User" && x.Remarks == "ProfilePicture").Select(x => x.DocumentId).FirstOrDefault(),
                         }).FirstOrDefault();
            }
            else
            {
                model = new UserViewModel
                {
                    UserId = 0,
                    InfoId = 0,
                    FullName = "",
                    UserTypeId = 0,
                    Gender = "",
                    Cnic = "",
                    Contact1 = "",
                    Contact2 = "",
                    DOB = DateTime.Now,
                    Email = "",
                    Password = "",
                    Address1 = "",
                    Address2 = "",
                    IsActive = false,
                    ImagePath = "/Areas/Admin/Content/dummy.png",
                    DocumentId = 0,
                    TypeName = "",
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateUser(UserViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                int? userId = model.UserId;
                int? infoId = model.InfoId;
                if (model.UserId > 0)
                {
                    if (!(string.IsNullOrEmpty(model.Email)))
                    {
                        bool check = _unitOfWork.Db.Set<tblInfo>().Where(x => x.InfoId == infoId).Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim());
                        if (!check)
                        {
                            bool check2 = _unitOfWork.Db.Set<tblInfo>().Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim());
                            if (check2)
                            {
                                response.Status = false;
                                response.Message = "Email already exists.";
                                return response;
                            }
                        }
                    }
                    insertUpdateStatus = "Update";
                    if (model.StatusString == "Yes")
                    {
                        model.IsActive = true;
                    }
                    else
                    {
                        model.IsActive = false;
                    }
                    if (model.IsChangePassword == 1)
                    {
                        if (!(string.IsNullOrEmpty(model.Password)))
                        {
                            model.Password = PasswordEncryptService.GetHash(model.Password);
                        }
                        else
                        {
                            model.Password = PasswordEncryptService.GetHash("1234");
                        }
                    }
                    else
                    {
                        model.Password = _unitOfWork.Db.Set<tblUser>().Where(x => x.UserId == model.UserId).Select(x => x.Password).FirstOrDefault() ?? "";
                    }
                }
                else
                {
                    if (!(string.IsNullOrEmpty(model.Email)))
                    {
                        bool check2 = _unitOfWork.Db.Set<tblInfo>().Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Email already exists.";
                            return response;
                        }
                    }
                    if (!(string.IsNullOrEmpty(model.Password)))
                    {
                        model.Password = PasswordEncryptService.GetHash(model.Password);
                    }
                    else
                    {
                        model.Password = PasswordEncryptService.GetHash("1234");
                    }
                    insertUpdateStatus = "Save";
                    model.IsActive = true;
                }
                ResultViewModel result = InsertUpdateUserDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "User Saved Successfully";
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
                                "UserRepository", "InsertUpdateUser");
            }
            return response;
        }
        private ResultViewModel InsertUpdateUserDb(UserViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateUser", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = st.UserId;
                        cmd.Parameters.Add("@FullName", SqlDbType.NVarChar).Value = st.FullName;
                        cmd.Parameters.Add("@Contact1", SqlDbType.NVarChar).Value = st.Contact1;
                        cmd.Parameters.Add("@Contact2", SqlDbType.NVarChar).Value = st.Contact2;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = st.Email;
                        cmd.Parameters.Add("@Cnic", SqlDbType.NVarChar).Value = st.Cnic;
                        cmd.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = st.Gender;
                        cmd.Parameters.Add("@DOB", SqlDbType.DateTime).Value = st.DOB;
                        cmd.Parameters.Add("@UserTypeId", SqlDbType.Int).Value = st.UserTypeId;
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = st.Password;
                        cmd.Parameters.Add("@IsActive", SqlDbType.Bit).Value = st.IsActive;
                        cmd.Parameters.Add("@Address1", SqlDbType.NVarChar).Value = st.Address1;
                        cmd.Parameters.Add("@Address2", SqlDbType.NVarChar).Value = st.Address2;
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
                                "UserRepository", "InsertUpdateUserDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteUser(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            UserViewModel model = new UserViewModel();
            model.UserId = id;
            ResultViewModel result = InsertUpdateUserDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "User Deleted Successfully";
                response.Id = result.Id;
                DeleteUserDocuments(id);
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
        public void DeleteUserDocuments(int id)
        {
            var itemDocument = _unitOfWork.Db.Set<tblDocument>().Where(x => x.TypeId == id.ToString() && x.DocumentType == "User").FirstOrDefault();
            if (itemDocument != null)
            {
                try
                {
                    int documentsId = itemDocument.DocumentId;
                    string documentExtension = itemDocument.DocumentExtension;
                    _unitOfWork.Db.Set<tblDocument>().Remove(itemDocument);
                    _unitOfWork.Db.SaveChanges();
                    string fileName = documentsId + "." + documentExtension;
                    //string path1 = HttpContext.Current.Server.MapPath("~/FormsDocuments/User/" + fileName);
                    string path1 = "/Areas/Admin/FormsDocuments/User/" + fileName;
                    FileInfo file = new FileInfo(path1);
                    if (file.Exists)//check file exsit or not
                    {
                        file.Delete();
                    }
                }
                catch { }
            }
        }
        #endregion
        #region Assign Roles
        public List<ModuleViewModel> GetModulesForAssign(int userId)
        {
            List<ModuleViewModel> modules = (from f in _unitOfWork.Db.Set<tblModule>()
                                             orderby f.ModuleName
                                             select new ModuleViewModel
                                             {
                                                 ModuleId = f.ModuleId,
                                                 ModuleName = f.ModuleName,
                                                 UserId = userId,
                                                 UserRoleId = _unitOfWork.Db.Set<tblUserRole>().Where(x => x.UserId == userId && x.ModuleId == f.ModuleId).Select(x => x.UserRoleId).FirstOrDefault(),
                                             }).ToList();
            foreach (ModuleViewModel c in modules)
            {
                c.ActionList = (from f in _unitOfWork.Db.Set<tblAssignAction>()
                                join a in _unitOfWork.Db.Set<tblAction>() on f.ActionId equals a.ActionId
                                where f.ModuleId == c.ModuleId
                                orderby a.ActionName
                                select new ModuleViewModel
                                {
                                    ActionId = a.ActionId,
                                    ActionName = a.ActionName,
                                    UserId = userId,
                                    UserRoleId = _unitOfWork.Db.Set<tblUserRole>().Where(x => x.UserId == userId && x.ModuleId == f.ModuleId && x.ActionId == f.ActionId).Select(x => x.UserRoleId).FirstOrDefault(),
                                }).ToList();
                if (c.ActionList.Count == 0)
                {
                    if (c.UserRoleId > 0)
                    {
                        c.AllActions = "Yes";
                    }
                    else
                    {
                        c.AllActions = "Not";
                    }
                }
                else
                {
                    if (c.ActionList.Any(x => x.UserRoleId == 0))
                    {
                        c.AllActions = "Not";
                    }
                    else
                    {
                        c.AllActions = "Yes";
                    }
                }
            }
            return modules;
        }
        public StatusMessageViewModel InsertUpdateAssignModule(int userId, string assignType, string data, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                List<ModuleViewModel> module = JsonConvert.DeserializeObject<List<ModuleViewModel>>(data);
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("ModuleId");
                dt.Columns.Add("ActionId");
                dt.Columns.Add("IsChecked");
                if (module.Count != 0)
                {
                    int counterLoop = 0;
                    foreach (ModuleViewModel c in module)
                    {
                        counterLoop++;
                        int moduleId = c.ModuleId;
                        dt.Rows.Clear();
                        for (int i = 0; i < c.ActionList.Count; i++)
                        {
                            dt.Rows.Add(new object[] { i + 1, moduleId, c.ActionList[i].ActionId, c.ActionList[i].IsChecked });
                        }
                        ResultViewModel result = InsertUpdateAssignModuleDb(userId, moduleId, assignType, counterLoop, dt, loggedInUserId);
                        if (result.Message == "Success")
                        {
                            response.Status = true;
                            response.Message = "Module Assigned Successfully Updated.";
                        }
                        else
                        {
                            response.Status = false;
                            response.Message = result.Message;
                        }
                    }
                }
                else
                {
                    dt.Rows.Add(new object[] { 0, 0, 0, "" });
                    ResultViewModel result = InsertUpdateAssignModuleDb(userId, 0, assignType, 1, dt, loggedInUserId);
                    if (result.Message == "Success")
                    {
                        response.Status = true;
                        response.Message = "Module Assigned Successfully Updated.";
                    }
                    else
                    {
                        response.Status = false;
                        response.Message = result.Message;
                    }
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message.ToString();
                response.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message.ToString(), "Web Application",
                                "UserRepository", "InsertUpdateAssignModule");
            }

            return response;
        }
        private ResultViewModel InsertUpdateAssignModuleDb(int userId, int moduleId, string assignType, int counter, DataTable dt, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateUserRole", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = userId;
                        cmd.Parameters.Add("@ModuleId", SqlDbType.Int).Value = moduleId;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = loggedInUserId;
                        cmd.Parameters.Add("@AssignType", SqlDbType.NVarChar).Value = assignType;
                        cmd.Parameters.Add("@dtUserRole", SqlDbType.Structured).Value = dt;
                        cmd.Parameters.Add("@CounterLoop", SqlDbType.Int).Value = counter;
                        cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = "Save";
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
                                "UserRepository", "InsertUpdateAssignModuleDb");
            }
            return result;
        }
        #endregion
        //Count Users
        public int GetUsers()
        {
            int usersCount = _unitOfWork.Db.Set<tblUser>()
                .Join(
                    _unitOfWork.Db.Set<tblUserType>(),
                    user => user.UserTypeId,
                    userType => userType.UserTypeId,
                    (user, userType) => new { user, userType })
                .Count();

            return usersCount;
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
