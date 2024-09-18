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
    public class ExpenseRepository : IExpenseRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExpenseRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<ExpenseViewModel> GetExpensesListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<ExpenseViewModel> list = new List<ExpenseViewModel>();
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

                    string query = "SELECT Count(s.ExpenseId) as MyRowCount FROM tblExpense s where " + whereCondition + " ";
                    query += " select s.ExpenseId,isnull(s.ExpenseName,'') as ExpenseName,isnull(dp.TypeName,'') as ExpenseTypeName,isnull(s.ExpenseAmount,0) as ExpenseAmount,isnull(s.ExpenseDate,'') as 'ExpenseDate',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblExpense s left join tblExpenseType dp on s.ExpenseTypeId = dp.ExpenseTypeId   where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new ExpenseViewModel()
                                {
                                    ExpenseId = Convert.ToInt32(reader["ExpenseId"]),
                                    ExpenseName = reader["ExpenseName"].ToString(),
                                    ExpenseTypeName = reader["ExpenseTypeName"].ToString(),
                                    ExpenseAmount = Convert.ToDecimal(reader["ExpenseAmount"].ToString()),

                                    ExpenseDate = Convert.ToDateTime(reader["ExpenseDate"].ToString()),
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
                                "ExpenseRepository", "GetExpensesListAndCount");
            }
            return list;
        }
        public ExpenseViewModel GetExpenseById(int id)
        {
            ExpenseViewModel model = new ExpenseViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblExpense>()
                         where f.ExpenseId == id
                         select new ExpenseViewModel
                         {
                             ExpenseId = f.ExpenseId,
                             ExpenseName = f.ExpenseName,
                             ExpenseTypeId = f.ExpenseTypeId ?? 0,
                             TypeName = _unitOfWork.Db.Set<tblExpenseType>().Where(x => x.ExpenseTypeId == f.ExpenseTypeId).Select(x => x.TypeName).FirstOrDefault() ?? "",
                             ExpenseDate = f.ExpenseDate,
                             ExpenseAmount = f.ExpenseAmount ?? 0,
                             PaidTo = f.PaidTo,
                             PaidFrom = f.PaidFrom
                         }).FirstOrDefault();
            }
            else
            {
                model = new ExpenseViewModel
                {
                    ExpenseId = 0,
                    ExpenseName = "",
                    ExpenseTypeId = 0,
                    ExpenseDate = DateTime.Now.AddDays(7),
                    ExpenseAmount = 0,
                    PaidTo = "",
                    PaidFrom = "",
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateExpense(ExpenseViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.ExpenseId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblExpense>().Where(x => x.ExpenseId == model.ExpenseId).Any(x => x.ExpenseName.ToLower().Trim() == model.ExpenseName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblExpense>().Any(x => x.ExpenseName.ToLower().Trim() == model.ExpenseName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Expense Name already exists.";
                            return response;
                        }
                    }

                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblExpense>().Any(x => x.ExpenseName.ToLower().Trim() == model.ExpenseName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Expense Name already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateExpenseDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Expense Saved Successfully";
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
                                "ExpenseRepository", "InsertUpdateExpense");
            }
            return response;
        }
        private ResultViewModel InsertUpdateExpenseDb(ExpenseViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateExpense", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ExpenseId", SqlDbType.Int).Value = st.ExpenseId;
                        cmd.Parameters.Add("@ExpenseName", SqlDbType.NVarChar).Value = st.ExpenseName;
                        cmd.Parameters.Add("@ExpenseTypeId", SqlDbType.Int).Value = st.ExpenseTypeId;
                        cmd.Parameters.Add("@ExpenseDate ", SqlDbType.DateTime).Value = (object)st.ExpenseDate;
                        cmd.Parameters.Add("@ExpenseAmount ", SqlDbType.Decimal).Value = st.ExpenseAmount;
                        cmd.Parameters.Add("@PaidTo", SqlDbType.NVarChar).Value = st.PaidTo;
                        cmd.Parameters.Add("@PaidFrom", SqlDbType.NVarChar).Value = st.PaidFrom;
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
                                "ExpenseRepository", "InsertUpdateExpenseDb");
            }
            return result;
        }
        public void DeleteExpenseDocuments(int id)
        {
            var itemDocument = _unitOfWork.Db.Set<tblDocument>().Where(x => x.TypeId == id.ToString() && x.DocumentType == "User").FirstOrDefault();
            if (itemDocument != null)
            {
                try
                {
                    int documentsId = itemDocument.DocumentId;
                    string documentExtension = itemDocument.DocumentExtension;
                    _unitOfWork.Db.Set<tblDocument>().Remove(itemDocument);
                    _unitOfWork.Db.SaveChanges();
                    string fileName = documentsId + "." + documentExtension;
                    //string path1 = HttpContext.Current.Server.MapPath("~/FormsDocuments/User/" + fileName);
                    string path1 = "/Areas/Admin/FormsDocuments/User/" + fileName;
                    FileInfo file = new FileInfo(path1);
                    if (file.Exists)//check file exsit or not
                    {
                        file.Delete();
                    }
                }
                catch { }
            }
        }
        public StatusMessageViewModel DeleteExpense(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            ExpenseViewModel model = new ExpenseViewModel();
            model.ExpenseId = id;
            ResultViewModel result = InsertUpdateExpenseDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Expense Deleted Successfully";
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



