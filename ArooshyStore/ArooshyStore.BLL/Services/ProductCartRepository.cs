using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;
using Newtonsoft.Json;

namespace ArooshyStore.BLL.Services
{
    public class ProductCartRepository : IProductCartRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductCartRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<ProductCartViewModel> GetCartItemCountByCookieName(int? UserId,string CookieName)
        {
            var cartItems = (from f in _unitOfWork.Db.Set<tblProductCart>()
                             where (UserId.HasValue && f.UserId == UserId) || f.CookieName == CookieName
                             select new ProductCartViewModel
                             {
                                 CartId = f.CartId,
                                 UserId = f.UserId,
                                 ProductId = f.ProductId,
                                 Quantity = f.Quantity,
                                 DiscountId = f.DiscountId,
                                 CookieName = f.CookieName,
                                 ActualSalePrice = f.ActualSalePrice ?? 0,
                                 DiscountAmount = f.DiscountAmount ?? 0,
                                 GivenSalePrice = f.GivenSalePrice ?? 0,

                                 ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                             .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                             .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                             .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                                 DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                             .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                             .Select(x => x.DocumentId)
                                             .FirstOrDefault(),
                                 AttributesList = (from pad in _unitOfWork.Db.Set<tblProductCartAttributeDetail>()
                                                   join ad in _unitOfWork.Db.Set<tblAttributeDetail>() on pad.AttributeDetailId equals ad.AttributeDetailId
                                                   join a in _unitOfWork.Db.Set<tblAttribute>() on ad.AttributeId equals a.AttributeId
                                                   where pad.CartId == f.CartId && a.Status == true && ad.Status == true
                                                   select new AttributeViewModel
                                                   {
                                                       AttributeId = a.AttributeId,
                                                       AttributeName = a.AttributeName,
                                                       AttributeDetails = new List<ProductAttributeDetailViewModel>
                                                       {
                                                   new ProductAttributeDetailViewModel
                                                   {
                                                       AttributeDetailId = ad.AttributeDetailId,
                                                       AttributeDetailName = ad.AttributeDetailName
                                                       }
                                                    }
                                                }).ToList(),
                               
        }).ToList();
           
            return cartItems;
        }
        public List<ProductCartViewModel> GetLatestCartItemsByCookieName(string userIdOrCookieName)
        {
            var cartItems = (from f in _unitOfWork.Db.Set<tblProductCart>()
                             where f.UserId.ToString() == userIdOrCookieName
                             orderby f.CartId descending
                             select new ProductCartViewModel
                             {
                                 CartId = f.CartId,
                                 UserId = f.UserId,
                                 ProductId = f.ProductId,
                                 Quantity = f.Quantity,
                                 DiscountId = f.DiscountId,
                                 CookieName = f.CookieName,
                                 ActualSalePrice = f.ActualSalePrice ?? 0,
                                 DiscountAmount = f.DiscountAmount ?? 0,
                                 GivenSalePrice = f.GivenSalePrice ?? 0,
                                 ProductName = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == f.ProductId).Select(x => x.ProductName).FirstOrDefault() ?? "",
                                 ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                             .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                             .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                             .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                                 DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                             .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                             .Select(x => x.DocumentId)
                                             .FirstOrDefault(),
                             }).Take(4).ToList();

            if (cartItems.Count == 0)
            {
                cartItems = (from f in _unitOfWork.Db.Set<tblProductCart>()
                             where f.CookieName == userIdOrCookieName
                             orderby f.CartId descending
                             select new ProductCartViewModel
                             {
                                 CartId = f.CartId,
                                 UserId = f.UserId,
                                 ProductId = f.ProductId,
                                 Quantity = f.Quantity,
                                 DiscountId = f.DiscountId,
                                 CookieName = f.CookieName,
                                 ActualSalePrice = f.ActualSalePrice ?? 0,
                                 DiscountAmount = f.DiscountAmount ?? 0,
                                 GivenSalePrice = f.GivenSalePrice ?? 0,
                                 ProductName = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == f.ProductId).Select(x => x.ProductName).FirstOrDefault() ?? "",
                                 ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                             .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                             .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                             .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                                 DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                             .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                             .Select(x => x.DocumentId)
                                             .FirstOrDefault(),
                             }).Take(3).ToList();
            }

            return cartItems;
        }
        public List<ProductCartViewModel> GetLatestCheckOutSidebarByCookieName(string userIdOrCookieName)
        {
            var cartItems = (from f in _unitOfWork.Db.Set<tblProductCart>()
                             where f.UserId.ToString() == userIdOrCookieName || f.CookieName == userIdOrCookieName
                             orderby f.CartId descending
                             select new ProductCartViewModel
                             {
                                 CartId = f.CartId,
                                 UserId = f.UserId,
                                 ProductId = f.ProductId,
                                 Quantity = f.Quantity,
                                 DiscountId = f.DiscountId,
                                 CookieName = f.CookieName,
                                 ActualSalePrice = f.ActualSalePrice ?? 0,
                                 DiscountAmount = f.DiscountAmount ?? 0,
                                 GivenSalePrice = f.GivenSalePrice ?? 0,
                                 ProductName = _unitOfWork.Db.Set<tblProduct>()
                                                .Where(x => x.ProductId == f.ProductId)
                                                .Select(x => x.ProductName)
                                                .FirstOrDefault() ?? "",
                                 ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                                .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png"
                             }).Take(4).ToList();

            return cartItems;
        }

        public StatusMessageViewModel InsertUpdateProductCart(ProductCartViewModel model, string AttributeDetailData, string cookieName)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                List<ProductCartAttributeDetailViewModel> list = JsonConvert.DeserializeObject<List<ProductCartAttributeDetailViewModel>>(AttributeDetailData);

                // Check if there is at least one AttributeDetailId
                if (list == null || list.Count == 0 || list.All(x => x.AttributeDetailId <= 0))
                {
                    response.Status = false;
                    response.Message = "Please select at least one Attribute Detail.";
                    return response;
                }

                DataTable dtAttributes = new DataTable();
                dtAttributes.Columns.Add("Id");
                dtAttributes.Columns.Add("ProductId");
                dtAttributes.Columns.Add("CartId");
                dtAttributes.Columns.Add("AttributeDetailId");

                dtAttributes.Rows.Clear();
                for (int i = 0; i < list.Count; i++)
                {
                    dtAttributes.Rows.Add(new object[] { i + 1, list[i].ProductId, list[i].CartId, list[i].AttributeDetailId });
                }

                if (model.CartId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblProductCart>()
                                .Where(x => x.ProductId == model.ProductId)
                                .Any(x => x.CookieName == model.CookieName);

                    if (!check)
                    {
                        bool productExists = _unitOfWork.Db.Set<tblProductCart>()
                           .Any(x => x.ProductId == model.ProductId && x.CookieName == cookieName);

                        if (productExists)
                        {
                            response.Status = false;
                            response.Message = "Product Already Exist In Your Cart.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblProductCart>()
                               .Any(x => x.ProductId == model.ProductId && x.CookieName == cookieName);

                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Product Already Exist In Your Cart.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }


                ResultViewModel result = InsertUpdateCartDb(model, insertUpdateStatus, dtAttributes);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Product Added Successfully";
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
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                  "ProductCartRepository", "InsertUpdateCart");
            }
            return response;
        }
        private ResultViewModel InsertUpdateCartDb(ProductCartViewModel st,  string insertUpdateStatus,DataTable dtAttributes)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateCart", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CartId", SqlDbType.Int).Value = st.CartId;
                        cmd.Parameters.Add("@CookieName", SqlDbType.NVarChar).Value = st.CookieName;
                        cmd.Parameters.Add("@Quantity", SqlDbType.Int).Value = st.Quantity;
                        cmd.Parameters.Add("@DiscountId", SqlDbType.Int).Value = st.DiscountId;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = st.UserId;
                        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = st.ProductId;
                        cmd.Parameters.Add("@ActualSalePrice", SqlDbType.Decimal).Value = st.ActualSalePrice;
                        cmd.Parameters.Add("@DiscountAmount", SqlDbType.Decimal).Value = st.DiscountAmount;
                        cmd.Parameters.Add("@GivenSalePrice", SqlDbType.Decimal).Value = st.GivenSalePrice;
                        cmd.Parameters.Add("@dtProductCartAttributeDetailType", SqlDbType.Structured).Value = dtAttributes;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = st.CartId;
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
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "ProductCartRepository", "InsertUpdateCartDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteProductCart(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            ProductCartViewModel model = new ProductCartViewModel();
            model.CartId = id;
            DataTable dtAttributes = new DataTable();
            dtAttributes.Columns.Add("Id");
            dtAttributes.Columns.Add("ProductId");
            dtAttributes.Columns.Add("CartId");
            dtAttributes.Columns.Add("AttributeDetailId");
            dtAttributes.Rows.Add(new object[] { 0, 0, 0,0 });

            ResultViewModel result = InsertUpdateCartDb(model, "Delete", dtAttributes);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Cart Product Deleted Successfully";
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

        public int GetCartItemCount(string cookieName)
        {
            return _unitOfWork.Db.Set<tblProductCart>().Count(x => x.CookieName == cookieName);
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
