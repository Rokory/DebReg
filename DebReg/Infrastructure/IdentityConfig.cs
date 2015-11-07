using DebReg.Data;
using DebReg.Models;
using DebReg.Security;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using System;

namespace DebReg.Web.Infrastructure
{
    public class IdentityConfig
    {
        public static string PublicClientId { get; set; }
        public OAuthAuthorizationServerOptions OAuthOptions { get; set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864

        public void Configuration(IAppBuilder app)
        {
            app.CreatePerOwinContext<IdentityDbContext<User>>(DebRegContext.Create);
            app.CreatePerOwinContext<DebRegUserManager>(DebRegUserManager.Create);


            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/User/Login")
            });

            // Configure the application for OAuth based flow

            PublicClientId = "self";
            OAuthOptions = new OAuthAuthorizationServerOptions
            {
                TokenEndpointPath = new PathString("/Token"),
                Provider = new ApplicationOAuthProvider(PublicClientId),
                AccessTokenExpireTimeSpan = TimeSpan.FromDays(30),
            };

#if DEBUG
            OAuthOptions.AllowInsecureHttp = true;
#endif

            // Enable the application to use bearer tokens to authenticate users
            //app.UseOAuthBearerTokens(OAuthOptions);

            OAuthBearerAuthenticationOptions oAuthBearerAuthenticationOptions = new OAuthBearerAuthenticationOptions
            {
                Provider = new QueryStringOAuthBearerProvider("token")
            };
            app.UseOAuthBearerAuthentication(oAuthBearerAuthenticationOptions);
            app.UseOAuthAuthorizationServer(OAuthOptions);

        }


    }
}