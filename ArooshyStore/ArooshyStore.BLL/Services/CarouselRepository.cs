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
    public class CarouselRepository : ICarouselRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CarouselRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<CarouselViewModel> GetCarouselsListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<CarouselViewModel> list = new List<CarouselViewModel>();
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
                    string query = "SELECT Count(s.CarouselId) as MyRowCount FROM tblCarousel s where " + whereCondition + " ";
                    //Get List
                   // query += " select s.CarouselId,isnull(s.Line1,'') as Line1,isnull(s.Line2,'') as Line2,isnull(s.Line3,'') as Line3,(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblCarousel s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    // query += " select s.CarouselId,isnull(s.Line1,'') as Line1,isnull(s.Line2,'') as Line2,isnull(s.Line3,'') as Line3,(case when isnull(s.Status,0) = 0 then 'In-Active' else 'Active' end) as 'StatusString',isnull((select '/Areas/Admin/FormsDocuments/Carousel/' + cast(isnull(dc.DocumentId,0) as varchar) + '.' +  isnull(dc.DocumentExtension,'')  from tblDocument dc where dc.TypeId = CAST(s.CarouselId as varchar)  and dc.DocumentType = 'Carousel' and dc.Remarks = 'ProfilePicture' ),'/Areas/Admin/Content/noimage.png') as 'ImagePath',isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblCarousel s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += "SELECT s.CarouselId, " + " " + "ISNULL(s.Line1, '') AS Line1, " + "ISNULL(s.Line2, '') AS Line2, " + "ISNULL(s.Line3, '') AS Line3, " + "(CASE WHEN ISNULL(dc.DocumentId, 0) = 0 " + "THEN '/Areas/Admin/Content/noimage.png' " + "ELSE '/Areas/Admin/FormsDocuments/Carousel/' + CAST(ISNULL(dc.DocumentId, 0) AS VARCHAR) + '.' + ISNULL(dc.DocumentExtension, '') END) AS ImagePath, " +
                              "(CASE WHEN ISNULL(s.Status, 0) = 0 THEN 'In-Active' ELSE 'Active' END) AS StatusString, " +
                              "ISNULL(CONVERT(VARCHAR, s.CreatedDate, 120), '') AS CreatedDate, " +
                              "(CASE WHEN ISNULL(s.CreatedBy, 0) = 0 THEN '' " +
                              "ELSE ISNULL((SELECT ISNULL(i.FullName, '') FROM tblUser u INNER JOIN tblInfo i ON u.InfoId = i.InfoId WHERE u.UserId = s.CreatedBy), 'Record Deleted') END) AS CreatedBy, " +
                              "ISNULL(CONVERT(VARCHAR, s.UpdatedDate, 120), '') AS UpdatedDate, " +
                              "(CASE WHEN ISNULL(s.UpdatedBy, 0) = 0 THEN '' " +
                              "ELSE ISNULL((SELECT ISNULL(i.FullName, '') FROM tblUser u INNER JOIN tblInfo i ON u.InfoId = i.InfoId WHERE u.UserId = s.UpdatedBy), 'Record Deleted') END) AS UpdatedBy " +
                              "FROM tblCarousel s " +
                              "LEFT JOIN tblDocument dc ON s.CarouselId = dc.TypeId AND dc.DocumentType = 'Carousel' AND dc.Remarks = 'ProfilePicture' " +
                              "WHERE " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS FETCH NEXT " + length + " ROWS ONLY";


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
                                list.Add(new CarouselViewModel()
                                {
                                    CarouselId = Convert.ToInt32(reader["CarouselId"]),
                                    Line1 = reader["Line1"].ToString(),
                                    Line2 = reader["Line2"].ToString(),
                                    Line3 = reader["Line3"].ToString(),
                                    StatusString = reader["StatusString"].ToString(),
                                   // ImagePath = reader["ImagePath"].ToString(),
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
                                "CarouselRepository", "GetCarouselsListAndCount");
            }
            return list;
        }
        public CarouselViewModel GetCarouselById(int id)
        {
            CarouselViewModel model = new CarouselViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblCarousel>()
                         where f.CarouselId == id
                         select new CarouselViewModel
                         {
                             CarouselId = f.CarouselId,
                             Line1 = f.Line1 ?? "",
                             Line2 = f.Line2 ?? "",
                             Line3 = f.Line3 ?? "",
                             ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                          .Where(x => x.TypeId == f.CarouselId.ToString() && x.DocumentType == "Carousel" && x.Remarks == "ProfilePicture")
                                          .Select(x => "/Areas/Admin/FormsDocuments/Carousel/" + x.DocumentId + "." + x.DocumentExtension)
                                          .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                             DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                          .Where(x => x.TypeId == f.CarouselId.ToString() && x.DocumentType == "Carousel" && x.Remarks == "ProfilePicture")
                                          .Select(x => x.DocumentId)
                                          .FirstOrDefault(),
                             Status = f.Status ?? false,

                         }).FirstOrDefault();
            }
            else
            {
                model = new CarouselViewModel
                {
                    CarouselId = 0,
                    Line1 = "",
                    Line2 = "",
                    Line3 = "",
                    ImagePath = "/Areas/Admin/Content/noimage.png",
                    DocumentId = 0,
                    Status = false,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateCarousel(CarouselViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.CarouselId > 0)
                {
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
                    model.Status = true;
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateCarouselDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Carousel Saved Successfully";
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
                                "CarouselRepository", "InsertUpdateCarousel");
            }
            return response;
        }
        private ResultViewModel InsertUpdateCarouselDb(CarouselViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateCarousel", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CarouselId", SqlDbType.Int).Value = st.CarouselId;
                        cmd.Parameters.Add("@Line1", SqlDbType.NVarChar).Value = st.Line1;
                        cmd.Parameters.Add("@Line2", SqlDbType.NVarChar).Value = st.Line2;
                        cmd.Parameters.Add("@Line3", SqlDbType.NVarChar).Value = st.Line3;
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
                                "CarouselRepository", "InsertUpdateCarouselDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteCarousel(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            CarouselViewModel model = new CarouselViewModel();
            model.CarouselId = id;
            ResultViewModel result = InsertUpdateCarouselDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Carousel Deleted Successfully";
                response.Id = result.Id;
            }
            else if (result.Message == "Fail")
            {
                response.Status = false;
                if (result.Id == -1)
                {
                    response.Message = "Error! Please delete all data from Technician Accounts against this Designation first.";
                }
                else
                {
                    response.Message = "Error! Please delete all data against this Designation first.";
                }
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
        public List<CarouselViewModel> GetAllCarousels()
        {
            return (from f in _unitOfWork.Db.Set<tblCarousel>()
                    where f.Status == true
                    select new CarouselViewModel
                    {
                        CarouselId = f.CarouselId,
                        Line1 = f.Line1,
                        Line2 = f.Line2,
                        Line3 = f.Line3,
                        ImagePath = _unitOfWork.Db.Set<tblDocument>()
                                          .Where(x => x.TypeId == f.CarouselId.ToString() && x.DocumentType == "Carousel" && x.Remarks == "ProfilePicture")
                                          .Select(x => "/Areas/Admin/FormsDocuments/Carousel/" + x.DocumentId + "." + x.DocumentExtension)
                                          .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                        DocumentId = _unitOfWork.Db.Set<tblDocument>()
                                          .Where(x => x.TypeId == f.CarouselId.ToString() && x.DocumentType == "Carousel" && x.Remarks == "ProfilePicture")
                                          .Select(x => x.DocumentId)
                                          .FirstOrDefault(),
                        Status = f.Status ?? false,

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
