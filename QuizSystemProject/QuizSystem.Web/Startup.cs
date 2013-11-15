using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(QuizSystem.Web.Startup))]
namespace QuizSystem.Web
{
    public partial class Startup 
    {
        public void Configuration(IAppBuilder app) 
        {
            ConfigureAuth(app);
        }
    }
}
