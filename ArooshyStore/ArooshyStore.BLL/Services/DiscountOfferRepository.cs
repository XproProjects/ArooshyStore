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
    public class DiscountOfferRepository : IDiscountOfferRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public DiscountOfferRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<DiscountOfferViewModel> GetDiscountOffersListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<DiscountOfferViewModel> list = new List<DiscountOfferViewModel>();
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
                    string query = "SELECT Count(s.OfferId) as MyRowCount FROM tblDiscountOffer s left join tblCategory c on s.CategoryId = c.CategoryId left join tblProduct p on s.ProductId = p.ProductId LEFT JOIN tblDocument dc ON s.OfferId = dc.TypeId AND dc.DocumentType = 'DiscountOffer' AND dc.Remarks = 'ProfilePicture' where " + whereCondition + " ";
                    //Get List
                    //query += " select s.OfferId,isnull(s.DiscountName,'') as DiscountName,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblDiscountOffer s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += " select s.OfferId,isnull(s.DiscountName,'') as DiscountName,(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',(case when ISNULL(dc.DocumentId , 0) = 0 then  '/Areas/Admin/Content/noimage.png' else  '/Areas/Admin/FormsDocuments/DiscountOffer/' + CAST(ISNULL(dc.DocumentId , 0) AS VARCHAR) + '.' + ISNULL(dc.DocumentExtension, '') end ) as ImagePath,isnull(s.DiscPercent,0) as DiscPercent,isnull(s.ExpiredOn,'') as 'ExpiredOn',isnull(s.SelectType,'') as 'SelectType',isnull(c.CategoryName,'') as 'CategoryName',isnull(p.ProductName,'') as 'ProductName',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy'  from tblDiscountOffer s left join tblCategory c on s.CategoryId = c.CategoryId left join tblProduct p on s.ProductId = p.ProductId LEFT JOIN tblDocument dc ON s.OfferId = dc.TypeId AND dc.DocumentType = 'DiscountOffer' AND dc.Remarks = 'ProfilePicture'  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";

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
                                list.Add(new DiscountOfferViewModel()
                                {
                                    OfferId = Convert.ToInt32(reader["OfferId"]),
                                    DiscountName = reader["DiscountName"].ToString(),
                                    DiscPercent = Convert.ToDecimal(reader["DiscPercent"].ToString()),
                                    ExpiredOn = Convert.ToDateTime(reader["ExpiredOn"].ToString()),
                                    SelectType = reader["SelectType"].ToString(),
                                    CategoryName = reader["CategoryName"].ToString(),
                                    ProductName = reader["ProductName"].ToString(),
                                    StatusString = reader["StatusString"].ToString(),
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
                                "DiscountOfferRepository", "GetDiscountOffersListAndCount");
            }
            return list;
        }
        public DiscountOfferViewModel GetDiscountOfferById(int id)
        {
            DiscountOfferViewModel model = new DiscountOfferViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblDiscountOffer>()
                         where f.OfferId == id
                         select new DiscountOfferViewModel
                         {
                             OfferId = f.OfferId,
                             DiscountName = f.DiscountName,
                             ProductId = f.ProductId ?? 0,
                             ProductName = _unitOfWork.Db.Set<tblProduct>().Where(x => x.ProductId == f.ProductId).Select(x => x.ProductName).FirstOrDefault() ?? "",
                             CategoryId = f.CategoryId ?? 0,
                             CategoryName = _unitOfWork.Db.Set<tblCategory>().Where(x => x.CategoryId == f.CategoryId).Select(x => x.CategoryName).FirstOrDefault() ?? "",
                             SelectType = f.SelectType ,
                             DiscPercent = f.DiscPercent ?? 0,
                             ExpiredOn = f.ExpiredOn ?? DateTime.Now,
                             Status = f.Status ?? false,
                             ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                          .Where(x => x.TypeId == f.OfferId.ToString() && x.DocumentType == "DiscountOffer" && x.Remarks == "ProfilePicture")
                                          .Select(x => "/Areas/Admin/FormsDocuments/DiscountOffer/" + x.DocumentId + "." + x.DocumentExtension)
                                          .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                             DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                          .Where(x => x.TypeId == f.OfferId.ToString() && x.DocumentType == "DiscountOffer" && x.Remarks == "ProfilePicture")
                                          .Select(x => x.DocumentId)
                                          .FirstOrDefault(),
                         }).FirstOrDefault();
            }
            else
            {
                model = new DiscountOfferViewModel
                {
                    OfferId = 0,
                    DiscountName = "",
                    CategoryId = 0,
                    CategoryName ="",
                    ProductId = 0,
                    ProductName = "",
                    SelectType = "",
                    DiscPercent = 0,
                    ExpiredOn = DateTime.Now.AddDays(7),
                    Status = false,
                    ImagePath = "/Areas/Admin/Content/noimage.png",
                    DocumentId = 0,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateDiscountOffer(DiscountOfferViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.OfferId > 0)
                {
                    //bool check = _unitOfWork.Db.Set<tblDiscountOffer>().Where(x => x.OfferId == model.OfferId).Any(x => x.DiscountName.ToLower().Trim() == model.DiscountName.ToLower().Trim());
                    //if (!check)
                    //{
                    //    bool check2 = _unitOfWork.Db.Set<tblDiscountOffer>().Any(x => x.DiscountName.ToLower().Trim() == model.DiscountName.ToLower().Trim());
                    //    if (check2)
                    //    {
                    //        response.Status = false;
                    //        response.Message = "Discount Offer already exists.";
                    //        return response;
                    //    }
                    //}
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
                    //bool check2 = _unitOfWork.Db.Set<tblDiscountOffer>().Any(x => x.DiscountName.ToLower().Trim() == model.DiscountName.ToLower().Trim());
                    //if (check2)
                    //{
                    //    response.Status = false;
                    //    response.Message = "Discount Offer already exists.";
                    //    return response;
                    //}
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateDiscountOfferDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Discount Offer Saved Successfully";
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
                                "DiscountOfferRepository", "InsertUpdateDiscountOffer");
            }
            return response;
        }
        private ResultViewModel InsertUpdateDiscountOfferDb(DiscountOfferViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateDiscountOffer", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@OfferId", SqlDbType.Int).Value = st.OfferId;
                        cmd.Parameters.Add("@DiscountName", SqlDbType.NVarChar).Value = st.DiscountName;
                        cmd.Parameters.Add("@SelectType", SqlDbType.NVarChar).Value = st.SelectType;
                        cmd.Parameters.Add("@CategoryId", SqlDbType.Int).Value = st.CategoryId;
                        cmd.Parameters.Add("@ProductId", SqlDbType.Int).Value = st.ProductId;
                        cmd.Parameters.Add("@DiscPercent", SqlDbType.Decimal).Value = st.DiscPercent;
                        cmd.Parameters.Add("@ExpiredOn", SqlDbType.DateTime).Value = st.ExpiredOn;
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
                                "DiscountOfferRepository", "InsertUpdateDiscountOfferDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteDiscountOffer(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            DiscountOfferViewModel model = new DiscountOfferViewModel();
            model.OfferId = id;
            ResultViewModel result = InsertUpdateDiscountOfferDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Discount Offer Deleted Successfully";
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
