using DebReg.Data;
using DebReg.Models.Comparers;
using DebReg.Security;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DebReg.Web.Areas.TournamentManagement.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IUnitOfWork unitOfWork;
        private ISecurityManager securityManager;
        private DebRegUserManager userManager;

        //  GET: TournamentManagement/Home
        public async Task<ActionResult> Index()
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournament = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournament;

            // Check if tournament exists and user has a role
            if (tournament == null ||
                tournament.UserRoles
                    .FirstOrDefault(r => r.UserId == ident.GetUserId()) == null)
            {

                return RedirectToAction("SelectTournament");
            }

            return View(tournament);
        }

        // GET: TournamentManagement/Home/SelectTournament
        public ActionResult SelectTournament()
        {
            var user = userManager.FindByName(HttpContext.User.Identity.Name);

            var tournamentRoles = user.TournamentRoles.OrderBy(
                r => r.Tournament.Start).Distinct(new TournamentUserRoleTournamentComparer());

            // If a user has no tournament roles, he should not be here

            if (tournamentRoles.Count() == 0)
            {
                return RedirectToAction("Index", "Home", new { Area = "" });
            }

            return View(tournamentRoles);
        }

        // POST: TournamentManagement/Home/SelectTournament
        [HttpPost]
        public async Task<ActionResult> SelectTournament(Guid id)
        {

            // Get user

            var user = userManager.FindByName(HttpContext.User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "User", new { Area = "", returnUrl = HttpContext.Request.RawUrl });
            }

            // Set currentTournamentId


            user.CurrentTournamentId = id;
            await userManager.UpdateAsync(user);

            // Repeat signin of user to compose claim

            await securityManager.LoginAsync(user);

            return RedirectToAction("Index");
        }


        public HomeController(IUnitOfWork unitOfWork, ISecurityManager securityManager, DebRegUserManager userManager)
        {
            this.unitOfWork = unitOfWork;
            this.securityManager = securityManager;
            this.userManager = userManager;
        }
    }
}