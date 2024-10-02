using System.Web.Mvc;
using ArooshyStore.Authentication.FormsAuthentication;

namespace ArooshyStore.Areas.User.Controllers
{
    public class BaseController : Controller
    {
        protected virtual new CustomPrincipal User
        {
            get { return HttpContext.User as CustomPrincipal; }
        }
    }
}