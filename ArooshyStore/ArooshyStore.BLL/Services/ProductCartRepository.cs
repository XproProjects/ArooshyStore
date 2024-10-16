﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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
        public List<ProductCartViewModel> GetCartItemsByUserId(int userId)
        {
           
                var cartItems = (from f in _unitOfWork.Db.Set<tblProductCart>()
                                     where f.UserId == userId
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

                                         //CategoryName = _unitOfWork.Db.Set<tblCategory>().Where(x => x.CategoryId == f.CategoryId).Select(x => x.CategoryName).FirstOrDefault() ?? "",
                                         ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                           .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                                           .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                                         DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == f.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                           .Select(x => x.DocumentId)
                                                           .FirstOrDefault(),
                                     }).ToList();
                return cartItems;

           
        }
        public StatusMessageViewModel InsertUpdateProductCart(ProductCartViewModel model,string AttributeDetailData)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                List<ProductCartAttributeDetailViewModel> list = JsonConvert.DeserializeObject<List<ProductCartAttributeDetailViewModel>>(AttributeDetailData);
                DataTable dtAttributes = new DataTable();
                dtAttributes.Columns.Add("Id");
                dtAttributes.Columns.Add("ProductId");
                dtAttributes.Columns.Add("CartId");
                dtAttributes.Columns.Add("AttributeDetailId");

                if (list.Count != 0)
                {
                    dtAttributes.Rows.Clear();
                    for (int i = 0; i < list.Count; i++)
                    {
                        dtAttributes.Rows.Add(new object[] { i + 1, list[i].ProductId, list[i].CartId, list[i].AttributeDetailId });
                    }
                }
                else
                {
                    dtAttributes.Rows.Add(new object[] { 0, 0, 0,0 });
                }
                if (model.CartId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblProductCart>().Any(x => x.CartId == model.CartId && x.ProductId == model.ProductId && x.UserId == model.UserId);

                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblProductCart>().Any(x => x.ProductId == model.ProductId && x.UserId == model.UserId);
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Product Already Exists In Your Cart.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblProductCart>().Any(x => x.ProductId == model.ProductId && x.UserId == model.UserId);
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Product Already Exists In Your Cart.";
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
