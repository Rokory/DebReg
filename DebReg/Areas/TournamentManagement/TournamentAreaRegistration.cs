using System.Web.Mvc;

namespace DebReg.Web.Areas.TournamentManagement
{
    public class TournamentManagementAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "TournamentManagement";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                name: "TournamentManagement_default",
                url: "TournamentManagement/{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional },
                namespaces: new string[] { 
                    "DebReg.Web.Areas.TournamentManagement.Controllers"
                }
            );
        }
    }
}