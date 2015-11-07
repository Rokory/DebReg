using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models
{
    public class TournamentUserRole
    {
        // public Guid Id { get; set; }
        [ForeignKey("Tournament")]
        [Key, Column(Order = 0)]
        public Guid TournamentId { get; set; }

        public virtual Tournament Tournament { get; set; }

        [ForeignKey("User")]
        [Key, Column(Order = 1)]
        public String UserId { get; set; }

        public virtual User User { get; set; }

        [Key, Column(Order = 2)]
        public TournamentRole Role { get; set; }

    }

    public enum TournamentRole
    {
        NoTournamentRole = 0,
        SlotManager = 1,
        OrganizationApprover = 2,
        FinanceManager = 3,
        ReportViewer = 4,
    }
}
