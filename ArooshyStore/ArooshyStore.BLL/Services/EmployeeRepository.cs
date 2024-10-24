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
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly IUnitOfWork _unitOfWork;
       
        public EmployeeRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<EmployeeViewModel> GetEmployeesListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<EmployeeViewModel> list = new List<EmployeeViewModel>();
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

                    string query = "SELECT Count(s.EmployeeId) as MyRowCount FROM tblEmployee s where " + whereCondition + " ";
                    query += " select s.EmployeeId,isnull(s.EmployeeName,'') as EmployeeName,isnull(s.HouseNo,'') as HouseNo,isnull(cc.CityName,'') as CityName,isnull(s.Contact1,'') as 'Contact1',isnull(s.Contact2,'') as 'Contact2',isnull(s.Email,'') as 'Email',isnull(s.Gender,'') as 'Gender',isnull(s.MaritalStatus,'') as 'MaritalStatus',isnull(s.PostalCode,'') as 'PostalCode',isnull(s.Street,'') as 'Street',isnull(s.ColonyOrVillageName,'') as 'ColonyOrVillageName',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull((select '/Areas/Admin/FormsDocuments/Employee/' + cast(isnull(dc.DocumentId,0) as varchar) + '.' +  isnull(dc.DocumentExtension,'')  from tblDocument dc where dc.TypeId = CAST(s.EmployeeId as varchar)  and dc.DocumentType = 'Employee' and dc.Remarks = 'ProfilePicture' ),'/Areas/Admin/Content/dummy.png') as 'ImagePath',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'Status',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblEmployee s left join tblCity cc on s.CityId = cc.CityId  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new EmployeeViewModel()
                                {
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    CityName = reader["CityName"].ToString(),
                                    HouseNo = reader["HouseNo"].ToString(),
                                    Contact1 = reader["Contact1"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    Gender = reader["Gender"].ToString(),
                                    ColonyOrVillageName = reader["ColonyOrVillageName"].ToString(),
                                    Contact2 = reader["Contact2"].ToString(),
                                    MaritalStatus = reader["MaritalStatus"].ToString(),
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
                                "EmployeeRepository", "GetEmployeesListAndCount");
            }
            return list;
        }
        public EmployeeViewModel GetEmployeeById(int id)
        {
            EmployeeViewModel model = new EmployeeViewModel();         
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblEmployee>()
                         where f.EmployeeId == id
                         select new EmployeeViewModel
                         {
                             EmployeeId = f.EmployeeId,
                             EmployeeName = f.EmployeeName,
                             SalaryType = f.SalaryType,
                             Contact1 = f.Contact1,
                             Contact2= f.Contact2,
                             Email= f.Email,
                             HouseNo = f.HouseNo,
                             Street= f.Street,
                             ColonyOrVillageName= f.ColonyOrVillageName,
                             PostalCode= f.PostalCode,
                             CityId= f.CityId,
                             CityName = _unitOfWork.Db.Set<tblCity>().Where(x => x.CityId == f.CityId).Select(x => x.CityName).FirstOrDefault() ?? "",
                             Gender = f.Gender,
                             Salary = f.Salary,
                             MaritalStatus= f.MaritalStatus,
                             Status = f.Status,
                             DateOfJoining = f.DateOfJoining ?? DateTime.Now,
                             ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == f.EmployeeId.ToString() && x.DocumentType == "Employee" && x.Remarks == "ProfilePicture")
                                                           .Select(x => "/Areas/Admin/FormsDocuments/Employee/" + x.DocumentId + "." + x.DocumentExtension)
                                                           .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                             DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == f.EmployeeId.ToString() && x.DocumentType == "Employee" && x.Remarks == "ProfilePicture")
                                                           .Select(x => x.DocumentId)
                                                           .FirstOrDefault(),
                         }).FirstOrDefault();
            }
            else
            {
                model = new EmployeeViewModel
                {
                    EmployeeId = 0,
                    EmployeeName = "",
                    SalaryType = "",
                    Contact1 = "",
                    Contact2 = "",
                    Email = "",
                    HouseNo = "",
                    Street = "",
                    ColonyOrVillageName = "",
                    PostalCode = "",
                    CityId = 0,
                    CityName= "",
                    Gender = "",
                    Salary = 0,
                    MaritalStatus = "",
                    DateOfJoining = DateTime.Now,
                    Status = false,
                    ImagePath = "/Areas/Admin/Content/dummy.png",
                    DocumentId = 0,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateEmployee(EmployeeViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.EmployeeId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblEmployee>().Where(x => x.EmployeeId == model.EmployeeId).Any(x => x.EmployeeName.ToLower().Trim() == model.EmployeeName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblEmployee>().Any(x => x.EmployeeName.ToLower().Trim() == model.EmployeeName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Employee Name already exists.";
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
                    bool check2 = _unitOfWork.Db.Set<tblEmployee>().Any(x => x.EmployeeName.ToLower().Trim() == model.CityName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Employee Name already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateEmployeeDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "City Saved Successfully";
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
                                "CityRepository", "InsertUpdateEmployee");
            }
            return response;
        }
        private ResultViewModel InsertUpdateEmployeeDb(EmployeeViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateEmployee", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = st.EmployeeId;
                        cmd.Parameters.Add("@EmployeeName", SqlDbType.NVarChar).Value = st.EmployeeName;
                        cmd.Parameters.Add("@Contact1", SqlDbType.NVarChar).Value = st.Contact1;
                        cmd.Parameters.Add("@Contact2", SqlDbType.NVarChar).Value = st.Contact2;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = st.Email;
                        cmd.Parameters.Add("@HouseNo", SqlDbType.NVarChar).Value = st.HouseNo;
                        cmd.Parameters.Add("@Street", SqlDbType.NVarChar).Value = st.Street;
                        cmd.Parameters.Add("@ColonyOrVillageName", SqlDbType.NVarChar).Value = st.ColonyOrVillageName;
                        cmd.Parameters.Add("@PostalCode", SqlDbType.NVarChar).Value = st.PostalCode;
                        cmd.Parameters.Add("@CityId", SqlDbType.Int).Value = st.CityId;
                        cmd.Parameters.Add("@Gender", SqlDbType.NVarChar).Value = st.Gender;
                        cmd.Parameters.Add("@MaritalStatus", SqlDbType.NVarChar).Value = st.MaritalStatus;
                        cmd.Parameters.Add("@Salary", SqlDbType.Decimal).Value = st.Salary;
                        cmd.Parameters.Add("@SalaryType", SqlDbType.NVarChar).Value = st.SalaryType;
                        cmd.Parameters.Add("@DateOfJoining", SqlDbType.DateTime).Value = st.DateOfJoining;
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
                                "EmployeeRepository", "InsertUpdateEmployeeDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteEmployee(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            EmployeeViewModel model = new EmployeeViewModel();
            model.EmployeeId = id;
            ResultViewModel result = InsertUpdateEmployeeDb(model, "Delete", loggedInUserId);
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
