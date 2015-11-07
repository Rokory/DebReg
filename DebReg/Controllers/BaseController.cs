using DebReg.Security;
using Microsoft.AspNet.Identity.Owin;
using System.Web;
using System.Web.Mvc;


namespace DebReg.Web.Controllers
{
    public abstract class BaseController : Controller
    {
        //protected DebRegUserManager UserManager
        //{
        //    get
        //    {
        //        if (HttpContext != null
        //            && HttpContext.GetOwinContext() != null)
        //        {
        //            return HttpContext.GetOwinContext().GetUserManager<DebRegUserManager>();
        //        }
        //        return null;
        //    }
        //}

        protected Microsoft.Owin.Security.IAuthenticationManager AuthManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

    }
}