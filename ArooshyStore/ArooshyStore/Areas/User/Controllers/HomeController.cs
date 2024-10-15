using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace ArooshyStore.Areas.User.Controllers
{
    public class HomeController : Controller
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
          
            var model = _category.GetCategoriesForHeader();
            return PartialView(model); 
           
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