using DebRegCommunication.Models;
using DebReg.Models;

namespace DebReg.Web.Areas.TournamentManagement.Models {
    public class OrganizationStatusMessage {
        public EMailMessage MailMessage { get; set; }
        public TournamentOrganizationRegistration Registration { get; set; }

        public User User { get; set; }
    }
}