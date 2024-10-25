using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ArooshyStore.App_Start;
using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.Models.ViewModels;
using AutoMapper;

namespace ArooshyStore.Areas.Admin.Controllers
{
    public class EmployeeAttendanceController : BaseController
    {
        private readonly IEmployeeAttendanceRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public EmployeeAttendanceController(IEmployeeAttendanceRepository repository, ICheckUserRoleRepository roles)
        {
            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;
            _roles = roles;
        }

        #region [--- Attendance ---]
        public ActionResult Index()
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                if (_roles.CheckModuleRoleId(User.UserId, "attendance") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "attendance");

                    return View(actionList);
                }
                else
                {
                    return RedirectToAction("accessdenied", "home");
                }
            }
            else
            {
                return RedirectToAction("login", "account");
            }

        }
        public ActionResult GetEmployeesForAttendance(DateTime attendanceDate)
        {
            List<EmployeeAttendanceViewModel> empList = _repository.GetEmployeesForAttendance(attendanceDate);
            var data = empList.ToList();
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult InsertUpdateEmployeeAttendance(string data)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateEmployeeAttendance(data, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }

       
        #endregion
        #region [--- Attendance Report ---]
        public ActionResult PrintAttendanceReport(DateTime id)
        {
            List<EmployeeAttendanceViewModel> listsub = new List<EmployeeAttendanceViewModel>();
            listsub = _repository.GetAttendanceDetail(id, User.UserId);

            DateFilterReportViewModel report2 = new DateFilterReportViewModel();
            report2.MonthDate = id;
            report2.FromUserType = "Home";
            report2.AttendancesList = listsub;
            return View(report2);
        }
        #endregion

    }
}
