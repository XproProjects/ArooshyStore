using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ArooshyStore.App_Start;
using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.Models.ViewModels;
using AutoMapper;
using Newtonsoft.Json;

namespace ArooshyStore.Areas.Admin.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IProductRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        public ProductController(IProductRepository repository, ICheckUserRoleRepository roles)
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
                if (_roles.CheckModuleRoleId(User.UserId, "product") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "product");

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
        public ActionResult GetAllProducts()
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
                var barcode = Request.Form.GetValues("columns[1][search][value]").FirstOrDefault();
                var categoryName = Request.Form.GetValues("columns[2][search][value]").FirstOrDefault();
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
                    sorting = " Order by s.ProductId asc";
                }
                if (!(string.IsNullOrEmpty(productName)))
                {
                    whereCondition += " LOWER(s.ProductName) like ('%" + productName.ToLower() + "%')";
                }
                else if (!(string.IsNullOrEmpty(barcode)))
                {
                    whereCondition += " LOWER(s.Barcode) like ('%" + barcode.ToLower() + "%')";
                }
                else if (!(string.IsNullOrEmpty(categoryName)))
                {
                    whereCondition += " LOWER(c.CategoryName) like ('%" + categoryName.ToLower() + "%')";
                }
                else
                {
                    whereCondition += " LOWER(s.ProductName) like ('%%')";
                }
                List<ProductViewModel> listsub = new List<ProductViewModel>();
                if (_roles.CheckActionRoleId(User.UserId, "product", "view") > 0)
                {
                    listsub = _repository.GetProductsListAndCount(whereCondition, start, length, sorting);
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
        public ActionResult InsertUpdateProduct(int id = 0)
        {
            if (User != null)
            {
                ProductViewModel user = _repository.GetProductById(id);
                return PartialView(user);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateProduct(ProductViewModel user, string data,string tags)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {

                response = _repository.InsertUpdateProduct(user, data,tags, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }

        public ActionResult DeleteProduct(int id)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {
                ProductViewModel user = new ProductViewModel();
                user.ProductId = id;
                response = _repository.DeleteProduct(id, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message } };
        }

        [HttpPost]
        public JsonResult GetProductDetailsByBarcode(string barcode)
        {
            var product = _repository.GetProductByBarcode(barcode);

            if (product != null)
            {
                return Json(new
                {
                    productId = product.ProductId,
                    productName = product.ProductName,
                    salePrice = product.SalePrice,
                    barcode = product.Barcode,
                    categoryId = product.CategoryId,
                    costPrice = product.CostPrice,
                    status = product.Status,
                    masterCategoryName = product.MasterCategoryName,
                    childCategoryName = product.ChildCategoryName,
                    CategoryName = product.CategoryName,
                    attribute = "",
                    attributeDetail = ""
                });
            }

            return Json(new
            {
                productId = 0,
                productName = "",
                salePrice = 0,
                barcode = "",
                categoryId = 0,
                costPrice = 0,
                status = "",
                CategoryName = "",
                masterCategoryName = "",
                childCategoryName = "",
                attribute = "",
                attributeDetail = ""
            });
        }
        [HttpPost]
        public JsonResult GetProductSalePrice(int productId)
        {
            var product = _repository.GetProductSalePrice(productId);

            if (product != null)
            {
                return Json(new { productName = product.ProductName, salePrice = product.SalePrice });
            }

            return Json(new { productName = "", salePrice = 0 });

        }
    }
}