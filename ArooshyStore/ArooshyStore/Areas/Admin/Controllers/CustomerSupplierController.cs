using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Windows.Forms;
using ArooshyStore.App_Start;
using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using AutoMapper;

namespace ArooshyStore.Areas.Admin.Controllers
{
    public class CustomerSupplierController : BaseController
    {
        private readonly ICustomerSupplierRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public CustomerSupplierController(ICustomerSupplierRepository repository, ICheckUserRoleRepository roles)
        {
            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;
            _roles = roles;
        }
        public ActionResult Index(string type = "customer")
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                if (_roles.CheckModuleRoleId(User.UserId, type.ToLower()) > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, type.ToLower());

                    ViewBag.Type = type;
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
        public ActionResult GetAllCustomerSupplier()
        {
            if (User != null)
            {
                var Type = Request.Form.GetValues("Type").FirstOrDefault();
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var userName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                string whereCondition = "";
                string sorting = "";
                if (Type.ToLower() == "customer")
                {
                    whereCondition = " s.CustomerSupplierType = 'Customer' and lower(s.Email) != 'cashcustomer@yahoo.com' ";
                }
                else
                {
                    whereCondition = " s.CustomerSupplierType = 'Supplier' ";
                }
                if (!(string.IsNullOrEmpty(sortColumn) && !(string.IsNullOrEmpty(sortColumnDir))))
                {
                    if (!string.IsNullOrEmpty(sortColumn))
                    {
                        sorting = " Order by " + sortColumn + " " + sortColumnDir + "";
                    }
                }
                else
                {
                    sorting = " Order by s.CustomerSupplierId asc";
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    whereCondition += " AND LOWER(s.CustomerSupplierName) like ('%" + userName.ToLower() + "%')";
                }
                else
                {
                    whereCondition += " AND LOWER(s.CustomerSupplierName) like ('%%')";
                }

                List<CustomerSupplierViewModel> listsub = new List<CustomerSupplierViewModel>();
                if (_roles.CheckActionRoleId(User.UserId, Type.ToLower(), "view") > 0)
                {
                    listsub = _repository.GetCustomerSuppliersListAndCount(whereCondition, start, length, sorting);
                    if (listsub.Count > 0)
                    {
                        recordsTotal = listsub.Select(x => x.TotalRecords).FirstOrDefault();
                    }
                }

                return Json(new { draw = draw, recordsFiltered = recordsTotal, recordsTotal = recordsTotal, data = listsub },
                    JsonRequestBehavior.AllowGet);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }


        [HttpGet]
        public ActionResult InsertUpdateCustomerSupplier(int id = 0, string type = "customer")
        {
            if (User != null)
            {
                CustomerSupplierViewModel user = _repository.GetCustomerSupplierById(id);
                ViewBag.Type = type;
               
                return PartialView(user);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateCustomerSupplier(CustomerSupplierViewModel user)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateCustomerSupplier(user, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }

        public ActionResult DeleteCustomerSupplier(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                CustomerSupplierViewModel user = new CustomerSupplierViewModel();
                user.CustomerSupplierId = id;
                response = _repository.DeleteCustomerSupplier(id, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }
        [HttpPost]
        public JsonResult GetDeliveryChargesForCustomer(int customerSupplierId)
        {
            // Fetch the delivery charges for the selected customer supplier
            var deliveryCharges = _repository.GetDeliveryChargesForCustomer(customerSupplierId);

            return Json(new { deliveryCharges = deliveryCharges });

        }
    }
}