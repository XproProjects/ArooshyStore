using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ArooshyStore.App_Start;
using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.Models.ViewModels;
using AutoMapper;
using static System.Data.Entity.Infrastructure.Design.Executor;

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
        public ActionResult SaleInvoiceIndex(string from = "all")
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
                var FilterType = Request.Form["FilterType"];
                var DateFilter = Request.Form["DateFilter"];
                var MonthFilter = Request.Form["MonthFilter"];
                var FromDateFilter = Request.Form["FromDateFilter"];
                var ToDateFilter = Request.Form["ToDateFilter"];
                var TextboxFilter = Request.Form["TextboxFilter"];
                var ProductId = Request.Form["ProductIdList"];
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                        + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //var userName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                string whereCondition = " Lower(s.InvoiceType)='" + Type.ToString().ToLower().Trim() + "' ";
                if (!string.IsNullOrEmpty(From))
                {
                    if (From.ToLower().Trim() != "all")
                    {
                        whereCondition += " And (select top(1) lower(ist.Status) from tblInvoiceStatus ist where ist.InvoiceNumber = s.InvoiceNumber order by ist.InvoiceStatusId desc) = '" + (From.ToString().ToLower().Trim() == "onhold" ? "on hold" : From.ToString().ToLower().Trim()) + "' ";
                    }
                }
                if (!(string.IsNullOrEmpty(ProductId)))
                {
                    whereCondition += " and s.InvoiceNumber in (select idd.InvoiceNumber from tblInvoiceDetail idd where idd.ProductId = " + ProductId + ") ";
                }
                if (!string.IsNullOrEmpty(FilterType))
                {
                    if (FilterType.ToLower().Trim() == "date" && DateFilter != null)
                    {
                        string monthString = Convert.ToDateTime(DateFilter).Month.ToString();
                        string dayString = Convert.ToDateTime(DateFilter).Day.ToString();
                        if (monthString.Length == 1)
                        {
                            monthString = "0" + monthString;
                        }
                        if (dayString.Length == 1)
                        {
                            dayString = "0" + dayString;
                        }
                        whereCondition += " And format(isnull(s.InvoiceDate,''),'dd/MM/yyyy') ='" + dayString + "/" + monthString + "/" + Convert.ToDateTime(DateFilter).Year + "' ";
                    }
                    else if (FilterType.ToLower().Trim() == "month" && MonthFilter != null)
                    {
                        string monthString = Convert.ToDateTime(MonthFilter).Month.ToString();
                        if (monthString.Length == 1)
                        {
                            monthString = "0" + monthString;
                        }
                        whereCondition += " And format(isnull(s.InvoiceDate,''),'MM/yyyy') ='" + monthString + "/" + Convert.ToDateTime(MonthFilter).Year + "' ";
                    }
                    else if (FilterType.ToLower().Trim() == "between dates" && FromDateFilter != null && ToDateFilter != null)
                    {
                        string fromMonthString = Convert.ToDateTime(FromDateFilter).Month.ToString();
                        string toMonthString = Convert.ToDateTime(ToDateFilter).Month.ToString();
                        string fromDayString = Convert.ToDateTime(FromDateFilter).Day.ToString();
                        string toDayString = Convert.ToDateTime(ToDateFilter).Day.ToString();
                        if (fromMonthString.Length == 1)
                        {
                            fromMonthString = "0" + fromMonthString;
                        }
                        if (toMonthString.Length == 1)
                        {
                            toMonthString = "0" + toMonthString;
                        }
                        if (fromDayString.Length == 1)
                        {
                            fromDayString = "0" + fromDayString;
                        }
                        if (toDayString.Length == 1)
                        {
                            toDayString = "0" + toDayString;
                        }
                        whereCondition += " And cast(s.InvoiceDate as date) >= '" + Convert.ToDateTime(FromDateFilter).Year + "-" + fromMonthString + "-" + fromDayString + "' and cast(s.InvoiceDate as date) <= '" + Convert.ToDateTime(ToDateFilter).Year + "-" + toMonthString + "-" + toDayString + "' ";
                    }
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
                if (!(string.IsNullOrEmpty(TextboxFilter)))
                {
                    whereCondition += " And LOWER(s.InvoiceNumber) like ('%" + TextboxFilter.ToString().ToLower() + "%')";
                }

                else
                {
                    whereCondition += " And LOWER(s.InvoiceNumber) like ('%%')";
                }
                List<InvoiceViewModel> listsub = new List<InvoiceViewModel>();
                if (_roles.CheckActionRoleId(User.UserId, Type.ToString().ToLower().Trim(), "view") > 0)
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
                if (type.ToLower() == "edit")
                {
                    actionName = "update";
                }
                else if (type.ToLower() == "exchange")
                {
                    actionName = "exchange";
                }
                else
                {
                    actionName = "create";
                }
                if (_roles.CheckActionRoleId(User.UserId, "sale invoice", actionName) > 0)
                {
                    ViewBag.CreateRole = _roles.CheckActionRoleId(User.UserId, "sale invoice", "create");
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
                if (type.ToLower() == "edit")
                {
                    actionName = "update";
                }
                else
                {
                    actionName = "create";
                }
                if (_roles.CheckActionRoleId(User.UserId, "sale return", actionName) > 0)
                {
                    ViewBag.CreateRole = _roles.CheckActionRoleId(User.UserId, "sale return", "create");
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
                if (type.ToLower() == "edit")
                {
                    actionName = "update";
                }
                else
                {
                    actionName = "create";
                }
                if (_roles.CheckActionRoleId(User.UserId, "purchase invoice", actionName) > 0)
                {
                    ViewBag.CreateRole = _roles.CheckActionRoleId(User.UserId, "purchase invoice", "create");
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
                if (type.ToLower() == "edit")
                {
                    actionName = "update";
                }
                else
                {
                    actionName = "create";
                }
                if (_roles.CheckActionRoleId(User.UserId, "purchase return", actionName) > 0)
                {
                    ViewBag.CreateRole = _roles.CheckActionRoleId(User.UserId, "purchase return", "create");
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

        public ActionResult DeleteInvoice(string id)
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

        public ActionResult ReturnInvoice(string id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {

                response = _repository.ReturnInvoice(id, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }

        [HttpGet]
        public ActionResult InvoiceDetail(string id)
        {

            if (User != null)
            {

                InvoiceViewModel invoice = _repository.GetInvoiceDetail(id, User.UserId);
                return View(invoice);

            }
            else
            {
                return RedirectToAction("login", "account");
            }

        }
        [HttpGet]
        public ActionResult PrintInvoice(string id)
        {

            if (User != null)
            {

                InvoiceViewModel invoice = _repository.GetInvoiceByIdForPrint(id, User.UserId);
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

        public ActionResult GetDeliveryCharges(int customerId = 0)
        {
            decimal deliveryCharges = _repository.GetDeliveryCharges(customerId, User.UserId);
            return Json(deliveryCharges, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetTotalInvoiceItems(string id = "")
        {
            int totalItems = _repository.GetTotalInvoiceItems(id);
            return Json(totalItems, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult InvoiceDetailsList(string id = "")
        {
            if (User != null)
            {
                List<InvoiceDetailViewModel> list = _repository.InvoiceDetailsList(id);
                return PartialView(list);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
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
        public ActionResult GetCashCustomer()
        {
            InvoiceViewModel model = new InvoiceViewModel();
            if (User != null)
            {
                model = _repository.GetCashCustomer();
            }

            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

    }
}