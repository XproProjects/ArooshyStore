using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using ArooshyStore.DAL.Entities;

namespace ArooshyStore.DAL.Data
{
    public class ModelDbContext : DbContext
    {
        public ModelDbContext() : base("ModelDb")
        {
        }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
           
        }
        public virtual DbSet<tblAction> tblAction { get; set; }
        public virtual DbSet<tblAssignAction> tblAssignAction { get; set; }
        public virtual DbSet<tblDocument> tblDocument { get; set; }
        public virtual DbSet<tblErrorsLog> tblErrorsLog { get; set; }
        public virtual DbSet<tblInfo> tblInfo { get; set; }
        public virtual DbSet<tblModule> tblModule { get; set; }
        public virtual DbSet<tblUser> tblUser { get; set; }
        public virtual DbSet<tblUserRole> tblUserRole { get; set; }
        public virtual DbSet<tblUserType> tblUserType { get; set; }
        public virtual DbSet<tblLogging> tblLogging { get; set; }
        public virtual DbSet<tblSecurityGroup> tblSecurityGroup { get; set; }
        public virtual DbSet<tblSecurityGroupDetail> tblSecurityGroupDetail { get; set; }
        public virtual DbSet<tblUnit> tblUnit { get; set; }
        public virtual DbSet<tblCategory> tblCategory { get; set; }
        public virtual DbSet<tblAttribute> tblAttribute { get; set; }
        public virtual DbSet<tblWarehouse> tblWarehouse { get; set; }
        public virtual DbSet<tblAttributeDetail> tblAttributeDetail { get; set; }
        public virtual DbSet<tblProduct> tblProduct { get; set; }
        public virtual DbSet<tblProductAttributeDetail> tblProductAttributeDetail { get; set; }
        public virtual DbSet<tblCity> tblCity { get; set; }
        public virtual DbSet<tblCustomerSupplier> tblCustomerSupplier { get; set; }
        public virtual DbSet<tblDiscountOffer> tblDiscountOffer { get; set; }
        public virtual DbSet<tblExpenseType> tblExpenseType { get; set; }
        public virtual DbSet<tblExpense> tblExpense { get; set; }
        public virtual DbSet<tblInvoice> tblInvoice { get; set; }
        public virtual DbSet<tblInvoiceDetail> tblInvoiceDetail { get; set; }
        public virtual DbSet<tblCarousel> tblCarousel { get; set; }
        public virtual DbSet<tblCompany> tblCompany { get; set; }
        public virtual DbSet<tblAbout> tblAbout { get; set; }
        public virtual DbSet<tblDeliveryCharges> tblDeliveryCharges { get; set; }
        public virtual DbSet<tblProductTags> tblProductTags { get; set; }
        public virtual DbSet<tblDeliveryInfo> tblDeliveryInfo { get; set; }
        public virtual DbSet<tblTagsForProducts> tblTagsForProducts { get; set; }
        public virtual DbSet<tblContactus> tblContactus { get; set; }
        public virtual DbSet<tblProductReview> tblProductReview { get; set; }
        public virtual DbSet<tblProductCart> tblProductCart { get; set; }
        public virtual DbSet<tblProductCartAttributeDetail> tblProductCartAttributeDetail { get; set; }
        public virtual DbSet<tblProductWishlist> tblProductWishlist { get; set; }
        public virtual DbSet<tblInvoiceStatus> tblInvoiceStatus { get; set; }
        public virtual DbSet<tblDesignation> tblDesignation { get; set; }
        public virtual DbSet<tblEmployee> tblEmployee { get; set; }
        public virtual DbSet<tblEmployeeAttendance> tblEmployeeAttendance { get; set; }
        public virtual DbSet<tblSalary> tblSalary { get; set; }
        public virtual DbSet<tblProductAttributeDetailBarcode> tblProductAttributeDetailBarcode { get; set; }
        public virtual DbSet<tblAttributeLog> tblAttributeLog { get; set; }
        public virtual DbSet<tblAttributeDetailLog> tblAttributeDetailLog { get; set; }
        public virtual DbSet<tblProductLog> tblProductLog { get; set; }
        public virtual DbSet<tblProductAttributeDetailLog> tblProductAttributeDetailLog { get; set; }
        public virtual DbSet<tblProductAttributeDetailBarcodeLog> tblProductAttributeDetailBarcodeLog { get; set; }
        public virtual DbSet<tblProductStock> tblProductStock { get; set; }
        public virtual DbSet<tblProductStockLog> tblProductStockLog { get; set; }
        public virtual DbSet<tblDiscountOfferLog> tblDiscountOfferLog { get; set; }
        public virtual DbSet<tblDiscountOfferDetail> tblDiscountOfferDetail { get; set; }
        public virtual DbSet<tblDiscountOfferDetailLog> tblDiscountOfferDetailLog { get; set; }
        public virtual DbSet<tblCustomerSupplierLog> tblCustomerSupplierLog { get; set; }
        public virtual DbSet<tblInvoiceLog> tblInvoiceLog { get; set; }
        public virtual DbSet<tblInvoiceDetailLog> tblInvoiceDetailLog { get; set; }
    }

}