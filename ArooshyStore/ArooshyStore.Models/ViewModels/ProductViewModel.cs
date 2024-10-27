using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ArooshyStore.Models.ViewModels
{
    public class ProductViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductNameUrdu { get; set; }
        public string ProductDescription { get; set; }
        public string ParentCategoryName { get; set; }
        public string MasterCategoryName { get; set; }
        public string ChildCategoryName { get; set; }
        public string DeliveryInfoDetail { get; set; }
        public string Barcode { get; set; }
        public int TagId { get; set; }
        public string TagName { get; set; }
        public string DeliveryInfoName { get; set; }
        public int? LoggedUserId { get; set; }

        public int? UnitId { get; set; }
        public int? DeliveryInfoId { get; set; }
        public string UnitName { get; set; }
        public int? CategoryId { get; set; }
        public int? AttributeId { get; set; }
        public int? UserId { get; set; }
        public string CategoryName { get; set; }
        public decimal CostPrice { get; set; }
        public decimal SalePrice { get; set; }
        public bool? Status { get; set; }
        public string StatusString { get; set; }
        public bool? IsFeatured { get; set; }
        public string IsFeaturedString { get; set; }
        public string ImagePath { get; set; }
        public int DocumentId { get; set; }
        public int? InfoId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? CreatedBy { get; set; }
        public string CreatedByString { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public int? UpdatedBy { get; set; }
        public string UpdatedByString { get; set; }
        public int TotalRecords { get; set; }
        public string AllAttributes { get; set; }
        public List<AttributeViewModel> AttributesList { get; set; }
        public List<ProductViewModel> Products { get; set; }
        public List<CategoryViewModel> Categories { get; set; }
        public List<TagsForProductsViewModel> Tags { get; set; }
        public List<DiscountOfferViewModel> DiscountOffer { get; set; }


    }
}
