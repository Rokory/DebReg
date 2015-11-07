using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models {
    public class Team : TrackableEntity {
        public Guid Id { get; set; }

        [ForeignKey("Tournament")]
        public Guid TournamentId { get; set; }

        [Display(Name = "Tournament", ResourceType = typeof(Resources.Models.Team.Strings))]
        public virtual Tournament Tournament { get; set; }

        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }

        [Display(Name = "Organization", ResourceType = typeof(Resources.Models.Team.Strings))]
        public virtual Organization Organization { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resources.Models.Team.Strings))]
        private String _name;

        public String Name {
            get {
                if (Tournament != null
                    && Tournament.FixedTeamNames
                    && Organization != null) {

                    return String.Format("{0} {1}", Organization.Abbreviation, AutoSuffix);
                }
                return _name;
            }
            set {
                if (Tournament != null && !Tournament.FixedTeamNames) {
                    _name = value;
                }
            }
        }

        public String AutoSuffix { get; set; }

        [Display(Name = "Speaker", ResourceType = typeof(Resources.Models.Team.Strings))]
        public virtual List<User> Speaker { get; set; }

        public Team() {
            Speaker = new List<User>();
        }
    }
}
