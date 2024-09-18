using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ArooshyStore.Startup))]
namespace ArooshyStore
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {          
            ConfigureAuth(app);

        }
    }
}
