using DebReg.Models;
using System;

namespace DebRegCommunication
{
    public interface ISendMail
    {
        void RequestPasswordReset(User user, String resetUrl);
        void UserRegistered(User user, String resetUrl);
        Organization SponsoringOrganization { get; set; }

        void ConfirmEMailAddress(User user, string confirmUrl);
        void UserRegisteredForTournament(User user, TournamentOrganizationRegistration registration, Boolean adjudicator, string personalDataLink);
    }
}
