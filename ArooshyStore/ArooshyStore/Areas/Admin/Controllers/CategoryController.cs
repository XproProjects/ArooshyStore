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
    public class CategoryController : BaseController
    {
        private readonly ICategoryRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public CategoryController(ICategoryRepository repository, ICheckUserRoleRepository roles)
        {
            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;
            _roles = roles;
        }
        public ActionResult Index(string from = "master")
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                //testing git

                if (_roles.CheckModuleRoleId(User.UserId, "product category") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "product category");

                    ViewBag.From = from;
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
        public ActionResult GetAllCategories()
        {
            if (User != null)
            {
                var From = Request.Form.GetValues("From").FirstOrDefault();
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                        + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var categoryName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                var parentCategoryName = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                string whereCondition = " ";
                if (From.ToString().ToLower() == "master")
                {
                    whereCondition = " isnull(s.ParentCategoryId,0) = 0 ";
                }
                else
                {
                    whereCondition = " isnull(s.ParentCategoryId,0) != 0  ";
                }
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
                    sorting = " Order by s.CategoryId asc";
                }
                if (!(string.IsNullOrEmpty(categoryName)))
                {
                    whereCondition += " and LOWER(s.CategoryName) like ('%" + categoryName.ToLower() + "%') ";
                }
                else if (!(string.IsNullOrEmpty(parentCategoryName)))
                {
                    whereCondition += " and LOWER(cc.CategoryName) like ('%" + parentCategoryName.ToLower() + "%') ";
                }
                else
                {
                    whereCondition += " and LOWER(s.CategoryName) like ('%%')";
                }
                List<CategoryViewModel> listsub = new List<CategoryViewModel>();
                if (_roles.CheckActionRoleId(User.UserId, "product category", "view") > 0)
                {
                    listsub = _repository.GetCategoriesListAndCount(whereCondition, start, length, sorting);
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
        public ActionResult InsertUpdateCategory(int id = 0, string from = "master")
        {
            if (User != null)
            {
                CategoryViewModel user = _repository.GetCategoryById(id);
                ViewBag.From = from;
                return PartialView(user);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateCategory(CategoryViewModel user)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateCategory(user, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }

        public ActionResult DeleteCategory(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                CategoryViewModel user = new CategoryViewModel();
                user.CategoryId = id;
                response = _repository.DeleteCategory(id, User.UserId);
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