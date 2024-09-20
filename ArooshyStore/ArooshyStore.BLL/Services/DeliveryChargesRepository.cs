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
using Newtonsoft.Json;

namespace ArooshyStore.BLL.Services
{
    public class DeliveryChargesRepository : IDeliveryChargesRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeliveryChargesRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public List<DeliveryChargesViewModel> GetDeliveryChargessListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<DeliveryChargesViewModel> list = new List<DeliveryChargesViewModel>();
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
                    string query = "SELECT Count(s.DeliveryId) as MyRowCount FROM tblDeliveryCharges s left join tblCity c on s.CityId = c.CityId where " + whereCondition + " ";

                    query += " select s.DeliveryId,isnull(s.DeliveryCharges,0) as DeliveryCharges,isnull(c.CityName,'') as CityName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblDeliveryCharges s  left join tblCity c on s.CityId = c.CityId  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    //query += " ";

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
                                list.Add(new DeliveryChargesViewModel()
                                {
                                    DeliveryId = Convert.ToInt32(reader["DeliveryId"]),
                                    DeliveryCharges = Convert.ToDecimal(reader["DeliveryCharges"].ToString()),
                                    CityName = reader["CityName"].ToString(),
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
                                "DeliveryChargesRepository", "GetDeliveryChargessListAndCount");
            }
            return list;
        }

        public DeliveryChargesViewModel GetDeliveryChargesById(int id)
        {
            DeliveryChargesViewModel model = new DeliveryChargesViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblDeliveryCharges>()
                         where f.DeliveryId == id
                         select new DeliveryChargesViewModel
                         {
                             DeliveryId = f.DeliveryId,
                             CityId = f.CityId,
                             CityName = _unitOfWork.Db.Set<tblCity>().Where(x => x.CityId == f.CityId).Select(x => x.CityName).FirstOrDefault() ?? "",
                             DeliveryCharges = f.DeliveryCharges ?? 0,
                         }).FirstOrDefault();
            }
            else
            {
                model = new DeliveryChargesViewModel
                {
                    DeliveryId = 0,
                    CityId = 0,
                    CityName = "",
                    DeliveryCharges = 0,
                };
            }
            return model;
        }
        public StatusMessageViewModel InsertUpdateDeliveryCharges(DeliveryChargesViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.DeliveryId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblDeliveryCharges>()
                                .Where(x => x.DeliveryId == model.DeliveryId)
                                .Any(x => x.CityId == model.CityId);

                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblDeliveryCharges>()
                                    .Any(x => x.CityId == model.CityId);

                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Delivery Charges already exist for this city.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblDeliveryCharges>()
                                .Any(x => x.CityId == model.CityId);

                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Delivery Charges already exist for this city.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateDeliveryChargesDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Delivery Charges Saved Successfully";
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
                error.InsertError(loggedInUserId, ex.Message.ToString(), "Web Application", "DeliveryChargesRepository", "InsertUpdateDeliveryCharges");
            }

            return response;
        }
        private ResultViewModel InsertUpdateDeliveryChargesDb(DeliveryChargesViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateDeliveryCharges", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@DeliveryId", SqlDbType.Int).Value = st.DeliveryId;
                        cmd.Parameters.Add("@CityId", SqlDbType.Int).Value = st.CityId;
                        cmd.Parameters.Add("@DeliveryCharges", SqlDbType.Decimal).Value = st.DeliveryCharges;
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
                                "DeliveryChargesRepository", "InsertUpdateDeliveryChargesDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteDeliveryCharges(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            DeliveryChargesViewModel model = new DeliveryChargesViewModel();
            model.DeliveryId = id;
            ResultViewModel result = InsertUpdateDeliveryChargesDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Delivery Charges Deleted Successfully";
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

    }
}