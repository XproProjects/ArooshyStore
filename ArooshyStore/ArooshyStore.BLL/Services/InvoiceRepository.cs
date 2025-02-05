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
                    string query = "SELECT Count(s.InvoiceNumber) as MyRowCount FROM tblInvoice s left join tblCustomerSupplier c on s.CustomerSupplierId = c.CustomerSupplierId where " + whereCondition + " ";
                    //Get List
                    //query += " select s.InvoiceNumber,isnull(s.InvoiceNumber,'') as InvoiceNumber,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblInvoice s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += " select s.InvoiceNumber,isnull(s.InvoiceDate,'') as 'InvoiceDate',isnull(c.CustomerSupplierName,'') as 'CustomerName',isnull(s.TotalAmount,0) as TotalAmount,isnull(s.DiscType,'') as 'DiscType',isnull(s.DiscRate,0) as DiscRate,isnull(s.DiscAmount,0) as DiscAmount,isnull(s.DeliveryCharges,0) as DeliveryCharges,isnull(s.NetAmount,0) as NetAmount,isnull((select top(1) isnull(iss.Status,'') from tblInvoiceStatus iss where iss.InvoiceNumber = s.InvoiceNumber order by iss.InvoiceStatusId desc),'') as Status,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy'  from tblInvoice s left join tblCustomerSupplier c on s.CustomerSupplierId = c.CustomerSupplierId   where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                    CustomerName = reader["CustomerName"].ToString(),
                                    TotalAmount = Convert.ToDecimal(reader["TotalAmount"].ToString()),
                                    DiscType = reader["DiscType"].ToString(),
                                    DiscRate = Convert.ToDecimal(reader["DiscRate"].ToString()),
                                    DiscAmount = Convert.ToDecimal(reader["DiscAmount"].ToString()),
                                    DeliveryCharges = Convert.ToDecimal(reader["DeliveryCharges"].ToString()),
                                    NetAmount = Convert.ToDecimal(reader["NetAmount"].ToString()),
                                    Status = reader["Status"].ToString(),
                                    InvoiceNumberWithStatus = reader["InvoiceNumber"].ToString() + "|" + reader["Status"].ToString(),
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

        public InvoiceViewModel GetInvoiceById(string id, string type)
        {
            InvoiceViewModel model = new InvoiceViewModel();

            string isNewOrEdit = "";
            if (type.ToLower() == "edit")
            {
                isNewOrEdit = "Update";
            }
            else if (type.ToLower() == "exchange")
            {
                isNewOrEdit = "Exchange";
            }
            else
            {
                isNewOrEdit = "New";
            }
            if (type.ToLower() == "edit" || type.ToLower() == "exchange")
            {
                model = (from f in _unitOfWork.Db.Set<tblInvoice>()
                         where f.InvoiceNumber == id
                         select new InvoiceViewModel
                         {
                             InvoiceNumber = f.InvoiceNumber ?? "",
                             InvoiceType = f.InvoiceType ?? "",
                             InvoiceDate = f.InvoiceDate ?? DateTime.Now,
                             CustomerSupplierId = f.CustomerSupplierId ?? 0,
                             CustomerName = _unitOfWork.Db.Set<tblCustomerSupplier>()
                                            .Where(x => x.CustomerSupplierId == f.CustomerSupplierId)
                                            .Select(x => x.CustomerSupplierName)
                                            .FirstOrDefault() ?? "",
                             TotalAmount = f.TotalAmount ?? 0,
                             DiscType = f.DiscType ?? "Rs.",
                             DiscRate = f.DiscRate ?? 0,
                             DiscAmount = f.DiscAmount ?? 0,
                             NetAmount = f.NetAmount ?? 0,
                             DeliveryCharges = f.DeliveryCharges ?? 0,
                             CashOrCredit = f.CashOrCredit ?? "",
                             IsNewOrEdit = isNewOrEdit,
                         }).FirstOrDefault();
            }
            else
            {
                model = new InvoiceViewModel
                {
                    InvoiceNumber = "",
                    InvoiceType = "",
                    InvoiceDate = DateTime.Now,
                    CustomerSupplierId = 0,
                    CustomerName = "",
                    TotalAmount = 0,
                    DiscType = "Rs.",
                    DiscRate = 0,
                    DiscAmount = 0,
                    NetAmount = 0,
                    DeliveryCharges = 0,
                    CashOrCredit = "Cash",
                    IsNewOrEdit = isNewOrEdit
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
                dt.Columns.Add("ProductId");
                dt.Columns.Add("ProductAttributeDetailBarcodeId");
                dt.Columns.Add("TotalAmount");
                dt.Columns.Add("Qty");
                dt.Columns.Add("Rate");
                dt.Columns.Add("OfferDetailId");
                dt.Columns.Add("DiscType");
                dt.Columns.Add("DiscRate");
                dt.Columns.Add("DiscAmount");
                dt.Columns.Add("NetAmount");

                if (list.Count != 0)
                {
                    for (int i = 0; i < list.Count; i++)
                    {
                        dt.Rows.Add(new object[] { i + 1, list[i].WarehouseId, list[i].ProductId, list[i].ProductAttributeDetailBarcodeId, list[i].TotalAmount, list[i].Qty, list[i].Rate, list[i].OfferDetailId, list[i].DiscType, list[i].DiscRate, list[i].DiscAmount, list[i].NetAmount });
                    }
                }
                else
                {
                    dt.Rows.Add(new object[] { 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0 });
                }

                if (model.IsNewOrEdit == "Update")
                {
                    insertUpdateStatus = "Update";
                }
                else if (model.IsNewOrEdit == "Exchange")
                {
                    insertUpdateStatus = "Exchange";
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
                        cmd.Parameters.Add("@CashOrCredit", SqlDbType.NVarChar).Value = st.CashOrCredit;
                        cmd.Parameters.Add("@AdminOrClient", SqlDbType.NVarChar).Value = st.AdminOrClient;
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
            dt.Columns.Add("ProductId");
            dt.Columns.Add("ProductAttributeDetailBarcodeId");
            dt.Columns.Add("TotalAmount");
            dt.Columns.Add("Qty");
            dt.Columns.Add("Rate");
            dt.Columns.Add("OfferDetailId");
            dt.Columns.Add("DiscType");
            dt.Columns.Add("DiscRate");
            dt.Columns.Add("DiscAmount");
            dt.Columns.Add("NetAmount");

            dt.Rows.Add(new object[] { 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0 });
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

        public StatusMessageViewModel ReturnInvoice(string id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            InvoiceViewModel model = new InvoiceViewModel
            {
                InvoiceNumber = id
            };

            DataTable dt = new DataTable();
            dt.Columns.Add("Id");
            dt.Columns.Add("WarehouseId");
            dt.Columns.Add("ProductId");
            dt.Columns.Add("ProductAttributeDetailBarcodeId");
            dt.Columns.Add("TotalAmount");
            dt.Columns.Add("Qty");
            dt.Columns.Add("Rate");
            dt.Columns.Add("OfferDetailId");
            dt.Columns.Add("DiscType");
            dt.Columns.Add("DiscRate");
            dt.Columns.Add("DiscAmount");
            dt.Columns.Add("NetAmount");

            dt.Rows.Add(new object[] { 0, 0, 0, 0, 0, 0, 0, 0, "", 0, 0, 0 });
            ResultViewModel result = InsertUpdateInvoiceDb(model, dt, "Return", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Invoice Returned Successfully";
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

        public InvoiceViewModel GetInvoiceByIdForPrint(string id, int loggedInUserId)
        {
            InvoiceViewModel model = (from f in _unitOfWork.Db.Set<tblInvoice>()
                                      where f.InvoiceNumber == id
                                      select new InvoiceViewModel
                                      {
                                          InvoiceNumber = f.InvoiceNumber ?? "",
                                          InvoiceType = f.InvoiceType ?? "",
                                          InvoiceDate = f.InvoiceDate ?? DateTime.Now,
                                          CustomerSupplierId = f.CustomerSupplierId ?? 0,
                                          CustomerName = _unitOfWork.Db.Set<tblCustomerSupplier>()
                                            .Where(x => x.CustomerSupplierId == f.CustomerSupplierId)
                                            .Select(x => x.CustomerSupplierName)
                                            .FirstOrDefault() ?? "",
                                          TotalAmount = f.TotalAmount ?? 0,
                                          DiscType = f.DiscType ?? "Rs.",
                                          DiscRate = f.DiscRate ?? 0,
                                          DiscAmount = f.DiscAmount ?? 0,
                                          NetAmount = f.NetAmount ?? 0,
                                          DeliveryCharges = f.DeliveryCharges ?? 0,
                                          CashOrCredit = f.CashOrCredit ?? "",
                                      }).FirstOrDefault() ?? new InvoiceViewModel();

            model.InvoiceDetailsList = (from c in _unitOfWork.Db.Set<tblInvoiceDetail>()
                                        join p in _unitOfWork.Db.Set<tblProduct>() on c.ProductId equals p.ProductId
                                        join ap in _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>() on c.ProductAttributeDetailBarcodeId equals ap.ProductAttributeDetailBarcodeId
                                        join a1 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId1 equals a1.AttributeId
                                        join a2 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId2 equals a2.AttributeId
                                        join ad1 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId1 equals ad1.AttributeDetailId
                                        join ad2 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId2 equals ad2.AttributeDetailId
                                        where c.InvoiceNumber == id
                                        select new InvoiceDetailViewModel
                                        {
                                            WarehouseId = c.WarehouseId,
                                            ProductId = c.ProductId,
                                            ProductName = p.ArticleNumber + " - " + p.ProductName ?? "",
                                            TotalAmount = c.TotalAmount ?? 0,
                                            Rate = c.Rate ?? 0,
                                            Qty = c.Qty ?? 0,
                                            DiscType = c.DiscType ?? "Rs.",
                                            DiscRate = c.DiscRate ?? 0,
                                            DiscAmount = c.DiscAmount ?? 0,
                                            NetAmount = c.NetAmount ?? 0,
                                            ProductAttributeDetailBarcodeId = c.ProductAttributeDetailBarcodeId ?? 0,
                                            AttributeName = ad1.AttributeDetailName + " - " + ad2.AttributeDetailName,
                                            OfferDetailId = c.OfferDetailId ?? 0,
                                        }).ToList();

            model.LoggedInUserName = (from u in _unitOfWork.Db.Set<tblUser>() join i in _unitOfWork.Db.Set<tblInfo>() on u.InfoId equals i.InfoId where u.UserId == loggedInUserId select i.FullName).FirstOrDefault() ?? "";
            var companyInfo = _unitOfWork.Db.Set<tblCompany>().FirstOrDefault();
            if (companyInfo != null)
            {
                model.CompanyLogoPath = _unitOfWork.Db.Set<tblDocument>()
                            .Where(x => x.TypeId == companyInfo.CompanyId.ToString() && x.DocumentType == "Company" && x.Remarks == "ProfilePicture")
                            .Select(x => "/Areas/Admin/FormsDocuments/Company/" + x.DocumentId + "." + x.DocumentExtension)
                            .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png";
                model.CompanyEmail = companyInfo.Email ?? "";
                model.CompanyAddress = companyInfo.Address ?? "";
                model.CompanyContact = companyInfo.Contact1 ?? "";
            }

            return model;
        }
        public List<InvoiceDetailViewModel> InvoiceDetailsList(string invoiceNo)
        {
            List<InvoiceDetailViewModel> list = new List<InvoiceDetailViewModel>();
            try
            {
                list = (from c in _unitOfWork.Db.Set<tblInvoiceDetail>()
                        join p in _unitOfWork.Db.Set<tblProduct>() on c.ProductId equals p.ProductId
                        join ap in _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>() on c.ProductAttributeDetailBarcodeId equals ap.ProductAttributeDetailBarcodeId
                        join a1 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId1 equals a1.AttributeId
                        join a2 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId2 equals a2.AttributeId
                        join ad1 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId1 equals ad1.AttributeDetailId
                        join ad2 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId2 equals ad2.AttributeDetailId
                        where c.InvoiceNumber == invoiceNo
                        select new InvoiceDetailViewModel
                        {
                            WarehouseId = c.WarehouseId,
                            ProductId = c.ProductId,
                            ProductName = p.ArticleNumber + " - " + p.ProductName ?? "",
                            TotalAmount = c.TotalAmount ?? 0,
                            Rate = c.Rate ?? 0,
                            Qty = c.Qty ?? 0,
                            DiscType = c.DiscType ?? "Rs.",
                            DiscRate = c.DiscRate ?? 0,
                            DiscAmount = c.DiscAmount ?? 0,
                            NetAmount = c.NetAmount ?? 0,
                            ProductAttributeDetailBarcodeId = c.ProductAttributeDetailBarcodeId ?? 0,
                            AttributeName = ad1.AttributeDetailName + " - " + ad2.AttributeDetailName,
                            OfferDetailId = c.OfferDetailId ?? 0,
                        }).ToList();
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message, "Web Application",
                    "InvoiceRepository", "InvoiceDetailsList");
            }
            return list;
        }
        public List<InvoiceDetailViewModel> GetInvoiceDetailsList(string invoiceNo)
        {
            List<InvoiceDetailViewModel> list = new List<InvoiceDetailViewModel>();
            try
            {
                list = (from c in _unitOfWork.Db.Set<tblInvoiceDetail>()
                        join p in _unitOfWork.Db.Set<tblProduct>() on c.ProductId equals p.ProductId
                        join ap in _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>() on c.ProductAttributeDetailBarcodeId equals ap.ProductAttributeDetailBarcodeId
                        join a1 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId1 equals a1.AttributeId
                        join a2 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId2 equals a2.AttributeId
                        join ad1 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId1 equals ad1.AttributeDetailId
                        join ad2 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId2 equals ad2.AttributeDetailId
                        where c.InvoiceNumber == invoiceNo
                        select new InvoiceDetailViewModel
                        {
                            WarehouseId = c.WarehouseId,
                            ProductId = c.ProductId,
                            ProductName = p.ArticleNumber + " - " + p.ProductName ?? "",
                            TotalAmount = c.TotalAmount ?? 0,
                            Rate = c.Rate ?? 0,
                            Qty = c.Qty ?? 0,
                            DiscType = c.DiscType ?? "Rs.",
                            DiscRate = c.DiscRate ?? 0,
                            DiscAmount = c.DiscAmount ?? 0,
                            NetAmount = c.NetAmount ?? 0,
                            ProductAttributeDetailBarcodeId = c.ProductAttributeDetailBarcodeId ?? 0,
                            AttributeName = ad1.AttributeDetailName + " - " + ad2.AttributeDetailName,
                            OfferDetailId = c.OfferDetailId ?? 0,
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

            int count = _unitOfWork.Db.Set<tblInvoice>().Where(x => x.InvoiceType == type && x.InvoiceDate.Value.Year == DateTime.Now.Year && x.InvoiceDate.Value.Month == DateTime.Now.Month).Count();
            if (count > 0)
            {
                maxCode = _unitOfWork.Db.Set<tblInvoice>().Where(x => x.InvoiceType == type && x.InvoiceDate.Value.Year == DateTime.Now.Year && x.InvoiceDate.Value.Month == DateTime.Now.Month).OrderByDescending(x => x.InvoiceNumber).Select(x => x.InvoiceNumber).FirstOrDefault();

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

        public decimal GetDeliveryCharges(int customerId, int loggedInUserId)
        {
            int cityId = _unitOfWork.Db.Set<tblCustomerSupplier>().Where(x => x.CustomerSupplierId == customerId).Select(x => x.CityId).FirstOrDefault() ?? 0;
            decimal deliveryCharges = _unitOfWork.Db.Set<tblDeliveryCharges>().Where(x => x.CityId == cityId).Select(x => x.DeliveryCharges).FirstOrDefault() ?? 0;
            return deliveryCharges;
        }

        public StatusMessageViewModel InsertUpdateInvoiceStatus(InvoiceStatusViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.InvoiceStatusId > 0)
                {
                    insertUpdateStatus = "Update";
                }
                else
                {
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
                         where f.InvoiceNumber.ToLower() == id.ToLower()
                         orderby f.InvoiceStatusId descending
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

        public InvoiceViewModel GetCashCustomer()
        {
            InvoiceViewModel model = new InvoiceViewModel();
            model = (from f in _unitOfWork.Db.Set<tblCustomerSupplier>()
                     where f.Email.ToLower() == "cashcustomer@yahoo.com"
                     select new InvoiceViewModel
                     {
                         CustomerSupplierId = f.CustomerSupplierId,
                         CustomerName = f.CustomerSupplierName ?? "",
                     }).FirstOrDefault() ?? new InvoiceViewModel();

            return model;
        }

        public int GetTotalInvoiceItems(string id)
        {
            int totalItems = _unitOfWork.Db.Set<tblInvoiceDetail>().Where(x => x.InvoiceNumber == id).Count();
            return totalItems;
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