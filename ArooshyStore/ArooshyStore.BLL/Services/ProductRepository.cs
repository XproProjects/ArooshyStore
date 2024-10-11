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
using Newtonsoft.Json;

namespace ArooshyStore.BLL.Services
{
    public class ProductRepository : IProductRepository
    {
        private readonly IUnitOfWork _unitOfWork;
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

                    string query = "SELECT Count(s.ProductId) as MyRowCount FROM tblProduct s left join tblCategory c on s.CategoryId = c.CategoryId where " + whereCondition + " ";
                    query += " select s.ProductId,isnull(s.ProductName,'') as ProductName,isnull(s.ProductDescription,'') as ProductDescription,isnull(s.Barcode,'') as Barcode,isnull(c.CategoryName,'') as CategoryName,isnull(s.ProductNameUrdu,'') as 'ProductNameUrdu',isnull(s.SalePrice,0) as SalePrice,isnull(s.CostPrice,0) as 'CostPrice',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',(case when isnull(s.IsFeatured,0) = 0 then 'No' else 'Yes' end) as 'IsFeaturedString',isnull((select '/Areas/Admin/FormsDocuments/Product/' + cast(isnull(dc.DocumentId,0) as varchar) + '.' +  isnull(dc.DocumentExtension,'')  from tblDocument dc where dc.TypeId = CAST(s.ProductId as varchar)  and dc.DocumentType = 'Product' and dc.Remarks = 'ProfilePicture' ),'/Areas/Admin/Content/noimage.png') as 'ImagePath',(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'Status',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblProduct s left join tblCategory c on s.CategoryId = c.CategoryId  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
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
                                    Barcode = reader["Barcode"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    CostPrice = Convert.ToDecimal(reader["CostPrice"].ToString()),
                                    SalePrice = Convert.ToDecimal(reader["SalePrice"].ToString()),
                                    StatusString = reader["StatusString"].ToString(),
                                    IsFeaturedString = reader["IsFeaturedString"].ToString(),
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
                         where f.ProductId == id
                         select new ProductViewModel
                         {
                             ProductId = f.ProductId,
                             ProductName = f.ProductName,
                             ProductNameUrdu = f.ProductNameUrdu,
                             ProductDescription = f.ProductDescription,
                             Barcode = f.Barcode,
                             UnitId = f.UnitId ?? 0,
                             UnitName = _unitOfWork.Db.Set<tblUnit>().Where(x => x.UnitId == f.UnitId).Select(x => x.UnitName).FirstOrDefault() ?? "",
                             CategoryId = f.CategoryId ?? 0,
                             CategoryName = _unitOfWork.Db.Set<tblCategory>().Where(x => x.CategoryId == f.CategoryId).Select(x => x.CategoryName).FirstOrDefault() ?? "",
                             DeliveryInfoId = f.DeliveryInfoId ?? 0,
                             DeliveryInfoName = _unitOfWork.Db.Set<tblDeliveryInfo>().Where(x => x.DeliveryInfoId == f.DeliveryInfoId).Select(x => x.DeliveryInfoName).FirstOrDefault() ?? "",
                             SalePrice = f.SalePrice ?? 0,
                             CostPrice = f.CostPrice ?? 0,
                             Status = f.Status,
                             IsFeatured = f.IsFeatured ?? false,
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
                    Barcode = "",
                    UnitId = 0,
                    CategoryId = 0,
                    CategoryName = "",
                    UnitName = "",
                    SalePrice = 0,
                    CostPrice = 0,
                    Status = false,
                    IsFeatured = false,
                    ImagePath = "/Areas/Admin/Content/noimage.png",
                    DocumentId = 0,
                };
            }
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
                                dtAttributes.Rows.Add(new object[] { k, list[i].AttributeId, j.AttributeDetailId, true });
                            }
                        }
                    }
                }
                else
                {
                    dtAttributes.Rows.Add(new object[] { 0, 0, 0, false });
                }
                DataTable dtTags = new DataTable();
                dtTags.Columns.Add("Id");
                dtTags.Columns.Add("TagId");
                dtTags.Columns.Add("ProductId");
                if (tagsList.Count != 0)
                {
                    dtTags.Rows.Clear();
                    for (int i = 0; i < tagsList.Count; i++)
                    {
                        dtTags.Rows.Add(new object[] { i + 1, tagsList[i].TagId, tagsList[i].ProductId });
                    }
                }
                else
                {
                    dtTags.Rows.Add(new object[] { 0, 0, 0 });
                }
                if (model.ProductId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == model.ProductId).Any(x => x.ProductName.ToLower().Trim() == model.ProductName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblProduct>().Any(x => x.ProductName.ToLower().Trim() == model.ProductName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Product Name already exists.";
                            return response;
                        }
                    }
                    model.Status = true ? model.StatusString == "Yes" : false;
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblProduct>().Any(x => x.ProductName.ToLower().Trim() == model.ProductName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Product Name already exists.";
                        return response;
                    }
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                model.IsFeatured = true ? model.IsFeaturedString == "Yes" : false;
                // Call method to interact with DB
                ResultViewModel result = InsertUpdateProductDb(model, insertUpdateStatus, dtAttributes, dtTags, loggedInUserId);
                if (result.Message == "Success")
                {
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
                        cmd.Parameters.Add("@Barcode", SqlDbType.NVarChar).Value = model.Barcode;
                        cmd.Parameters.Add("@DeliveryInfoId", SqlDbType.Int).Value = model.DeliveryInfoId;
                        cmd.Parameters.Add("@UnitId", SqlDbType.Int).Value = model.UnitId;
                        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = model.CategoryId;
                        cmd.Parameters.Add("@CostPrice", SqlDbType.Decimal).Value = model.CostPrice;
                        cmd.Parameters.Add("@SalePrice", SqlDbType.Decimal).Value = model.SalePrice;
                        cmd.Parameters.Add("@Status", SqlDbType.Bit).Value = model.Status;
                        cmd.Parameters.Add("@IsFeatured", SqlDbType.Bit).Value = model.IsFeatured;
                        cmd.Parameters.Add("@dtProductAttributeDetailType", SqlDbType.Structured).Value = dtAttributes;
                        cmd.Parameters.Add("@dtTagsForProductsType", SqlDbType.Structured).Value = dtTags;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = loggedInUserId;
                        cmd.Parameters.Add("@InsertUpdateStatus", SqlDbType.NVarChar, 50).Value = insertUpdateStatus;
                        cmd.Parameters.Add("@CheckReturn", SqlDbType.NVarChar, 300).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@CheckReturn2", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.ExecuteNonQuery();
                        result.Message = cmd.Parameters["@CheckReturn"].Value.ToString();
                        result.Id = Convert.ToInt32(cmd.Parameters["@CheckReturn2"].Value);
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
            dtAttributes.Columns.Add("AttributeId");
            dtAttributes.Columns.Add("AttributeDetailId");
            dtAttributes.Columns.Add("Status");
            DataTable dtTags = new DataTable();
            dtTags.Columns.Add("Id");
            dtTags.Columns.Add("TagId");
            dtTags.Columns.Add("ProductId");


            dtAttributes.Rows.Add(new object[] { 0, 0, 0, false });
            dtTags.Rows.Add(new object[] { 0, 0, 0 });

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
                var product = _unitOfWork.Db.Set<tblProduct>()
                    .Where(f => f.Barcode == barcode)
                    .Select(f => new
                    {
                        f.ProductId,
                        f.ProductName,
                        f.ProductNameUrdu,
                        f.Barcode,
                        f.UnitId,
                        f.CategoryId,
                        f.SalePrice,
                        f.CostPrice,
                        f.Status,
                        Category = _unitOfWork.Db.Set<tblCategory>()
                            .Where(c => c.CategoryId == f.CategoryId)
                            .Select(c => new
                            {
                                c.CategoryName,
                                c.ParentCategoryId
                            })
                            .FirstOrDefault()
                    })
                    .FirstOrDefault();

                if (product != null)
                {
                    var masterCategory = "";
                    var childCategory = "";

                    if (product.Category.ParentCategoryId == 0)
                    {
                        masterCategory = product.Category.CategoryName; 
                    }
                    else
                    {
                        childCategory = product.Category.CategoryName; 
                                                                       
                        masterCategory = _unitOfWork.Db.Set<tblCategory>()
                            .Where(c => c.CategoryId == product.Category.ParentCategoryId)
                            .Select(c => c.CategoryName)
                            .FirstOrDefault() ?? "";
                    }

                    model = new ProductViewModel
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductNameUrdu = product.ProductNameUrdu,
                        Barcode = product.Barcode,
                        UnitId = product.UnitId ?? 0,
                        CategoryId = product.CategoryId ?? 0,
                        CategoryName = product.Category.CategoryName ?? "",
                        MasterCategoryName = masterCategory ?? "",
                        ChildCategoryName = childCategory ?? "",
                        UnitName = _unitOfWork.Db.Set<tblUnit>()
                                    .Where(x => x.UnitId == product.UnitId)
                                    .Select(x => x.UnitName)
                                    .FirstOrDefault() ?? "",
                        SalePrice = product.SalePrice ?? 0,
                        CostPrice = product.CostPrice ?? 0,
                        Status = product.Status
                    };
                }
            }
            else
            {
                model = new ProductViewModel
                {
                    ProductId = 0,
                    ProductName = "",
                    ProductNameUrdu = "",
                    Barcode = "",
                    UnitId = 0,
                    CategoryId = 0,
                    CategoryName = "",
                    MasterCategoryName = "",
                    ChildCategoryName = "",
                    UnitName = "",
                    SalePrice = 0,
                    CostPrice = 0,
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
                    where f.Status == true && f.IsFeatured == true
                    select new ProductViewModel
                    {
                        ProductId = f.ProductId,
                        ProductName = f.ProductName,
                        ProductNameUrdu = f.ProductNameUrdu,
                        Barcode = f.Barcode,
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
                        SalePrice = f.SalePrice ?? 0,
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
                             SalePrice = f.SalePrice ?? 0,
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
                        Barcode = f.Barcode,
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
                        SalePrice = f.SalePrice ?? 0,
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

                    }).ToList();
        }
        public List<ProductViewModel> GetNewArrivalProducts()
        {
            return (from f in _unitOfWork.Db.Set<tblProduct>()
                    where f.Status == true
                    orderby f.ProductId descending
                    select new ProductViewModel
                    {
                        ProductId = f.ProductId,
                        ProductName = f.ProductName,
                        Barcode = f.Barcode,
                        CategoryId = f.CategoryId ?? 0,
                        CategoryName = _unitOfWork.Db.Set<tblCategory>().Where(x => x.CategoryId == f.CategoryId).Select(x => x.CategoryName).FirstOrDefault() ?? "",
                        UnitName = _unitOfWork.Db.Set<tblUnit>()
                                        .Where(x => x.UnitId == f.UnitId)
                                        .Select(x => x.UnitName)
                                        .FirstOrDefault() ?? "",
                        SalePrice = f.SalePrice ?? 0,
                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                        .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                        .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                        .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png"
                    }).ToList();
        }
        public List<ProductViewModel> GetProductsForShop()
        {
            var query = from f in _unitOfWork.Db.Set<tblProduct>()
                        where f.Status == true
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
                SalePrice = f.SalePrice ?? 0,
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
                                     DiscPercent = d.DiscPercent ?? 0,
                                     Status = d.Status ?? false,
                                     SelectType = d.SelectType ?? "",
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
        public List<ProductViewModel> GetFilteredProducts(bool? categoryCheckbox, int[] category,
                                                           bool? attributeCheckbox, int[] attribute,
                                                           bool? discountCheckbox, int[] discount,
                                                           decimal? minPrice, decimal? maxPrice,
                                                           string sortBy)
        {
            var query = from f in _unitOfWork.Db.Set<tblProduct>()
                        where f.Status == true
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
                query = query.Where(p => p.SalePrice >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.SalePrice <= maxPrice.Value);
            }
            switch (sortBy)
            {
                case "date":
                    query = query.OrderByDescending(p => p.CreatedDate); 
                    break;
                case "price":
                    query = query.OrderBy(p => p.SalePrice);
                    break;
                case "price-desc":
                    query = query.OrderByDescending(p => p.SalePrice);
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
                SalePrice = f.SalePrice ?? 0,
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