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
    //[ChildActionOnly]
    public class ModuleController : BaseController
    {
        private readonly IModuleRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public ModuleController(IModuleRepository repository, ICheckUserRoleRepository roles)
        {
            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;
            _roles = roles;
        }
        
        public ActionResult Index()
        {
            if (User != null)
            {
               return View();
            }
            else
            {
                return RedirectToAction("login", "account");
            }

        }
        // GET: /Home/
        public ActionResult GetAllModules()
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
                    sorting = " Order by s.ModuleId asc";
                }
                if (!(string.IsNullOrEmpty(userName)))
                {
                    whereCondition += " LOWER(s.ModuleName) like ('%" + userName.ToLower() + "%')";
                }
                else
                {
                    whereCondition += " LOWER(s.ModuleName) like ('%%')";
                }
                List<ModuleViewModel> listsub = new List<ModuleViewModel>();
                ////Check if user has access of this action or not
                //if (role.CheckActionRoleId(Module.ModuleId, "user type", "view") > 0)
                //{
                listsub = _repository.GetModulesListAndCount(whereCondition, start, length, sorting);
                if (listsub.Count > 0)
                {
                    recordsTotal = listsub.Select(x => x.TotalRecords).FirstOrDefault();
                }
                //}

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
        public ActionResult InsertUpdateModule(int id)
        {
            if (User != null)
            {
                ModuleViewModel user = _repository.GetModuleById(id);
                return PartialView(user);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateModule(ModuleViewModel user)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateModule(user, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }

        public ActionResult DeleteModule(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                ModuleViewModel user = new ModuleViewModel();
                user.ModuleId = id;
                response = _repository.DeleteModule(id, User.UserId);
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