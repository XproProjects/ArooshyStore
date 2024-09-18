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
    public class WarehouseRepository : IWarehouseRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public WarehouseRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<WarehouseViewModel> GetWarehousesListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<WarehouseViewModel> list = new List<WarehouseViewModel>();
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
                    string query = "SELECT Count(s.WarehouseId) as MyRowCount FROM tblWarehouse s where " + whereCondition + " ";
                    //Get List
                    query += " select s.WarehouseId,isnull(s.WarehouseName,'') as WarehouseName,isnull(s.Address,'') as 'Address',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull(s.Contact1,'') as 'Contact1',isnull(s.Contact2,'') as 'Contact2',isnull(s.Email,'') as 'Email',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'Status',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblWarehouse s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new WarehouseViewModel()
                                {
                                    WarehouseId = Convert.ToInt32(reader["WarehouseId"]),
                                    WarehouseName = reader["WarehouseName"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    Contact1 = reader["Contact1"].ToString(),
                                    Contact2 = reader["Contact2"].ToString(),
                                    Email = reader["Email"].ToString(),
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
                                "WarehouseRepository", "GetWarehousesListAndCount");
            }
            return list;
        }
        public WarehouseViewModel GetWarehouseById(int id)
        {
            WarehouseViewModel model = new WarehouseViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblWarehouse>()
                         where f.WarehouseId == id
                         select new WarehouseViewModel
                         {
                             WarehouseId = f.WarehouseId,
                             WarehouseName = f.WarehouseName,
                             Address = f.Address,
                             Contact1 = f.Contact1,
                             Contact2 = f.Contact2,
                             Email = f.Email,
                             Status = f.Status,
                         }).FirstOrDefault();
            }
            else
            {
                model = new WarehouseViewModel
                {
                    WarehouseId = 0,
                    WarehouseName = "",
                    Address = "",
                    Contact1 = "",
                    Contact2 = "",
                    Email = "",
                    Status = false,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateWarehouse(WarehouseViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {

                string insertUpdateStatus = "";
                if (model.WarehouseId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblWarehouse>().Where(x => x.WarehouseId == model.WarehouseId).Any(x => x.WarehouseName.ToLower().Trim() == model.WarehouseName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblWarehouse>().Any(x => x.WarehouseName.ToLower().Trim() == model.WarehouseName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Warehouse Name already exists.";
                            return response;
                        }
                    }
                    if(model.StatusString== "Yes")
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
                    bool check2 = _unitOfWork.Db.Set<tblWarehouse>().Any(x => x.WarehouseName.ToLower().Trim() == model.WarehouseName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Warehouse Name already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateWarehouseDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Warehouse Saved Successfully";
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
                                "WarehouseRepository", "InsertUpdateWarehouse");
            }
            return response;
        }
        private ResultViewModel InsertUpdateWarehouseDb(WarehouseViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateWarehouse", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@WarehouseId", SqlDbType.Int).Value = st.WarehouseId;
                        cmd.Parameters.Add("@WarehouseName", SqlDbType.NVarChar).Value = st.WarehouseName;
                        cmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = st.Address;
                        cmd.Parameters.Add("@Contact1", SqlDbType.NVarChar).Value = st.Contact1;
                        cmd.Parameters.Add("@Contact2", SqlDbType.NVarChar).Value = st.Contact2;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = st.Email;
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
                                "WarehouseRepository", "InsertUpdateWarehouseDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteWarehouse(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            WarehouseViewModel model = new WarehouseViewModel();
            model.WarehouseId = id;
            ResultViewModel result = InsertUpdateWarehouseDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Warehouse Deleted Successfully";
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
