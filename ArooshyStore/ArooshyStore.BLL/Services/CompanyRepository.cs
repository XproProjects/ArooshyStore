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
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<CompanyViewModel> GetCompanysListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<CompanyViewModel> list = new List<CompanyViewModel>();
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
                    string query = "SELECT Count(s.CompanyId) as MyRowCount FROM tblCompany s where " + whereCondition + " ";
                    //Get List
                    //query += " select s.CompanyId,isnull(s.CompanyName,'') as CompanyName,isnull(s.Contact1,'') as Contact1,isnull(s.Contact2,'') as Contact2,isnull(s.Email,'') as Email,isnull(s.Address,'') as Address,isnull(s.FacebookId,'') as FacebookId,isnull(s.InstagramId,'') as InstagramId,isnull(s.LinkedInId,'') as LinkedInId,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblCompany s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    //query += " select s.CompanyId,isnull(s.CompanyName,'') as CompanyName,isnull(s.Contact1,'') as Contact1,isnull((select '/Areas/Admin/FormsDocuments/Company/' + cast(isnull(dc.DocumentId,0) as varchar) + '.' +  isnull(dc.DocumentExtension,'')  from tblDocument dc where dc.TypeId = CAST(s.CompanyId as varchar)  and dc.DocumentType = 'Company' and dc.Remarks = 'ProfilePicture' ),'/Areas/Admin/Content/noimage.png') as 'ImagePath',isnull(s.Contact2,'') as Contact2,isnull(s.Email,'') as Email,isnull(s.FacebookId,'') as FacebookId,isnull(s.InstagramId,'') as InstagramId,isnull(s.LinkedInId,'') as LinkedInId,isnull(s.Address,'') as Address,isnull(s.Longitude,'') as Longitude,isnull(s.Latitude,'') as Latitude,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblCompany s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += "SELECT s.CompanyId, " + " " + "ISNULL(s.CompanyName, '') AS CompanyName, " + "ISNULL(s.Contact1, '') AS Contact1, " + "ISNULL(s.Contact2, '') AS Contact2, " + "ISNULL(s.Email, '') AS Email, " + "ISNULL(s.Address, '') AS Address, " + "ISNULL(s.FacebookId, '') AS FacebookId, " + "ISNULL(s.InstagramId, '') AS InstagramId, " + "ISNULL(s.LinkedInId, '') AS LinkedInId, " + "(CASE WHEN ISNULL(dc.DocumentId, 0) = 0 " + "THEN '/Areas/Admin/Content/noimage.png' " + "ELSE '/Areas/Admin/FormsDocuments/Company/' + CAST(ISNULL(dc.DocumentId, 0) AS VARCHAR) + '.' + ISNULL(dc.DocumentExtension, '') END) AS ImagePath, " +
                            "ISNULL(CONVERT(VARCHAR, s.CreatedDate, 120), '') AS CreatedDate, " +
                            "(CASE WHEN ISNULL(s.CreatedBy, 0) = 0 THEN '' " +
                            "ELSE ISNULL((SELECT ISNULL(i.FullName, '') FROM tblUser u INNER JOIN tblInfo i ON u.InfoId = i.InfoId WHERE u.UserId = s.CreatedBy), 'Record Deleted') END) AS CreatedBy, " +
                            "ISNULL(CONVERT(VARCHAR, s.UpdatedDate, 120), '') AS UpdatedDate, " +
                            "(CASE WHEN ISNULL(s.UpdatedBy, 0) = 0 THEN '' " +
                            "ELSE ISNULL((SELECT ISNULL(i.FullName, '') FROM tblUser u INNER JOIN tblInfo i ON u.InfoId = i.InfoId WHERE u.UserId = s.UpdatedBy), 'Record Deleted') END) AS UpdatedBy " +
                            "FROM tblCompany s " +
                            "LEFT JOIN tblDocument dc ON s.CompanyId = dc.TypeId AND dc.DocumentType = 'Company' AND dc.Remarks = 'ProfilePicture' " +
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

                            // this advances to the next resultset 
                            reader.NextResult();

                            while (reader.Read())
                            {
                                list.Add(new CompanyViewModel()
                                {
                                    CompanyId = Convert.ToInt32(reader["CompanyId"]),
                                    CompanyName = reader["CompanyName"].ToString(),
                                    Contact1 = reader["Contact1"].ToString(),
                                    Contact2 = reader["Contact2"].ToString(),
                                    Email = reader["Email"].ToString(),
                                    FacebookId = reader["FacebookId"].ToString(),
                                    InstagramId = reader["InstagramId"].ToString(),
                                    LinkedInId = reader["LinkedInId"].ToString(),
                                    Address = reader["Address"].ToString(),
                                    //Longitude = reader["Longitude"].ToString(),
                                    //Latitude = reader["Latitude"].ToString(),
                                    ImagePath = reader["ImagePath"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"].ToString()),
                                    CreatedByString = reader["CreatedBy"].ToString(),
                                    UpdatedDate = Convert.ToDateTime(reader["UpdatedDate"].ToString()),
                                    UpdatedByString = reader["UpdatedBy"].ToString(),
                                    TotalRecords = totalCount
                                }) ;
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
                                "CompanyRepository", "GetCompanysListAndCount");
            }
            return list;
        }
        public CompanyViewModel GetCompanyById(int id)
        {
            CompanyViewModel model = new CompanyViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblCompany>()
                         where f.CompanyId == id
                         select new CompanyViewModel
                         {
                             CompanyId = f.CompanyId,
                             CompanyName = f.CompanyName,
                             Contact1 = f.Contact1,
                             Contact2 = f.Contact2,
                             Email = f.Email,
                             FacebookId = f.FacebookId,
                             InstagramId = f.InstagramId,
                             LinkedInId = f.LinkedInId,
                             Address  = f.Address,
                             Longitude = f.Longitude,
                             Latitude = f.Latitude,
                             ImagePath = _unitOfWork.Db.Set<tblDocument>()
                            .Where(x => x.TypeId == f.CompanyId.ToString() && x.DocumentType == "Company" && x.Remarks == "ProfilePicture")
                            .Select(x => "/Areas/Admin/FormsDocuments/Company/" + x.DocumentId + "." + x.DocumentExtension)
                            .FirstOrDefault() ?? "/Areas/Admin/Content/noimage.png",
                             DocumentId = _unitOfWork.Db.Set<tblDocument>()
                            .Where(x => x.TypeId == f.CompanyId.ToString() && x.DocumentType == "Company" && x.Remarks == "ProfilePicture")
                            .Select(x => x.DocumentId)
                            .FirstOrDefault(),

                         }).FirstOrDefault();
            }
            else
            {
                model = new CompanyViewModel
                {
                    CompanyId = 0,
                    CompanyName = "",
                    Contact1 = "",
                    Contact2 = "",
                    Email = "",
                    FacebookId = "",
                    InstagramId = "",
                    LinkedInId = "",
                    Address = "",
                    Longitude = "",
                    Latitude = "",
                    ImagePath = "/Areas/Admin/Content/noimage.png",
                    DocumentId = 0,
                };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateCompany(CompanyViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.CompanyId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblCompany>().Where(x => x.CompanyId == model.CompanyId).Any(x => x.CompanyName.ToLower().Trim() == model.CompanyName.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblCompany>().Any(x => x.CompanyName.ToLower().Trim() == model.CompanyName.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "Company Name already exists.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblCompany>().Any(x => x.CompanyName.ToLower().Trim() == model.CompanyName.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "Company Name already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateCompanyDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Company Saved Successfully";
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
                                "CompanyRepository", "InsertUpdateCompany");
            }
            return response;
        }
        private ResultViewModel InsertUpdateCompanyDb(CompanyViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateCompany", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@CompanyId", SqlDbType.Int).Value = st.CompanyId;
                        cmd.Parameters.Add("@CompanyName", SqlDbType.NVarChar).Value = st.CompanyName;
                        cmd.Parameters.Add("@Contact1", SqlDbType.NVarChar).Value = st.Contact1;
                        cmd.Parameters.Add("@Contact2", SqlDbType.NVarChar).Value = st.Contact2;
                        cmd.Parameters.Add("@Email", SqlDbType.NVarChar).Value = st.Email;
                        cmd.Parameters.Add("@FacebookId", SqlDbType.NVarChar).Value = st.FacebookId;
                        cmd.Parameters.Add("@InstagramId", SqlDbType.NVarChar).Value = st.InstagramId;
                        cmd.Parameters.Add("@LinkedInId", SqlDbType.NVarChar).Value = st.LinkedInId;
                        cmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = st.Address;
                        cmd.Parameters.Add("@Longitude", SqlDbType.NVarChar).Value = st.Longitude;
                        cmd.Parameters.Add("@Latitude", SqlDbType.NVarChar).Value = st.Latitude;
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
                                "CompanyRepository", "InsertUpdateCompanyDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteCompany(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            CompanyViewModel model = new CompanyViewModel();
            model.CompanyId = id;
            ResultViewModel result = InsertUpdateCompanyDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "Company Deleted Successfully";
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
