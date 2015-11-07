using System;
using System.Collections.Generic;
using System.Linq;

namespace DebReg.Web.APIModels
{
    public class Tournament
    {
        public Guid id { get; set; }
        public String name { get; set; }
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public DateTime? registrationStart { get; set; }
        public DateTime? registrationEnd { get; set; }
        public List<TournamentRole> roles { get; set; }

        public virtual ICollection<User> attendees { get; set; }

        public Tournament()
        {
            roles = new List<TournamentRole>();
        }

        public Tournament(DebReg.Models.Tournament tournament)
            : this()
        {
            id = tournament.Id;
            name = tournament.Name;
            start = tournament.Start;
            end = tournament.End;
            registrationStart = tournament.RegistrationStart;
            registrationEnd = tournament.RegistrationEnd;
        }

        public Tournament(DebReg.Models.Tournament tournament, DebReg.Models.User user) :
            this(tournament)
        {
            var roles = from role in user.TournamentRoles
                        where role.TournamentId == tournament.Id
                        select role;

            foreach (var role in roles)
            {
                TournamentRole tournamentRole;
                switch (role.Role)
                {
                    case DebReg.Models.TournamentRole.SlotManager:
                        tournamentRole = TournamentRole.Manager;
                        break;
                    case DebReg.Models.TournamentRole.OrganizationApprover:
                        tournamentRole = TournamentRole.Manager;
                        break;
                    case DebReg.Models.TournamentRole.FinanceManager:
                        tournamentRole = TournamentRole.Manager;
                        break;
                    default:
                        tournamentRole = TournamentRole.None;
                        break;
                }

                if (!this.roles.Any(r => r == tournamentRole))
                {
                    this.roles.Add(tournamentRole);

                }

            }

            if (user.Teams.Any(t => t.TournamentId == tournament.Id))
            {
                this.roles.Add(TournamentRole.Speaker);
            }

            if (user.Adjudicator.Any(a => a.TournamentId == tournament.Id))
            {
                this.roles.Add(TournamentRole.Adjudicator);
            }
        }

    }



    public enum TournamentRole
    {
        None = 0,
        Speaker = 1,
        Adjudicator = 2,
        Manager = 3
    }
}