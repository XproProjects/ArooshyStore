using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.Mvc;
using ArooshyStore.App_Start;
using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.Models.ViewModels;
using AutoMapper;

namespace ArooshyStore.Areas.Admin.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IReportRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public ReportController(IReportRepository repository, ICheckUserRoleRepository roles)
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
                if (_roles.CheckModuleRoleId(User.UserId, "reports") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "reports");

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
        #region Sale / Purchase Report
        public ActionResult SalePurchaseReport(string type = "sale")
        {
            if (User != null)
            {
                DateFilterReportViewModel report = new DateFilterReportViewModel();
                report.InvoiceType = type.ToLower().Trim() == "sale" ? "Sale Invoice" : "Purchase Invoice";
                return PartialView(report);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        public ActionResult PrintSalePurchaseReport(DateFilterReportViewModel report)
        {
            List<InvoiceViewModel> invoicesList = _repository.GetInvoicesList(report, User.UserId);
            DateFilterReportViewModel report2 = new DateFilterReportViewModel();
            report2.FromDate = report.FromDate;
            report2.ToDate = report.ToDate;
            report2.InvoicesList = invoicesList;
            return View(report2);
        }
        #endregion
        #region Sale / Purchase (Status Wise) Report
        public ActionResult SalePurchaseStatusWiseReport(string type = "sale")
        {
            if (User != null)
            {
                DateFilterReportViewModel report = new DateFilterReportViewModel();
                report.InvoiceType = type.ToLower().Trim() == "sale" ? "Sale Invoice" : "Purchase Invoice";
                return PartialView(report);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        public ActionResult PrintSalePurchaseStatusWiseReport(DateFilterReportViewModel report)
        {
            List<InvoiceViewModel> invoicesList = _repository.GetInvoicesStatusWiseList(report, User.UserId);
            DateFilterReportViewModel report2 = new DateFilterReportViewModel();
            report2.FromDate = report.FromDate;
            report2.ToDate = report.ToDate;
            report2.InvoicesList = invoicesList;
            report2.InvoiceStatusesList = (from item in invoicesList
                                           group item by item.Status into grouped
                                           select new InvoiceViewModel
                                           {
                                               Status = grouped.Key,
                                               InvoiceNumber = grouped.FirstOrDefault().InvoiceNumber,
                                           }).ToList();
            return View(report2);
        }
        #endregion
        #region Product Sale Report
        public ActionResult ProductSaleReport()
        {
            if (User != null)
            {
                return PartialView();
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        public ActionResult PrintProductSaleReport(DateFilterReportViewModel report)
        {
            List<InvoiceDetailViewModel> productsList = _repository.GetProductSale(report, User.UserId);
            DateFilterReportViewModel report2 = new DateFilterReportViewModel();
            report2.FromDate = report.FromDate;
            report2.ToDate = report.ToDate;
            report2.InvoiceDetailsList = productsList;
            report2.CategoriesList = (from item in productsList
                                      group item by item.MasterCategoryId into grouped
                                      select new InvoiceDetailViewModel
                                      {
                                          MasterCategoryId = grouped.Key,
                                          MasterCategoryName = grouped.FirstOrDefault().MasterCategoryName,
                                      }).ToList();
            report2.ProductsList = (from item in productsList
                                    group item by item.ProductId into grouped
                                    select new InvoiceDetailViewModel
                                    {
                                        ProductId = grouped.Key,
                                        ProductName = grouped.FirstOrDefault().ProductName,
                                    }).ToList();
            return View(report2);
        }
        #endregion
        #region Product Stock Report
        public ActionResult ProductStockReport()
        {
            if (User != null)
            {
                return PartialView();
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        public ActionResult PrintProductStockReport(DateFilterReportViewModel report)
        {
            List<InvoiceDetailViewModel> productsList = _repository.GetProductStock(report, User.UserId);
            DateFilterReportViewModel report2 = new DateFilterReportViewModel();
            report2.InvoiceDetailsList = productsList;
            return View(report2);
        }
        #endregion
    }
}
