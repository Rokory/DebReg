using DebReg.Models;
using DebReg.Security;
using DebRegCommunication;
using DebRegComponents;
using DebReg.Web.APIModels;
using Microsoft.AspNet.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace DebReg.Web.APIControllers
{
    [Authorize]
    public class SpeakerController : ApiController
    {
        private DebRegUserManager userManager;
        private TournamentRegistrationsManager tournamentRegistrationsManager;

        [HttpPost]
        public async Task<SetTeamOrAdjudicatorResult> PostSpeaker(Speaker speaker)
        {
            var ident = HttpContext.Current.User.Identity as ClaimsIdentity;
            String userId = ident.GetUserId();
            var user = await userManager.FindByIdAsync(userId);

            var team = tournamentRegistrationsManager.GetTeam(speaker.teamId);

            if (team == null)
            {
                return SetTeamOrAdjudicatorResult.TeamNotFound;
            }

            // Check if user has permissions

            if (!userManager.HasOrganizationRole(userId, team.OrganizationId, OrganizationRole.Delegate))
            {
                // User does not have permissions
                return SetTeamOrAdjudicatorResult.NotAuthorized;
            }

            // Add speaker to team

            var result = await tournamentRegistrationsManager.AddSpeakerAsync(speaker.teamId, speaker.userId, user);
            return result;
        }

        [HttpDelete]
        public async Task<SetTeamOrAdjudicatorResult> DeleteSpeaker(Speaker speaker)
        {
            var ident = HttpContext.Current.User.Identity as ClaimsIdentity;
            String userId = ident.GetUserId();
            var user = await userManager.FindByIdAsync(userId);

            var team = tournamentRegistrationsManager.GetTeam(speaker.teamId);

            if (team == null)
            {
                return SetTeamOrAdjudicatorResult.TeamNotFound;
            }

            // Check if user has permissions

            if (!userManager.HasOrganizationRole(userId, team.OrganizationId, OrganizationRole.Delegate))
            {
                // User does not have permissions
                return SetTeamOrAdjudicatorResult.NotAuthorized;
            }

            var result = tournamentRegistrationsManager.RemoveSpeaker(speaker.teamId, speaker.userId, user);
            return result;
        }

        public SpeakerController(DebRegUserManager userManager, TournamentRegistrationsManager tournamentRegistrationsManager)
        {
            this.userManager = userManager;
            this.tournamentRegistrationsManager = tournamentRegistrationsManager;
        }
    }
}
