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
using Newtonsoft.Json;

namespace ArooshyStore.BLL.Services
{
    public class EmployeeAttendanceRepository : IEmployeeAttendanceRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public EmployeeAttendanceRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public List<EmployeeAttendanceViewModel> GetEmployeesForAttendance(DateTime attendanceDate)
        {
            List<EmployeeAttendanceViewModel> empList = new List<EmployeeAttendanceViewModel>();
            try
            {
                if (attendanceDate == null)
                {
                    attendanceDate = DateTime.Now.Date;
                }
                attendanceDate = attendanceDate.Date;

                // Fetch employees and their attendance data
                empList = (from c in _unitOfWork.Db.Set<tblEmployee>()
                           orderby c.EmployeeId ascending
                           select new EmployeeAttendanceViewModel
                           {
                               EmployeeId = c.EmployeeId,
                               EmployeeName = c.EmployeeName,
                               // Fetching the attendance data for the employee on the given date
                               Attendance = _unitOfWork.Db.Set<tblEmployeeAttendance>()
                                   .Where(x => x.AttendanceDate == attendanceDate && x.EmployeeId == c.EmployeeId)
                                   .Select(x => x.Attendance)
                                   .FirstOrDefault() ?? "P", // Default is "Present" (P)
                               AttendanceId = _unitOfWork.Db.Set<tblEmployeeAttendance>()
                                   .Where(x => x.AttendanceDate == attendanceDate && x.EmployeeId == c.EmployeeId)
                                   .Select(x => x.AttendanceId)
                                   .FirstOrDefault(),
                               CheckInTime = _unitOfWork.Db.Set<tblEmployeeAttendance>()
                                   .Where(x => x.AttendanceDate == attendanceDate && x.EmployeeId == c.EmployeeId)
                                   .Select(x => x.CheckInTime)
                                   .FirstOrDefault(),
                               CheckOutTime = _unitOfWork.Db.Set<tblEmployeeAttendance>()
                                   .Where(x => x.AttendanceDate == attendanceDate && x.EmployeeId == c.EmployeeId)
                                   .Select(x => x.CheckOutTime)
                                   .FirstOrDefault(),
                               AttendanceDate = attendanceDate,
                           }).ToList();

                DateTime endDate = attendanceDate;
                DateTime startDate = new DateTime(endDate.Year, endDate.Month, 1);
                List<EmployeeAttendanceViewModel> DatesList = new List<EmployeeAttendanceViewModel>();

                // Generating a list of dates from the start of the month to the given date
                for (DateTime dt = startDate; dt < endDate; dt = dt.AddDays(1))
                {
                    EmployeeAttendanceViewModel dateObj = new EmployeeAttendanceViewModel
                    {
                        AttendanceDate = dt
                    };
                    DatesList.Add(dateObj);
                }

                // Processing attendance for each employee
                foreach (EmployeeAttendanceViewModel e in empList)
                {
                    if (e.CheckInTime != null)
                    {
                        DateTime? dt = new DateTime(2050, 01, 01);
                        dt = dt + e.CheckInTime;
                        e.InTimeDateTime = dt;
                    }
                    if (e.CheckOutTime != null)
                    {
                        DateTime? dt = new DateTime(2050, 01, 01);
                        dt = dt + e.CheckOutTime;
                        e.OutTimeDateTime = dt;
                    }

                    // Adding the attendance list for each date
                    e.DateList = new List<EmployeeAttendanceViewModel>();
                    foreach (EmployeeAttendanceViewModel d in DatesList)
                    {
                        var attendanceRecord = _unitOfWork.Db.Set<tblEmployeeAttendance>()
                            .Where(x => x.AttendanceDate == d.AttendanceDate && x.EmployeeId == e.EmployeeId)
                            .Select(x => x.Attendance)
                            .FirstOrDefault() ?? "";

                        e.DateList.Add(new EmployeeAttendanceViewModel
                        {
                            AttendanceDate = d.AttendanceDate,
                            Attendance = attendanceRecord
                        });
                    }
                    e.DateList = e.DateList.OrderByDescending(x => x.AttendanceDate).ToList();
                }
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                  "EmployeeAttendanceRepository", "GetEmployeesForAttendance");
            }

            return empList;
        }
        public StatusMessageViewModel InsertUpdateEmployeeAttendance(string data, int loggedInUserId)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            try
            {
                List<EmployeeAttendanceViewModel> attendance = JsonConvert.DeserializeObject<List<EmployeeAttendanceViewModel>>(data);
                DataTable dt = new DataTable();
                dt.Columns.Add("Id");
                dt.Columns.Add("AttendanceId");
                dt.Columns.Add("Attendance");
                dt.Columns.Add("CheckInTime");
                dt.Columns.Add("CheckOutTime");
                dt.Columns.Add("Minutes");
                dt.Columns.Add("EmployeeId");
                DateTime? attendanceDate = null;
                if (attendance.Count != 0)
                {
                    attendanceDate = attendance[0].AttendanceDate;
                    for (int i = 0; i < attendance.Count; i++)
                    {
                        if (attendance[i].Attendance != ""
                            && attendance[i].Attendance != "P"
                            && attendance[i].Attendance != "H"
                            && attendance[i].Attendance != "A")
                       
                        dt.Rows.Add(new object[] { i + 1, attendance[i].AttendanceId, attendance[i].Attendance,
                                                attendance[i].CheckInTime, attendance[i].CheckOutTime,
                                                attendance[i].Minutes, attendance[i].EmployeeId });
                    }
                }
                else
                {
                    attendanceDate = DateTime.Now;
                    dt.Rows.Add(new object[] { 0, 0, "", null, null, 0, 0, 0 });
                }
                ResultViewModel result = InsertUpdateEmployeeAttendanceDb(attendanceDate, dt, loggedInUserId);
                if (result.Message == "Success")
                {
                    response.Status = true;
                    response.Message = "Attendance Saved Successfully.";
                }
                else
                {
                    response.Status = false;
                    response.Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "EmployeeAttendanceRepository", "GetAttendanceListAndCount");
            }
            return response;
        }
        private ResultViewModel InsertUpdateEmployeeAttendanceDb(DateTime? attendanceDate, DataTable dt, int loggedInUserId)
        {
            ResultViewModel result = new ResultViewModel();
            try
            {
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {

                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("spInsertUpdateEmployeeAttendance", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@AttendanceDate", SqlDbType.Date).Value = attendanceDate;
                        cmd.Parameters.Add("@ActionByUserId", SqlDbType.Int).Value = loggedInUserId;
                        cmd.Parameters.Add("@dtAttendance", SqlDbType.Structured).Value = dt;
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
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "EmployeeAttendanceRepository", "GetAttendanceListAndCount");
            }
            return result;
        }
        public List<EmployeeAttendanceViewModel> GetAttendanceDetail(DateTime attendateDate, int loggedInUserId)
        {
            List<EmployeeAttendanceViewModel> list = new List<EmployeeAttendanceViewModel>();
            try
            {
                string whereCondition = " ";
                string connection = System.Configuration.ConfigurationManager.ConnectionStrings["ADO"].ConnectionString;
                using (SqlConnection con = new SqlConnection(connection))
                {
                    con.Open();
                    //Get Count
                    string query = " select s.EmployeeId  where " + whereCondition + " ";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Clear();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                list.Add(new EmployeeAttendanceViewModel()
                                {
                                    EmployeeId = Convert.ToInt32(reader["EmployeeId"]),
                                    EmployeeName = reader["EmployeeName"].ToString(),
                                });
                            }
                        }
                        cmd.Dispose();
                    }
                    con.Close();
                    con.Dispose();
                    SqlConnection.ClearPool(con);
                }

                #region Attendance Detail
                DateTime endDate = new DateTime(attendateDate.Year, attendateDate.Month, DateTime.DaysInMonth(attendateDate.Year, attendateDate.Month));
                DateTime startDate = new DateTime(endDate.Year, endDate.Month, 1);
                List<EmployeeAttendanceViewModel> DatesList = new List<EmployeeAttendanceViewModel>();
                for (DateTime dt = startDate; dt <= endDate; dt = dt.AddDays(1))
                {
                    DateTime date = dt;
                    EmployeeAttendanceViewModel dateObj = new EmployeeAttendanceViewModel();
                    dateObj.AttendanceDate = date;
                    DatesList.Add(dateObj);
                }
                foreach (EmployeeAttendanceViewModel e in list)
                {
                    e.DateList = new List<EmployeeAttendanceViewModel>();
                    EmployeeAttendanceViewModel empp;
                    foreach (EmployeeAttendanceViewModel d in DatesList)
                    {
                        empp = new EmployeeAttendanceViewModel();
                        empp.AttendanceDate = d.AttendanceDate;
                        empp.Attendance = _unitOfWork.Db.Set<tblEmployeeAttendance>().Where(x => x.AttendanceDate == d.AttendanceDate && x.EmployeeId == e.EmployeeId).Select(x => x.Attendance).FirstOrDefault() ?? "";
                        e.DateList.Add(empp);
                    }
                    e.DateList = e.DateList.OrderByDescending(x => x.AttendanceDate).ToList();
                }
                #endregion
            }
            catch (Exception ex)
            {
                ErrorHandler error = ErrorHandler.GetInstance;
                error.InsertError(0, ex.Message.ToString(), "Web Application",
                                "EmployeeAttendanceRepository", "GetAttendanceListAndCount");
            }
            return list;
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
