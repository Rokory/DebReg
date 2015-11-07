using System;

namespace DebReg.Web.APIModels
{
    public class Adjudicator
    {
        public Guid tournamentId { get; set; }
        public Guid? organizationId { get; set; }
        public String userId { get; set; }
        public User user { get; set; }
        public Organization organization { get; set; }

        public Adjudicator()
        {

        }

        public Adjudicator(DebReg.Models.Adjudicator adjudicator)
        {
            tournamentId = adjudicator.TournamentId;
            organizationId = adjudicator.OrganizationId;
            userId = adjudicator.UserId;
            user = new User(adjudicator.User, tournamentId);
            organization = new Organization(adjudicator.Organization);
        }
    }
}