using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.DAL.Entities;
using ArooshyStore.Models.ViewModels;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ArooshyStore.BLL.Services
{
    public class ReportRepository : IReportRepository
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReportRepository(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public List<InvoiceViewModel> GetInvoicesList(DateFilterReportViewModel report, int loggedInUserId)
        {
            List<InvoiceViewModel> model = new List<InvoiceViewModel>();
            List<InvoiceViewModel> list = new List<InvoiceViewModel>();
            model = (from f in _unitOfWork.Db.Set<tblInvoice>()
                     where f.InvoiceDate >= report.FromDate && f.InvoiceDate <= report.ToDate
                     && f.InvoiceType == report.InvoiceType
                     select new InvoiceViewModel
                     {
                         InvoiceNumber = f.InvoiceNumber ?? "",
                         InvoiceType = f.InvoiceType ?? "",
                         InvoiceDate = f.InvoiceDate ?? DateTime.Now,
                         CustomerSupplierId = f.CustomerSupplierId ?? 0,
                         CustomerName = _unitOfWork.Db.Set<tblCustomerSupplier>()
                                        .Where(x => x.CustomerSupplierId == f.CustomerSupplierId)
                                        .Select(x => x.CustomerSupplierName)
                                        .FirstOrDefault() ?? "",
                         TotalAmount = f.TotalAmount ?? 0,
                         DiscType = f.DiscType ?? "Rs.",
                         DiscRate = f.DiscRate ?? 0,
                         DiscAmount = f.DiscAmount ?? 0,
                         NetAmount = f.NetAmount ?? 0,
                         DeliveryCharges = f.DeliveryCharges ?? 0,
                         CashOrCredit = f.CashOrCredit ?? "",
                         Status = _unitOfWork.Db.Set<tblInvoiceStatus>().Where(x => x.InvoiceNumber == f.InvoiceNumber).OrderByDescending(x => x.InvoiceStatusId).Select(x => x.Status).FirstOrDefault() ?? "",
                     }).ToList();

            model = model.Where(x => x.Status.ToLower().Trim() == "dispatched" || x.Status.ToLower().Trim() == "delivered" || x.Status.ToLower().Trim() == "exchanged").ToList();
            int i;
            foreach (InvoiceViewModel d in model)
            {
                i = 0;
                d.InvoiceDetailsList = (from c in _unitOfWork.Db.Set<tblInvoiceDetail>()
                                        join p in _unitOfWork.Db.Set<tblProduct>() on c.ProductId equals p.ProductId
                                        join ca in _unitOfWork.Db.Set<tblCategory>() on p.CategoryId equals ca.CategoryId
                                        join cp in _unitOfWork.Db.Set<tblCategory>() on ca.ParentCategoryId equals cp.CategoryId
                                        join ap in _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>() on c.ProductAttributeDetailBarcodeId equals ap.ProductAttributeDetailBarcodeId
                                        join a1 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId1 equals a1.AttributeId
                                        join a2 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId2 equals a2.AttributeId
                                        join ad1 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId1 equals ad1.AttributeDetailId
                                        join ad2 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId2 equals ad2.AttributeDetailId
                                        where c.InvoiceNumber == d.InvoiceNumber
                                        select new InvoiceDetailViewModel
                                        {
                                            WarehouseId = c.WarehouseId,
                                            ProductId = c.ProductId,
                                            ProductName = p.ArticleNumber + " - " + p.ProductName ?? "",
                                            ChildCategoryId = ca.CategoryId,
                                            ChildCategoryName = ca.CategoryName ?? "",
                                            MasterCategoryId = cp.ParentCategoryId,
                                            MasterCategoryName = cp.CategoryName ?? "",
                                            TotalAmount = c.TotalAmount ?? 0,
                                            Rate = c.Rate ?? 0,
                                            Qty = c.Qty ?? 0,
                                            DiscType = c.DiscType ?? "Rs.",
                                            DiscRate = c.DiscRate ?? 0,
                                            DiscAmount = c.DiscAmount ?? 0,
                                            NetAmount = c.NetAmount ?? 0,
                                            ProductAttributeDetailBarcodeId = c.ProductAttributeDetailBarcodeId ?? 0,
                                            AttributeName = ad1.AttributeDetailName + " - " + ad2.AttributeDetailName,
                                            OfferDetailId = c.OfferDetailId ?? 0,
                                        }).ToList();
                if (report.ProductId > 0)
                {
                    if (d.InvoiceDetailsList.Any(x => x.ProductId == report.ProductId))
                    {
                        list.AddRange(model);
                    }
                    i++;
                }
                if (report.ChildCategoryId > 0 && i == 0)
                {
                    if (d.InvoiceDetailsList.Any(x => x.ChildCategoryId == report.ChildCategoryId))
                    {
                        list.AddRange(model);
                    }
                    i++;
                }
                if (report.ParentCategoryId > 0 && i == 0)
                {
                    if (d.InvoiceDetailsList.Any(x => x.MasterCategoryId == report.ParentCategoryId))
                    {
                        list.AddRange(model);
                    }
                    i++;
                }
            }

            return list;
        }
        public List<InvoiceViewModel> GetInvoicesStatusWiseList(DateFilterReportViewModel report, int loggedInUserId)
        {
            List<InvoiceViewModel> model = new List<InvoiceViewModel>();
            List<InvoiceViewModel> list = new List<InvoiceViewModel>();
            model = (from f in _unitOfWork.Db.Set<tblInvoice>()
                     where f.InvoiceDate >= report.FromDate && f.InvoiceDate <= report.ToDate
                     && f.InvoiceType == report.InvoiceType
                     select new InvoiceViewModel
                     {
                         InvoiceNumber = f.InvoiceNumber ?? "",
                         InvoiceType = f.InvoiceType ?? "",
                         InvoiceDate = f.InvoiceDate ?? DateTime.Now,
                         CustomerSupplierId = f.CustomerSupplierId ?? 0,
                         CustomerName = _unitOfWork.Db.Set<tblCustomerSupplier>()
                                        .Where(x => x.CustomerSupplierId == f.CustomerSupplierId)
                                        .Select(x => x.CustomerSupplierName)
                                        .FirstOrDefault() ?? "",
                         TotalAmount = f.TotalAmount ?? 0,
                         DiscType = f.DiscType ?? "Rs.",
                         DiscRate = f.DiscRate ?? 0,
                         DiscAmount = f.DiscAmount ?? 0,
                         NetAmount = f.NetAmount ?? 0,
                         DeliveryCharges = f.DeliveryCharges ?? 0,
                         CashOrCredit = f.CashOrCredit ?? "",
                         Status = _unitOfWork.Db.Set<tblInvoiceStatus>().Where(x => x.InvoiceNumber == f.InvoiceNumber).OrderByDescending(x => x.InvoiceStatusId).Select(x => x.Status).FirstOrDefault() ?? "",
                     }).ToList();

            if (!string.IsNullOrEmpty(report.Status))
            {
                model = model.Where(x => x.Status.ToLower().Trim() == report.Status.ToLower().Trim()).ToList();
            }
            int i;
            foreach (InvoiceViewModel d in model)
            {
                i = 0;
                d.InvoiceDetailsList = (from c in _unitOfWork.Db.Set<tblInvoiceDetail>()
                                        join p in _unitOfWork.Db.Set<tblProduct>() on c.ProductId equals p.ProductId
                                        join ca in _unitOfWork.Db.Set<tblCategory>() on p.CategoryId equals ca.CategoryId
                                        join cp in _unitOfWork.Db.Set<tblCategory>() on ca.ParentCategoryId equals cp.CategoryId
                                        join ap in _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>() on c.ProductAttributeDetailBarcodeId equals ap.ProductAttributeDetailBarcodeId
                                        join a1 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId1 equals a1.AttributeId
                                        join a2 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId2 equals a2.AttributeId
                                        join ad1 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId1 equals ad1.AttributeDetailId
                                        join ad2 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId2 equals ad2.AttributeDetailId
                                        where c.InvoiceNumber == d.InvoiceNumber
                                        select new InvoiceDetailViewModel
                                        {
                                            WarehouseId = c.WarehouseId,
                                            ProductId = c.ProductId,
                                            ProductName = p.ArticleNumber + " - " + p.ProductName ?? "",
                                            ChildCategoryId = ca.CategoryId,
                                            ChildCategoryName = ca.CategoryName ?? "",
                                            MasterCategoryId = cp.ParentCategoryId,
                                            MasterCategoryName = cp.CategoryName ?? "",
                                            TotalAmount = c.TotalAmount ?? 0,
                                            Rate = c.Rate ?? 0,
                                            Qty = c.Qty ?? 0,
                                            DiscType = c.DiscType ?? "Rs.",
                                            DiscRate = c.DiscRate ?? 0,
                                            DiscAmount = c.DiscAmount ?? 0,
                                            NetAmount = c.NetAmount ?? 0,
                                            ProductAttributeDetailBarcodeId = c.ProductAttributeDetailBarcodeId ?? 0,
                                            AttributeName = ad1.AttributeDetailName + " - " + ad2.AttributeDetailName,
                                            OfferDetailId = c.OfferDetailId ?? 0,
                                        }).ToList();
                if (report.ProductId > 0)
                {
                    if (d.InvoiceDetailsList.Any(x => x.ProductId == report.ProductId))
                    {
                        list.AddRange(model);
                    }
                    i++;
                }
                if (report.ChildCategoryId > 0 && i == 0)
                {
                    if (d.InvoiceDetailsList.Any(x => x.ChildCategoryId == report.ChildCategoryId))
                    {
                        list.AddRange(model);
                    }
                    i++;
                }
                if (report.ParentCategoryId > 0 && i == 0)
                {
                    if (d.InvoiceDetailsList.Any(x => x.MasterCategoryId == report.ParentCategoryId))
                    {
                        list.AddRange(model);
                    }
                    i++;
                }
            }

            return list;
        }
        public List<InvoiceDetailViewModel> GetProductSale(DateFilterReportViewModel report, int loggedInUserId)
        {
            List<InvoiceDetailViewModel> list = new List<InvoiceDetailViewModel>();
            list = (from f in _unitOfWork.Db.Set<tblInvoice>()
                    join c in _unitOfWork.Db.Set<tblInvoiceDetail>() on f.InvoiceNumber equals c.InvoiceNumber
                    join p in _unitOfWork.Db.Set<tblProduct>() on c.ProductId equals p.ProductId
                    join ca in _unitOfWork.Db.Set<tblCategory>() on p.CategoryId equals ca.CategoryId
                    join cp in _unitOfWork.Db.Set<tblCategory>() on ca.ParentCategoryId equals cp.CategoryId
                    join ap in _unitOfWork.Db.Set<tblProductAttributeDetailBarcode>() on c.ProductAttributeDetailBarcodeId equals ap.ProductAttributeDetailBarcodeId
                    join a1 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId1 equals a1.AttributeId
                    join a2 in _unitOfWork.Db.Set<tblAttribute>() on ap.AttributeId2 equals a2.AttributeId
                    join ad1 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId1 equals ad1.AttributeDetailId
                    join ad2 in _unitOfWork.Db.Set<tblAttributeDetail>() on ap.AttributeDetailId2 equals ad2.AttributeDetailId
                    where f.InvoiceDate >= report.FromDate && f.InvoiceDate <= report.ToDate
                    && f.InvoiceType == "Sale Invoice"
                    && report.ParentCategoryId > 0 ? ca.ParentCategoryId == report.ParentCategoryId : true
                    && report.ChildCategoryId > 0 ? ca.CategoryId == report.ParentCategoryId : true
                    && report.ProductId > 0 ? p.ProductId == report.ProductId : true
                    && report.ProductAttributeDetailBarcodeId > 0 ? c.ProductAttributeDetailBarcodeId == report.ProductAttributeDetailBarcodeId : true
                    select new InvoiceDetailViewModel
                    {
                        InvoiceNumber = f.InvoiceNumber ?? "",
                        InvoiceDate = f.InvoiceDate ?? DateTime.Now,
                        WarehouseId = c.WarehouseId,
                        ProductId = c.ProductId,
                        ProductName = p.ArticleNumber + " - " + p.ProductName ?? "",
                        ChildCategoryId = ca.CategoryId,
                        ChildCategoryName = ca.CategoryName ?? "",
                        MasterCategoryId = cp.ParentCategoryId,
                        MasterCategoryName = cp.CategoryName ?? "",
                        TotalAmount = c.TotalAmount ?? 0,
                        Rate = c.Rate ?? 0,
                        Qty = c.Qty ?? 0,
                        DiscType = c.DiscType ?? "Rs.",
                        DiscRate = c.DiscRate ?? 0,
                        DiscAmount = c.DiscAmount ?? 0,
                        NetAmount = c.NetAmount ?? 0,
                        ProductAttributeDetailBarcodeId = c.ProductAttributeDetailBarcodeId ?? 0,
                        AttributeName = ad1.AttributeDetailName + " - " + ad2.AttributeDetailName,
                        Status = _unitOfWork.Db.Set<tblInvoiceStatus>().Where(x => x.InvoiceNumber == f.InvoiceNumber).OrderByDescending(x => x.InvoiceStatusId).Select(x => x.Status).FirstOrDefault() ?? "",
                    }).ToList();

            return list;
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