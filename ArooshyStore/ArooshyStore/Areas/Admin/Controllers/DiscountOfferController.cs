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
    public class DiscountOfferController : BaseController
    {
        private readonly IDiscountOfferRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public DiscountOfferController(IDiscountOfferRepository repository, ICheckUserRoleRepository roles)
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
                if (_roles.CheckModuleRoleId(User.UserId, "discount offer") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "discount offer");

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
        public ActionResult GetAllDiscountOffers()
        {
            if (User != null)
            {
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                        + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var productName = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                var offerName = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
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
                    sorting = " Order by s.OfferId asc";
                }
                if (!(string.IsNullOrEmpty(productName)))
                {
                    whereCondition += " s.OfferId in (select od.OfferId from tblDiscountOfferDetail od left join tblProduct pd on od.ProductId = pd.ProductId where LOWER(pd.ProductName) like ('%" + productName.ToLower() + "%') or LOWER(pd.ArticleNumber) like ('%" + productName.ToLower() + "%')) ";
                }
                else if (!(string.IsNullOrEmpty(offerName)))
                {
                    whereCondition += " LOWER(s.DiscountName) like ('%" + offerName.ToLower() + "%')";
                }
                else
                {
                    whereCondition += " LOWER(s.DiscountName) like ('%%')";
                }
                List<DiscountOfferViewModel> listsub = new List<DiscountOfferViewModel>();
                if (_roles.CheckActionRoleId(User.UserId, "discount offer", "view") > 0)
                {
                    listsub = _repository.GetDiscountOffersListAndCount(whereCondition, start, length, sorting);
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
        public ActionResult InsertUpdateDiscountOffer(int id = 0)
        {
            if (User != null)
            {
                if (_roles.CheckActionRoleId(User.UserId, "discount offer", "create") > 0)
                {
                    DiscountOfferViewModel user = _repository.GetDiscountOfferById(id);
                    return View(user);
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
        public ActionResult InsertUpdateDiscountOffer(DiscountOfferViewModel user, string data)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                response = _repository.InsertUpdateDiscountOffer(user, data, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }

        public ActionResult DeleteDiscountOffer(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                DiscountOfferViewModel user = new DiscountOfferViewModel();
                user.OfferId = id;
                response = _repository.DeleteDiscountOffer(id, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }
        [HttpGet]
        public ActionResult ProductsList(int id = 0)
        {
            if (User != null)
            {
                List<DiscountOfferViewModel> list = _repository.ProductsList(id);
                return PartialView(list);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
    }
}