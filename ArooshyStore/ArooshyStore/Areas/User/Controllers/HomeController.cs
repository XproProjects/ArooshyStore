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


        public HomeController(IProductRepository repository, ICategoryRepository category, ICarouselRepository carousel)
        {
            _repository = repository;
            _category = category;
            _carousel = carousel;

        }
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult FeaturedProducts()
        {
            var featuredProducts = _repository.GetFeaturedProducts();
            return PartialView("FeaturedProducts", featuredProducts);
        }
        public ActionResult LatestProducts()
        {
            var categories = _category.GetCategories();
            var products = _repository.GetAllProducts();
            var model = new ProductViewModel
            {
                Categories = categories.ToList(),
                Products = products.ToList()
            };
            return PartialView("LatestProducts", model);

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

        public ActionResult Categories()
        {
            var categories = _category.GetCategories();
            return PartialView("Categories", categories);
        }
        public ActionResult About()
        {
            return View();
        }
    }
}