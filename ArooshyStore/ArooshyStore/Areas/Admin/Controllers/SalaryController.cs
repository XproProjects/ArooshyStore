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
    public class SalaryController : BaseController
    {
        private readonly ISalaryRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public SalaryController(ISalaryRepository repository, ICheckUserRoleRepository roles)
        {
            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;
            _roles = roles;
        }
        public ActionResult Index()
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                if (_roles.CheckModuleRoleId(User.UserId, "salary") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "salary");

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
        // GET: /Home/
        public ActionResult GetAllSalarys()
        {
            if (User != null)
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                        + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var userName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                string whereCondition = " ";
                string sorting = "";
                if (!(string.IsNullOrEmpty(sortColumn) && !(string.IsNullOrEmpty(sortColumnDir))))
                {
                    if (!string.IsNullOrEmpty(sortColumn))
                    {
                        sorting = " Order by " + sortColumn + " " + sortColumnDir + "";
                    }
                }
                else
                {
                    sorting = " Order by s.SalaryId asc";
                }
                if (!(string.IsNullOrEmpty(userName)))
                {
                    whereCondition += " LOWER(s.BasicSalary) like ('%" + userName.ToLower() + "%')";
                }
               
                else
                {
                    whereCondition += " LOWER(s.BasicSalary) like ('%%')";
                }
                List<SalaryViewModel> listsub = new List<SalaryViewModel>();
                if (_roles.CheckActionRoleId(User.UserId, "salary", "view") > 0)
                {
                    listsub = _repository.GetSalaryListAndCount(whereCondition, start, length, sorting);
                    if (listsub.Count > 0)
                    {
                        recordsTotal = listsub.Select(x => x.TotalRecords).FirstOrDefault();
                    }
                }
                var data = listsub.ToList();
                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = data },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        [HttpGet]
        public ActionResult InsertUpdateSalary(int id = 0)
        {
            if (User != null)
            {
                SalaryViewModel user = _repository.GetSalaryById(id);
                return PartialView(user);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateSalary(SalaryViewModel user)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateSalary(user, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }

        public ActionResult DeleteSalary(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                SalaryViewModel user = new SalaryViewModel();
                user.SalaryId = id;
                response = _repository.DeleteSalary(id, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }

       

    }
}
