using DebRegCommunication.Models;
using DebReg.Models;

namespace DebRegOrchestration.Models {
    class SlotAssignmentStatusMessage {
        public EMailMessage MailMessage { get; set; }
        public TournamentOrganizationRegistration Registration { get; set; }
        public User User { get; set; }
    }
}
