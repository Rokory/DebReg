using System;
using System.Collections.Generic;

namespace DebReg.Web.APIModels
{
    public class Team
    {
        public Guid id { get; set; }
        public Guid tournamentId { get; set; }
        public Guid organizationId { get; set; }
        public Organization organization { get; set; }
        public String name { get; set; }
        public List<User> speakers { get; set; }

        public Team()
        {
            speakers = new List<User>();
        }

        public Team(DebReg.Models.Team team)
            : this()
        {
            this.id = team.Id;
            this.tournamentId = team.TournamentId;
            this.organizationId = team.OrganizationId;
            this.organization = new Organization(team.Organization);
            this.name = team.Name;
            foreach (var speaker in team.Speaker)
            {
                speakers.Add(new User(speaker, tournamentId));
            }
        }
    }
}