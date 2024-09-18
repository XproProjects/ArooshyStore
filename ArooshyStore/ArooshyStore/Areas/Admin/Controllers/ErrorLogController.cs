using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ArooshyStore.App_Start;
using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.Models.ViewModels;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;

namespace ArooshyStore.Areas.Admin.Controllers
{
    public class ErrorLogController : BaseController
    {
        private readonly IErrorLogRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public ErrorLogController(IErrorLogRepository repository, ICheckUserRoleRepository roles)
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
        public ActionResult SetSearchString(bool? classCheckbox, string errorClass,
           bool? errorSourceCheckbox, string errorSourceId,
           bool? errorLineNumbersCheckbox, string errorLineNumberId,
           bool? errorActionCheckbox, string errorActionId
           )
        {
            string searchString = "";

            if (classCheckbox == true && !(string.IsNullOrEmpty(errorClass)))
            {
                searchString += " isnull(s.ErrorClass,'') in ('" + errorClass + "') and ";
            }
            if (errorSourceCheckbox == true && !(string.IsNullOrEmpty(errorSourceId)))
            {
                searchString += " isnull(s.ErrorSource,'') in ('" + errorSourceId + "') and ";
            }
            if (errorLineNumbersCheckbox == true && !(string.IsNullOrEmpty(errorLineNumberId)))
            {
                searchString += " isnull(s.ErrorLineNumber,'') in ('" + errorLineNumberId + "') and ";
            }
            if (errorActionCheckbox == true && !(string.IsNullOrEmpty(errorActionId)))
            {
                searchString += " isnull(s.ErrorAction,'') in ('" + errorActionId + "') and ";
            }

            return Json(new
            {
                searchString = searchString
            }, JsonRequestBehavior.AllowGet);
        }
        // GET: /Home/
        public ActionResult GetAllErrorLogs()
        {
            if (User != null)
            {
                var SearchString = Request.Form["SearchString"];

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
                string whereCondition = SearchString +" LOWER(s.ErrorClass) like('%%')";
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
                    sorting = " Order by s.ErrorLogId asc";
                }
               
                List<ErrorLogViewModel> listsub = new List<ErrorLogViewModel>();
                listsub = _repository.GetErrorLogsListAndCount(whereCondition, start, length, sorting);
                if (listsub.Count > 0)
                {
                    recordsTotal = listsub.Select(x => x.TotalRecords).FirstOrDefault();
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
        //public ActionResult SetSearchString(  bool? employeeCheckbox , int[] errorId )


        //{
        //    string searchString =  " ";


        //    if(employeeCheckbox == true && errorId != null)
        //    {
        //        searchString += " and isnull((select isnull(count(ee.ErrorId),0) from tblErrorsLog  and  isnull(s.ErrorLineNumber, '') as ErrorLineNumber != isnull((select top(1) COUNT(*) OVER () from tblErrorsLog),0) ";
        //    }
        //    return Json(new
        //    {
        //        searchString = searchString
        //    },
        //            JsonRequestBehavior.AllowGet);
        //}
       




    }
}
