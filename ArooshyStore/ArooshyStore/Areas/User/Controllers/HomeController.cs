using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace ArooshyStore.Areas.User.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IProductRepository _repository;
        private readonly ICategoryRepository _category;
        private readonly ICarouselRepository _carousel;
        private readonly IAboutRepository _aboutUs;
        public HomeController(IProductRepository repository, ICategoryRepository category, ICarouselRepository carousel, IAboutRepository aboutUs)
        {
            _repository = repository;
            _category = category;
            _carousel = carousel;
            _aboutUs = aboutUs;

        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Header()
        {

            var model = new HeaderViewModel
            {
               
            };

            if (User != null)
            {
                var userId = User.UserId;
                model.ReviewByCustomerId = userId;
               
            }

            var categories = _category.GetCategoriesForHeader();
            return PartialView(categories); 
           
        }
        public ActionResult FeaturedProducts()
        {
            var featuredProducts = _repository.GetFeaturedProducts();
            return PartialView("FeaturedProducts", featuredProducts);
        }
        public ActionResult GetProductDetails(int productId)
        {
            var product = _repository.GetProductWithAttributes(productId);
            return View(product);
        }
        public ActionResult GetSimilarProducts(int productId)
        {
            var product = _repository.GetSimilrProducts(productId);
            return PartialView("GetSimilarProducts", product);
        }
        public ActionResult NewArrivalProducts()
        {
            var newArrivalProducts = _repository.GetNewArrivalProducts();
            return PartialView("NewArrivalProducts", newArrivalProducts);
        }
        public ActionResult MasterCategories()
        {
            var masterCategories = _category.GetMasterCategories();           
            return PartialView("MasterCategories", masterCategories);

        }
        [HttpGet]
        public ActionResult ExpiredProducts()
        {
            var masterCategories = _repository.GetExpiredProducts();
            return PartialView("ExpiredProducts", masterCategories);

        }
        public ActionResult ExpiredIndex()
        {
            return View();
        }
        public ActionResult SetSearchString(bool? categoryCheckbox, int[] category, bool? attributeCheckbox, int[] attribute,
           bool? discountCheckbox, int[] discount, decimal? minPrice, decimal? maxPrice, string sortBy)
        {
            var filteredProducts = _repository.GetFilteredProductsForExpired(categoryCheckbox, category, attributeCheckbox, attribute, discountCheckbox, discount, minPrice, maxPrice, sortBy);
            return PartialView("ExpiredProducts", filteredProducts);
        }
        [HttpGet]
        public ActionResult ExpiredByMasterCategory(int masterCategoryId)
        {
            var expiredProducts = _repository.GetProductsByMasterCategory(masterCategoryId);
            return View(expiredProducts);
        }
        public ActionResult Carousels()
        {
            var carousels = _carousel.GetAllCarousels();
            var model = new CarouselViewModel
            {
                Carousels = carousels
            };
            return PartialView("Carousels", model);

        }

        public ActionResult BrowseCategories()
        {
            var categories = _category.GetBrowseCategories();
            return PartialView("BrowseCategories", categories);
        }
        public ActionResult AboutUs()
        {
            var aboutUs = _aboutUs.GetAboutUs();
            return View(aboutUs);
        }
    }
}