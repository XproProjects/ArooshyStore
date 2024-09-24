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
    public class AboutRepository : IAboutRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public AboutRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<AboutViewModel> GetAboutsListAndCount(string whereCondition, string start, string length, string sorting)
        {
            List<AboutViewModel> list = new List<AboutViewModel>();
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
                    string query = "SELECT Count(s.AboutId) as MyRowCount FROM tblAbout s where " + whereCondition + " ";
                    //Get List
                    //query += " select s.AboutId,isnull(s.Description,'') as Description,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblAbout s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";
                    query += " select s.AboutId,isnull(s.Description,'') as Description,isnull(s.Service1Icon,'') as Service1Icon,isnull(s.Service1Description,'') as Service1Description,isnull(s.Service1Name,'') as Service1Name,isnull(s.Service2Name,'') as Service2Name,isnull(s.Service2Icon,'') as Service2Icon,isnull(s.Service3Icon,'') as Service3Icon,isnull(s.Service2Description,'') as Service2Description,isnull(s.Service3Description,'') as Service3Description,isnull(s.Service3Name,'') as Service3Name,isnull(s.CreatedDate,'') as 'CreatedDate',(case when isnull(s.CreatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.CreatedBy) , 'Record Deleted')End) as 'CreatedBy',isnull(s.UpdatedDate,'') as 'UpdatedDate',(case when isnull(s.UpdatedBy,0) = 0 then '' else isnull((select isnull(i.FullName,'')  from tblUser u inner join tblInfo i on u.InfoId = i.InfoId where u.UserId = s.UpdatedBy) , 'Record Deleted')End) as 'UpdatedBy' from tblAbout s  where " + whereCondition + " " + sorting + " OFFSET " + offset + " ROWS  FETCH NEXT " + length + " ROWS ONLY ";

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
                                list.Add(new AboutViewModel()
                                {
                                    AboutId = Convert.ToInt32(reader["AboutId"]),
                                    Description = reader["Description"].ToString(),
                                    Service1Name = reader["Service1Name"].ToString(),
                                    Service2Name = reader["Service2Name"].ToString(),
                                    Service3Name = reader["Service3Name"].ToString(),
                                    Service1Description = reader["Service1Description"].ToString(),
                                    Service2Description = reader["Service2Description"].ToString(),
                                    Service3Description = reader["Service3Description"].ToString(),
                                    Service1Icon = reader["Service1Icon"].ToString(),
                                    Service2Icon = reader["Service2Icon"].ToString(),
                                    Service3Icon = reader["Service3Icon"].ToString(),

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
                                "AboutRepository", "GetAboutsListAndCount");
            }
            return list;
        }
        public AboutViewModel GetAboutById(int id)
        {
            AboutViewModel model = new AboutViewModel();
            if (id > 0)
            {
                model = (from f in _unitOfWork.Db.Set<tblAbout>()
                         where f.AboutId == id
                         select new AboutViewModel
                         {
                             AboutId = f.AboutId,
                             Description = f.Description,
                             Service1Name = f.Service1Name,
                             Service2Name = f.Service2Name,
                             Service3Name = f.Service3Name,
                             Service1Icon = f.Service1Icon,
                             Service2Icon = f.Service2Icon,
                             Service3Icon = f.Service3Icon,
                             Service1Description = f.Service1Description,
                             Service2Description = f.Service2Description,
                             Service3Description = f.Service3Description,

                         }).FirstOrDefault();
            }
            else
            {
                model = new AboutViewModel
                {
                    AboutId = 0,
                    Description = "",
              };
            }
            return model;
        }

        public StatusMessageViewModel InsertUpdateAbout(AboutViewModel model, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                string insertUpdateStatus = "";
                if (model.AboutId > 0)
                {
                    bool check = _unitOfWork.Db.Set<tblAbout>().Where(x => x.AboutId == model.AboutId).Any(x => x.Description.ToLower().Trim() == model.Description.ToLower().Trim());
                    if (!check)
                    {
                        bool check2 = _unitOfWork.Db.Set<tblAbout>().Any(x => x.Description.ToLower().Trim() == model.Description.ToLower().Trim());
                        if (check2)
                        {
                            response.Status = false;
                            response.Message = "About Name already exists.";
                            return response;
                        }
                    }
                    insertUpdateStatus = "Update";
                }
                else
                {
                    bool check2 = _unitOfWork.Db.Set<tblAbout>().Any(x => x.Description.ToLower().Trim() == model.Description.ToLower().Trim());
                    if (check2)
                    {
                        response.Status = false;
                        response.Message = "About Name already exists.";
                        return response;
                    }
                    insertUpdateStatus = "Save";
                }
                ResultViewModel result = InsertUpdateAboutDb(model, insertUpdateStatus, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "About Saved Successfully";
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
                                "AboutRepository", "InsertUpdateAbout");
            }
            return response;
        }
        private ResultViewModel InsertUpdateAboutDb(AboutViewModel st, string insertUpdateStatus, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateAbout", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@AboutId", SqlDbType.Int).Value = st.AboutId;
                        cmd.Parameters.Add("@Description", SqlDbType.NVarChar).Value = st.Description;
                        cmd.Parameters.Add("@Service1Name", SqlDbType.NVarChar).Value = st.Service1Name;
                        cmd.Parameters.Add("@Service2Name", SqlDbType.NVarChar).Value = st.Service2Name;
                        cmd.Parameters.Add("@Service3Name", SqlDbType.NVarChar).Value = st.Service3Name;
                        cmd.Parameters.Add("@Service1Icon", SqlDbType.NVarChar).Value = st.Service1Icon;
                        cmd.Parameters.Add("@Service2Icon", SqlDbType.NVarChar).Value = st.Service2Icon;
                        cmd.Parameters.Add("@Service3Icon", SqlDbType.NVarChar).Value = st.Service3Icon;
                        cmd.Parameters.Add("@Service1Description", SqlDbType.NVarChar).Value = st.Service1Description;
                        cmd.Parameters.Add("@Service2Description", SqlDbType.NVarChar).Value = st.Service2Description;
                        cmd.Parameters.Add("@Service3Description", SqlDbType.NVarChar).Value = st.Service3Description;
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
                                "AboutRepository", "InsertUpdateAboutDb");
            }
            return result;
        }
        public StatusMessageViewModel DeleteAbout(int id, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            AboutViewModel model = new AboutViewModel();
            model.AboutId = id;
            ResultViewModel result = InsertUpdateAboutDb(model, "Delete", loggedInUserId);
            if (result.Message == "Success")
            {
                response.Status = true;
                response.Message = "About Deleted Successfully";
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
        public AboutViewModel GetAboutUs()
        {
            var model = _unitOfWork.Db.Set<tblAbout>().Select(f => new AboutViewModel
            {
                AboutId = f.AboutId,
                Description = f.Description,
                Service1Name = f.Service1Name,
                Service2Name = f.Service2Name,
                Service3Name = f.Service3Name,
                Service1Icon = f.Service1Icon,
                Service2Icon = f.Service2Icon,
                Service3Icon = f.Service3Icon,
                Service1Description = f.Service1Description,
                Service2Description = f.Service2Description,
                Service3Description = f.Service3Description
            }).SingleOrDefault();


            return model;
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
