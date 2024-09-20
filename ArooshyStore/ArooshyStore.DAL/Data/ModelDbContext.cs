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
    }

}