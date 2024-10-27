using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;
using Newtonsoft.Json;

namespace ArooshyStore.BLL.Services
{
    public class InvoiceRepository : IInvoiceRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public InvoiceRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<InvoiceViewModel> GetInvoicesListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<InvoiceViewModel> list = new List<InvoiceViewModel>();
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
                    string query = "SELECT Count(s.[InvoiceNumber]) as MyRowCount FROM tblInvoice s where " + whereCondition + " ";
                    //Get List
                    //query += " select s.InvoiceNumber,isnull(s.InvoiceNumber,'') as InvoiceNumber,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblInvoice s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += " select s.InvoiceNumber,isnull(s.InvoiceDate,'') as 'InvoiceDate',isnull(s.NetAmount,0) as NetAmount,isnull(cc.Status,'') as Status,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblInvoice s left join tblInvoiceStatus cc on s.InvoiceNumber = cc.InvoiceNumber   where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new InvoiceViewModel()
                                {
                                    InvoiceNumber = reader["InvoiceNumber"].ToString(),
                                    InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"].ToString()),
                                    NetAmount = Convert.ToDecimal(reader["NetAmount"].ToString()),
                                    Status = reader["Status"].ToString(),
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
                                "InvoiceRepository", "GetInvoicesListAndCount");
            }
            return list;
        }
        public InvoiceViewModel GetInvoiceByIdForPrint(string id, string type)
        {
            InvoiceViewModel model = (from f in _unitOfWork.Db.Set<tblInvoice>()
                                      where f.InvoiceNumber == id
                                      select new InvoiceViewModel
                                      {
                                          InvoiceNumber = f.InvoiceNumber ?? "",
                                          InvoiceType = f.InvoiceType ?? "",
                                          InvoiceDate = f.InvoiceDate,
                                          CustomerSupplierId = f.CustomerSupplierId ?? 0,
                                          CustomerName = _unitOfWork.Db.Set<tblCustomerSupplier>()
                                                              .Where(x => x.CustomerSupplierId == f.CustomerSupplierId)
                                                              .Select(x => x.CustomerSupplierName)
                                                              .FirstOrDefault() ?? "",
                                          TotalAmount = f.TotalAmount ?? 0,
                                          DiscType = f.DiscType ?? "%",
                                          DiscRate = f.DiscRate ?? 0,
                                          DeliveryCharges = f.DeliveryCharges ?? 0,
                                          DiscAmount = f.DiscAmount ?? 0,
                                          NetAmount = f.NetAmount ?? 0,
                                          
                                      }).FirstOrDefault() ?? new InvoiceViewModel();

            model.InvoiceDetailsList = (from c in _unitOfWork.Db.Set<tblInvoiceDetail>()
                                        where c.InvoiceNumber == id
                                        select new InvoiceDetailViewModel 
                                        {
                                            AttributeId = c.AttributeId,
                                            AttributeName = _unitOfWork.Db.Set<tblAttribute>()
                                                             .Where(x => x.AttributeId == c.AttributeId)
                                                             .Select(x => x.AttributeName)
                                                             .FirstOrDefault() ?? "",
                                            AttributeDetailId = c.AttributeDetailId,
                                            AttributeDetailName = _unitOfWork.Db.Set<tblAttributeDetail>()
                                                                 .Where(x => x.AttributeDetailId == c.AttributeDetailId)
                                                                 .Select(x => x.AttributeDetailName)
                                                                 .FirstOrDefault() ?? "",
                                            Rate = c.Rate ?? 0,
                                            Qty = c.Qty ?? 0,
                                            DiscType = c.DiscType ?? "%",
                                            DiscRate = c.DiscRate ?? 0,
                                            DiscAmount = c.DiscAmount ?? 0,
                                            NetAmount = c.NetAmount ?? 0,
                                            ProductId = c.ProductId ?? 0,
                                            ProductName = _unitOfWork.Db.Set<tblProduct>()
                                                            .Where(x => x.ProductId == c.ProductId)
                                                            .Select(x => x.ProductName)
                                                            .FirstOrDefault() ?? "",
                                            SalePrice = _unitOfWork.Db.Set<tblProduct>()
                                              .Where(x => x.ProductId == c.ProductId)
                                              .Select(x => x.SalePrice)
                                              .FirstOrDefault() ?? 0
                                        }).ToList();
            var companyInfo = _unitOfWork.Db.Set<tblCompany>().FirstOrDefault();
            if (companyInfo != null)
            {
                model.CompanyEmail = companyInfo.Email ?? "";
                model.CompanyAddress = companyInfo.Address ?? "";
                model.CompanyContact = companyInfo.Contact1 ?? "";

            }

            return model;
        }
        public InvoiceViewModel GetInvoiceById(string id, string type)
        {
            InvoiceViewModel model = new InvoiceViewModel();

            if (id != "0")
            {
                model = (from f in _unitOfWork.Db.Set<tblInvoice>()
                         where f.InvoiceNumber == id
                         select new InvoiceViewModel
                         {
                             InvoiceNumber = f.InvoiceNumber ?? "",
                             InvoiceType = f.InvoiceType ?? "",
                             InvoiceDate = f.InvoiceDate ?? DateTime.Now,
                             TotalAmount = f.TotalAmount ?? 0,
                             CustomerSupplierId = f.CustomerSupplierId ?? 0,
                             DiscType = f.DiscType ?? "%",
                             DiscRate = f.DiscRate ?? 0,
                             DiscAmount = f.DiscAmount ?? 0,
                             DeliveryCharges = f.DeliveryCharges ?? 0,
                             CustomerName = _unitOfWork.Db.Set<tblCustomerSupplier>()
                                            .Where(x => x.CustomerSupplierId == f.CustomerSupplierId)
                                            .Select(x => x.CustomerSupplierName)
                                            .FirstOrDefault() ?? "",
                             IsNewOrEdit = "Update",
                             NetAmount = f.NetAmount ?? 0
                         }).FirstOrDefault();
            }
            else
            {
                model = new InvoiceViewModel
                {
                    InvoiceNumber = "",
                    InvoiceType = "",
                    InvoiceDate = DateTime.Now,
                    TotalAmount = 0,
                    CustomerSupplierId = 0,
                    DiscType = "%",
                    DiscRate = 0,
                    DiscAmount = 0,
                    NetAmount = 0,
                    DeliveryCharges = 0,
                    CustomerName = "",
                    IsNewOrEdit = "New"
                };
            }

            return model;
        }

        public StatusMessageViewModel InsertUpdateInvoice(InvoiceViewModel model, string detail, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                List<InvoiceDetailViewModel> list = JsonConvert.DeserializeObject<List<InvoiceDetailViewModel>>(detail);
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("WarehouseId");
                dt.Columns.Add("MasterCategoryId");
                dt.Columns.Add("ChildCategoryId");
                dt.Columns.Add("ProductId");
                dt.Columns.Add("AttributeId");
                dt.Columns.Add("AttributeDetailId");
                dt.Columns.Add("UnitId");
                dt.Columns.Add("DiscountOfferId");
                dt.Columns.Add("TotalAmount");
                dt.Columns.Add("Qty");
                dt.Columns.Add("Rate");
                dt.Columns.Add("DiscType");
                dt.Columns.Add("DiscRate");
                dt.Columns.Add("DiscAmount");
                dt.Columns.Add("NetAmount");

                if (list.Count != 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, list[i].WarehouseId, list[i].MasterCategoryId, list[i].ChildCategoryId, list[i].ProductId, list[i].AttributeId, list[i].AttributeDetailId, list[i].UnitId, list[i].DiscountOfferId, list[i].TotalAmount, list[i].Qty, list[i].Rate, list[i].DiscType, list[i].DiscRate, list[i].DiscAmount, list[i].NetAmount });
                    }
                }
                else
                {
                    dt.Rows.Add(new object[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0 });
                }

                if (model.IsNewOrEdit == "Update")
                {
                    insertUpdateStatus = "Update";
                }
                else
                {
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateInvoiceDb(model, dt, insertUpdateStatus, loggedInUserId);

                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Invoice Saved Successfully";
                    response.IdString = result.IdString;
                }
                else
                {
                    response.Status = false;
                    response.Message = result.Message;
                    response.IdString = result.IdString;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                response.IdString = "0";
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message, "Web Application", "InvoiceRepository", "InsertUpdateInvoice");
            }

            return response;
        }
        private ResultViewModel InsertUpdateInvoiceDb(InvoiceViewModel st, DataTable dt, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateInvoice", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar).Value = st.InvoiceNumber;
                        cmd.Parameters.Add("@CustomerSupplierId", SqlDbType.Int).Value = st.CustomerSupplierId;
                        cmd.Parameters.Add("@InvoiceType", SqlDbType.NVarChar).Value = st.InvoiceType;
                        cmd.Parameters.Add("@InvoiceDate", SqlDbType.DateTime).Value = st.InvoiceDate;
                        cmd.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = st.TotalAmount;
                        cmd.Parameters.Add("@DiscType", SqlDbType.NVarChar).Value = st.DiscType;
                        cmd.Parameters.Add("@DiscRate", SqlDbType.Decimal).Value = st.DiscRate;
                        cmd.Parameters.Add("@DiscAmount", SqlDbType.Decimal).Value = st.DiscAmount;
                        cmd.Parameters.Add("@NetAmount", SqlDbType.Decimal).Value = st.NetAmount;
                        cmd.Parameters.Add("@DeliveryCharges", SqlDbType.Decimal).Value = st.DeliveryCharges;
                        cmd.Parameters.Add("@dtInvoiceDetail", SqlDbType.Structured).Value = dt;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = loggedInUserId;
                        cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = insertUpdateStatus;
                        cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@CheckReturn2", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        result.Message = cmd.Parameters["@CheckReturn"].Value.ToString();
                        result.IdString = cmd.Parameters["@CheckReturn2"].Value.ToString();
                        cmd.Dispose();
                    }
                    con.Close();
                    con.Dispose();
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message.ToString();
                result.IdString = "0";
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message.ToString(), "Web Application",
                                "InvoiceRepository", "InsertUpdateInvoiceDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteInvoice(string id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            InvoiceViewModel model = new InvoiceViewModel
            {
                InvoiceNumber = id
            };

            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("WarehouseId");
            dt.Columns.Add("MasterCategoryId");
            dt.Columns.Add("ChildCategoryId");
            dt.Columns.Add("ProductId");
            dt.Columns.Add("AttributeId");
            dt.Columns.Add("AttributeDetailId");
            dt.Columns.Add("UnitId");
            dt.Columns.Add("DiscountOfferId");
            dt.Columns.Add("TotalAmount");
            dt.Columns.Add("Rate");
            dt.Columns.Add("Qty");
            dt.Columns.Add("DiscType");
            dt.Columns.Add("DiscRate");
            dt.Columns.Add("DiscAmount");
            dt.Columns.Add("NetAmount");
            dt.Rows.Add(new object[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0 });
            ResultViewModel result = InsertUpdateInvoiceDb(model, dt, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Invoice Deleted Successfully";
                response.IdString = result.IdString;
            }
            else
            {
                response.Status = false;
                response.Message = result.Message;
                response.Id = result.Id;
            }

            return response;
        }
        public List<InvoiceDetailViewModel> GetInvoiceDetailsList(string invoiceNo)
        {
            List<InvoiceDetailViewModel> list = new List<InvoiceDetailViewModel>();
            try
            {
                list = (from c in _unitOfWork.Db.Set<tblInvoiceDetail>()
                        where c.InvoiceNumber == invoiceNo
                        select new InvoiceDetailViewModel
                        {
                            WarehouseId = c.WarehouseId,
                            MasterCategoryId = c.MasterCategoryId,
                            MasterCategoryName = _unitOfWork.Db.Set<tblCategory>() .Where(x => x.CategoryId == c.MasterCategoryId).Select(x => x.CategoryName).FirstOrDefault() ?? string.Empty,
                            ChildCategoryId = c.ChildCategoryId,
                            ChildCategoryName = _unitOfWork.Db.Set<tblCategory>().Where(x => x.CategoryId == c.ChildCategoryId).Select(x => x.CategoryName).FirstOrDefault() ?? string.Empty,
                            ProductId = c.ProductId,
                            ProductName = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == c.ProductId).Select(x => x.ProductName).FirstOrDefault() ?? "",
                            AttributeId = c.AttributeId,
                            AttributeName = _unitOfWork.Db.Set<tblAttribute>().Where(x => x.AttributeId == c.AttributeId).Select(x => x.AttributeName).FirstOrDefault() ?? "",
                            AttributeDetailId = c.AttributeDetailId,
                            AttributeDetailName = _unitOfWork.Db.Set<tblAttributeDetail>().Where(x => x.AttributeDetailId == c.AttributeDetailId).Select(x => x.AttributeDetailName).FirstOrDefault() ?? "",
                            TotalAmount = c.TotalAmount ?? 0,
                            Rate = c.Rate ?? 0,
                            Qty = c.Qty ?? 0,
                            DiscType = c.DiscType ?? "%",
                            DiscRate = c.DiscRate ?? 0,
                            DiscAmount = c.DiscAmount ?? 0,
                            NetAmount = c.NetAmount ?? 0,
                        }).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message, "Web Application",
                    "InvoiceRepository", "GetInvoiceDetailsList");
            }
            return list;
        }
        public string GetMaxCodeForInvoice(string type, int loggedInUserId)
        {
            string maxCode = "";
            string currentYear = DateTime.Now.Year.ToString();
            string currentMonth = DateTime.Now.Month.ToString();
            if (currentMonth.Length == 1)
            {
                currentMonth = "0" + currentMonth;
            }
            currentYear = currentYear.Substring(2, 2);
            string InvoiceNo = "";
            if (type == "Sale Invoice")
            {
                InvoiceNo = "SI-" + currentYear + currentMonth;
            }
            else if (type == "Sale Return")
            {
                InvoiceNo = "SR-" + currentYear + currentMonth;
            }
            else if (type == "Purchase Invoice")
            {
                InvoiceNo = "PI-" + currentYear + currentMonth;
            }
            else if (type == "Purchase Return")
            {
                InvoiceNo = "PR-" + currentYear + currentMonth;
            }
            else
            {
                InvoiceNo = "NU-" + currentYear + currentMonth;
            }

            int count = _unitOfWork.Db.Set<tblInvoice>().Where(x => x.InvoiceType == type).Count();
            if (count > 0)
            {
                maxCode = _unitOfWork.Db.Set<tblInvoice>().Where(x => x.InvoiceType == type).OrderByDescending(x => x.InvoiceNumber).Select(x => x.InvoiceNumber).FirstOrDefault();

                maxCode = maxCode.Split('-')[1];
                maxCode = maxCode.Substring(4, 4);
                int codeInIntegar = Convert.ToInt32(maxCode);
                codeInIntegar += 1;
                if (codeInIntegar.ToString().Length == 1)
                {
                    maxCode = InvoiceNo + "000" + codeInIntegar.ToString();
                }
                else if (codeInIntegar.ToString().Length == 2)
                {
                    maxCode = InvoiceNo + "00" + codeInIntegar.ToString();
                }
                else if (codeInIntegar.ToString().Length == 3)
                {
                    maxCode = InvoiceNo + "0" + codeInIntegar.ToString();
                }
                else if (codeInIntegar.ToString().Length == 4)
                {
                    maxCode = InvoiceNo + codeInIntegar.ToString();
                }
            }
            else
            {
                maxCode = InvoiceNo + "0001";
            }
            return maxCode;
        }

        public StatusMessageViewModel InsertUpdateInvoiceStatus(InvoiceStatusViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.InvoiceStatusId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblInvoiceStatus>().Where(x => x.InvoiceStatusId == model.InvoiceStatusId).Any(x => x.InvoiceNumber.ToLower().Trim() == model.InvoiceNumber.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblInvoiceStatus>().Any(x => x.InvoiceNumber.ToLower().Trim() == model.InvoiceNumber.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Invoice Number already exists.";
                            return response;
                        }
                    }
                   
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblInvoiceStatus>().Any(x => x.InvoiceNumber.ToLower().Trim() == model.InvoiceNumber.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Status already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateInvoiceStatusDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Status Saved Successfully";
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
                                "InvoiceRepository", "InsertUpdateInvoiceStatus");
            }
            return response;
        }
        private ResultViewModel InsertUpdateInvoiceStatusDb(InvoiceStatusViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateInvoiceStatus", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@InvoiceStatusId", SqlDbType.Int).Value = st.InvoiceStatusId;
                        cmd.Parameters.Add("@InvoiceNumber", SqlDbType.NVarChar).Value = st.InvoiceNumber;
                        cmd.Parameters.Add("@Status", SqlDbType.NVarChar).Value = st.Status;
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
                                "InvoiceRepository", "InsertUpdateInvoiceStatusDb");
            }
            return result;
        }
        public InvoiceStatusViewModel GetInvoiceStatusById(string id)
        {
            InvoiceStatusViewModel model = new InvoiceStatusViewModel();

            if (id != "0")
            {
                model = (from f in _unitOfWork.Db.Set<tblInvoiceStatus>()
                         where f.InvoiceNumber == id.ToLower()
                         select new InvoiceStatusViewModel
                         {
                             InvoiceStatusId = f.InvoiceStatusId,
                             InvoiceNumber = f.InvoiceNumber ?? "",
                             Status = f.Status,
                         }).FirstOrDefault();
            }
            else
            {
                model = new InvoiceStatusViewModel
                {
                    InvoiceNumber = "",
                    Status = "",
                   
                };
            }

            return model;
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