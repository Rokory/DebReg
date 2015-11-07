using DebReg.Web.APIModels;
using DebReg.Web.App_Start;
using Microsoft.Owin.Security.OAuth;
using Microsoft.Practices.Unity.Mvc;
using System.Linq;
using System.Web.Http;
//using System.Web.Http.OData.Builder;
using System.Web.Mvc;
using System.Web.OData.Builder;
using System.Web.OData.Extensions;

namespace DebReg.Web
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {

            // Configure Dependency Injection

            var container = UnityConfig.GetConfiguredContainer();

            FilterProviders.Providers.Remove(FilterProviders.Providers.OfType<FilterAttributeFilterProvider>().First());
            FilterProviders.Providers.Add(new UnityFilterAttributeFilterProvider(container));

            config.DependencyResolver = new UnityResolver(container);

            // Configure OAuth

            //config.SuppressDefaultHostAuthentication();
            config.Filters.Add(new HostAuthenticationFilter(OAuthDefaults.AuthenticationType));


            // Configure OData
            //System.Web.OData.Builder.ODataModelBuilder builder4 = new System.Web.OData.Builder.ODataConventionModelBuilder();
            //builder4.EntitySet<Tournament>("Tournaments");
            //builder4.EntitySet<User>("Attendees");
            //var edmModel4 = builder4.GetEdmModel();

            ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
            builder.EntitySet<Tournament>("Tournaments");
            //builder.EntitySet<User>("Attendees");
            var edmModel = builder.GetEdmModel();


            // Configure routes

            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApiWithId",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            //config.MapODataServiceRoute
            //(
            //    routeName: "OData4Route",
            //    routePrefix: "odata4",
            //    model: edmModel4

            //);

            config.MapODataServiceRoute
            (
                routeName: "ODataRoute",
                routePrefix: "odata",
                model: edmModel
            );
        }
    }
}