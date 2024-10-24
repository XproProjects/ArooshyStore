using System;
using System.Collections.Generic;
using ArooshyStore.Domain.DomainModels;
using System.Threading.Tasks;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface ICustomerSupplierRepository : IDisposable
    {
        List<CustomerSupplierViewModel> GetCustomerSuppliersListAndCount(string whereCondition, string start, string length, string sorting);
        CustomerSupplierViewModel GetCustomerSupplierById(int id);
        StatusMessageViewModel InsertUpdateCustomerSupplier(CustomerSupplierViewModel model, int loggedInUserId);
        StatusMessageViewModel DeleteCustomerSupplier(int id, int loggedInUserId);
        decimal GetDeliveryChargesForCustomer(int customerSupplierId);
        UserDomainModel GetCustomerSupplierByEmailAndPassword(CustomerSupplierViewModel model);
        CustomerSupplierViewModel GetCustomerById(int id);
        StatusMessageViewModel InsertUpdateCustomer(CustomerSupplierViewModel model);
        Task<bool> ChangePassword(int userId, string password);
        StatusMessageViewModel ForgotPassword(string email);
        Task<bool> ResetPassword(int userId, string password);
        List<InvoiceViewModel> GetSalesOrderCustomerById(int id);
        InvoiceViewModel GetSalesOrderById(string invoiceNumber);


    }
}
