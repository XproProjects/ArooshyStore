using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Data.Entity;
using System.Threading.Tasks;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.PasswordEncrypt;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;
using ArooshyStore.BLL.EmailService;
using ArooshyStore.Domain.DomainModels;

namespace ArooshyStore.BLL.Services
{
    public class CustomerSupplierRepository : ICustomerSupplierRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private string siteUrl = ConfigurationManager.AppSettings["SiteUrl"].ToString();
        private string linkExpiredTime = ConfigurationManager.AppSettings["LinkExpiredTime"].ToString();
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
                    query += " select s.CustomerSupplierId,isnull(s.CustomerSupplierName,'') as CustomerSupplierName,isnull(s.CreditDays,0) as CreditDays,isnull(s.CreditLimit,0) as CreditLimit,isnull(s.HouseNo,'') as HouseNo,isnull(cc.CityName,'') as CityName,isnull(s.Contact1,'') as 'Contact1',isnull(s.Contact2,'') as 'Contact2',isnull(s.Email,'') as 'Email',isnull(s.CompleteAddress,'') as 'CompleteAddress',isnull(s.Remarks,'') as 'Remarks',isnull(s.PostalCode,'') as 'PostalCode',isnull(s.Street,'') as 'Street',isnull(s.ColonyOrVillageName,'') as 'ColonyOrVillageName',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull((select '/Areas/Admin/FormsDocuments/'+s.CustomerSupplierType+'/' + cast(isnull(dc.DocumentId,0) as varchar) + '.' +  isnull(dc.DocumentExtension,'')  from tblDocument dc where dc.TypeId = CAST(s.CustomerSupplierId as varchar)  and dc.DocumentType = s.CustomerSupplierType and dc.Remarks = 'ProfilePicture' ),'/Areas/Admin/Content/dummy.png') as 'ImagePath',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblCustomerSupplier s left join tblCity cc on s.CityId = cc.CityId  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                    CreditDays = Convert.ToInt32(reader["CreditDays"]),
                                    CreditLimit = Convert.ToDecimal(reader["CreditLimit"]),
                                    HouseNo = reader["HouseNo"].ToString(),
                                    CityName = reader["CityName"].ToString(),
                                    Contact1 = reader["Contact1"].ToString(),
                                    Contact2 = reader["Contact2"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    CompleteAddress = reader["CompleteAddress"].ToString(),
                                    Remarks = reader["Remarks"].ToString(),
                                    PostalCode = reader["PostalCode"].ToString(),
                                    Street = reader["Street"].ToString(),
                                    ColonyOrVillageName = reader["ColonyOrVillageName"].ToString(),
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
                             CustomerSupplierType = f.CustomerSupplierType ?? "",
                             Contact1= f.Contact1 ?? "",
                             Contact2= f.Contact2 ?? "",
                             Email= f.Email ?? "",
                             Password = f.Password ?? "",
                             HouseNo = f.HouseNo ?? "",
                             Street= f.Street ?? "",
                             ColonyOrVillageName= f.ColonyOrVillageName ?? "",
                             PostalCode= f.PostalCode ?? "",
                             CityId= f.CityId ?? 0,
                             CityName = _unitOfWork.Db.Set<tblCity>().Where(x => x.CityId == f.CityId).Select(x => x.CityName).FirstOrDefault() ?? "",
                             CompleteAddress = f.CompleteAddress ?? "",
                             CreditDays = f.CreditDays ?? 0,
                             CreditLimit= f.CreditLimit ?? 0,
                             Remarks= f.Remarks ?? "",
                             Status = f.Status ?? false,
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
                    Password = "",
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
                        bool check2 = _unitOfWork.Db.Set<tblCustomerSupplier>().Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim() && x.CustomerSupplierType.ToLower().Trim() == model.CustomerSupplierType.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "" + model.CustomerSupplierType + " Email already exists.";
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
                        model.Password = _unitOfWork.Db.Set<tblCustomerSupplier>().Where(x => x.CustomerSupplierId == model.CustomerSupplierId).Select(x => x.Password).FirstOrDefault() ?? "";
                    }

                    insertUpdateStatus = "Update";
                }
                else 
                {
                    bool check2 = _unitOfWork.Db.Set<tblCustomerSupplier>().Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim() && x.CustomerSupplierType.ToLower().Trim() == model.CustomerSupplierType.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "" + model.CustomerSupplierType + " Email already exists.";
                        return response;
                    }
                    model.Status = true;
                    if (!(string.IsNullOrEmpty(model.Password)))
                    {
                        model.Password = PasswordEncryptService.GetHash(model.Password); 
                    }
                    else
                    {
                        model.Password = PasswordEncryptService.GetHash("1234"); 
                    }

                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateCustomerSupplierDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "" + model.CustomerSupplierType + " Saved Successfully";
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
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = st.Password;
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
        #region Customer Account
        public StatusMessageViewModel InsertUpdateCustomer(CustomerSupplierViewModel model)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";

                if (model.CustomerSupplierId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblCustomerSupplier>()
                        .Where(x => x.CustomerSupplierId == model.CustomerSupplierId)
                        .Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim());

                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblCustomerSupplier>()
                            .Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim() && x.CustomerSupplierType == model.CustomerSupplierType);

                        if (check2)
                        {
                            response.Status = false;
                            response.Message = $"{model.CustomerSupplierType} Email already exists.";
                            return response;
                        }
                    }

                    model.Status = model.StatusString == "Yes";

                    if (model.IsChangePassword == 1)
                    {
                        model.Password = string.IsNullOrEmpty(model.Password)
                            ? PasswordEncryptService.GetHash("1234") // Default password if empty
                            : PasswordEncryptService.GetHash(model.Password);
                    }
                    else
                    {
                        model.Password = _unitOfWork.Db.Set<tblCustomerSupplier>()
                            .Where(x => x.CustomerSupplierId == model.CustomerSupplierId)
                            .Select(x => x.Password)
                            .FirstOrDefault() ?? "";
                    }

                    insertUpdateStatus = "Update";
                }
                else
                {
                    // Check for existing email for insert
                    bool check2 = _unitOfWork.Db.Set<tblCustomerSupplier>()
                        .Any(x => x.Email.ToLower().Trim() == model.Email.ToLower().Trim() && x.CustomerSupplierType == model.CustomerSupplierType);

                    if (check2)
                    {
                        response.Status = false;
                        response.Message = $"{model.CustomerSupplierType} Email already exists.";
                        return response;
                    }

                    model.Status = true;

                    model.Password = string.IsNullOrEmpty(model.Password)
                        ? PasswordEncryptService.GetHash("1234") // Default password if empty
                        : PasswordEncryptService.GetHash(model.Password);

                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateCustomerDb(model, insertUpdateStatus);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = $"{model.CustomerSupplierType} Saved Successfully";
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
                error.InsertError(0, ex.Message.ToString(), "Web Application", "CustomerSupplierRepository", "InsertUpdateCustomerSupplier"); // Logging without user ID
            }

            return response;
        }
        private ResultViewModel InsertUpdateCustomerDb(CustomerSupplierViewModel st, string insertUpdateStatus)
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
                        cmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = st.Password;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = 0; // Set to 0 or remove if not needed
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
                // Handle logging without user ID
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application", "CustomerSupplierRepository", "InsertUpdateCustomerSupplierDb"); // Logging without user ID
            }
            return result;
        }

        public UserDomainModel GetCustomerSupplierByEmailAndPassword(CustomerSupplierViewModel model)
        {
            UserDomainModel customerSupplier = null;
            try
            {
                string hashPassword = PasswordEncryptService.GetHash(model.Password);
                customerSupplier = _unitOfWork.Db.Set<tblCustomerSupplier>()
                                    .Where(x => x.Email.ToLower() == model.Email.ToLower().Trim()
                                             && x.Password.Trim() == hashPassword && x.Status == true
                                             )
                                    .Select(cs => new UserDomainModel
                                    {
                                        UserId = cs.CustomerSupplierId,
                                        FullName = cs.CustomerSupplierName,
                                        Email = cs.Email,
                                        TypeName = cs.CustomerSupplierType
                                    }).FirstOrDefault();
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                  "CustomerSupplierRepository", "GetCustomerSupplierByEmailAndPassword");
            }
            return customerSupplier;
        }
        public CustomerSupplierViewModel GetCustomerById(int id)
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
                             Contact1 = f.Contact1,
                             Contact2 = f.Contact2,
                             Email = f.Email,
                             Password = f.Password,
                             HouseNo = f.HouseNo,
                             Street = f.Street,
                             ColonyOrVillageName = f.ColonyOrVillageName,
                             PostalCode = f.PostalCode,
                             CityId = f.CityId,
                             CityName = _unitOfWork.Db.Set<tblCity>().Where(x => x.CityId == f.CityId).Select(x => x.CityName).FirstOrDefault() ?? "",
                             CompleteAddress = f.CompleteAddress,
                             CreditDays = f.CreditDays,
                             CreditLimit = f.CreditLimit,
                             Remarks = f.Remarks,
                             Status = f.Status,
                             ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == f.CustomerSupplierId.ToString() && x.DocumentType == f.CustomerSupplierType && x.Remarks == "ProfilePicture")
                                                           .Select(x => "/Areas/Admin/FormsDocuments/" + f.CustomerSupplierType + "/" + x.DocumentId + "." + x.DocumentExtension)
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
                    CustomerSupplierType = "",
                    Contact1 = "",
                    Contact2 = "",
                    Email = "",
                    Password = "",
                    HouseNo = "",
                    Street = "",
                    ColonyOrVillageName = "",
                    PostalCode = "",
                    CityId = 0,
                    CityName = "",
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
        public StatusMessageViewModel ForgotPassword(string email)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            var customerSupplierInfo = (from cs in _unitOfWork.Db.Set<tblCustomerSupplier>()
                            where cs.Email.ToLower().Trim() == email.ToLower().Trim()
                            && cs.Status == true
                            select new CustomerSupplierViewModel
                            {
                                CustomerSupplierId = cs.CustomerSupplierId,
                                CustomerSupplierName = cs.CustomerSupplierName,
                                Email = cs.Email,
                                CustomerSupplierType = cs.CustomerSupplierType,
                            }).FirstOrDefault();
            if ( customerSupplierInfo != null)
            {
                #region Send Email
                string subject = "ArooshyStore Forgot Password";
                string body = "";

                Random random = new Random();
                int randomNum = random.Next(1, 10000);

                DateTime dateTime = new DateTime();
                TimeSpan minutes = TimeSpan.FromMinutes(Convert.ToInt32(linkExpiredTime));
                dateTime = DateTime.Now.Add(minutes);

                var url = siteUrl + "/user/customeraccount/resetpassword/?id=" + customerSupplierInfo.CustomerSupplierId + "|" + dateTime.ToString().Replace(' ', '_') + "|" + randomNum;
                body += "<span>Hi " + customerSupplierInfo.CustomerSupplierName + "</span><br />" +
                    "<span> Please click on the link below to reset your password.</span><br /><br />" +
                    "" + url + "<br /><br />" +
                    "<span><b>Regards:<b></span><br />" +
                    "<span>ArooshyStore</span>";

                string toEmailAddress = customerSupplierInfo.Email;
                string message = EmailServiceRepository.SendEmailString(toEmailAddress, subject, body);
                if (message == "Success")
                {
                    response.Status = true;
                    response.Message = "Success! A Password Reset Link has been sent to your Email.";
                }
                else
                {
                    response.Status = false;
                    response.Message = message;
                }
                #endregion
            }
            else
            {
                response.Status = false;
                response.Message = "Oops! Email does not exist in our database.";
            }
            return response;
        }
        public async Task<bool> ResetPassword(int customerId, string password)
        {
            bool status = false;
            try
            {
                var user = await _unitOfWork.Db.Set<tblCustomerSupplier>().Where(x => x.CustomerSupplierId == customerId).FirstOrDefaultAsync();
                string hashPassword = PasswordEncryptService.GetHash(password);
                user.Password = hashPassword;
                await _unitOfWork.Db.SaveChangesAsync();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "CustomerSupplierRepository", "ResetPassword");
            }
            return status;
        }
        public async Task<bool> ChangePassword(int userId, string password)
        {
            bool status = false;
            try
            {
                var user = await _unitOfWork.Db.Set<tblCustomerSupplier>().Where(x => x.CustomerSupplierId == userId).FirstOrDefaultAsync();
                string hashPassword = PasswordEncryptService.GetHash(password);
                user.Password = hashPassword;
                await _unitOfWork.Db.SaveChangesAsync();
                status = true;
            }
            catch (Exception ex)
            {
                status = false;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "CustomerSupplierRepository", "ChangePassword");
            }
            return status;
        }
        public List<InvoiceViewModel> GetSalesOrderCustomerById(int id)
        {
            // Initialize the list to hold the invoice records
            List<InvoiceViewModel> invoiceList = new List<InvoiceViewModel>();

            if (id > 0)
            {
                invoiceList = (from i in _unitOfWork.Db.Set<tblInvoice>()
                               where i.CustomerSupplierId == id && i.InvoiceType == "Sale Invoice"
                               select new InvoiceViewModel
                               {
                                   InvoiceNumber = i.InvoiceNumber,
                                   CustomerSupplierId = i.CustomerSupplierId,
                                   InvoiceType = i.InvoiceType,
                                   InvoiceDate = i.InvoiceDate,
                                   TotalAmount = i.TotalAmount,
                                   NetAmount = i.NetAmount,
                                   Status = _unitOfWork.Db.Set<tblInvoiceStatus>().Where(x => x.InvoiceNumber == i.InvoiceNumber).Select(x => x.Status).FirstOrDefault() ?? "",
                                   CreatedDate = i.CreatedDate,
                               }).ToList();
            }

            return invoiceList;
        }

        public InvoiceViewModel GetSalesOrderById(string invoiceNumber)
        {
            var order = (from i in _unitOfWork.Db.Set<tblInvoice>()
                         where i.InvoiceNumber == invoiceNumber
                         select new InvoiceViewModel
                         {
                             InvoiceNumber = i.InvoiceNumber,
                             InvoiceDate = i.InvoiceDate,
                             Status = _unitOfWork.Db.Set<tblInvoiceStatus>().Where(x => x.InvoiceNumber == i.InvoiceNumber).Select(x => x.Status).FirstOrDefault() ?? "",
                             CreatedDate = i.CreatedDate,
                         }).FirstOrDefault();

            return order;
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
