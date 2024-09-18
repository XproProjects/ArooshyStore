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
    public class ExpenseTypeRepository : IExpenseTypeRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ExpenseTypeRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<ExpenseTypeViewModel> GetExpenseTypesListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<ExpenseTypeViewModel> list = new List<ExpenseTypeViewModel>();
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
                    string query = "SELECT Count(s.ExpenseTypeId) as MyRowCount FROM tblExpenseType s where " + whereCondition + " ";
                    //Get List
                    //query += " select s.ExpenseTypeId,isnull(s.TypeName,'') as TypeName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblExpenseType s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += " select s.ExpenseTypeId,isnull(s.TypeName,'') as TypeName,(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblExpenseType s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new ExpenseTypeViewModel()
                                {
                                    ExpenseTypeId = Convert.ToInt32(reader["ExpenseTypeId"]),
                                    TypeName = reader["TypeName"].ToString(),
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
                                "ExpenseTypeRepository", "GetExpenseTypesListAndCount");
            }
            return list;
        }
        public ExpenseTypeViewModel GetExpenseTypeById(int id)
        {
            ExpenseTypeViewModel model = new ExpenseTypeViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblExpenseType>()
                         where f.ExpenseTypeId == id
                         select new ExpenseTypeViewModel
                         {
                             ExpenseTypeId = f.ExpenseTypeId,
                             TypeName = f.TypeName,
                             Status = f.Status,
                         }).FirstOrDefault();
            }
            else
            {
                model = new ExpenseTypeViewModel
                {
                    ExpenseTypeId = 0,
                    TypeName = "",
                    Status = false,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateExpenseType(ExpenseTypeViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.ExpenseTypeId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblExpenseType>().Where(x => x.ExpenseTypeId == model.ExpenseTypeId).Any(x => x.TypeName.ToLower().Trim() == model.TypeName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblExpenseType>().Any(x => x.TypeName.ToLower().Trim() == model.TypeName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Expense Type already exists.";
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
                    bool check2 = _unitOfWork.Db.Set<tblExpenseType>().Any(x => x.TypeName.ToLower().Trim() == model.TypeName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Expense Type already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateExpenseTypeDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "ExpenseType Saved Successfully";
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
                                "ExpenseTypeRepository", "InsertUpdateExpenseType");
            }
            return response;
        }
        private ResultViewModel InsertUpdateExpenseTypeDb(ExpenseTypeViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateExpenseType", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ExpenseTypeId", SqlDbType.Int).Value = st.ExpenseTypeId;
                        cmd.Parameters.Add("@TypeName", SqlDbType.NVarChar).Value = st.TypeName;
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
                                "ExpenseTypeRepository", "InsertUpdateExpenseTypeDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteExpenseType(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            ExpenseTypeViewModel model = new ExpenseTypeViewModel();
            model.ExpenseTypeId = id;
            ResultViewModel result = InsertUpdateExpenseTypeDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "ExpenseType Deleted Successfully";
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
