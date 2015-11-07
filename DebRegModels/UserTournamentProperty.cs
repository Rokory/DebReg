using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models
{
    public class UserTournamentProperty
    {
        [ForeignKey("UserProperty")]
        [Key, Column(Order = 0)]
        public Guid UserPropertyId { get; set; }

        public virtual UserProperty UserProperty { get; set; }

        [ForeignKey("Tournament")]
        [Key, Column(Order = 1)]
        public Guid TournamentId { get; set; }

        public virtual Tournament Tournament { get; set; }

        public Boolean Required { get; set; }

        public int Order { get; set; }
    }
}
