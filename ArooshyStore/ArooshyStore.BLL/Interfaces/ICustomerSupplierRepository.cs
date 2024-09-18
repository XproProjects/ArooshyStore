using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface ICustomerSupplierRepository : IDisposable
    {
        List<CustomerSupplierViewModel> GetCustomerSuppliersListAndCount(string whereCondition, string start, string length, string sorting);
        CustomerSupplierViewModel GetCustomerSupplierById(int id);
        StatusMessageViewModel InsertUpdateCustomerSupplier(CustomerSupplierViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteCustomerSupplier(int id, int loggedInUserId);

    }
}
