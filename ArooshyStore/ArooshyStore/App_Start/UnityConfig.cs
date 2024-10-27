using System.Web.Mvc;
using ArooshyStore.BLL.GenericRepository;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using Unity;
using Unity.Mvc5;

namespace ArooshyStore
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();
            container.RegisterType<IAccountRepository, AccountRepository>();
            container.RegisterType<IUnitOfWork, UnitOfWork>();
            container.RegisterType<IHomeRepository, HomeRepository>();
            container.RegisterType<ICheckUserRoleRepository, CheckUserRoleRepository>();
            container.RegisterType<ICombolistRepository, CombolistRepository>();
            container.RegisterType<IDocumentRepository, DocumentRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IUserTypeRepository, UserTypeRepository>();
            container.RegisterType<IModuleRepository, ModuleRepository>();
            container.RegisterType<IActionRepository, ActionRepository>();
            container.RegisterType<IUnitRepository, UnitRepository>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<IWarehouseRepository, WarehouseRepository>();
            container.RegisterType<IAttributeRepository, AttributeRepository>();
            container.RegisterType<IProductRepository, ProductRepository>();
            container.RegisterType<ICityRepository, CityRepository>();
            container.RegisterType<ICustomerSupplierRepository, CustomerSupplierRepository>();
            container.RegisterType<IDiscountOfferRepository, DiscountOfferRepository>();
            container.RegisterType<IExpenseTypeRepository, ExpenseTypeRepository>();
            container.RegisterType<IExpenseRepository, ExpenseRepository>();
            container.RegisterType<IErrorLogRepository, ErrorLogRepository>();
            container.RegisterType<IInvoiceRepository, InvoiceRepository>();
            container.RegisterType<ICarouselRepository, CarouselRepository>();
            container.RegisterType<ICompanyRepository, CompanyRepository>();
            container.RegisterType<IAboutRepository, AboutRepository>();
            container.RegisterType<IDeliveryChargesRepository, DeliveryChargesRepository>();
            container.RegisterType<IDeliveryInfoRepository, DeliveryInfoRepository>();
            container.RegisterType<IProductTagRepository, ProductTagRepository>();
            container.RegisterType<IProductReviewRepository, ProductReviewRepository>();
            container.RegisterType<IProductWishlistRepository, ProductWishlistRepository>();
            container.RegisterType<IProductCartRepository, ProductCartRepository>();
            container.RegisterType<IEmployeeRepository, EmployeeRepository>();
            container.RegisterType<IDesignationRepository, DesignationRepository>();
            container.RegisterType<IEmployeeAttendanceRepository, EmployeeAttendanceRepository>();
            container.RegisterType<ISalaryRepository, SalaryRepository>();

            DependencyResolver.SetResolver(new UnityDependencyResolver(container));
        }
    }
}