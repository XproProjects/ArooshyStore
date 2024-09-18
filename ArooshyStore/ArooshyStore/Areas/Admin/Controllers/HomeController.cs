using System.Collections.Generic;
using System.Web.Mvc;
using ArooshyStore.App_Start;
using ArooshyStore.BLL.BusinessInfo;
using ArooshyStore.BLL.Interfaces;
using ArooshyStore.Models.ViewModels;
using AutoMapper;
using Serilog;

namespace ArooshyStore.Areas.Admin.Controllers
{
    public class HomeController : BaseController
    {
        private readonly IUserRepository _Users;
        private readonly IHomeRepository _repository;
        private readonly ICheckUserRoleRepository _roles;
        private readonly IMapper _mapper;
        private readonly Serilog.ILogger _logger;

        public HomeController(IHomeRepository repository, ICheckUserRoleRepository roles,IUserRepository Users)
        {

            _repository = repository;
            _mapper = AutoMapperConfig.Mapper;
            _roles = roles;
            _Users = Users;
            _logger = Log.ForContext<HomeController>();
        }
        //When we call it from other actions or after login
        //[Route("admin/home/index")]
        public ActionResult Index()
        {
            if (User != null)
            {
                //Check if user has access of this module or not
                if (_roles.CheckModuleRoleId(User.UserId, "dashboard") > 0)
                {
                    //Get list of all actions of this module
                    List<ModuleViewModel> actionList = new List<ModuleViewModel>();
                    actionList = _roles.ActionsList(User.UserId, "dashboard");

                    return View(actionList);
                }
                else
                {
                    return RedirectToAction("accessdenied", "home");
                }
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        
        //[Route("")]
        //public ActionResult Index(string id = "login")
        //{
        //    if (User != null)
        //    {
        //        //Check if user has access of this module or not
        //        if (_roles.CheckModuleRoleId(User.UserId, "dashboard") > 0)
        //        {
        //            //Get list of all actions of this module
        //            List<ModuleViewModel> actionList = new List<ModuleViewModel>();
        //            actionList = _roles.ActionsList(User.UserId, "dashboard");

        //            return View(actionList);
        //        }
        //        else
        //        {
        //            return RedirectToAction("accessdenied", "home");
        //        }
        //    }
        //    else
        //    {
        //        return RedirectToAction("login", "account");
        //    }
        //}
        public ActionResult AccessDenied()
        {
            if (User != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        public ActionResult UnderDevelopment()
        {
            if (User != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
        //[ChildActionOnly]
        //[ActionName("_SideMenu")]
        public ActionResult AsideMenu()
        {
            #region Set Business Detail
            BusinessInfo business = BusinessInfo.GetInstance;
            List<string> businessString = new List<string>();
            businessString = business.GetBusinessInfo();
            ViewBag.BusinessName = businessString[0];
            ViewBag.BusinessLogo = businessString[1];
            //ViewBag.BannerImage = businessString[2];
            ViewBag.BannerImage = businessString[2];
            ViewBag.BgColor = businessString[3];
            #endregion
            ModuleViewModel role = new ModuleViewModel();
            role.ModuleList = _roles.ModulesList(User.UserId);
            return PartialView(role);
        }
        public ActionResult QuickAccess()
        {
            var a = User.UserId;
            ModuleViewModel role = new ModuleViewModel();
            role.ModuleList = _roles.ModulesList(User.UserId);
            return PartialView(role);
        }
        //Action Method for header top shortcut
        public ActionResult ModalHeaderShortcut()
        {
            var a = User.UserId;
            ModuleViewModel role = new ModuleViewModel();
            role.ModuleList = _roles.ModulesList(User.UserId);
            return PartialView(role);
        }
        public ActionResult Setting(string layoutType)
        {
            if (User != null)
            {
                ModuleViewModel role = new ModuleViewModel();
                role.ModuleList = _roles.ModulesList(User.UserId);
                return View(role);
            }
            else
            {
                return RedirectToAction("login", "account");
            }
        }
    }


}