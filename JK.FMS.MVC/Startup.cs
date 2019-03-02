
using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

[assembly: OwinStartupAttribute(typeof(JK.FMS.MVC.Startup))]
namespace JK.FMS.MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           // EntityFrameworkCache.Initialize(new InMemoryCache());
            ConfigureAuth(app);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/JKControl/User"),
                ExpireTimeSpan = TimeSpan.FromMinutes(720)
            });

        }
    }
}
