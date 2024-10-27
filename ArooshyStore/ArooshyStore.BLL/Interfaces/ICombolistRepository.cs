using System;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface ICombolistRepository : IDisposable
    {
        Select2PagedResultViewModel GetCategoriesList(string searchTerm, int pageSize, int pageNumber, string type);
        Select2PagedResultViewModel GetUnitsList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetUserTypesList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetCitiesList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetProductsList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetExpenseTypesList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultStringViewModel GetErrorLineNumberList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultStringViewModel GetErrorSourceList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultStringViewModel GetErrorClassList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultStringViewModel GetErrorActionList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetCustomerSupplierList(string searchTerm, int pageSize, int pageNumber, string type);
        Select2PagedResultViewModel GetAttributesList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetDiscountOffersList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetAttributeDetailList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetDeliveryInfoList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetProductTagList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetDesignationsList(string searchTerm, int pageSize, int pageNumber);
        Select2PagedResultViewModel GetEmployeesList(string searchTerm, int pageSize, int pageNumber);

    }
}
