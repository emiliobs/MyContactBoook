using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyContactBoook.Startup))]
namespace MyContactBoook
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
