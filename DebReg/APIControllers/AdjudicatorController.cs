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
    public class AdjudicatorController : ApiController
    {
        private DebRegUserManager userManager;
        private ITournamentRegistrationsManager tournamentRegistrationsManager;

        // GET api/<controller>?tournamentId=7376ec52-3192-4d47-a9ae-9836c0af155c
        [HttpGet]
        public IEnumerable<Adjudicator> GetAdjudicators(Guid tournamentId)
        {
            var ident = HttpContext.Current.User.Identity as ClaimsIdentity;
            var userId = ident.GetUserId();

            if (userManager.HasTournamentRole(userId, tournamentId, DebReg.Models.TournamentRole.FinanceManager)
                    || userManager.HasTournamentRole(userId, tournamentId, DebReg.Models.TournamentRole.OrganizationApprover)
                    || userManager.HasTournamentRole(userId, tournamentId, DebReg.Models.TournamentRole.SlotManager)
                    || userManager.HasTournamentRole(userId, tournamentId, DebReg.Models.TournamentRole.ReportViewer))
            {
                var adjudicators = tournamentRegistrationsManager.GetAdjudicators(tournamentId);
                return from a in adjudicators
                       select new Adjudicator(a);
            }
            return null;

        }


        [HttpPost]
        public async Task<SetTeamOrAdjudicatorResult> PostAdjudicator(APIModels.Adjudicator adjudicator)
        {
            var ident = HttpContext.Current.User.Identity as ClaimsIdentity;
            String userId = ident.GetUserId();
            var user = await userManager.FindByIdAsync(userId);


            // Check if user has permissions

            if (adjudicator.organizationId == null
                || !userManager.HasOrganizationRole(userId, (Guid)adjudicator.organizationId, DebReg.Models.OrganizationRole.Delegate))
            {
                // User does not have permissions
                return SetTeamOrAdjudicatorResult.NotAuthorized;
            }

            // Add adjudicator

            var result = await tournamentRegistrationsManager.AddAdjudicatorAsync(adjudicator.tournamentId, (Guid)adjudicator.organizationId, adjudicator.userId, user);

            return result;
        }

        [HttpDelete]
        public async Task<SetTeamOrAdjudicatorResult> DeleteAdjudicator(APIModels.Adjudicator adjudicator)
        {
            var ident = HttpContext.Current.User.Identity as ClaimsIdentity;
            String userId = ident.GetUserId();
            var user = await userManager.FindByIdAsync(userId);

            // Check if user has permissions


            if (adjudicator.organizationId == null
                || !userManager.HasOrganizationRole(userId, (Guid)adjudicator.organizationId, DebReg.Models.OrganizationRole.Delegate))
            {
                // User does not have permissions
                return SetTeamOrAdjudicatorResult.NotAuthorized;
            }

            var result = tournamentRegistrationsManager.RemoveAdjudicator(adjudicator.tournamentId, adjudicator.userId, user);
            return result;
        }

        public AdjudicatorController(DebRegUserManager userManager, ITournamentRegistrationsManager tournamentRegistrationsManager)
        {
            this.userManager = userManager;
            this.tournamentRegistrationsManager = tournamentRegistrationsManager;
        }
    }
}
