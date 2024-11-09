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
    public class ProductWishlistRepository : IProductWishlistRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductWishlistRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<ProductWishlistViewModel> GetWishlistItemsByUserId(int userId)
        {
            var wishlistItems = (from f in _unitOfWork.Db.Set<tblProductWishlist>()
                                 join p in _unitOfWork.Db.Set<tblProduct>() on f.ProductId equals p.ProductId 
                                 where f.UserId == userId
                                 select new ProductWishlistViewModel
                                 {
                                     WishlistId = f.WishlistId,
                                     UserId = f.UserId,
                                     ProductId = f.ProductId,
                                     ProductName = p.ProductName, 
                                     SalePrice = p.SalePrice ?? 0,     
                                     CostPrice = p.CostPrice ?? 0,
                                     SalePriceForWebsite = p.SalePriceForWebsite ?? 0,
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

            return wishlistItems;
        }

        public StatusMessageViewModel InsertUpdateProductWishlist(ProductWishlistViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.WishlistId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblProductWishlist>().Where(x => x.WishlistId == model.WishlistId).Any(x => x.ProductId== model.ProductId);
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblProductWishlist>().Any(x => x.ProductId == model.ProductId);
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Product already exists.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblProductWishlist>().Any(x => x.ProductId == model.ProductId);
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Product already exists in Wishlist.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateWishlistDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Product  Added Successfully";
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
                                "ProductWishlistRepository", "InsertUpdateWishlist");
            }
            return response;
        }
        private ResultViewModel InsertUpdateWishlistDb(ProductWishlistViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateWishlist", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@WishlistId", SqlDbType.Int).Value = st.WishlistId;
                        cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = st.UserId;
                        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = st.ProductId;
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
                                "ProductWishlistRepository", "InsertUpdateWishlistDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteProductWishlist(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            ProductWishlistViewModel model = new ProductWishlistViewModel();
            model.WishlistId = id;
            ResultViewModel result = InsertUpdateWishlistDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Wishlist Product Deleted Successfully";
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
