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

namespace ArooshyStore.BLL.Services
{
    public class CustomerSupplierRepository : ICustomerSupplierRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CustomerSupplierRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<CustomerSupplierViewModel> GetCustomerSuppliersListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<CustomerSupplierViewModel> list = new List<CustomerSupplierViewModel>();
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

                    string query = "SELECT Count(s.CustomerSupplierId) as MyRowCount FROM tblCustomerSupplier s where " + whereCondition + " ";
                    query += " select s.CustomerSupplierId,isnull(s.CustomerSupplierName,'') as CustomerSupplierName,isnull(s.HouseNo,'') as HouseNo,isnull(cc.CityName,'') as CityName,isnull(s.Contact1,'') as 'Contact1',isnull(s.Contact2,'') as 'Contact2',isnull(s.Email,'') as 'Email',isnull(s.CompleteAddress,'') as 'CompleteAddress',isnull(s.Remarks,'') as 'Remarks',isnull(s.PostalCode,'') as 'PostalCode',isnull(s.Street,'') as 'Street',isnull(s.ColonyOrVillageName,'') as 'ColonyOrVillageName',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull((select '/Areas/Admin/FormsDocuments/'+s.CustomerSupplierType+'/' + cast(isnull(dc.DocumentId,0) as varchar) + '.' +  isnull(dc.DocumentExtension,'')  from tblDocument dc where dc.TypeId = CAST(s.CustomerSupplierId as varchar)  and dc.DocumentType = s.CustomerSupplierType and dc.Remarks = 'ProfilePicture' ),'/Areas/Admin/Content/dummy.png') as 'ImagePath',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'Status',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblCustomerSupplier s left join tblCity cc on s.CityId = cc.CityId  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new CustomerSupplierViewModel()
                                {
                                    CustomerSupplierId = Convert.ToInt32(reader["CustomerSupplierId"]),
                                    CustomerSupplierName = reader["CustomerSupplierName"].ToString(),
                                    CityName = reader["CityName"].ToString(),
                                    HouseNo = reader["HouseNo"].ToString(),
                                    Contact1 = reader["Contact1"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    CompleteAddress = reader["CompleteAddress"].ToString(),
                                    ColonyOrVillageName = reader["ColonyOrVillageName"].ToString(),
                                    Contact2 = reader["Contact2"].ToString(),
                                    Remarks = reader["Remarks"].ToString(),
                                    Street = reader["Street"].ToString(),
                                    ////CreditLimit = reader["CreditLimit"].ToString(),
                                    //HouseNo = reader["HouseNo"].ToString(),
                                    PostalCode = reader["PostalCode"].ToString(),

                                    StatusString = reader["StatusString"].ToString(),

                                    ImagePath = reader["ImagePath"].ToString(),
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
                                "CustomerSupplierRepository", "GetCustomerSuppliersListAndCount");
            }
            return list;
        }
        public CustomerSupplierViewModel GetCustomerSupplierById(int id)
        {
            CustomerSupplierViewModel model = new CustomerSupplierViewModel();         
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblCustomerSupplier>()
                         where f.CustomerSupplierId == id
                         select new CustomerSupplierViewModel
                         {
                             CustomerSupplierId = f.CustomerSupplierId,
                             CustomerSupplierName = f.CustomerSupplierName,
                             CustomerSupplierType = f.CustomerSupplierType,
                             Contact1= f.Contact1,
                             Contact2= f.Contact2,
                             Email= f.Email,
                             HouseNo= f.HouseNo,
                             Street= f.Street,
                             ColonyOrVillageName= f.ColonyOrVillageName,
                             PostalCode= f.PostalCode,
                             CityId= f.CityId,
                             CityName = _unitOfWork.Db.Set<tblCity>().Where(x => x.CityId == f.CityId).Select(x => x.CityName).FirstOrDefault() ?? "",
                             CompleteAddress = f.CompleteAddress,
                             CreditDays = f.CreditDays,
                             CreditLimit= f.CreditLimit,
                             Remarks= f.Remarks,
                             Status = f.Status,
                             ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == f.CustomerSupplierId.ToString() && x.DocumentType == f.CustomerSupplierType && x.Remarks == "ProfilePicture")
                                                           .Select(x => "/Areas/Admin/FormsDocuments/"+f.CustomerSupplierType+"/" + x.DocumentId + "." + x.DocumentExtension)
                                                           .FirstOrDefault() ?? "/Areas/Admin/Content/dummy.png",
                             DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == f.CustomerSupplierId.ToString() && x.DocumentType == f.CustomerSupplierType && x.Remarks == "ProfilePicture")
                                                           .Select(x => x.DocumentId)
                                                           .FirstOrDefault(),
                         }).FirstOrDefault();
            }
            else
            {
                model = new CustomerSupplierViewModel
                {
                    CustomerSupplierId = 0,
                    CustomerSupplierName = "",
                    CustomerSupplierType ="",
                    Contact1 = "",
                    Contact2 = "",
                    Email = "",
                    HouseNo = "",
                    Street = "",
                    ColonyOrVillageName = "",
                    PostalCode = "",
                    CityId = 0,
                    CityName= "",
                    CompleteAddress = "",
                    CreditDays = 0,
                    CreditLimit = 0.00m,
                    Remarks = "",
                    Status = false,
                    ImagePath = "/Areas/Admin/Content/dummy.png",
                    DocumentId = 0,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateCustomerSupplier(CustomerSupplierViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.CustomerSupplierId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblCustomerSupplier>().Where(x => x.CustomerSupplierId == model.CustomerSupplierId).Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblCustomerSupplier>().Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim() && x.CustomerSupplierType == model.CustomerSupplierType);
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = ""+model.CustomerSupplierType+ " Email already exists.";
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
                    bool check2 = _unitOfWork.Db.Set<tblCustomerSupplier>().Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim() && x.CustomerSupplierType == model.CustomerSupplierType);
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = ""+model.CustomerSupplierType+" Email already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateCustomerSupplierDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = ""+model.CustomerSupplierType+" Saved Successfully";
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
                                "CustomerSupplierRepository", "InsertUpdateCustomerSupplier");
            }
            return response;
        }
        private ResultViewModel InsertUpdateCustomerSupplierDb(CustomerSupplierViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateCustomerSupplier", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CustomerSupplierId", SqlDbType.Int).Value = st.CustomerSupplierId;
                        cmd.Parameters.Add("@CustomerSupplierName", SqlDbType.NVarChar).Value = st.CustomerSupplierName;
                        cmd.Parameters.Add("@CustomerSupplierType", SqlDbType.NVarChar).Value = st.CustomerSupplierType;
                        cmd.Parameters.Add("@Contact1", SqlDbType.NVarChar).Value = st.Contact1;
                        cmd.Parameters.Add("@Contact2", SqlDbType.NVarChar).Value = st.Contact2;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = st.Email;
                        cmd.Parameters.Add("@HouseNo", SqlDbType.NVarChar).Value = st.HouseNo;
                        cmd.Parameters.Add("@Street", SqlDbType.NVarChar).Value = st.Street;
                        cmd.Parameters.Add("@ColonyOrVillageName", SqlDbType.NVarChar).Value = st.ColonyOrVillageName;
                        cmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar).Value = st.PostalCode;
                        cmd.Parameters.Add("@CityId", SqlDbType.Int).Value = st.CityId;
                        cmd.Parameters.Add("@CompleteAddress", SqlDbType.NVarChar).Value = st.CompleteAddress;
                        cmd.Parameters.Add("@CreditDays", SqlDbType.Int).Value = st.CreditDays;
                        cmd.Parameters.Add("@CreditLimit", SqlDbType.Decimal).Value = st.CreditLimit;
                        cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = st.Remarks;
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
                                "CustomerSupplierRepository", "InsertUpdateCustomerSupplierDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteCustomerSupplier(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            CustomerSupplierViewModel model = new CustomerSupplierViewModel();
            model.CustomerSupplierId = id;
            ResultViewModel result = InsertUpdateCustomerSupplierDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Customer / Supplier Deleted Successfully";
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
        public decimal GetDeliveryChargesForCustomer(int customerSupplierId)
        {
            var result = (from cs in _unitOfWork.Db.Set<tblCustomerSupplier>()
                          join dc in _unitOfWork.Db.Set<tblDeliveryCharges>()
                          on cs.CityId equals dc.CityId
                          where cs.CustomerSupplierId == customerSupplierId
                          select dc.DeliveryCharges).FirstOrDefault();

            return result.GetValueOrDefault(); 
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
