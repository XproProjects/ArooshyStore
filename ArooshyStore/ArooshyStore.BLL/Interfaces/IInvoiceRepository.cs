﻿using System;
using System.Collections.Generic;
using ArooshyStore.Models.ViewModels;

namespace ArooshyStore.BLL.Interfaces
{
    public interface IInvoiceRepository : IDisposable
    {
        List<InvoiceViewModel> GetInvoicesListAndCount(string whereCondition, string start, string length, string sorting);
        InvoiceViewModel GetInvoiceById(string id, string type);
        StatusMessageViewModel InsertUpdateInvoice(InvoiceViewModel model, string detail, int loggedInUserId);
        StatusMessageViewModel DeleteInvoice(string id, int loggedInUserId);
        StatusMessageViewModel ReturnInvoice(string id, int loggedInUserId);
        List<InvoiceDetailViewModel> GetInvoiceDetailsList(string InvoiceNo);
        string GetMaxCodeForInvoice(string type, int loggedInUserId);
        decimal GetDeliveryCharges(int customerId, int loggedInUserId);
        StatusMessageViewModel InsertUpdateInvoiceStatus(InvoiceStatusViewModel model, int loggedInUserId);
        InvoiceStatusViewModel GetInvoiceStatusById(string id);
        InvoiceViewModel GetInvoiceByIdForPrint(string id, int loggedInUserId);
        InvoiceViewModel GetInvoiceDetail(string id, int loggedInUserId);
        InvoiceViewModel GetCashCustomer();
        int GetTotalInvoiceItems(string id);
        List<InvoiceDetailViewModel> InvoiceDetailsList(string InvoiceNo);
    }
}