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
    public class ProductReviewRepository : IProductReviewRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductReviewRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public StatusMessageViewModel InsertUpdateProductReview(ProductReviewViewModel model)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.ReviewId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblProductReview>().Where(x => x.ReviewId == model.ReviewId).Any(x => x.ReviewByName.ToLower().Trim() == model.ReviewByName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblProductReview>().Any(x => x.ReviewByName.ToLower().Trim() == model.ReviewByName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Product Review Name already exists.";
                            return response;
                        }
                    }
                    
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblProductReview>().Any(x => x.ReviewByName.ToLower().Trim() == model.ReviewByName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Product Review Name already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateProductReviewDb(model, insertUpdateStatus);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Product Review Saved Successfully";
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
                error.InsertError(0, ex.Message.ToString(), "Web Application", "CustomerSupplierRepository", "InsertUpdateCustomerSupplier"); // Logging without user ID
            
        }
            return response;
        }
        private ResultViewModel InsertUpdateProductReviewDb(ProductReviewViewModel st, string insertUpdateStatus)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateProductReview", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@ReviewId", SqlDbType.Int).Value = st.ReviewId;
                        cmd.Parameters.Add("@ReviewByCustomerId", SqlDbType.Int).Value = st.ReviewByCustomerId;
                        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = st.ProductId;
                        cmd.Parameters.Add("@Rating", SqlDbType.Int).Value = st.Rating;
                        cmd.Parameters.Add("@ReviewByName", SqlDbType.NVarChar).Value = st.ReviewByName;
                        cmd.Parameters.Add("@ReviewByEmail", SqlDbType.NVarChar).Value = st.ReviewByEmail;
                        cmd.Parameters.Add("@ReviewDetail", SqlDbType.NVarChar).Value = st.ReviewDetail;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = st.ReviewId;
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
                error.InsertError(0, ex.Message.ToString(), "Web Application", "CustomerSupplierRepository", "InsertUpdateCustomerSupplier"); // Logging without user ID
            
        }
            return result;
        }
        public List<ProductReviewViewModel> GetProductReviews(int productId)
        {
            return (from r in _unitOfWork.Db.Set<tblProductReview>()
                    where r.ProductId == productId
                    orderby r.ReviewId descending
                    select new ProductReviewViewModel
                    {
                        ReviewId = r.ReviewId,
                        ProductId = r.ProductId,
                        ReviewByName = r.ReviewByName,
                        ReviewByEmail = r.ReviewByEmail,
                        Rating = r.Rating,
                        ReviewDetail = r.ReviewDetail,
                        CreatedDate = r.CreatedDate,
                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == r.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                           .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                                           .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                           .Where(x => x.TypeId == r.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                           .Select(x => x.DocumentId)
                                                           .FirstOrDefault(),
                    }).Take(3).ToList();
        }
        public List<ProductReviewViewModel> GetAllReviews()
        {
            return (from r in _unitOfWork.Db.Set<tblProductReview>()
                    select new ProductReviewViewModel
                    {
                        ReviewId = r.ReviewId,
                        ProductId = r.ProductId,
                        ReviewByName = r.ReviewByName,
                        ReviewByEmail = r.ReviewByEmail,
                        Rating = r.Rating,
                        ReviewDetail = r.ReviewDetail,
                        CreatedDate = r.CreatedDate,
                        ProductName = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == r.ProductId).Select(x => x.ProductName).FirstOrDefault() ?? "",

                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                                   .Where(x => x.TypeId == r.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                    .Select(x => "/Areas/Admin/FormsDocuments/Product/" + x.DocumentId + "." + x.DocumentExtension)
                                                   .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                                    .Where(x => x.TypeId == r.ProductId.ToString() && x.DocumentType == "Product" && x.Remarks == "ProfilePicture")
                                                   .Select(x => x.DocumentId)
                                                   .FirstOrDefault(),
                    }).ToList();
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
