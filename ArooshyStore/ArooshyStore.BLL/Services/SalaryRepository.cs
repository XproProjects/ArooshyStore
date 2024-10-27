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
    public class SalaryRepository : ISalaryRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public SalaryRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<SalaryViewModel> GetSalaryListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<SalaryViewModel> list = new List<SalaryViewModel>();
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
                    string query = "SELECT Count(s.SalaryId) as MyRowCount FROM tblSalary s where " + whereCondition + " ";
                    //Get List
                    //query += " select s.SalaryId,isnull(s.CityName,'') as CityName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblSalary s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += " select s.SalaryId,isnull(s.ForMonth,'') as ForMonth,isnull(e.EmployeeName,'') as 'EmployeeName',isnull(s.NetSalary, 0) as 'NetSalary',isnull(s.Loan, 0) as 'Loan',isnull(s.DeductionAmount, 0) as 'DeductionAmount',isnull(s.BonusAmount, 0) as 'BonusAmount',isnull(s.AdvanceSalary, 0) as 'AdvanceSalary',isnull(s.BasicSalary, 0) as 'BasicSalary',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblSalary s  inner join tblEmployee e on s.EmployeeId = e.EmployeeId  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY "; 
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
                                list.Add(new SalaryViewModel()
                                {
                                    SalaryId = Convert.ToInt32(reader["SalaryId"].ToString()),
                                    ForMonth = Convert.ToDateTime(reader["ForMonth"].ToString()),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                    NetSalary = Convert.ToDecimal(reader["NetSalary"].ToString()),
                                    Loan = Convert.ToDecimal(reader["Loan"].ToString()),
                                    DeductionAmount = Convert.ToDecimal(reader["DeductionAmount"].ToString()),
                                    BonusAmount = Convert.ToDecimal(reader["BonusAmount"].ToString()),
                                    AdvanceSalary = Convert.ToDecimal(reader["AdvanceSalary"].ToString()),
                                    BasicSalary = Convert.ToDecimal(reader["BasicSalary"].ToString()),
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
                                "SalaryRepository", "GetCitysListAndCount");
            }
            return list;
        }
        public SalaryViewModel GetSalaryById(int id)
        {
            SalaryViewModel model = new SalaryViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblSalary>()
                         where f.SalaryId == id
                         select new SalaryViewModel
                         {
                             SalaryId = f.SalaryId,
                             ForMonth = f.ForMonth ?? DateTime.Now,
                             EmployeeId = f.EmployeeId ?? 0,
                             EmployeeName = _unitOfWork.Db.Set<tblEmployee>()
                                            .Where(x => x.EmployeeId == f.EmployeeId)
                                            .Select(x => x.EmployeeName)
                                            .FirstOrDefault() ?? "",
                             BasicSalary = f.BasicSalary ?? 0,
                             GrossSalary = f.GrossSalary ?? 0,
                             BonusAmount = f.BonusAmount ?? 0,
                             DeductionAmount = f.DeductionAmount ?? 0,
                             AdvanceSalary = f.AdvanceSalary ?? 0,
                             Loan = f.Loan ?? 0,
                             NetSalary = f.NetSalary ?? 0,
                             Remarks = f.Remarks ?? "",
                            
                         }).FirstOrDefault();
            }
            else
            {
                model = new SalaryViewModel
                {
                    SalaryId = 0,
                    ForMonth = DateTime.Now,
                    EmployeeId = 0,
                    BasicSalary = 0,
                    BonusAmount =0,
                    DeductionAmount = 0,
                    GrossSalary = 0,
                    AdvanceSalary = 0,
                    Loan = 0,
                    NetSalary = 0,
                    Remarks = "",
                    EmployeeName = "",

                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateSalary(SalaryViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.SalaryId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblSalary>()
                        .Where(x => x.SalaryId == model.SalaryId)
                        .Any(x => x.ForMonth == model.ForMonth);
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblSalary>()
                            .Any(x => x.ForMonth == model.ForMonth);
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Salary entry for the specified month already exists.";
                            return response;
                        }
                    }

                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblSalary>()
                        .Any(x => x.ForMonth == model.ForMonth);
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Salary entry for the specified month already exists.";
                        return response;
                    }

                    insertUpdateStatus = "Save";
                }

                ResultViewModel result = InsertUpdateSalaryDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Salary Saved Successfully";
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
                                  "SalaryRepository", "InsertUpdateSalary");
            }
            return response;
        }
        private ResultViewModel InsertUpdateSalaryDb(SalaryViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateSalary", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@SalaryId", SqlDbType.Int).Value = st.SalaryId;
                        cmd.Parameters.Add("@ForMonth", SqlDbType.Date).Value = st.ForMonth;
                        cmd.Parameters.Add("@EmployeeId", SqlDbType.Int).Value = st.EmployeeId;
                        cmd.Parameters.Add("@BasicSalary", SqlDbType.Decimal).Value = st.BasicSalary;
                        cmd.Parameters.Add("@GrossSalary", SqlDbType.Decimal).Value = st.GrossSalary;
                        cmd.Parameters.Add("@AdvanceSalary", SqlDbType.Decimal).Value = st.AdvanceSalary;
                        cmd.Parameters.Add("@BonusAmount", SqlDbType.Decimal).Value = st.BonusAmount;
                        cmd.Parameters.Add("@DeductionAmount", SqlDbType.Decimal).Value = st.DeductionAmount;
                        cmd.Parameters.Add("@Loan", SqlDbType.Decimal).Value = st.Loan;
                        cmd.Parameters.Add("@NetSalary", SqlDbType.Decimal).Value = st.NetSalary;
                        cmd.Parameters.Add("@Remarks", SqlDbType.NVarChar).Value = st.Remarks;
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
                                "SalaryRepository", "InsertUpdateSalaryDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteSalary(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            SalaryViewModel model = new SalaryViewModel();
            model.SalaryId = id;
            ResultViewModel result = InsertUpdateSalaryDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Salary Deleted Successfully";
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
