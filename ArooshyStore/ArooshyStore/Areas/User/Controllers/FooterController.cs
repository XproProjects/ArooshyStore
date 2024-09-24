using ArooshyStore.BLL.Interfaces;
using ArooshyStore.BLL.Services;
using ArooshyStore.Models.ViewModels;
using System.Linq;
using System.Web.Mvc;

namespace ArooshyStore.Areas.User.Controllers
{
    public class FooterController : Controller
    {
        private readonly ICompanyRepository _repository;


        public FooterController(ICompanyRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            var categories = _repository.GetFooterDataForCompany();
            return PartialView("Index", categories);
        }


    }
}