using DebReg.Models;
using System.Collections.Generic;

namespace DebReg.Web.Areas.TournamentManagement.Models {
    public class SlotViewModel {
        public IEnumerable<SlotAssignmentViewModel> SlotAssignments { get; set; }
        public IEnumerable<TournamentOrganizationRegistration> TeamWaitlist { get; set; }
        public IEnumerable<TournamentOrganizationRegistration> AdjudicatorWaitlist { get; set; }
    }
}