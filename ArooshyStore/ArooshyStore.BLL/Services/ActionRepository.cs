using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;
using Newtonsoft.Json;

namespace ArooshyStore.BLL.Services
{
    public class ActionRepository : IActionRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ActionRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #region Action
        public List<ActionViewModel> GetActionsListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<ActionViewModel> list = new List<ActionViewModel>();
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
                    string query = "SELECT Count(s.ActionId) as MyRowCount FROM tblAction s where " + whereCondition + " ";
                    //Get List
                    query += " select s.ActionId,isnull(s.ActionName,'') as ActionName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblAction s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new ActionViewModel()
                                {
                                    ActionId = Convert.ToInt32(reader["ActionId"]),
                                    ActionName = reader["ActionName"].ToString(),
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
                                "ActionRepository", "GetActionsListAndCount");
            }
            return list;
        }
        public ActionViewModel GetActionById(int id)
        {
            ActionViewModel model = new ActionViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblAction>()
                         where f.ActionId == id
                         select new ActionViewModel
                         {
                             ActionId = f.ActionId,
                             ActionName = f.ActionName,
                         }).FirstOrDefault();
            }
            else
            {
                model = new ActionViewModel
                {
                    ActionId = 0,
                    ActionName = "",
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateAction(ActionViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.ActionId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblAction>().Where(x => x.ActionId == model.ActionId).Any(x => x.ActionName.ToLower().Trim() == model.ActionName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblAction>().Any(x => x.ActionName.ToLower().Trim() == model.ActionName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Action Name already exists.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblAction>().Any(x => x.ActionName.ToLower().Trim() == model.ActionName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Action Name already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateActionDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Action Saved Successfully";
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
                                "ActionRepository", "InsertUpdateAction");
            }
            return response;
        }
        private ResultViewModel InsertUpdateActionDb(ActionViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateAction", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ActionId", SqlDbType.Int).Value = st.ActionId;
                        cmd.Parameters.Add("@ActionName", SqlDbType.NVarChar).Value = st.ActionName;
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
                                "ActionRepository", "InsertUpdateActionDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteAction(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            ActionViewModel model = new ActionViewModel();
            model.ActionId = id;
            ResultViewModel result = InsertUpdateActionDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Action Deleted Successfully";
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
        #endregion
        #region Assign Action to Module
        public List<ModuleViewModel> GetActionsListForAssign(int id)
        {
            List<ModuleViewModel> model = (from f in _unitOfWork.Db.Set<tblAction>()
                                           orderby f.ActionName
                                           select new ModuleViewModel
                                           {
                                               ActionId = f.ActionId,
                                               ActionName = f.ActionName,
                                               ModuleId = id,
                                               AssignId = _unitOfWork.Db.Set<tblAssignAction>().Where(x => x.ModuleId == id && x.ActionId == f.ActionId).Select(x => x.AssignId).FirstOrDefault(),
                                           }).ToList();
            return model;
        }
        public StatusMessageViewModel InsertUpdateAssign(int moduleId, string data, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                List<ModuleViewModel> user = JsonConvert.DeserializeObject<List<ModuleViewModel>>(data);
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("ModuleId");
                dt.Columns.Add("ActionId");
                for (int i = 0; i < user.Count; i++)
                {
                    dt.Rows.Add(new object[] { i + 1, user[i].ModuleId, user[i].ActionId });
                }
                ResultViewModel result = InsertUpdateAssignDb(moduleId, dt, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Action Assigned Successfully Updated";
                }
                else
                {
                    response.Status = false;
                    response.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message.ToString();
                response.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message.ToString(), "Web Application",
                                "ActionRepository", "InsertUpdateAssign");
            }
            return response;
        }
        private ResultViewModel InsertUpdateAssignDb(int moduleId, DataTable dt, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spAssignActionToModule", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ModuleId", SqlDbType.Int).Value = moduleId;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = loggedInUserId;
                        cmd.Parameters.Add("@dtActionAssign", SqlDbType.Structured).Value = dt;
                        cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = "Save";
                        cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        result.Message = cmd.Parameters["@CheckReturn"].Value.ToString();
                        result.Id = 0;
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
                                "ActionRepository", "InsertUpdateAssignDb");
            }
            return result;
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
