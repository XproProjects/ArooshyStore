using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Configuration;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Services
{
    public class CombolistRepository : ICombolistRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        private IQueryable<SelectListViewModel> AllItemsList;
        private IQueryable<SelectListStringViewModel> AllItemsListString;
        public CombolistRepository()
        {
            this._unitOfWork = new UnitOfWork();
        }
        public CombolistRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        #region Master Category
        public Select2PagedResultViewModel GetCategoriesList(string searchTerm, int pageSize, int pageNumber, string type)
        {
            if (type.ToLower() == "master")
            {
                AllItemsList = AllParentCategoriesList(type);
            }
            else
            {
                AllItemsList = AllChildCategoriesList(type);
            }
            
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> AllParentCategoriesList(string type)
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblCategory>()
                    orderby c.CategoryName
                    where c.ParentCategoryId == 0
                    && c.Status == true
                    select new SelectListViewModel
                    {
                        id = c.CategoryId,
                        text = c.CategoryName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        public IQueryable<SelectListViewModel> AllChildCategoriesList(string type)
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblCategory>()
                    join p in _unitOfWork.Db.Set<tblCategory>() on c.ParentCategoryId equals p.CategoryId
                    orderby c.CategoryName
                    where c.ParentCategoryId != 0
                    && c.Status == true
                    select new SelectListViewModel
                    {
                        id = c.CategoryId,
                        text = c.CategoryName + " - " + p.CategoryName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region CustomerSupplier Category
        public Select2PagedResultViewModel GetCustomerSupplierList(string searchTerm, int pageSize, int pageNumber, string type)
        {
            AllItemsList = AllCustomerSuppliersList(type);
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> AllCustomerSuppliersList(string type)
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblCustomerSupplier>()
                    orderby c.CustomerSupplierName
                    where (type.ToLower() == "customer" ? c.CustomerSupplierType.ToLower() == "customer" : c.CustomerSupplierType.ToLower() == "supplier")
                    && c.Status == true
                    select new SelectListViewModel
                    {
                        id = c.CustomerSupplierId,
                        text = c.CustomerSupplierName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Attributes
        public Select2PagedResultViewModel GetAttributesList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetAttributeList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetAttributeList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblAttribute>()
                    orderby c.AttributeName
                    select new SelectListViewModel
                    {
                        id = c.AttributeId,
                        text = c.AttributeName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Attributes Detail
        public Select2PagedResultViewModel GetAttributeDetailList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetAttributesDetailList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetAttributesDetailList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblAttributeDetail>()
                    orderby c.AttributeDetailName
                    select new SelectListViewModel
                    {
                        id = c.AttributeDetailId,
                        text = c.AttributeDetailName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Units
        public Select2PagedResultViewModel GetUnitsList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetUnitsList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetUnitsList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblUnit>()
                    orderby c.UnitName
                    select new SelectListViewModel
                    {
                        id = c.UnitId,
                        text = c.UnitName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region User Types
        public Select2PagedResultViewModel GetUserTypesList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetUserTypesList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetUserTypesList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblUserType>()
                    where c.TypeName.ToLower() != "super admin"
                    orderby c.TypeName
                    select new SelectListViewModel
                    {
                        id = c.UserTypeId,
                        text = c.TypeName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Discount Offers
        public Select2PagedResultViewModel GetDiscountOffersList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetDiscountOfferList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetDiscountOfferList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblDiscountOffer>()
                    orderby c.DiscountName
                    select new SelectListViewModel
                    {
                        id = c.OfferId,
                        text = c.DiscountName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Cities
        public Select2PagedResultViewModel GetCitiesList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetCitiesList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetCitiesList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblCity>()
                    where  c.Status == true && c.CityName.ToLower() != "shop"
                    orderby c.CityName
                    select new SelectListViewModel
                    {
                        id = c.CityId,
                        text = c.CityName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Products
        public Select2PagedResultViewModel GetProductsList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetProductsList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetProductsList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblProduct>()
                    where c.Status == true
                    orderby c.ArticleNumber
                    select new SelectListViewModel
                    {
                        id = c.ProductId,
                        text = c.ArticleNumber + " - " + c.ProductName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region ExpenseTypes
        public Select2PagedResultViewModel GetExpenseTypesList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetExpenseTypesList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetExpenseTypesList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblExpenseType>()
                    where c.Status == true
                    orderby c.TypeName
                    select new SelectListViewModel
                    {
                        id = c.ExpenseTypeId,
                        text = c.TypeName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Delivery Info
        public Select2PagedResultViewModel GetDeliveryInfoList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetDeliveryInfoLists();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetDeliveryInfoLists()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblDeliveryInfo>()
                    orderby c.DeliveryInfoName
                    select new SelectListViewModel
                    {
                        id = c.DeliveryInfoId,
                        text = c.DeliveryInfoName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Product Tag
        public Select2PagedResultViewModel GetProductTagList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetProductTagsList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetProductTagsList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblProductTags>()
                    orderby c.TagId
                    select new SelectListViewModel
                    {
                        id = c.TagId,
                        text = c.TagName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Designation
        public Select2PagedResultViewModel GetDesignationsList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetDesignationList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetDesignationList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblDesignation>()
                    orderby c.DesignationId
                    select new SelectListViewModel
                    {
                        id = c.DesignationId,
                        text = c.DesignationName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Employees
        public Select2PagedResultViewModel GetEmployeesList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsList = GetEmployeeList();
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> GetEmployeeList()
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblEmployee>()
                    orderby c.DesignationId
                    select new SelectListViewModel
                    {
                        id = c.EmployeeId,
                        text = c.EmployeeName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Error Line Number
        public Select2PagedResultStringViewModel GetErrorLineNumberList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsListString = GetErrorLineNumbers();
            var select2pagedResult = new Select2PagedResultStringViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptionsString(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListStringViewModel> GetErrorLineNumbers()
        {
            List<SelectListStringViewModel> item = new List<SelectListStringViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblErrorsLog>()
                    
                    orderby c.ErrorLineNumber
                    select new SelectListStringViewModel
                    {
                        id = c.ErrorLineNumber,
                        text = c.ErrorLineNumber
                    }).Distinct().ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion

        #region Error Source
        public Select2PagedResultStringViewModel GetErrorSourceList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsListString = GetErrorSources();
            var select2pagedResult = new Select2PagedResultStringViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptionsString(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListStringViewModel> GetErrorSources()
        {
            List<SelectListStringViewModel> item = new List<SelectListStringViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblErrorsLog>()

                    orderby c.ErrorSource
                    select new SelectListStringViewModel
                    {
                        id = c.ErrorSource,
                        text = c.ErrorSource
                    }).Distinct().ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Error Class
        public Select2PagedResultStringViewModel GetErrorClassList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsListString = GetErrorClasses();
            var select2pagedResult = new Select2PagedResultStringViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptionsString(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListStringViewModel> GetErrorClasses()
        {
            List<SelectListStringViewModel> item = new List<SelectListStringViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblErrorsLog>()

                    orderby c.ErrorClass
                    select new SelectListStringViewModel
                    {
                        id = c.ErrorClass,
                        text = c.ErrorClass
                    }).Distinct().ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Error Action
        public Select2PagedResultStringViewModel GetErrorActionList(string searchTerm, int pageSize, int pageNumber)
        {
            AllItemsListString = GetErrorActions();
            var select2pagedResult = new Select2PagedResultStringViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptionsString(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListStringViewModel> GetErrorActions()
        {
            List<SelectListStringViewModel> item = new List<SelectListStringViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblErrorsLog>()

                    orderby c.ErrorAction
                    select new SelectListStringViewModel
                    {
                        id = c.ErrorAction,
                        text = c.ErrorAction
                    }).Distinct().ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        #region Products Attributes from tblProductDetailBarcode
        public Select2PagedResultViewModel GetProductListAttributesFromBarcodeTable(string searchTerm, int pageSize, int pageNumber, int productId)
        {
            AllItemsList = AllProductListAttributesFromBarcodeTable(productId);
            var select2pagedResult = new Select2PagedResultViewModel();
            var totalResults = 0;
            select2pagedResult.Results = GetPagedListOptions(searchTerm, pageSize, pageNumber, out totalResults);
            select2pagedResult.Total = totalResults;
            return select2pagedResult;
        }

        public IQueryable<SelectListViewModel> AllProductListAttributesFromBarcodeTable(int productId)
        {
            List<SelectListViewModel> item = new List<SelectListViewModel>();
            item = (from c in _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>()
                    join a1 in _unitOfWork.Db.Set<tblAttribute>() on c.AttributeId1 equals a1.AttributeId
                    join a2 in _unitOfWork.Db.Set<tblAttribute>() on c.AttributeId2 equals a2.AttributeId
                    join ad1 in _unitOfWork.Db.Set<tblAttributeDetail>() on c.AttributeDetailId1 equals ad1.AttributeDetailId
                    join ad2 in _unitOfWork.Db.Set<tblAttributeDetail>() on c.AttributeDetailId2 equals ad2.AttributeDetailId
                    where c.ProductId == productId
                    && c.Status == true
                    orderby ad1.AttributeDetailName
                    select new SelectListViewModel
                    {
                        id = c.ProductAttributeDetailBarcodeId,
                        text = ad1.AttributeDetailName + " - " + ad2.AttributeDetailName
                    }).ToList();
            var result = item.AsQueryable();
            return result;
        }
        #endregion
        List<SelectListViewModel> GetPagedListOptions(string searchTerm, int pageSize, int pageNumber, out int totalSearchRecords)
        {
            var allSearchedResults = GetAllSearchResults(searchTerm);
            totalSearchRecords = allSearchedResults.Count;
            return allSearchedResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }

        List<SelectListViewModel> GetAllSearchResults(string searchTerm)
        {
            //AllItemsList = AllItemsListDetail();
            var resultList = new List<SelectListViewModel>();

            if (!string.IsNullOrEmpty(searchTerm))
                resultList = AllItemsList.Where(n => n.text.ToLower().Contains(searchTerm.ToLower())).ToList();
            else
                resultList = AllItemsList.ToList();
            return resultList;
        }
        List<SelectListStringViewModel> GetPagedListOptionsString(string searchTerm, int pageSize, int pageNumber, out int totalSearchRecords)
        {
            var allSearchedResults = GetAllSearchResultsString(searchTerm);
            totalSearchRecords = allSearchedResults.Count;
            return allSearchedResults.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
        }
        List<SelectListStringViewModel> GetAllSearchResultsString(string searchTerm)
        {
            //AllItemsList = AllItemsListDetail();
            var resultList = new List<SelectListStringViewModel>();

            if (!string.IsNullOrEmpty(searchTerm))
                resultList = AllItemsListString.Where(n => n.text.ToLower().Contains(searchTerm.ToLower())).ToList();
            else
                resultList = AllItemsListString.ToList();
            return resultList;
        }
        private bool disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    _unitOfWork.Dispose();
                }
            }
            this.disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        //public void Dispose()
        //{
        //}
    }
}
