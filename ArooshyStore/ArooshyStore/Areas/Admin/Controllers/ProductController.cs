﻿using System;
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
        private readonly IProductReviewRepository _review;

        private readonly IMapper _mapper;
        public ProductController(IProductRepository repository, ICheckUserRoleRepository roles, IProductReviewRepository review)
        {
            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;
            _roles = roles;
            _review = review;
        }
        #region Product
        public ActionResult Index()
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                if (_roles.CheckModuleRoleId(User.UserId, "product") > 0)
                {
                    ProductViewModel model = new ProductViewModel();

                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "product");

                    model.ModulesList = actionList;
                    model.Categories = _repository.GetMasterCategoriesList();
                    return View(model);
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
                var MasterCategoryId = Request.Form["MasterCategoryIdList"];
                var ChildCategoryId = Request.Form["ChildCategoryIdList"];
                var ProductId = Request.Form["ProductIdList"];
                var TextboxFilter = Request.Form["TextboxFilter"];
                var draw = Request.Form.GetValues("draw").FirstOrDefault();
                var start = Request.Form.GetValues("start").FirstOrDefault();
                var length = Request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault()
                                        + "][name]").FirstOrDefault();
                var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
                //var articleNumber = Request.Form.GetValues("columns[0][search][value]").FirstOrDefault();
                int pageSize = length != null ? Convert.ToInt32(length) : 0;
                int skip = start != null ? Convert.ToInt32(start) : 0;
                int recordsTotal = 0;
                string whereCondition = " ";
                if (!(string.IsNullOrEmpty(MasterCategoryId)))
                {
                    whereCondition = " p.CategoryId = " + MasterCategoryId + " and ";
                }
                if (!(string.IsNullOrEmpty(ChildCategoryId)))
                {
                    whereCondition += " c.CategoryId = " + ChildCategoryId + " and ";
                }
                if (!(string.IsNullOrEmpty(ProductId)))
                {
                    whereCondition += " s.ProductId = " + ProductId + " and ";
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
                    sorting = " Order by s.ProductId asc";
                }
                if (!(string.IsNullOrEmpty(TextboxFilter)))
                {
                    whereCondition += " s.ProductId in (select pd.ProductId from tblProductAttributeDetailBarcode pd where pd.Barcode = '" + TextboxFilter.ToString().ToLower().Trim() + "' ) ";
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
        [HttpGet]
        public ActionResult AttributesList(int id = 0)
        {
            if (User != null)
            {
                ProductViewModel user = _repository.GetProductAttributesById(id);
                return PartialView(user);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateProduct(ProductViewModel user, string data, string tags)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {

                response = _repository.InsertUpdateProduct(user, data, tags, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }

        [HttpGet]
        public ActionResult UpdateCostPrice(int id = 0)
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
        public ActionResult UpdateCostPrice(ProductViewModel user)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {

                response = _repository.UpdateCostPrice(user, User.UserId);
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
        public ActionResult GetProductTotalStock(int productId = 0)
        {
            int totalStock = _repository.GetProductTotalStock(productId, User.UserId);
            return Json(totalStock, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult ProductStockDetail(int id = 0)
        {
            if (User != null)
            {
                List<ProductAttributeDetailViewModel> list = new List<ProductAttributeDetailViewModel>();
                if (_roles.CheckActionRoleId(User.UserId, "product", "view stock") > 0)
                {
                    list = _repository.GetProductStockDetailByProductId(id);
                }
                return PartialView(list);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }

        [HttpGet]
        public ActionResult ProductAttributesBarcodesList(int id = 0)
        {
            if (User != null)
            {
                ViewBag.ViewStock = _roles.CheckActionRoleId(User.UserId, "product", "view stock");
                ViewBag.UpdateStock = _roles.CheckActionRoleId(User.UserId, "product", "update stock");
                List<ProductAttributeDetailViewModel> list = _repository.GetProductAttributesListByProductId(id);
                return PartialView(list);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }

        public ActionResult PrintBarcodeStickers(string data)
        {
            if (User != null)
            {
                List<ProductAttributeDetailViewModel> list = _repository.GetBarcodesDataForPrint(data);
                return View(list);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        public ActionResult PrintBarcodeStickersForTesting()
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

        public ActionResult ProductDetail(int id)
        {
            if (User != null)
            {
                ProductViewModel model = _repository.GetProductDetailById(id);
                return View(model);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        //public ActionResult PrintBarcodeStickers()
        //{
        //    if (User != null)
        //    {
        //        return View();
        //    }
        //    else
        //    {
        //        return RedirectToAction("login", "account");
        //    }
        //}
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
                    productAttributeDetailBarcodeId = product.AttributeDetailId,
                    attributeName = product.AttributeName,
                    offerDetailId = product.OfferDetailId,
                    salePrice = product.SalePrice,
                    quantity = product.Quantity,
                    discountType = product.DiscountType,
                    discountRate = product.DiscountRate,
                    discountAmount = product.DiscountAmount,
                    netAmount = product.NetAmount,
                });
            }

            return Json(new
            {
                productId = 0,
                productName = "",
                productAttributeDetailBarcodeId = 0,
                attributeName = "",
                offerDetailId = 0,
                salePrice = 0,
                quantity = 0,
                discountType = "",
                discountRate = 0,
                discountAmount = 0,
                netAmount = 0,
            });
        }

        [HttpPost]
        public JsonResult GetProductSalePrice(int productId)
        {
            var product = _repository.GetProductSalePrice(productId);

            if (product != null)
            {
                return Json(new
                {
                    productName = product.ProductName,
                    salePrice = product.SalePrice,
                    offerDetailId = product.OfferDetailId,
                    discountType = product.DiscountType,
                    discountRate = product.DiscountRate
                });
            }

            return Json(new
            {
                productName = "",
                salePrice = 0,
                offerDetailId = 0,
                discountType = "Rs.",
                discountRate = 0
            });

        }

        [HttpGet]
        public ActionResult ProductReviews(int productId)
        {
            var reviews = _review.GetProductReviews(productId);
            return View(reviews);
        }
        #endregion
        #region Product Stock
        public ActionResult ProductStock()
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
        [HttpGet]
        public ActionResult GetProductAttributes(string barcode)
        {
            if (User != null)
            {
                List<ProductAttributeDetailViewModel> list = new List<ProductAttributeDetailViewModel>();
                if (_roles.CheckActionRoleId(User.UserId, "product", "view stock") > 0 && !(string.IsNullOrEmpty(barcode)) && !(string.IsNullOrWhiteSpace(barcode)))
                {
                    list = _repository.GetProductAttributesListByBarcode(barcode);
                }
                return PartialView(list);
            }
            else
            {
                return PartialView("_UserLoggedOut");
            }
        }
        [HttpPost]
        public ActionResult InsertUpdateProductStock(string data)
        {
            StatusMessageViewModel response = new StatusMessageViewModel();
            if (User != null)
            {

                response = _repository.InsertUpdateProductStock(data, User.UserId);
            }
            else
            {
                BusinessInfo business = BusinessInfo.GetInstance;
                response = business.UserLoggedOut();
            }
            return new JsonResult { Data = new { status = response.Status, message = response.Message, Id = response.Id } };
        }
        #endregion
    }
}