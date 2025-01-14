using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography.Xml;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Domain.DomainModels;
using ArooshyStore.Models.ViewModels;
using Newtonsoft.Json;

namespace ArooshyStore.BLL.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        List<string> barcodesList = new List<string>();
        public ProductRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #region Admin Side
        public List<ProductViewModel> GetProductsListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<ProductViewModel> list = new List<ProductViewModel>();
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

                    string query = "SELECT Count(s.ProductId) as MyRowCount FROM tblProduct s left join tblCategory c on s.CategoryId = c.CategoryId left join tblCategory p on c.ParentCategoryId = p.CategoryId where " + whereCondition + " ";
                    query += " select s.ProductId,isnull(s.ProductName,'') as ProductName,isnull(s.ArticleNumber,'') as ArticleNumber,isnull(s.ProductDescription,'') as ProductDescription,isnull(c.CategoryName,'') + ' - ' + isnull(p.CategoryName,'') as CategoryName,isnull(s.ProductNameUrdu,'') as 'ProductNameUrdu',isnull(s.SalePrice,0) as SalePrice,isnull(s.SalePriceForWebsite,0) as SalePriceForWebsite,isnull(s.SalePriceAfterExpired,0) as SalePriceAfterExpired,isnull(s.CostPrice,0) as 'CostPrice',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',(case when isnull(s.IsExpired,0) = 0 then 'No' else 'Yes' end) as 'IsExpiredString',(case when isnull(s.ShowOnWebsite,0) = 0 then 'No' else 'Yes' end) as 'ShowOnWebsiteString',(case when isnull(s.IsFeatured,0) = 0 then 'No' else 'Yes' end) as 'IsFeaturedString',isnull((select '/Areas/Admin/FormsDocuments/Product/' + cast(isnull(dc.DocumentId,0) as varchar) + '.' +  isnull(dc.DocumentExtension,'')  from tblDocument dc where dc.TypeId = CAST(s.ProductId as varchar)  and dc.DocumentType = 'Product' and dc.Remarks = 'ProfilePicture' ),'/Areas/Admin/Content/noimage.png') as 'ImagePath',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'Status',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblProduct s left join tblCategory c on s.CategoryId = c.CategoryId left join tblCategory p on c.ParentCategoryId = p.CategoryId  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                list.Add(new ProductViewModel()
                                {
                                    ProductId = Convert.ToInt32(reader["ProductId"]),
                                    ProductName = reader["ProductName"].ToString(),
                                    ProductNameUrdu = reader["ProductNameUrdu"].ToString(),
                                    ProductDescription = reader["ProductDescription"].ToString(),
                                    ArticleNumber = reader["ArticleNumber"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    CostPrice = Convert.ToDecimal(reader["CostPrice"].ToString()),
                                    SalePrice = Convert.ToDecimal(reader["SalePrice"].ToString()),
                                    SalePriceForWebsite = Convert.ToDecimal(reader["SalePriceForWebsite"].ToString()),
                                    SalePriceAfterExpired = Convert.ToDecimal(reader["SalePriceAfterExpired"].ToString()),
                                    StatusString = reader["StatusString"].ToString(),
                                    IsFeaturedString = reader["IsFeaturedString"].ToString(),
                                    IsExpiredString = reader["IsExpiredString"].ToString(),
                                    ShowOnWebsiteString = reader["ShowOnWebsiteString"].ToString(),
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
                                "ProductRepository", "GetProductsListAndCount");
            }
            return list;
        }

        public ProductViewModel GetProductById(int id)
        {
            ProductViewModel model = new ProductViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblProduct>()
                         join c in _unitOfWork.Db.Set<tblCategory>() on f.CategoryId equals c.CategoryId
                         join p in _unitOfWork.Db.Set<tblCategory>() on c.ParentCategoryId equals p.CategoryId
                         where f.ProductId == id
                         select new ProductViewModel
                         {
                             ProductId = f.ProductId,
                             ProductName = f.ProductName,
                             ProductNameUrdu = f.ProductNameUrdu,
                             ProductDescription = f.ProductDescription,
                             ArticleNumber = f.ArticleNumber ?? "",
                             UnitId = f.UnitId ?? 0,
                             UnitName = _unitOfWork.Db.Set<tblUnit>().Where(x => x.UnitId == f.UnitId).Select(x => x.UnitName).FirstOrDefault() ?? "",
                             CategoryId = f.CategoryId ?? 0,
                             CategoryName = (c.CategoryName + " - " +p.CategoryName) ?? "",
                             DeliveryInfoId = f.DeliveryInfoId ?? 0,
                             DeliveryInfoName = _unitOfWork.Db.Set<tblDeliveryInfo>().Where(x => x.DeliveryInfoId == f.DeliveryInfoId).Select(x => x.DeliveryInfoName).FirstOrDefault() ?? "",
                             SalePrice = f.SalePrice ?? 0,
                             CostPrice = f.CostPrice ?? 0,
                             SalePriceAfterExpired = f.SalePriceAfterExpired ?? 0,
                             SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                             //TagId = i.TagId,
                             //TagName = _unitOfWork.Db.Set<tblProductTags>().Where(x => x.TagId == i.TagId).Select(x => x.TagName).FirstOrDefault() ?? "",
                             IsExpired = f.IsExpired,
                             Status = f.Status,
                             IsFeatured = f.IsFeatured ?? false,
                             ShowOnWebsite = f.ShowOnWebsite ?? false,
                             ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                           .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                                           .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                             DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                           .Select(x => x.DocumentId)
                                                           .FirstOrDefault(),

                         }).FirstOrDefault();
            }
            else
            {
                model = new ProductViewModel
                {
                    ProductId = 0,
                    ProductName = "",
                    ProductNameUrdu = "",
                    ProductDescription = "",
                    DeliveryInfoId = 0,
                    DeliveryInfoName = "",
                    ArticleNumber = "",
                    UnitId = 0,
                    CategoryId = 0,
                    CategoryName = "",
                    UnitName = "",
                    SalePrice = 0,
                    CostPrice = 0,
                    SalePriceAfterExpired = 0,
                    SalePriceForWebsite = 0,
                    Status = false,
                    IsFeatured = false,
                    IsExpired = false,
                    ShowOnWebsite = false,
                    ImagePath = "/Areas/Admin/Content/noimage.png",
                    DocumentId = 0,
                };
            }
            return model;
        }
        public ProductViewModel GetProductAttributesById(int id)
        {
            ProductViewModel model = new ProductViewModel();
            List<AttributeViewModel> AttributesList = (from a in _unitOfWork.Db.Set<tblAttribute>()
                                                       where a.Status == true
                                                       && _unitOfWork.Db.Set<tblAttributeDetail>().Where(x => x.AttributeId == a.AttributeId).Count() > 0
                                                       select new AttributeViewModel
                                                       {
                                                           AttributeId = a.AttributeId,
                                                           AttributeName = a.AttributeName,
                                                       }).ToList();

            foreach (AttributeViewModel c in AttributesList)
            {
                c.AttributeDetails = (from f in _unitOfWork.Db.Set<tblAttributeDetail>()
                                      where f.AttributeId == c.AttributeId
                                      && f.Status == true
                                      select new ProductAttributeDetailViewModel
                                      {
                                          AttributeDetailId = f.AttributeDetailId,
                                          AttributeDetailName = f.AttributeDetailName,
                                          ProductAttributeDetailId = _unitOfWork.Db.Set<tblProductAttributeDetail>().Where(x => x.ProductId == id && x.AttributeId == c.AttributeId && x.AttributeDetailId == f.AttributeDetailId).Select(x => x.ProductAttributeDetailId).FirstOrDefault(),
                                      }).OrderBy(x => x.AttributeDetailId).ToList();
                if (c.AttributeDetails.Any(x => x.ProductAttributeDetailId == 0))
                {
                    c.AllAttributes = "Not";
                }
                else
                {
                    c.AllAttributes = "Yes";
                }
            }
            model.AttributesList = AttributesList.OrderBy(x => x.AttributeName).ToList();
            return model;
        }

        public StatusMessageViewModel InsertUpdateProduct(ProductViewModel model, string AttributeDetailData, string Tagsdata, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                List<AttributeViewModel> list = JsonConvert.DeserializeObject<List<AttributeViewModel>>(AttributeDetailData);
                List<TagsForProductsViewModel> tagsList = JsonConvert.DeserializeObject<List<TagsForProductsViewModel>>(Tagsdata);

                DataTable dtAttributes = new DataTable();
                dtAttributes.Columns.Add("Id");
                dtAttributes.Columns.Add("ProductAttributeDetailId");
                dtAttributes.Columns.Add("AttributeId");
                dtAttributes.Columns.Add("AttributeDetailId");
                dtAttributes.Columns.Add("Status");

                if (list.Count != 0)
                {
                    int k = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        foreach (ProductAttributeDetailViewModel j in list[i].AttributeDetails)
                        {
                            if (j.IsChecked.ToLower() == "yes")
                            {
                                k++;
                                dtAttributes.Rows.Add(new object[] { k, j.ProductAttributeDetailId, list[i].AttributeId, j.AttributeDetailId, true });
                            }
                        }
                    }
                }
                else
                {
                    dtAttributes.Rows.Add(new object[] { 0, 0, 0, 0, false });
                }
                DataTable dtTags = new DataTable();
                dtTags.Columns.Add("Id");
                dtTags.Columns.Add("TagId");
                if (tagsList.Count != 0)
                {
                    dtTags.Rows.Clear();
                    for (int i = 0; i < tagsList.Count; i++)
                    {
                        dtTags.Rows.Add(new object[] { i + 1, tagsList[i].TagId });
                    }
                }
                else
                {
                    dtTags.Rows.Add(new object[] { 0, 0 });
                }
                if (model.ProductId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == model.ProductId).Any(x => x.ArticleNumber.ToLower().Trim() == model.ArticleNumber.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblProduct>().Any(x => x.ArticleNumber.ToLower().Trim() == model.ArticleNumber.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Article Number already exists.";
                            return response;
                        }
                    }
                    model.Status = true ? model.StatusString == "Yes" : false;
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblProduct>().Any(x => x.ArticleNumber.ToLower().Trim() == model.ArticleNumber.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Article Number already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                model.IsFeatured = true ? model.IsFeaturedString == "Yes" : false;
                model.IsExpired = true ? model.IsExpiredString == "Yes" : false;
                model.ShowOnWebsite = true ? model.ShowOnWebsiteString == "Yes" : false;

                // Call method to interact with DB
                ResultViewModel result = InsertUpdateProductDb(model, insertUpdateStatus, dtAttributes, dtTags, loggedInUserId);
                if (result.Message == "Success")
                {
                    SaveBarcodes(result.Id, insertUpdateStatus, loggedInUserId);

                    response.Status = true;
                    response.Message = "Product Saved Successfully";
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
                response.Message = ex.Message;
                response.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message, "Web Application", "ProductRepository", "InsertUpdateProduct");
            }
            return response;
        }

        public StatusMessageViewModel UpdateCostPrice(ProductViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "Update Cost";

                DataTable dtAttributes = new DataTable();
                dtAttributes.Columns.Add("Id");
                dtAttributes.Columns.Add("ProductAttributeDetailId");
                dtAttributes.Columns.Add("AttributeId");
                dtAttributes.Columns.Add("AttributeDetailId");
                dtAttributes.Columns.Add("Status");

                dtAttributes.Rows.Add(new object[] { 0, 0, 0, 0, false });
                DataTable dtTags = new DataTable();
                dtTags.Columns.Add("Id");
                dtTags.Columns.Add("TagId");
                dtTags.Rows.Add(new object[] { 0, 0 });

                // Call method to interact with DB
                ResultViewModel result = InsertUpdateProductDb(model, insertUpdateStatus, dtAttributes, dtTags, loggedInUserId);
                if (result.Message == "Success")
                {
                    SaveBarcodes(result.Id, insertUpdateStatus, loggedInUserId);

                    response.Status = true;
                    response.Message = "Product Cost Saved Successfully";
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
                response.Message = ex.Message;
                response.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message, "Web Application", "ProductRepository", "UpdateCostPrice");
            }
            return response;
        }

        private ResultViewModel InsertUpdateProductDb(ProductViewModel model, string insertUpdateStatus, DataTable dtAttributes, DataTable dtTags, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateProduct", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = model.ProductId;
                        cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar).Value = model.ProductName;
                        cmd.Parameters.Add("@ProductNameUrdu", SqlDbType.NVarChar).Value = model.ProductNameUrdu;
                        cmd.Parameters.Add("@ProductDescription", SqlDbType.NVarChar).Value = model.ProductDescription;
                        cmd.Parameters.Add("@ArticleNumber", SqlDbType.NVarChar).Value = model.ArticleNumber;
                        cmd.Parameters.Add("@DeliveryInfoId", SqlDbType.Int).Value = model.DeliveryInfoId;
                        cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = model.UnitId;
                        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = model.CategoryId;
                        cmd.Parameters.Add("@CostPrice", SqlDbType.Decimal).Value = model.CostPrice;
                        cmd.Parameters.Add("@SalePrice", SqlDbType.Decimal).Value = model.SalePrice;
                        cmd.Parameters.Add("@SalePriceForWebsite", SqlDbType.Decimal).Value = model.SalePriceForWebsite;
                        cmd.Parameters.Add("@SalePriceAfterExpired", SqlDbType.Decimal).Value = model.SalePriceAfterExpired;
                        cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = model.Status;
                        cmd.Parameters.Add("@IsFeatured", SqlDbType.Bit).Value = model.IsFeatured;
                        cmd.Parameters.Add("@IsExpired", SqlDbType.Bit).Value = model.IsExpired;
                        cmd.Parameters.Add("@ShowOnWebsite", SqlDbType.Bit).Value = model.ShowOnWebsite;
                        cmd.Parameters.Add("@dtProductAttributeDetailType", SqlDbType.Structured).Value = dtAttributes;
                        cmd.Parameters.Add("@dtTagsForProductsType", SqlDbType.Structured).Value = dtTags;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = loggedInUserId;
                        cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = insertUpdateStatus;
                        cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@CheckReturn2", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        result.Message = cmd.Parameters["@CheckReturn"].Value.ToString();
                        result.Id = Convert.ToInt32(cmd.Parameters["@CheckReturn2"].Value.ToString());
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message, "Web Application", "ProductRepository", "InsertUpdateProductDb");
            }
            return result;
        }

        public StatusMessageViewModel DeleteProduct(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            ProductViewModel model = new ProductViewModel();
            model.ProductId = id;
            DataTable dtAttributes = new DataTable();
            dtAttributes.Columns.Add("Id");
            dtAttributes.Columns.Add("ProductAttributeDetailId");
            dtAttributes.Columns.Add("AttributeId");
            dtAttributes.Columns.Add("AttributeDetailId");
            dtAttributes.Columns.Add("Status");
            DataTable dtTags = new DataTable();
            dtTags.Columns.Add("Id");
            dtTags.Columns.Add("TagId");

            dtAttributes.Rows.Add(new object[] { 0, 0, 0, 0, false });
            dtTags.Rows.Add(new object[] { 0, 0 });

            ResultViewModel result = InsertUpdateProductDb(model, "Delete", dtAttributes, dtTags, loggedInUserId);

            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Unit Deleted Successfully";
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
        public void SaveBarcodes(int productId, string insertUpdateStatus, int loggedInUserId)
        {
            try
            {
                barcodesList = new List<string>();
                List<ProductAttributeDetailViewModel> AttributesList = (from ap in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                                                        join a in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId equals a.AttributeId
                                                                        where ap.ProductId == productId
                                                                        select new ProductAttributeDetailViewModel
                                                                        {
                                                                            AttributeId = a.AttributeId,
                                                                            AttributeName = a.AttributeName,
                                                                        }).Distinct().OrderBy(x => x.AttributeId).ToList() ?? new List<ProductAttributeDetailViewModel>();
                if (AttributesList.Count > 0)
                {
                    List<ProductAttributeDetailViewModel> AttributesDetailsList1 = new List<ProductAttributeDetailViewModel>();
                    List<ProductAttributeDetailViewModel> AttributesDetailsList2 = new List<ProductAttributeDetailViewModel>();
                    List<ProductAttributeDetailViewModel> AttributesDetailsList3 = new List<ProductAttributeDetailViewModel>();
                    for (int i = 0; i < AttributesList.Count; i++)
                    {
                        int attributeId = AttributesList[i].AttributeId ?? 0;
                        if (i == 0)
                        {
                            AttributesDetailsList1 = (from ap in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                                      where ap.ProductId == productId && ap.AttributeId == attributeId
                                                      select new ProductAttributeDetailViewModel
                                                      {
                                                          AttributeId = ap.AttributeId,
                                                          AttributeDetailId = ap.AttributeDetailId,
                                                          Status = ap.Status
                                                      }).ToList() ?? new List<ProductAttributeDetailViewModel>();
                        }
                        else if (i == 1)
                        {
                            AttributesDetailsList2 = (from ap in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                                      where ap.ProductId == productId && ap.AttributeId == attributeId
                                                      select new ProductAttributeDetailViewModel
                                                      {
                                                          AttributeId = ap.AttributeId,
                                                          AttributeDetailId = ap.AttributeDetailId,
                                                          Status = ap.Status
                                                      }).ToList() ?? new List<ProductAttributeDetailViewModel>();
                        }
                        else if (i == 2)
                        {
                            AttributesDetailsList3 = (from ap in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                                      where ap.ProductId == productId && ap.AttributeId == attributeId
                                                      select new ProductAttributeDetailViewModel
                                                      {
                                                          AttributeId = ap.AttributeId,
                                                          AttributeDetailId = ap.AttributeDetailId,
                                                          Status = ap.Status
                                                      }).ToList() ?? new List<ProductAttributeDetailViewModel>();
                        }
                    }

                    DataTable dtAttributes = new DataTable();
                    dtAttributes.Columns.Add("Id");
                    dtAttributes.Columns.Add("ProductAttributeDetailBarcodeId");
                    dtAttributes.Columns.Add("AttributeId1");
                    dtAttributes.Columns.Add("AttributeDetailId1");
                    dtAttributes.Columns.Add("AttributeId2");
                    dtAttributes.Columns.Add("AttributeDetailId2");
                    dtAttributes.Columns.Add("Barcode");
                    dtAttributes.Columns.Add("Status");

                    int j = 0;
                    string barcode = "";
                    foreach (ProductAttributeDetailViewModel p1 in AttributesDetailsList1)
                    {
                        foreach (ProductAttributeDetailViewModel p2 in AttributesDetailsList2)
                        {
                            j++;
                            barcode = GetBarcode();
                            bool status = p1.Status == true && p2.Status == true ? true : false;
                            dtAttributes.Rows.Add(new object[] { j, 0, p1.AttributeId, p1.AttributeDetailId, p2.AttributeId, p2.AttributeDetailId, barcode, status });
                        }
                    }

                    ResultViewModel result = InsertUpdateProductAttributeBarcodesDb(productId, insertUpdateStatus, dtAttributes, loggedInUserId);
                }
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message, "Web Application", "ProductRepository", "SaveBarcodes");
            }
        }

        private ResultViewModel InsertUpdateProductAttributeBarcodesDb(int productId, string insertUpdateStatus, DataTable dtAttributes, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateProductAttributeBarcodes", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = productId;
                        cmd.Parameters.Add("@dtProductAttributeDetailBarcodeType", SqlDbType.Structured).Value = dtAttributes;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = loggedInUserId;
                        cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = insertUpdateStatus;
                        cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@CheckReturn2", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        result.Message = cmd.Parameters["@CheckReturn"].Value.ToString();
                        result.Id = Convert.ToInt32(cmd.Parameters["@CheckReturn2"].Value.ToString());
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message, "Web Application", "ProductRepository", "InsertUpdateProductAttributeBarcodesDb");
            }
            return result;
        }
        public string GetBarcode()
        {
            string barcode = "";
            Random rnd = new Random();

        GenerateRandomNumber:
            int myRandomNo = rnd.Next(10000000, 99999999);
            bool checkRandomNumber = _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>().Any(x => x.Barcode.ToLower().Trim() == myRandomNo.ToString().ToLower().Trim());
            if (checkRandomNumber)
            {
                goto GenerateRandomNumber;
            }

            bool checkRandomNumber2 = barcodesList.Any(s => myRandomNo.ToString().ToLower().Trim().Contains(s.ToLower().Trim()));
            if (checkRandomNumber2)
            {
                goto GenerateRandomNumber;
            }

            barcode = myRandomNo.ToString();
            barcodesList.Add(barcode);
            return barcode;
        }
        public List<ProductAttributeDetailViewModel> GetProductAttributesListByProductId(int productId)
        {
            List<ProductAttributeDetailViewModel> AttributesList = (from ap in _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>()
                                                                    join a1 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId1 equals a1.AttributeId
                                                                    join a2 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId2 equals a2.AttributeId
                                                                    join ad1 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId1 equals ad1.AttributeDetailId
                                                                    join ad2 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId2 equals ad2.AttributeDetailId
                                                                    where ap.Status == true && ap.ProductId == productId
                                                                    select new ProductAttributeDetailViewModel
                                                                    {
                                                                        ProductAttributeDetailBarcodeId = ap.ProductAttributeDetailBarcodeId,
                                                                        ProductId = ap.ProductId ?? 0,
                                                                        AttributeId1 = ap.AttributeId1,
                                                                        AttributeName1 = a1.AttributeName,
                                                                        AttributeId2 = ap.AttributeId2,
                                                                        AttributeName2 = a2.AttributeName,
                                                                        AttributeDetailId1 = ap.AttributeDetailId1,
                                                                        AttributeDetailName1 = ad1.AttributeDetailName,
                                                                        AttributeDetailId2 = ap.AttributeDetailId2,
                                                                        AttributeDetailName2 = ad2.AttributeDetailName,
                                                                        Barcode = ap.Barcode ?? "",
                                                                        Status = ap.Status ?? false,
                                                                        Stock = (_unitOfWork.Db.Set<tblProductStock>().Where(x => x.ProductAttributeDetailBarcodeId == ap.ProductAttributeDetailBarcodeId).Sum(x => x.InQty) ?? 0) - (_unitOfWork.Db.Set<tblProductStock>().Where(x => x.ProductAttributeDetailBarcodeId == ap.ProductAttributeDetailBarcodeId).Sum(x => x.OutQty) ?? 0)
                                                                    }).OrderBy(x => x.AttributeDetailName1).ThenBy(x => x.AttributeDetailName2).ToList() ?? new List<ProductAttributeDetailViewModel>();

            return AttributesList;
        }
        public List<ProductAttributeDetailViewModel> GetProductAttributesListByBarcode(string barcode)
        {
            int productId = _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>().Where(x => x.Barcode.ToLower().Trim() == barcode.ToLower().Trim()).Select(x => x.ProductId).FirstOrDefault() ?? 0;
            List<ProductAttributeDetailViewModel> AttributesList = (from ap in _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>()
                                                                    join a1 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId1 equals a1.AttributeId
                                                                    join a2 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId2 equals a2.AttributeId
                                                                    join ad1 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId1 equals ad1.AttributeDetailId
                                                                    join ad2 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId2 equals ad2.AttributeDetailId
                                                                    where ap.Status == true && ap.ProductId == productId
                                                                    select new ProductAttributeDetailViewModel
                                                                    {
                                                                        ProductAttributeDetailBarcodeId = ap.ProductAttributeDetailBarcodeId,
                                                                        ProductId = ap.ProductId ?? 0,
                                                                        ArticleNumber = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == ap.ProductId).Select(x => x.ArticleNumber).FirstOrDefault() ?? "",
                                                                        ProductName = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == ap.ProductId).Select(x => x.ProductName).FirstOrDefault() ?? "",
                                                                        AttributeId1 = ap.AttributeId1,
                                                                        AttributeName1 = a1.AttributeName,
                                                                        AttributeId2 = ap.AttributeId2,
                                                                        AttributeName2 = a2.AttributeName,
                                                                        AttributeDetailId1 = ap.AttributeDetailId1,
                                                                        AttributeDetailName1 = ad1.AttributeDetailName,
                                                                        AttributeDetailId2 = ap.AttributeDetailId2,
                                                                        AttributeDetailName2 = ad2.AttributeDetailName,
                                                                        Barcode = ap.Barcode ?? "",
                                                                        Status = ap.Status,
                                                                        Stock = (_unitOfWork.Db.Set<tblProductStock>().Where(x=>x.ProductAttributeDetailBarcodeId == ap.ProductAttributeDetailBarcodeId).Sum(x=>x.InQty) ?? 0) - (_unitOfWork.Db.Set<tblProductStock>().Where(x => x.ProductAttributeDetailBarcodeId == ap.ProductAttributeDetailBarcodeId).Sum(x => x.OutQty) ?? 0)
                                                                    }).OrderBy(x => x.AttributeDetailName1).ThenBy(x => x.AttributeDetailName2).ToList() ?? new List<ProductAttributeDetailViewModel>();

            return AttributesList;
        }

        public StatusMessageViewModel InsertUpdateProductStock(string AttributeDetailData, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "Save";
                List<ProductAttributeDetailViewModel> list = JsonConvert.DeserializeObject<List<ProductAttributeDetailViewModel>>(AttributeDetailData);

                DataTable dtAttributes = new DataTable();
                dtAttributes.Columns.Add("Id");
                dtAttributes.Columns.Add("ProductStockId");
                dtAttributes.Columns.Add("StockType");
                dtAttributes.Columns.Add("ProductAttributeDetailBarcodeId");
                dtAttributes.Columns.Add("Qty");
                dtAttributes.Columns.Add("ReferenceId");
                dtAttributes.Columns.Add("WarehouseId");

                if (list.Count != 0)
                {
                    int k = 0;
                    for (int i = 0; i < list.Count; i++)
                    {
                        dtAttributes.Rows.Add(new object[] { i + 1, 0, list[i].StockType, list[i].ProductAttributeDetailBarcodeId, list[i].Stock, list[i].ReferenceId,list[i].WarehouseId });
                    }
                }
                else
                {
                    dtAttributes.Rows.Add(new object[] { 0, 0, "", 0, 0, "", 0 });
                }

                // Call method to interact with DB
                ResultViewModel result = InsertUpdateProductStockDb(insertUpdateStatus, dtAttributes, loggedInUserId);
                if (result.Message == "Success")
                {
                    SaveBarcodes(result.Id, insertUpdateStatus, loggedInUserId);

                    response.Status = true;
                    response.Message = "Product Saved Successfully";
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
                response.Message = ex.Message;
                response.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message, "Web Application", "ProductRepository", "InsertUpdateProductStock");
            }
            return response;
        }

        private ResultViewModel InsertUpdateProductStockDb(string insertUpdateStatus, DataTable dtAttributes, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateProductStock", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@dtProductStock", SqlDbType.Structured).Value = dtAttributes;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = loggedInUserId;
                        cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar).Value = insertUpdateStatus;
                        cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@CheckReturn2", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        result.Message = cmd.Parameters["@CheckReturn"].Value.ToString();
                        result.Id = Convert.ToInt32(cmd.Parameters["@CheckReturn2"].Value.ToString());
                        cmd.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Id = 0;
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(loggedInUserId, ex.Message, "Web Application", "ProductRepository", "InsertUpdateProductStockDb");
            }
            return result;
        }
        public List<ProductAttributeDetailViewModel> GetBarcodesDataForPrint(string data)
        {
            List<ProductAttributeDetailViewModel> barcodesList = JsonConvert.DeserializeObject<List<ProductAttributeDetailViewModel>>(data);
            foreach (ProductAttributeDetailViewModel p in barcodesList)
            {
                p.ProductName = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == p.ProductId).Select(x => x.ProductName).FirstOrDefault() ?? "";
                p.ArticleNumber = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == p.ProductId).Select(x => x.ArticleNumber).FirstOrDefault() ?? "";
                p.Price = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == p.ProductId).Select(x => x.SalePrice).FirstOrDefault() ?? 0;
                p.AttributeDetailName1 = _unitOfWork.Db.Set<tblAttributeDetail>().Where(x => x.AttributeDetailId == p.AttributeDetailId1).Select(x => x.AttributeDetailName).FirstOrDefault() ?? "";
                p.AttributeDetailName2 = _unitOfWork.Db.Set<tblAttributeDetail>().Where(x => x.AttributeDetailId == p.AttributeDetailId2).Select(x => x.AttributeDetailName).FirstOrDefault() ?? "";
            }
            return barcodesList;
        }

        public ProductViewModel GetProductDetailById(int id)
        {
            ProductViewModel model = new ProductViewModel();
            model = (from f in _unitOfWork.Db.Set<tblProduct>()
                     join c in _unitOfWork.Db.Set<tblCategory>() on f.CategoryId equals c.CategoryId
                     join p in _unitOfWork.Db.Set<tblCategory>() on c.ParentCategoryId equals p.CategoryId
                     where f.ProductId == id
                     select new ProductViewModel
                     {
                         ProductId = f.ProductId,
                         ProductName = f.ProductName,
                         ProductNameUrdu = f.ProductNameUrdu,
                         ProductDescription = f.ProductDescription,
                         ArticleNumber = f.ArticleNumber ?? "",
                         CategoryId = f.CategoryId ?? 0,
                         CategoryName = (c.CategoryName + " - " + p.CategoryName) ?? "",
                         DeliveryInfoId = f.DeliveryInfoId ?? 0,
                         DeliveryInfoName = _unitOfWork.Db.Set<tblDeliveryInfo>().Where(x => x.DeliveryInfoId == f.DeliveryInfoId).Select(x => x.DeliveryInfoName).FirstOrDefault() ?? "",
                         SalePrice = f.SalePrice ?? 0,
                         CostPrice = f.CostPrice ?? 0,
                         SalePriceAfterExpired = f.SalePriceAfterExpired ?? 0,
                         SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                         //TagId = i.TagId,
                         //TagName = _unitOfWork.Db.Set<tblProductTags>().Where(x => x.TagId == i.TagId).Select(x => x.TagName).FirstOrDefault() ?? "",
                         IsExpired = f.IsExpired,
                         Status = f.Status,
                         IsFeatured = f.IsFeatured ?? false,
                         ShowOnWebsite = f.ShowOnWebsite ?? false,
                         ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                       .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                       .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                                       .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                     }).FirstOrDefault();
            List<AttributeViewModel> AttributesList = (from a in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                                       join at in _unitOfWork.Db.Set<tblAttribute>() on a.AttributeId equals at.AttributeId
                                                       join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on a.AttributeDetailId equals ad.AttributeDetailId
                                                       where a.Status == true && a.ProductId == id
                                                       select new AttributeViewModel
                                                       {
                                                           AttributeId = a.AttributeId ?? 0,
                                                           AttributeName = at.AttributeName ?? "",
                                                       }).Distinct().ToList();
            List<AttributeViewModel> AttributeDetailsList = (from a in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                                             join at in _unitOfWork.Db.Set<tblAttribute>() on a.AttributeId equals at.AttributeId
                                                             join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on a.AttributeDetailId equals ad.AttributeDetailId
                                                             where a.Status == true && a.ProductId == id
                                                             select new AttributeViewModel
                                                             {
                                                                 AttributeId = a.AttributeId ?? 0,
                                                                 AttributeName = at.AttributeName ?? "",
                                                                 AttributeDetailId = a.AttributeDetailId ?? 0,
                                                                 AttributeDetailName = ad.AttributeDetailName
                                                             }).ToList();

            List<DocumentViewModel> documents = (from d in _unitOfWork.Db.Set<tblDocument>()
                                                 where d.TypeId == id.ToString() && d.DocumentType == "Product"
                                                 select new DocumentViewModel
                                                 {
                                                     DocumentId = d.DocumentId,
                                                     DocumentExtension = d.DocumentExtension,
                                                     DocumentType = d.DocumentType,
                                                     TypeId = d.TypeId,
                                                     Remarks = d.Remarks ?? "",
                                                     ImagePath = "/Areas/Admin/FormsDocuments/" + d.DocumentType + "/" + d.DocumentId.ToString() + "." + d.DocumentExtension
                                                 }).ToList() ?? new List<DocumentViewModel>();

            model.AttributesList = AttributesList.ToList();
            model.AttributeDetailsList = AttributeDetailsList.ToList();
            model.DocumentsList = documents.ToList();
            return model;
        }
        public ProductViewModel GetProductSalePrice(int productId)
        {
            var product = _unitOfWork.Db.Set<tblProduct>()
                                        .Where(p => p.ProductId == productId)
                                        .Select(p => new { p.ProductName, p.SalePrice })
                                        .FirstOrDefault();

            if (product != null)
            {
                return new ProductViewModel
                {
                    ProductName = product.ProductName,
                    SalePrice = product.SalePrice ?? 0
                };
            }

            return new ProductViewModel
            {
                ProductName = "",
                SalePrice = 0
            };
        }
        public ProductViewModel GetProductByBarcode(string barcode)
        {
            ProductViewModel model = new ProductViewModel();

            if (!string.IsNullOrEmpty(barcode))
            {
                var productAttributeDetail = _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>()
                    .Where(f => f.Barcode == barcode)
                    .Select(f => new
                    {
                        ProductId = f.ProductId,
                        AttributeId = f.AttributeId1,
                        AttributeDetailId = f.AttributeDetailId1,
                        Status = f.Status,
                        SalePrice = _unitOfWork.Db.Set<tblProduct>().Where(c => c.ProductId == f.ProductId).Select(c => c.SalePrice).FirstOrDefault() ?? 0,
                        ProductName = _unitOfWork.Db.Set<tblProduct>().Where(c => c.ProductId == f.ProductId).Select(c => c.ProductName).FirstOrDefault() ?? "",
                        AttributeDetailName = _unitOfWork.Db.Set<tblAttributeDetail>().Where(c => c.AttributeDetailId == f.AttributeDetailId1).Select(c => c.AttributeDetailName).FirstOrDefault() ?? "",
                        AttributeName = _unitOfWork.Db.Set<tblAttribute>().Where(c => c.AttributeId == f.AttributeId1).Select(c => c.AttributeName).FirstOrDefault() ?? "",

                        // Get the category details
                        CategoryId = _unitOfWork.Db.Set<tblProduct>().Where(p => p.ProductId == f.ProductId).Select(p => p.CategoryId).FirstOrDefault(),
                        ChildCategoryId = _unitOfWork.Db.Set<tblCategory>().Where(cat => cat.CategoryId == _unitOfWork.Db.Set<tblProduct>().Where(p => p.ProductId == f.ProductId).Select(p => p.CategoryId).FirstOrDefault()).Select(cat => cat.CategoryId).FirstOrDefault(),
                        ChildCategoryName = _unitOfWork.Db.Set<tblCategory>().Where(cat => cat.CategoryId == _unitOfWork.Db.Set<tblProduct>().Where(p => p.ProductId == f.ProductId).Select(p => p.CategoryId).FirstOrDefault()).Select(cat => cat.CategoryName).FirstOrDefault() ?? "",
                        MasterCategoryId = _unitOfWork.Db.Set<tblCategory>().Where(cat => cat.CategoryId == _unitOfWork.Db.Set<tblProduct>().Where(p => p.ProductId == f.ProductId).Select(p => p.CategoryId).FirstOrDefault()).Select(cat => cat.ParentCategoryId).FirstOrDefault(),
                        MasterCategoryName = _unitOfWork.Db.Set<tblCategory>().Where(cat => cat.CategoryId == _unitOfWork.Db.Set<tblProduct>().Where(p => p.ProductId == f.ProductId).Select(p => p.CategoryId).FirstOrDefault() && cat.ParentCategoryId != null).Select(cat => _unitOfWork.Db.Set<tblCategory>().Where(pCat => pCat.CategoryId == cat.ParentCategoryId).Select(pCat => pCat.CategoryName).FirstOrDefault()).FirstOrDefault() ?? ""
                    })
                    .FirstOrDefault();

                if (productAttributeDetail != null)
                {
                    model = new ProductViewModel
                    {
                        ProductId = productAttributeDetail.ProductId ?? 0,
                        ProductName = productAttributeDetail.ProductName,
                        AttributeId = productAttributeDetail.AttributeId ?? 0,
                        AttributeDetailId = productAttributeDetail.AttributeDetailId ?? 0,
                        MasterCategoryId = productAttributeDetail.MasterCategoryId ?? 0,
                        AttributeName = productAttributeDetail.AttributeName,
                        AttributeDetailName = productAttributeDetail.AttributeDetailName,
                        MasterCategoryName = productAttributeDetail.MasterCategoryName,
                        ChildCategoryId = productAttributeDetail.ChildCategoryId,
                        ChildCategoryName = productAttributeDetail.ChildCategoryName,
                        SalePrice = productAttributeDetail.SalePrice,
                        Status = productAttributeDetail.Status ?? false
                    };
                }
            }
            else
            {
                model = new ProductViewModel
                {
                    ProductId = 0,
                    ProductName = "",
                    AttributeId = 0,
                    AttributeDetailId = 0,
                    MasterCategoryId = 0,
                    ChildCategoryId = 0,
                    AttributeName = "",
                    AttributeDetailName = "",
                    MasterCategoryName = "",
                    ChildCategoryName = "",
                    SalePrice = 0,
                    Status = false
                };
            }

            return model;
        }

        #endregion
        #region User Side
        public List<ProductViewModel> GetFeaturedProducts()
        {
            return (from f in _unitOfWork.Db.Set<tblProduct>()
                    where f.Status == true && f.IsFeatured == true && f.IsExpired == false
                    select new ProductViewModel
                    {
                        ProductId = f.ProductId,
                        ProductName = f.ProductName,
                        ProductNameUrdu = f.ProductNameUrdu,
                        UnitId = f.UnitId ?? 0,
                        CategoryId = f.CategoryId ?? 0,
                        CategoryName = _unitOfWork.Db.Set<tblCategory>()
                                        .Where(x => x.CategoryId == f.CategoryId)
                                        .Select(x => x.CategoryName)
                                        .FirstOrDefault() ?? "",
                        UnitName = _unitOfWork.Db.Set<tblUnit>()
                                        .Where(x => x.UnitId == f.UnitId)
                                        .Select(x => x.UnitName)
                                        .FirstOrDefault() ?? "",
                        SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                        CostPrice = f.CostPrice ?? 0,
                        Status = f.Status,
                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                        .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                        .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                        .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                        .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                        .Select(x => x.DocumentId)
                                        .FirstOrDefault(),
                        AttributesList = (from pad in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                          join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on pad.AttributeDetailId equals ad.AttributeDetailId
                                          join a in _unitOfWork.Db.Set<tblAttribute>() on pad.AttributeId equals a.AttributeId
                                          where pad.ProductId == f.ProductId && a.Status == true && ad.Status == true
                                          group ad by new { a.AttributeId, a.AttributeName } into g
                                          select new AttributeViewModel
                                          {
                                              AttributeId = g.Key.AttributeId,
                                              AttributeName = g.Key.AttributeName,
                                              AttributeDetails = g.Select(detail => new ProductAttributeDetailViewModel
                                              {
                                                  AttributeDetailId = detail.AttributeDetailId,
                                                  AttributeDetailName = detail.AttributeDetailName
                                              }).ToList()
                                          }).ToList()

                    }).ToList();
        }
        public ProductViewModel GetProductWithAttributes(int productId)
        {
            var model = (from f in _unitOfWork.Db.Set<tblProduct>()
                         where f.ProductId == productId
                         select new ProductViewModel
                         {
                             ProductId = f.ProductId,
                             ProductName = f.ProductName,
                             ProductDescription = f.ProductDescription,
                             DeliveryInfoId = f.DeliveryInfoId ?? 0,
                             DeliveryInfoDetail = _unitOfWork.Db.Set<tblDeliveryInfo>().Where(x => x.DeliveryInfoId == f.DeliveryInfoId).Select(x => x.DeliveryInfoDetail).FirstOrDefault() ?? "",
                             SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                             CostPrice = f.CostPrice ?? 0,
                             ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                 .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                 .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                 .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                             AttributesList = (from pad in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                               join ad in _unitOfWork.Db.Set<tblAttributeDetail>()
                                               on pad.AttributeDetailId equals ad.AttributeDetailId
                                               join a in _unitOfWork.Db.Set<tblAttribute>()
                                               on pad.AttributeId equals a.AttributeId
                                               where pad.ProductId == productId && a.Status == true && ad.Status == true
                                               group ad by new { a.AttributeId, a.AttributeName } into g
                                               select new AttributeViewModel
                                               {
                                                   AttributeId = g.Key.AttributeId,
                                                   AttributeName = g.Key.AttributeName,
                                                   AttributeDetails = g.Select(detail => new ProductAttributeDetailViewModel
                                                   {
                                                       AttributeDetailId = detail.AttributeDetailId,
                                                       AttributeDetailName = detail.AttributeDetailName
                                                   }).ToList()
                                               }).ToList(),
                             Tags = (from tp in _unitOfWork.Db.Set<tblTagsForProducts>()
                                     join t in _unitOfWork.Db.Set<tblProductTags>()
                                     on tp.TagId equals t.TagId
                                     where tp.ProductId == productId
                                     select new TagsForProductsViewModel
                                     {
                                         ProductTagId = tp.ProductTagId,
                                         TagId = tp.TagId,
                                         TagName = t.TagName
                                     }).ToList()

                         }).FirstOrDefault();

            return model;
        }
        public List<ProductViewModel> GetSimilrProducts(int productId)
        {
            int categoryid = _unitOfWork.Db.Set<tblProduct>()
        .Where(x => x.ProductId == productId)
        .Select(x => x.CategoryId)
        .FirstOrDefault() ?? 0;
            return (from f in _unitOfWork.Db.Set<tblProduct>()
                    where f.CategoryId == categoryid && f.ProductId != productId
                    select new ProductViewModel
                    {
                        ProductId = f.ProductId,
                        ProductName = f.ProductName,
                        ProductNameUrdu = f.ProductNameUrdu,
                        ArticleNumber = f.ArticleNumber ?? "",
                        UnitId = f.UnitId ?? 0,
                        CategoryId = f.CategoryId ?? 0,
                        CategoryName = _unitOfWork.Db.Set<tblCategory>()
                                        .Where(x => x.CategoryId == f.CategoryId)
                                        .Select(x => x.CategoryName)
                                        .FirstOrDefault() ?? "",

                        SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                        CostPrice = f.CostPrice ?? 0,
                        Status = f.Status,
                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                        .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                        .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                        .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                        .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                        .Select(x => x.DocumentId)
                                        .FirstOrDefault(),
                        AttributesList = (from pad in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                          join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on pad.AttributeDetailId equals ad.AttributeDetailId
                                          join a in _unitOfWork.Db.Set<tblAttribute>() on pad.AttributeId equals a.AttributeId
                                          where pad.ProductId == f.ProductId && a.Status == true && ad.Status == true
                                          group ad by new { a.AttributeId, a.AttributeName } into g
                                          select new AttributeViewModel
                                          {
                                              AttributeId = g.Key.AttributeId,
                                              AttributeName = g.Key.AttributeName,
                                              AttributeDetails = g.Select(detail => new ProductAttributeDetailViewModel
                                              {
                                                  AttributeDetailId = detail.AttributeDetailId,
                                                  AttributeDetailName = detail.AttributeDetailName
                                              }).ToList()
                                          }).ToList()

                    }).ToList();
        }
        public List<ProductViewModel> GetNewArrivalProducts()
        {
            return (from f in _unitOfWork.Db.Set<tblProduct>()
                    where f.Status == true && f.IsExpired == false
                    orderby f.ProductId descending
                    select new ProductViewModel
                    {
                        ProductId = f.ProductId,
                        ProductName = f.ProductName,
                        ArticleNumber = f.ArticleNumber ?? "",
                        CategoryId = f.CategoryId ?? 0,
                        CategoryName = _unitOfWork.Db.Set<tblCategory>().Where(x => x.CategoryId == f.CategoryId).Select(x => x.CategoryName).FirstOrDefault() ?? "",
                        UnitName = _unitOfWork.Db.Set<tblUnit>()
                                        .Where(x => x.UnitId == f.UnitId)
                                        .Select(x => x.UnitName)
                                        .FirstOrDefault() ?? "",
                        SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                        .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                        .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                        .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                        .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                        .Select(x => x.DocumentId)
                                        .FirstOrDefault(),
                        AttributesList = (from pad in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                          join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on pad.AttributeDetailId equals ad.AttributeDetailId
                                          join a in _unitOfWork.Db.Set<tblAttribute>() on pad.AttributeId equals a.AttributeId
                                          where pad.ProductId == f.ProductId && a.Status == true && ad.Status == true
                                          group ad by new { a.AttributeId, a.AttributeName } into g
                                          select new AttributeViewModel
                                          {
                                              AttributeId = g.Key.AttributeId,
                                              AttributeName = g.Key.AttributeName,
                                              AttributeDetails = g.Select(detail => new ProductAttributeDetailViewModel
                                              {
                                                  AttributeDetailId = detail.AttributeDetailId,
                                                  AttributeDetailName = detail.AttributeDetailName
                                              }).ToList()
                                          }).ToList()
                    }).ToList();
        }
        public List<ProductViewModel> GetProductsForShop()
        {
            var query = from f in _unitOfWork.Db.Set<tblProduct>()
                        where f.Status == true && f.IsExpired == false
                        select f;

            var result = query.Select(f => new ProductViewModel
            {
                ProductId = f.ProductId,
                ProductName = f.ProductName,
                CategoryId = f.CategoryId ?? 0,
                CategoryName = _unitOfWork.Db.Set<tblCategory>()
                                .Where(x => x.CategoryId == f.CategoryId)
                                .Select(x => x.CategoryName)
                                .FirstOrDefault() ?? "",
                SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                CostPrice = f.CostPrice ?? 0,
                Status = f.Status,
                ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                .Select(x => x.DocumentId)
                                .FirstOrDefault(),
                AttributesList = (from pad in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                  join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on pad.AttributeDetailId equals ad.AttributeDetailId
                                  join a in _unitOfWork.Db.Set<tblAttribute>() on pad.AttributeId equals a.AttributeId
                                  where pad.ProductId == f.ProductId && a.Status == true && ad.Status == true
                                  group ad by new { a.AttributeId, a.AttributeName } into g
                                  select new AttributeViewModel
                                  {
                                      AttributeId = g.Key.AttributeId,
                                      AttributeName = g.Key.AttributeName,
                                      AttributeDetails = g.Select(detail => new ProductAttributeDetailViewModel
                                      {
                                          AttributeDetailId = detail.AttributeDetailId,
                                          AttributeDetailName = detail.AttributeDetailName
                                      }).ToList()
                                  }).ToList(),
            }).ToList();

            return result;
        }
        public List<ProductFilterViewModel> GetFiltersList()
        {
            var categories = (from f in _unitOfWork.Db.Set<tblCategory>()
                              where f.Status == true && f.ParentCategoryId != 0
                              select new CategoryViewModel
                              {
                                  CategoryId = f.CategoryId,
                                  CategoryName = f.CategoryName ?? "",
                                  ParentCategoryId = f.ParentCategoryId ?? 0,
                                  Status = f.Status ?? false,
                                  ParentCategoryName = _unitOfWork.Db.Set<tblCategory>()
                                      .Where(x => x.CategoryId == f.ParentCategoryId && f.ParentCategoryId != 0)
                                      .Select(x => x.CategoryName)
                                      .FirstOrDefault() ?? "",
                                  ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                      .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                      .Select(x => "/Areas/Admin/FormsDocuments/Category/" + x.DocumentId + "." + x.DocumentExtension)
                                      .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                                  DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                      .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                      .Select(x => x.DocumentId)
                                      .FirstOrDefault(),
                              }).ToList();

            var groupedAttributes = (from a in _unitOfWork.Db.Set<tblAttribute>()
                                     join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on a.AttributeId equals ad.AttributeId
                                     where a.Status == true && ad.Status == true
                                     select new
                                     {
                                         AttributeId = a.AttributeId,
                                         AttributeName = a.AttributeName,
                                         AttributeDetailId = ad.AttributeDetailId,
                                         AttributeDetailName = ad.AttributeDetailName
                                     })
                        .GroupBy(attr => new { attr.AttributeId, attr.AttributeName })
                        .Select(g => new AttributeViewModel
                        {
                            AttributeId = g.Key.AttributeId,
                            AttributeName = g.Key.AttributeName,
                            AttributeDetails = g.Select(d => new ProductAttributeDetailViewModel
                            {
                                AttributeDetailId = d.AttributeDetailId,
                                AttributeDetailName = d.AttributeDetailName
                            }).ToList()
                        }).ToList();


            var discountOffer = (from d in _unitOfWork.Db.Set<tblDiscountOffer>()
                                 where d.Status == true
                                 select new DiscountOfferViewModel
                                 {
                                     OfferId = d.OfferId,
                                     DiscountName = d.DiscountName ?? "",
                                     //DiscPercent = d.DiscPercent ?? 0,
                                     Status = d.Status ?? false,
                                     //SelectType = d.SelectType ?? "",
                                 }).ToList();

            return new List<ProductFilterViewModel>
    {
        new ProductFilterViewModel
        {
            AttributesList = groupedAttributes,
            Categories = categories,
            DiscountOffer = discountOffer
        }
    };
        }
        public List<ProductViewModel> GetExpiredProducts()
        {
            var expiredProducts = (from f in _unitOfWork.Db.Set<tblProduct>()
                                   where f.Status == true && f.IsExpired == true
                                   select new ProductViewModel
                                   {
                                       ProductId = f.ProductId,
                                       ProductName = f.ProductName,
                                       ProductNameUrdu = f.ProductNameUrdu,
                                       ArticleNumber = f.ArticleNumber ?? "",
                                       UnitId = f.UnitId ?? 0,
                                       CategoryId = f.CategoryId ?? 0,
                                       CategoryName = _unitOfWork.Db.Set<tblCategory>()
                                                       .Where(x => x.CategoryId == f.CategoryId)
                                                       .Select(x => x.CategoryName)
                                                       .FirstOrDefault() ?? "",
                                       UnitName = _unitOfWork.Db.Set<tblUnit>()
                                                       .Where(x => x.UnitId == f.UnitId)
                                                       .Select(x => x.UnitName)
                                                       .FirstOrDefault() ?? "",
                                       SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                                       CostPrice = f.CostPrice ?? 0,
                                       Status = f.Status,
                                       ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                       .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                       .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                                       .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                                       DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                       .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                       .Select(x => x.DocumentId)
                                                       .FirstOrDefault(),
                                       AttributesList = (from pad in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                                         join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on pad.AttributeDetailId equals ad.AttributeDetailId
                                                         join a in _unitOfWork.Db.Set<tblAttribute>() on pad.AttributeId equals a.AttributeId
                                                         where pad.ProductId == f.ProductId && a.Status == true && ad.Status == true
                                                         group ad by new { a.AttributeId, a.AttributeName } into g
                                                         select new AttributeViewModel
                                                         {
                                                             AttributeId = g.Key.AttributeId,
                                                             AttributeName = g.Key.AttributeName,
                                                             AttributeDetails = g.Select(detail => new ProductAttributeDetailViewModel
                                                             {
                                                                 AttributeDetailId = detail.AttributeDetailId,
                                                                 AttributeDetailName = detail.AttributeDetailName
                                                             }).ToList()
                                                         }).ToList(),
                                       Categories = (from c in _unitOfWork.Db.Set<tblCategory>()
                                                     where c.ParentCategoryId == 0 // Get only root categories
                                                     select new CategoryViewModel
                                                     {
                                                         CategoryId = c.CategoryId,
                                                         CategoryName = c.CategoryName ?? "",
                                                         ParentCategoryId = c.ParentCategoryId ?? 0,
                                                         Status = c.Status ?? false,
                                                         ParentCategoryName = _unitOfWork.Db.Set<tblCategory>()
                                                             .Where(x => x.CategoryId == c.ParentCategoryId && c.ParentCategoryId == 0)
                                                             .Select(x => x.CategoryName)
                                                             .FirstOrDefault() ?? "",
                                                         ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                             .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                                             .Select(x => "/Areas/Admin/FormsDocuments/Category/" + x.DocumentId + "." + x.DocumentExtension)
                                                             .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                                                         DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                             .Where(x => x.TypeId == f.CategoryId.ToString() && x.DocumentType == "Category" && x.Remarks == "ProfilePicture")
                                                             .Select(x => x.DocumentId)
                                                             .FirstOrDefault(),
                                                     }).ToList()
                                   }).ToList();

            return expiredProducts;
        }
        public List<ProductViewModel> GetProductsByMasterCategory(int masterCategoryId)
        {
            var expiredProducts = (from product in _unitOfWork.Db.Set<tblProduct>()
                                   where product.Status == true && product.IsExpired == true
                                   join category in _unitOfWork.Db.Set<tblCategory>() on product.CategoryId equals category.CategoryId
                                   where category.ParentCategoryId == masterCategoryId
                                   select new ProductViewModel
                                   {
                                       ProductId = product.ProductId,
                                       ProductName = product.ProductName,
                                       ArticleNumber = product.ArticleNumber ?? "",
                                       UnitId = product.UnitId ?? 0,
                                       CostPrice = product.CostPrice ?? 0,
                                       SalePriceForWebsite = product.SalePriceForWebsite ?? 0,
                                       ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                           .Where(doc => doc.TypeId == product.ProductId.ToString() && doc.DocumentType == "Product" && doc.Remarks == "ProfilePicture")
                                           .Select(doc => "/Areas/Admin/FormsDocuments/Product/" + doc.DocumentId + "." + doc.DocumentExtension)
                                           .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png"
                                   }).ToList();

            return expiredProducts;
        }

        public List<ProductViewModel> GetFilteredProductsForExpired(bool? categoryCheckbox, int[] category,
                                                          bool? attributeCheckbox, int[] attribute,
                                                          bool? discountCheckbox, int[] discount,
                                                          decimal? minPrice, decimal? maxPrice,
                                                          string sortBy)
        {
            var query = from f in _unitOfWork.Db.Set<tblProduct>()
                        where f.Status == true && f.IsExpired == true
                        select f;

            if (categoryCheckbox == true && category != null && category.Length > 0)
            {
                query = query.Where(p => category.Contains(p.CategoryId ?? 0));
            }
            if (attributeCheckbox == true && attribute != null && attribute.Length > 0)
            {
                var productIdsWithAttributes = _unitOfWork.Db.Set<tblProductAttributeDetail>()
                    .Where(pad => attribute.Contains(pad.AttributeDetailId ?? 0))
                    .Select(pad => pad.ProductId)
                    .Distinct();

                query = query.Where(p => productIdsWithAttributes.Contains(p.ProductId));
            }


            if (minPrice.HasValue)
            {
                query = query.Where(p => p.SalePriceForWebsite >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.SalePriceForWebsite <= maxPrice.Value);
            }
            switch (sortBy)
            {
                case "date":
                    query = query.OrderByDescending(p => p.CreatedDate);
                    break;
                case "price":
                    query = query.OrderBy(p => p.SalePriceForWebsite);
                    break;
                case "price-desc":
                    query = query.OrderByDescending(p => p.SalePriceForWebsite);
                    break;
                default:
                    break;
            }

            var result = query.Select(f => new ProductViewModel
            {
                ProductId = f.ProductId,
                ProductName = f.ProductName,
                CategoryId = f.CategoryId ?? 0,
                CategoryName = _unitOfWork.Db.Set<tblCategory>()
                                .Where(x => x.CategoryId == f.CategoryId)
                                .Select(x => x.CategoryName)
                                .FirstOrDefault() ?? "",
                SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                CostPrice = f.CostPrice ?? 0,
                Status = f.Status,
                ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                .Select(x => x.DocumentId)
                                .FirstOrDefault(),
                AttributesList = (from pad in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                  join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on pad.AttributeDetailId equals ad.AttributeDetailId
                                  join a in _unitOfWork.Db.Set<tblAttribute>() on pad.AttributeId equals a.AttributeId
                                  where pad.ProductId == f.ProductId && a.Status == true && ad.Status == true
                                  group ad by new { a.AttributeId, a.AttributeName } into g
                                  select new AttributeViewModel
                                  {
                                      AttributeId = g.Key.AttributeId,
                                      AttributeName = g.Key.AttributeName,
                                      AttributeDetails = g.Select(detail => new ProductAttributeDetailViewModel
                                      {
                                          AttributeDetailId = detail.AttributeDetailId,
                                          AttributeDetailName = detail.AttributeDetailName
                                      }).ToList()
                                  }).ToList(),
            }).ToList();

            return result;
        }


        public List<ProductViewModel> GetFilteredProducts(bool? categoryCheckbox, int[] category,
                                                           bool? attributeCheckbox, int[] attribute,
                                                           bool? discountCheckbox, int[] discount,
                                                           decimal? minPrice, decimal? maxPrice,
                                                           string sortBy)
        {
            var query = from f in _unitOfWork.Db.Set<tblProduct>()
                        where f.Status == true && f.IsExpired == false
                        select f;

            if (categoryCheckbox == true && category != null && category.Length > 0)
            {
                query = query.Where(p => category.Contains(p.CategoryId ?? 0));
            }
            if (attributeCheckbox == true && attribute != null && attribute.Length > 0)
            {
                var productIdsWithAttributes = _unitOfWork.Db.Set<tblProductAttributeDetail>()
                    .Where(pad => attribute.Contains(pad.AttributeDetailId ?? 0))
                    .Select(pad => pad.ProductId)
                    .Distinct();

                query = query.Where(p => productIdsWithAttributes.Contains(p.ProductId));
            }


            if (minPrice.HasValue)
            {
                query = query.Where(p => p.SalePriceForWebsite >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.SalePriceForWebsite <= maxPrice.Value);
            }
            switch (sortBy)
            {
                case "date":
                    query = query.OrderByDescending(p => p.CreatedDate);
                    break;
                case "price":
                    query = query.OrderBy(p => p.SalePriceForWebsite);
                    break;
                case "price-desc":
                    query = query.OrderByDescending(p => p.SalePriceForWebsite);
                    break;
                default:
                    break;
            }

            var result = query.Select(f => new ProductViewModel
            {
                ProductId = f.ProductId,
                ProductName = f.ProductName,
                CategoryId = f.CategoryId ?? 0,
                CategoryName = _unitOfWork.Db.Set<tblCategory>()
                                .Where(x => x.CategoryId == f.CategoryId)
                                .Select(x => x.CategoryName)
                                .FirstOrDefault() ?? "",
                SalePriceForWebsite = f.SalePriceForWebsite ?? 0,
                CostPrice = f.CostPrice ?? 0,
                Status = f.Status,
                ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                .Select(x => x.DocumentId)
                                .FirstOrDefault(),
                AttributesList = (from pad in _unitOfWork.Db.Set<tblProductAttributeDetail>()
                                  join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on pad.AttributeDetailId equals ad.AttributeDetailId
                                  join a in _unitOfWork.Db.Set<tblAttribute>() on pad.AttributeId equals a.AttributeId
                                  where pad.ProductId == f.ProductId && a.Status == true && ad.Status == true
                                  group ad by new { a.AttributeId, a.AttributeName } into g
                                  select new AttributeViewModel
                                  {
                                      AttributeId = g.Key.AttributeId,
                                      AttributeName = g.Key.AttributeName,
                                      AttributeDetails = g.Select(detail => new ProductAttributeDetailViewModel
                                      {
                                          AttributeDetailId = detail.AttributeDetailId,
                                          AttributeDetailName = detail.AttributeDetailName
                                      }).ToList()
                                  }).ToList(),
            }).ToList();

            return result;
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