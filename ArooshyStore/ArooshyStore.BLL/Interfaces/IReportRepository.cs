using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IReportRepository : IDisposable
    {
        List<InvoiceViewModel> GetInvoicesList(DateFilterReportViewModel report, int loggedInUserId);
        List<InvoiceViewModel> GetInvoicesStatusWiseList(DateFilterReportViewModel report, int loggedInUserId);
        List<InvoiceDetailViewModel> GetProductSale(DateFilterReportViewModel report, int loggedInUserId);
        List<InvoiceDetailViewModel> GetProductStock(DateFilterReportViewModel report, int loggedInUserId);
    }
}