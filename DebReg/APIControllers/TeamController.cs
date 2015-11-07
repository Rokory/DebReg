using DebReg.Security;
using DebReg.Web.APIModels;
using DebRegComponents;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DebReg.Web.APIControllers
{
    [Authorize]
    public class TeamController : ApiController
    {
        private TournamentRegistrationsManager tournamentRegistrationsManager;
        private DebRegUserManager userManager;


        // GET api/<controller>/5
        public async Task<Team> Get(Guid id)
        {
            var ident = HttpContext.Current.User.Identity as ClaimsIdentity;
            var userId = ident.GetUserId();

            var team = tournamentRegistrationsManager.GetTeam(id);


            if (userManager.HasTournamentRole(userId, team.TournamentId, DebReg.Models.TournamentRole.FinanceManager)
                || userManager.HasTournamentRole(userId, team.TournamentId, DebReg.Models.TournamentRole.OrganizationApprover)
                || userManager.HasTournamentRole(userId, team.TournamentId, DebReg.Models.TournamentRole.SlotManager))
            {

                return new Team(team);

            }
            else
            {
                return new Team { name = "Not authorized." };
            };
        }

        // GET api/<controller>?tournamentId=7376ec52-3192-4d47-a9ae-9836c0af155c
        public IEnumerable<Team> GetByTournamentId(Guid tournamentId)
        {
            var ident = HttpContext.Current.User.Identity as ClaimsIdentity;
            var userId = ident.GetUserId();

            if (userManager.HasTournamentRole(userId, tournamentId, DebReg.Models.TournamentRole.FinanceManager)
                    || userManager.HasTournamentRole(userId, tournamentId, DebReg.Models.TournamentRole.OrganizationApprover)
                    || userManager.HasTournamentRole(userId, tournamentId, DebReg.Models.TournamentRole.SlotManager)
                    || userManager.HasTournamentRole(userId, tournamentId, DebReg.Models.TournamentRole.ReportViewer))
            {
                var teams = tournamentRegistrationsManager.GetTeams(tournamentId);
                return from t in teams
                       select new Team(t);
            }
            return null;
        }


        //// POST api/<controller>
        //public void Post([FromBody]string value)
        //{
        //}

        //// PUT api/<controller>/5
        //public void Put(int id, [FromBody]string value)
        //{
        //}

        //// DELETE api/<controller>/5
        //public void Delete(int id)
        //{
        //}

        public TeamController(TournamentRegistrationsManager tournamentRegistrationsManager, DebRegUserManager userManager)
        {
            this.tournamentRegistrationsManager = tournamentRegistrationsManager;
            this.userManager = userManager;
        }
    }
}