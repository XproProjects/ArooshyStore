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
    public class UnitRepository : IUnitRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public UnitRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<UnitViewModel> GetUnitsListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<UnitViewModel> list = new List<UnitViewModel>();
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
                    string query = "SELECT Count(s.UnitId) as MyRowCount FROM tblUnit s where " + whereCondition + " ";
                    //Get List
                    query += " select s.UnitId,isnull(s.UnitName,'') as UnitName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblUnit s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new UnitViewModel()
                                {
                                    UnitId = Convert.ToInt32(reader["UnitId"]),
                                    UnitName = reader["UnitName"].ToString(),
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
                                "UnitRepository", "GetUnitsListAndCount");
            }
            return list;
        }
        public UnitViewModel GetUnitById(int id)
        {
            UnitViewModel model = new UnitViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblUnit>()
                         where f.UnitId == id
                         select new UnitViewModel
                         {
                             UnitId = f.UnitId,
                             UnitName = f.UnitName,
                         }).FirstOrDefault();
            }
            else
            {
                model = new UnitViewModel
                {
                    UnitId = 0,
                    UnitName = "",
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateUnit(UnitViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.UnitId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblUnit>().Where(x => x.UnitId == model.UnitId).Any(x => x.UnitName.ToLower().Trim() == model.UnitName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblUnit>().Any(x => x.UnitName.ToLower().Trim() == model.UnitName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Unit Name already exists.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblUnit>().Any(x => x.UnitName.ToLower().Trim() == model.UnitName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Unit Name already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateUnitDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Unit Saved Successfully";
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
                                "UnitRepository", "InsertUpdateUnit");
            }
            return response;
        }
        private ResultViewModel InsertUpdateUnitDb(UnitViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateUnit", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = st.UnitId;
                        cmd.Parameters.Add("@UnitName", SqlDbType.NVarChar).Value = st.UnitName;
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
                                "UnitRepository", "InsertUpdateUnitDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteUnit(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            UnitViewModel model = new UnitViewModel();
            model.UnitId = id;
            ResultViewModel result = InsertUpdateUnitDb(model, "Delete", loggedInUserId);
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
