﻿using System;
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
    public class DeliveryInfoRepository : IDeliveryInfoRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public DeliveryInfoRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<DeliveryInfoViewModel> GetDeliveryInfosListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<DeliveryInfoViewModel> list = new List<DeliveryInfoViewModel>();
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
                    string query = "SELECT Count(s.DeliveryInfoId) as MyRowCount FROM tblDeliveryInfo s where " + whereCondition + " ";
                    //Get List
                    //query += " select s.DeliveryInfoId,isnull(s.DeliveryInfoName,'') as DeliveryInfoName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblDeliveryInfo s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += " select s.DeliveryInfoId,isnull(s.DeliveryInfoName,'') as DeliveryInfoName,isnull(s.DeliveryInfoDetail,'') as DeliveryInfoDetail,(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblDeliveryInfo s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";

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
                                list.Add(new DeliveryInfoViewModel()
                                {
                                    DeliveryInfoId = Convert.ToInt32(reader["DeliveryInfoId"]),
                                    DeliveryInfoName = reader["DeliveryInfoName"].ToString(),
                                    DeliveryInfoDetail = reader["DeliveryInfoDetail"].ToString(),
                                    StatusString = reader["StatusString"].ToString(),
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
                                "DeliveryInfoRepository", "GetDeliveryInfosListAndCount");
            }
            return list;
        }
        public DeliveryInfoViewModel GetDeliveryInfoById(int id)
        {
            DeliveryInfoViewModel model = new DeliveryInfoViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblDeliveryInfo>()
                         where f.DeliveryInfoId == id
                         select new DeliveryInfoViewModel
                         {
                             DeliveryInfoId = f.DeliveryInfoId,
                             DeliveryInfoName = f.DeliveryInfoName,
                             DeliveryInfoDetail = f.DeliveryInfoDetail,
                             Status = f.Status ?? false,
                         }).FirstOrDefault();
            }
            else
            {
                model = new DeliveryInfoViewModel
                {
                    DeliveryInfoId = 0,
                    DeliveryInfoName = "",
                    DeliveryInfoDetail = "",
                    Status = false,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateDeliveryInfo(DeliveryInfoViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.DeliveryInfoId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblDeliveryInfo>().Where(x => x.DeliveryInfoId == model.DeliveryInfoId).Any(x => x.DeliveryInfoName.ToLower().Trim() == model.DeliveryInfoName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblDeliveryInfo>().Any(x => x.DeliveryInfoName.ToLower().Trim() == model.DeliveryInfoName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Delivery Information Name already exists.";
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
                    bool check2 = _unitOfWork.Db.Set<tblDeliveryInfo>().Any(x => x.DeliveryInfoName.ToLower().Trim() == model.DeliveryInfoName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Delivery Information Name already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateDeliveryInfoDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Delivery Information Saved Successfully";
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
                                "DeliveryInfoRepository", "InsertUpdateDeliveryInfo");
            }
            return response;
        }
        private ResultViewModel InsertUpdateDeliveryInfoDb(DeliveryInfoViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateDeliveryInfo", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@DeliveryInfoId", SqlDbType.Int).Value = st.DeliveryInfoId;
                        cmd.Parameters.Add("@DeliveryInfoName", SqlDbType.NVarChar).Value = st.DeliveryInfoName;
                        cmd.Parameters.Add("@DeliveryInfoDetail", SqlDbType.NVarChar).Value = st.DeliveryInfoDetail;
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
                                "DeliveryInfoRepository", "InsertUpdateDeliveryInfoDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteDeliveryInfo(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            DeliveryInfoViewModel model = new DeliveryInfoViewModel();
            model.DeliveryInfoId = id;
            ResultViewModel result = InsertUpdateDeliveryInfoDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Delivery Information Deleted Successfully";
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
        //public void Dispose()
        //{
        //}
    }
}