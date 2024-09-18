using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Services
{
    public class ModuleRepository : IModuleRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ModuleRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<ModuleViewModel> GetModulesListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<ModuleViewModel> list = new List<ModuleViewModel>();
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
                    string query = "SELECT Count(s.ModuleId) as MyRowCount FROM tblModule s where " + whereCondition + " ";
                    //Get List
                    query += " select s.ModuleId,isnull(s.ModuleName,'') as ModuleName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblModule s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new ModuleViewModel()
                                {
                                    ModuleId = Convert.ToInt32(reader["ModuleId"]),
                                    ModuleName = reader["ModuleName"].ToString(),
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
                                "ModuleRepository", "GetModulesListAndCount");
            }
            return list;
        }
        public ModuleViewModel GetModuleById(int id)
        {
            ModuleViewModel model = new ModuleViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblModule>()
                         where f.ModuleId == id
                         select new ModuleViewModel
                         {
                             ModuleId = f.ModuleId,
                             ModuleName = f.ModuleName,
                         }).FirstOrDefault();
            }
            else
            {
                model = new ModuleViewModel
                {
                    ModuleId = 0,
                    ModuleName = "",
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateModule(ModuleViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.ModuleId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblModule>().Where(x => x.ModuleId == model.ModuleId).Any(x => x.ModuleName.ToLower().Trim() == model.ModuleName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblModule>().Any(x => x.ModuleName.ToLower().Trim() == model.ModuleName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Module Name already exists.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblModule>().Any(x => x.ModuleName.ToLower().Trim() == model.ModuleName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Module Name already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateModuleDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Module Saved Successfully";
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
                                "ModuleRepository", "InsertUpdateModule");
            }
            return response;
        }
        private ResultViewModel InsertUpdateModuleDb(ModuleViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateModule", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ModuleId", SqlDbType.Int).Value = st.ModuleId;
                        cmd.Parameters.Add("@ModuleName", SqlDbType.NVarChar).Value = st.ModuleName;
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
                                "ModuleRepository", "InsertUpdateModuleDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteModule(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            ModuleViewModel model = new ModuleViewModel();
            model.ModuleId = id;
            ResultViewModel result = InsertUpdateModuleDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Module Deleted Successfully";
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
