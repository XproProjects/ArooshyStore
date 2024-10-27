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
    public class InvoiceController : BaseController
    {
        private readonly IInvoiceRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public InvoiceController(IInvoiceRepository repository, ICheckUserRoleRepository roles)
        {
            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;
            _roles = roles;
        }
        public ActionResult SaleInvoiceIndex(string from = "ordered")
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                if (_roles.CheckModuleRoleId(User.UserId, "sale invoice") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "sale invoice");
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
        public ActionResult SaleReturnIndex()
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                if (_roles.CheckModuleRoleId(User.UserId, "sale return") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "sale return");
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
        public ActionResult PurchaseInvoiceIndex()
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                if (_roles.CheckModuleRoleId(User.UserId, "purchase invoice") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "purchase invoice");
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
        public ActionResult PurchaseReturnIndex()
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                if (_roles.CheckModuleRoleId(User.UserId, "purchase return") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "purchase return");
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

        public ActionResult GetAllInvoices()
        {
            if (User != null)
            {
                var From = Request.Form.GetValues("From").FirstOrDefault();
                var Type = Request.Form.GetValues("Type").FirstOrDefault();
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
                string whereCondition = " Lower(s.InvoiceType)='" + Type.ToString().ToLower().Trim()+"' ";
                if(!string.IsNullOrEmpty(From))
                {
                    whereCondition += " And s.InvoiceNumber in (select top(5) ist.InvoiceNumber from tblInvoiceStatus ist where lower(ist.Status) = '"+From.ToString().ToLower().Trim()+"' order by ist.InvoiceStatusId desc) ";
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
                    sorting = " Order by s.InvoiceNumber asc";
                }
                if (!(string.IsNullOrEmpty(userName)))
                {
                    whereCondition += " And LOWER(s.InvoiceNumber) like ('%" + userName.ToLower() + "%')";
                }

                else
                {
                    whereCondition += " And LOWER(s.InvoiceNumber) like ('%%')";
                }
                List<InvoiceViewModel> listsub = new List<InvoiceViewModel>();
                if (_roles.CheckActionRoleId(User.UserId, "sale invoice", "view") > 0)
                {
                    listsub = _repository.GetInvoicesListAndCount(whereCondition, start, length, sorting);
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
        public ActionResult InsertUpdateSaleInvoice(string id, string type)
        {
            if (User != null)
            {
                string actionName = "";
                if (id != "0")
                {
                    actionName = "update";
                }
                else
                {
                    actionName = "create";
                }
                if (_roles.CheckActionRoleId(User.UserId, "sale invoice", actionName) > 0)
                {
                    InvoiceViewModel invoice = _repository.GetInvoiceById(id, type);
                    return View(invoice);
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
        [HttpGet]
        public ActionResult InsertUpdateSaleReturn(string id, string type)
        {
            if (User != null)
            {
                string actionName = "";
                if (id != "0")
                {
                    actionName = "update";
                }
                else
                {
                    actionName = "create";
                }
                if (_roles.CheckActionRoleId(User.UserId, "sale return", actionName) > 0)
                {
                    InvoiceViewModel invoice = _repository.GetInvoiceById(id, type);
                    return View(invoice);
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
        [HttpGet]
        public ActionResult InsertUpdatePurchaseInvoice(string id, string type)
        {
            if (User != null)
            {
                string actionName = "";
                if (id != "0")
                {
                    actionName = "update";
                }
                else
                {
                    actionName = "create";
                }
                if (_roles.CheckActionRoleId(User.UserId, "purchase invoice", actionName) > 0)
                {
                    InvoiceViewModel invoice = _repository.GetInvoiceById(id, type);
                    return View(invoice);
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
        [HttpGet]
        public ActionResult InsertUpdatePurchaseReturn(string id, string type)
        {
            if (User != null)
            {
                string actionName = "";
                if (id != "0")
                {
                    actionName = "update";
                }
                else
                {
                    actionName = "create";
                }
                if (_roles.CheckActionRoleId(User.UserId, "purchase return", actionName) > 0)
                {
                    InvoiceViewModel invoice = _repository.GetInvoiceById(id, type);
                    return View(invoice);
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


        [HttpPost]
        public ActionResult InsertUpdateInvoice(InvoiceViewModel user, string detail)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateInvoice(user, detail, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.IdString } };
        }

        [HttpGet]
        public ActionResult PrintInvoice(string id, string type = "")
        {

            if (User != null)
            {
               
              InvoiceViewModel invoice = _repository.GetInvoiceByIdForPrint(id, type);
              return View(invoice);
               
            }
            else
            {
                return RedirectToAction("login", "account");
            }

        }
        public ActionResult GetMaxCodeForInvoice(string type)
        {
            string maxCode = _repository.GetMaxCodeForInvoice(type, User.UserId);
            return Json(maxCode, JsonRequestBehavior.AllowGet);
        }
        public ActionResult DeleteSaleInvoice(string id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {

                response = _repository.DeleteInvoice(id, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }


        public ActionResult GetInvoiceDetailsList(string id)
        {
            List<InvoiceDetailViewModel> list = new List<InvoiceDetailViewModel>();
            if (User != null)
            {
                list = _repository.GetInvoiceDetailsList(id);
            }

            return Json(new { data = list }, JsonRequestBehavior.AllowGet); 
        }

        [HttpGet]
        public ActionResult InsertUpdateInvoiceStatus(string id)
        {
            if (User != null)
            {
                InvoiceStatusViewModel user = _repository.GetInvoiceStatusById(id);
                return PartialView(user);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateInvoiceStatus(InvoiceStatusViewModel user)
        {
            System.Diagnostics.Debug.WriteLine($"InvoiceNumber: {user.InvoiceNumber}");

            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateInvoiceStatus(user, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }


    }
}