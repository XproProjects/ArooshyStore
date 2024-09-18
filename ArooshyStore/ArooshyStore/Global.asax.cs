using System;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Script.Serialization;
using System.Web.Security;
using System.Web.UI.WebControls;
using ArooshyStore.App_Start;
using ArooshyStore.Areas.Admin;
using ArooshyStore.Authentication.FormsAuthentication;
using Serilog;

namespace ArooshyStore
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            UnityConfig.RegisterComponents();
            AutoMapperConfig.Init();
        }
        protected void Application_PostAuthenticateRequest(Object sender, EventArgs e)
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];

            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                CustomPrincipalSerializeModel serializeModel = serializer.Deserialize<CustomPrincipalSerializeModel>(authTicket.UserData);
                CustomPrincipal newUser = new CustomPrincipal(authTicket.Name);
                newUser.UserId = serializeModel.UserId;
                newUser.Name = serializeModel.Name;
                newUser.Email = serializeModel.Email;
                newUser.UserType = serializeModel.UserType;
                newUser.CompanyId = serializeModel.CompanyId;
                newUser.BranchId = serializeModel.BranchId;
                newUser.BranchName = serializeModel.BranchName;
                HttpContext.Current.User = newUser;
            }
        }
        protected void Application_Error(object sender_, CommandEventArgs e_)
        {
            Exception exception = Server.GetLastError();
            if (exception is CryptographicException)
            {
                FormsAuthentication.SignOut();
            }
        }
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception exception = Server.GetLastError();
            Log.Error(exception, "Unhandled application error");
            Server.ClearError(); // Clear the error to prevent default error handling
        }
    }
}
