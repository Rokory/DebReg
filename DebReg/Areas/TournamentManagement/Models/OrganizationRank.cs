using System;

namespace DebReg.Web.Areas.TournamentManagement.Models {
    public class OrganizationRank {
        public Guid OrganizationId { get; set; }
        public Decimal Rank { get; set; }
    }
}