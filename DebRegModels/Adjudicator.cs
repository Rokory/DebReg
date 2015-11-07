using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models {
    public class Adjudicator : TrackableEntity {
        [ForeignKey("Tournament")]
        [Key, Column(Order = 0)]
        public Guid TournamentId { get; set; }

        public virtual Tournament Tournament { get; set; }

        [ForeignKey("User")]
        [Key, Column(Order = 1)]
        public String UserId { get; set; }

        public virtual User User { get; set; }


        [ForeignKey("Organization")]
        public Guid? OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

    }
}
