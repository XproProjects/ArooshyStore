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
    public class UserController : BaseController
    {
        private readonly IUserRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public UserController(IUserRepository repository, ICheckUserRoleRepository roles)
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
                if (_roles.CheckModuleRoleId(User.UserId, "user") > 0)
                {
                    List<ModuleViewModel> AllActionList = new List<ModuleViewModel>();

                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "user");
                    AllActionList.AddRange(actionList);

                    return View(AllActionList);
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
        public ActionResult GetAllUsers()
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
                var email = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
                var userType = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
                var cnic = Request.Form.GetValues("columns[3][search][value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                //string whereCondition = " isnull(t.TypeName,'') != 'Super Admin' ";
                string whereCondition = " isnull(t.TypeName,'') != 'Super Adminn' ";
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
                    sorting = " Order by s.UserId asc";
                }
                if (!(string.IsNullOrEmpty(userName)))
                {
                    whereCondition += " and LOWER(c.FullName) like ('%" + userName.ToLower() + "%')";
                }
                else if (!(string.IsNullOrEmpty(email)))
                {
                    whereCondition += " and LOWER(c.Email) like ('%" + email.ToLower() + "%')";
                }
                else if (!(string.IsNullOrEmpty(userType)))
                {
                    whereCondition += " and LOWER(t.TypeName) like ('%" + userType.ToLower() + "%')";
                }
                else if (!(string.IsNullOrEmpty(cnic)))
                {
                    whereCondition += " and LOWER(c.Cnic) like ('%" + cnic.ToLower() + "%')";
                }
                else
                {
                    whereCondition += " and LOWER(c.FullName) like ('%%')";
                }
                List<UserViewModel> listsub = new List<UserViewModel>();
                ////Check if user has access of this action or not
                if (_roles.CheckActionRoleId(User.UserId, "user", "view") > 0)
                {
                    listsub = _repository.GetUsersListAndCount(whereCondition, start, length, sorting);
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
        public ActionResult InsertUpdateUser(int id)
        {
            if (User != null)
            {
                UserViewModel user = _repository.GetUserById(id);
                return PartialView(user);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateUser(UserViewModel user)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateUser(user, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }

        public ActionResult DeleteUser(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                UserViewModel user = new UserViewModel();
                user.UserId = id;
                response = _repository.DeleteUser(id, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }
        #region User Permissions
        [HttpGet]
        public ActionResult AssignModule(int userId)
        {
            if (User != null)
            {
                List<ModuleViewModel> module = _repository.GetModulesForAssign(userId);
                return PartialView(module);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateAssignModule(int userId, string assignType, string data)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateAssignModule(userId, assignType, data, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }
        #endregion
    }
}