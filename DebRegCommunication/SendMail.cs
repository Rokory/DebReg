using DebReg.Models;
using DebRegCommunication.Models;
using System;
using System.Collections.Generic;

namespace DebRegCommunication
{
    public class SendMail : ISendMail
    {
        private IEMailService mailService;

        #region ISendMail Members



        private Organization _sponsoringOrganization;
        public Organization SponsoringOrganization
        {
            get { return _sponsoringOrganization; }
            set
            {
                _sponsoringOrganization = value;
                if (_sponsoringOrganization.SMTPHostConfiguration != null)
                {
                    mailService.FromAddress = _sponsoringOrganization.SMTPHostConfiguration.FromAddress;
                    mailService.Host = _sponsoringOrganization.SMTPHostConfiguration.Host;
                    mailService.Username = _sponsoringOrganization.SMTPHostConfiguration.Username;
                    mailService.Password = _sponsoringOrganization.SMTPHostConfiguration.Password;
                    mailService.Port = _sponsoringOrganization.SMTPHostConfiguration.Port;
                    mailService.SSL = _sponsoringOrganization.SMTPHostConfiguration.SSL;
                }
            }
        }


        public void RequestPasswordReset(User user, String resetUrl)
        {
            var message = CreateEMailMessage(new List<String> { user.Email });
            message.Subject = Resources.DebRegCommunication.SendMail.Strings.RequestPasswordResetSubject;
            message.Body = String.Format(Resources.DebRegCommunication.SendMail.Strings.RequestPasswordResetBody, user.FirstName, resetUrl);
            message.HTMLBody = String.Format(Resources.DebRegCommunication.SendMail.Strings.RequestPasswordResetBodyHTML, user.FirstName, resetUrl);
            mailService.SendCompleted += mailService_SendCompleted;
            mailService.Send(message);
        }

        public void UserRegistered(User user, String resetUrl)
        {
            var message = CreateEMailMessage(new List<String> { user.Email });
            message.Subject = Resources.DebRegCommunication.SendMail.Strings.UserRegisteredSubject;
            message.Body = String.Format(Resources.DebRegCommunication.SendMail.Strings.UserRegisteredBody, user.FirstName, resetUrl);
            message.HTMLBody = String.Format(Resources.DebRegCommunication.SendMail.Strings.UserRegisteredBodyHTML, user.FirstName, resetUrl);
            mailService.SendCompleted += mailService_SendCompleted;
            mailService.Send(message);
        }

        public void ConfirmEMailAddress(User user, string confirmUrl)
        {
            var message = CreateEMailMessage(new List<String> { user.NewEMail });
            message.Subject = Resources.DebRegCommunication.SendMail.Strings.ConfirmEMailSubject;
            message.Body = String.Format
            (
                Resources.DebRegCommunication.SendMail.Strings.ConfirmEMailBody,
                user.FirstName,
                user.NewEMail,
                confirmUrl
            );
            message.HTMLBody = String.Format
            (
                Resources.DebRegCommunication.SendMail.Strings.ConfirmEMailBodyHTML,
                user.FirstName,
                user.NewEMail,
                confirmUrl
            );
            mailService.SendCompleted += mailService_SendCompleted;
            mailService.Send(message);
        }

        public void UserRegisteredForTournament(User user, TournamentOrganizationRegistration registration, Boolean adjudicator, string personalDataLink)
        {
            var role = adjudicator ? Resources.Strings.Adjudicator : Resources.Strings.Speaker;
            var message = CreateEMailMessage(new List<String> { user.NewEMail });
            message.Subject = String.Format
            (
                Resources.DebRegCommunication.SendMail.Strings.UserRegisteredForTournamentSubject,
                role
            );
            message.Body = String.Format
            (
                Resources.DebRegCommunication.SendMail.Strings.UserRegisteredForTournamentBody,
                user.FirstName,
                registration.Organization.Name,
                role,
                registration.Tournament.Name,
                personalDataLink
            );
            message.HTMLBody = String.Format
            (
                Resources.DebRegCommunication.SendMail.Strings.UserRegisteredForTournamentBodyHTML,
                user.FirstName,
                registration.Organization.Name,
                role,
                registration.Tournament.Name,
                personalDataLink
            );
            mailService.SendCompleted += mailService_SendCompleted;
            mailService.Send(message);
        }

        #endregion

        void mailService_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            // TODO: Add logging
        }


        private EMailMessage CreateEMailMessage(IEnumerable<String> recipients)
        {
            EMailMessage message = new EMailMessage();

            if (SponsoringOrganization != null
                && SponsoringOrganization.SMTPHostConfiguration != null)
            {
                message.Bcc.Add(SponsoringOrganization.SMTPHostConfiguration.FromAddress);

#if DEBUG
                message.To.Add(SponsoringOrganization.SMTPHostConfiguration.FromAddress);
#endif
            }
#if ! DEBUG
            foreach (var recipient in recipients)
            {
                message.To.Add(recipient);
            }
#endif
            return message;
        }



        public SendMail(IEMailService mailService)
        {
            this.mailService = mailService;
        }

    }

}
