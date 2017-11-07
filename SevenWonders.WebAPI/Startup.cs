using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;

[assembly: OwinStartup(typeof(SevenWonders.WebAPI.Startup))]

namespace SevenWonders.WebAPI
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
           // ConfigureAuth(app);

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                LoginPath = new PathString("/Account/Login"),
            });
        }
    }
}
