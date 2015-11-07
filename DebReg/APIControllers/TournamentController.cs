using DebReg.Models.Comparers;
using DebReg.Security;
using DebRegComponents;
using DebReg.Web.APIModels;
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
    public class TournamentController : ApiController
    {
        private DebRegUserManager userManager;
        private TournamentManager tournamentManager;

        // GET api/<controller>
        /// <summary>
        /// Get all tournaments, where the current user has either any role, is speaker, or adjudicator
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Tournament>> Get()
        {
            var ident = HttpContext.Current.User.Identity as ClaimsIdentity;
            var userId = ident.GetUserId();
            var user = await userManager.FindByIdAsync(userId);


            if (user != null)
            {
                List<DebReg.Models.Tournament> tournaments = new List<DebReg.Models.Tournament>();
                foreach (var team in user.Teams)
                {
                    tournaments.Add(team.Tournament);
                }

                foreach (var adjudicator in user.Adjudicator)
                {
                    tournaments.Add(adjudicator.Tournament);
                }

                foreach (var role in user.TournamentRoles)
                {
                    tournaments.Add(role.Tournament);
                }

                tournaments = tournaments.Distinct(new TournamentComparer()).ToList();
                return from tournament in tournaments
                       select new Tournament(tournament, user);
            }

            return null;
        }

        // GET api/<controller>/5
        /// <summary>
        /// Gets the tournament specified by id.
        /// </summary>
        /// <param name="id">
        /// The id of the tournament.
        /// </param>
        /// <returns></returns>
        public Tournament Get(Guid id)
        {
            return new Tournament(tournamentManager.GetTournament(id));
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

        public TournamentController(TournamentManager tournamentManager, DebRegUserManager userManager)
        {
            this.tournamentManager = tournamentManager;
            this.userManager = userManager;

        }
    }
}