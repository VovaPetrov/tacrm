using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebApplicationMVC.Startup))]
namespace WebApplicationMVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {


           

            ConfigureAuth(app);
        }
    }
}
