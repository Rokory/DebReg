using DebReg.Data;
using DebReg.Models;
using System;
using System.Web.Mvc;

namespace DebReg.Web.Controllers {
    [Authorize]
    public class TournamentController : Controller {
        private IUnitOfWork unitOfWork;

        // GET: Tournament
        public ActionResult Index() {
            return View();
        }

        [ChildActionOnly]
        public ActionResult DisplayPartial(Guid tournamentId) {
            var tournament = unitOfWork.GetRepository<Tournament>().GetById(tournamentId);
            if (tournament == null) {
                tournament = new Tournament();
            }
            return PartialView(tournament);
        }

        public TournamentController(IUnitOfWork unitOfWork) {
            this.unitOfWork = unitOfWork;
        }
    }
}