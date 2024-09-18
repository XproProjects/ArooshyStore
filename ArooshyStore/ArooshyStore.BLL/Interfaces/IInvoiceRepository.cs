using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IInvoiceRepository : IDisposable
    {
        List<InvoiceViewModel> GetInvoicesListAndCount(string whereCondition, string start, string length, string sorting);
        InvoiceViewModel GetSaleInvoiceById(string id, string type);
        StatusMessageViewModel InsertUpdateSaleInvoice(InvoiceViewModel model, string detail, int loggedInUserId);
        StatusMessageViewModel DeleteSaleInvoice(string id, int loggedInUserId);
        string GetMaxCodeForInvoice(string type, int loggedInUserId);

    }
}