using System.Web.Mvc;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;

namespace ArooshyStore.Areas.Admin.Controllers
{
    public class CombolistController : BaseController
    {
        private readonly ICombolistRepository _repository;
        private readonly ICategoryRepository _categories;

        public CombolistController()
        {
            _repository = new CombolistRepository();
        }
        public CombolistController(ICombolistRepository repository ,ICategoryRepository categories)
        {
            _repository = repository;
            _categories = categories;
        }
        #region Categories
        public JsonResult GetCategoryOptionList(string searchTerm, int pageSize, int pageNumber, string type = "child")
        {
            var select2pagedResult = _repository.GetCategoriesList(searchTerm, pageSize, pageNumber, type);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region All Categories
        public JsonResult GetAllCategoryOptionList(string searchTerm, int pageSize, int pageNumber, string type = "child")
        {
            var select2pagedResult = _repository.GetAllCategoriesList(searchTerm, pageSize, pageNumber, type);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Child Categories by Parent Category Id
        public JsonResult GetChildCategoriesByParentCategoryIdOptionList(string searchTerm, int pageSize, int pageNumber, int parentCategoryId = 0)
        {
            var select2pagedResult = _repository.GetChildCategoriesByParentCategoryIdOptionList(searchTerm, pageSize, pageNumber, parentCategoryId);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Customers Supplier
        public JsonResult GetCustomerSupplierOptionList(string searchTerm, int pageSize, int pageNumber, string type = "customer")
        {
            var select2pagedResult = _repository.GetCustomerSupplierList(searchTerm, pageSize, pageNumber,type);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetSupplierOptionList(string searchTerm, int pageSize, int pageNumber, string type = "supplier")
        {
            var select2pagedResult = _repository.GetCustomerSupplierList(searchTerm, pageSize, pageNumber, type);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Attributes
        public JsonResult GetAttributesOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetAttributesList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Attributes Detail
        public JsonResult GetAttributesDetailOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetAttributeDetailList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Units
        public JsonResult GetUnitsOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetUnitsList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Discount Offers
        public JsonResult GetDiscountOffersOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetDiscountOffersList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region User Types
        public JsonResult GetUserTypesOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetUserTypesList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Cities
        public JsonResult GetCitiesOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetCitiesList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Designation
        public JsonResult GetDesignationsOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetDesignationsList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
        #region Employees
        public JsonResult GetEmployeesOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetEmployeesList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Products
        public JsonResult GetProductsOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetProductsList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region All Products
        public JsonResult GetAllProductsOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetAllProductsList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Products with Category Id
        public JsonResult GetProductsByCategoryIdOptionList(string searchTerm, int pageSize, int pageNumber, int parentCategoryId, int childCategoryId = 0)
        {
            var select2pagedResult = _repository.GetProductsByCategoryIdList(searchTerm, pageSize, pageNumber, parentCategoryId, childCategoryId);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region ExpenseTypes
        public JsonResult GetExpenseTypesOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetExpenseTypesList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Error Line Number
        public JsonResult GetErrorLineNumberOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetErrorLineNumberList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Delivery Info
        public JsonResult GetDeliveryInfoList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetDeliveryInfoList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Product Tag
        public JsonResult GetProductTagsList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetProductTagList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Error Source
        public JsonResult GetErrorSourceOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetErrorSourceList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Error Line Number
        public JsonResult GetErrorClassOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetErrorClassList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Error Source
        public JsonResult GetErrorActionOptionList(string searchTerm, int pageSize, int pageNumber)
        {
            var select2pagedResult = _repository.GetErrorActionList(searchTerm, pageSize, pageNumber);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Products Attributes from tblProductDetailBarcode
        public JsonResult GetProductAttributesFromBarcodeTableOptionList(string searchTerm, int pageSize, int pageNumber, int productId = 0)
        {
            var select2pagedResult = _repository.GetProductListAttributesFromBarcodeTable(searchTerm, pageSize, pageNumber, productId);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
        #region Products All Attributes from tblProductDetailBarcode
        public JsonResult GetProductAllAttributesFromBarcodeTableOptionList(string searchTerm, int pageSize, int pageNumber, int productId = 0)
        {
            var select2pagedResult = _repository.GetProductListAllAttributesFromBarcodeTable(searchTerm, pageSize, pageNumber, productId);
            var result = select2pagedResult;
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}