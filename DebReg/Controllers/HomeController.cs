using DebReg.Security;
using DebRegComponents;
using DebReg.Models;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;


namespace DebReg.Web.Controllers {
    [Authorize]
    public class HomeController : Controller {

        private Microsoft.Owin.Security.IAuthenticationManager AuthManager {
            get {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private DebRegUserManager UserManager {
            get {
                return HttpContext.GetOwinContext().GetUserManager<DebRegUserManager>();
            }
        }


        #region Actions

        // GET: Home
        public async Task<ActionResult> Index() {
            return RedirectToAction("Index", "TournamentRegistration");
        }

        #endregion

        #region Constructors
        #endregion
    }
}