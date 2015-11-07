using DebReg.Models;

namespace DebRegOrchestration.Models {
    public class RegistrationAssignment {
        public TournamentOrganizationRegistration Registration { get; set; }
        public SlotAssignment Assignment { get; set; }
    }
}
