using DebReg.Web;
using System.Configuration;
using System.Data.Entity.Migrations;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace DebReg.Web
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            if (bool.Parse(ConfigurationManager.AppSettings["MigrateDatabaseToLatestVersion"]))
            {
                var configuration = new DebReg.Data.Migrations.Configuration();
                var migrator = new DbMigrator(configuration);
                migrator.Update();
            }

            GlobalConfiguration.Configure(WebApiConfig.Register); // register Web API

            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            ModelBinderConfig.RegisterModelBinders(ModelBinders.Binders);

        }
    }
}
