using DebRegCommunication;
using DebRegCommunication.Models;
using DebRegComponents;
using DebRegOrchestration.Models;
using DebReg.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DebRegOrchestration {
    public class PaymentManager : IPaymentManager {
        private ITournamentRegistrationsManager registrationsManager;
        private IBookingManager bookingManager;
        private IEMailService mailService;
        private Dictionary<Guid, SlotAssignmentStatusMessage> sentMessages = new Dictionary<Guid, SlotAssignmentStatusMessage>();
        private Guid lastTournamentId = Guid.Empty;


        #region IPaymentManager Members

        public void ConfirmSlots(Guid tournamentId, Guid organizationId, int paidTeams, int paidAdjudicators, String paymentPageUrl, User user) {
            var registration = registrationsManager.SetTeamsAndAdjudicatorsPaid(tournamentId, organizationId, paidTeams, paidAdjudicators, user);

            if (registration != null
                && registration.Tournament != null
                && registration.Tournament.HostingOrganization != null
                && registration.Tournament.HostingOrganization.SMTPHostConfiguration != null) {

                if (lastTournamentId != tournamentId) {
                    while (sentMessages.Count > 0) {
                        // wait for messages to be sent
                    }
                    ConfigureMailService(registration.Tournament.HostingOrganization.SMTPHostConfiguration);
                }

                paymentPageUrl = String.Format(paymentPageUrl, registration.TournamentId, registration.OrganizationId);
                SendAssignmentNotification(registration, paymentPageUrl, user);
            }
        }

        public CalculatePaidSlotsResult CalculatePaidSlots(Guid tournamentId, Guid organizationId, Decimal value) {
            var registration = registrationsManager.GetRegistration(tournamentId, organizationId);
            var result = new CalculatePaidSlotsResult {
                Teams = registration.TeamsPaid,
                Adjudicators = registration.AdjudicatorsPaid
            };


            // Find team and adj product for tournament

            var teamProduct = registration.Tournament.TeamProduct;
            var adjProduct = registration.Tournament.AdjudicatorProduct;


            // find bookings for teams and adj

            var bookings = bookingManager.GetBookings(registration.OrganizationId, registration.TournamentId);

            var teamBookings = from b in bookings
                               where b.ProductId == teamProduct.Id
                               select b;

            var adjBookings = from b in bookings
                              where b.ProductId == adjProduct.Id
                              select b;

            // calculate totals and averages

            var totalTeamsBooked = teamBookings.Sum(b => b.Quantity * (b.Credit ? -1 : 1));
            var totalTeamValues = teamBookings.Sum(b => b.Value * (b.Credit ? -1 : 1));

            var totalAdjBooked = adjBookings.Sum(b => b.Quantity * (b.Credit ? -1 : 1));
            var totalAdjValues = adjBookings.Sum(b => b.Value * (b.Credit ? -1 : 1));

            var avgTeamPrice = totalTeamsBooked == 0 ? 0 : totalTeamValues / totalTeamsBooked;
            var avgAdjPrice = totalAdjBooked == 0 ? 0 : totalAdjValues / totalAdjBooked;

            // Calculate all payments

            var payments = from b in bookings
                           where b.ProductId == Guid.Empty || b.ProductId == null
                           select b;
            var paymentSum = payments.Sum(b => b.Value * (b.Credit ? 1 : -1));
            paymentSum += value;

            // Init
            int teamsPaid = 0;
            int adjudicatorsPaid = 0;
            int adjUnpaid = registration.AdjudicatorsGranted;
            int teamsUnpaid = registration.TeamsGranted;
            Boolean teamsOrAdjChanged = true;
            while (paymentSum > 0 && teamsOrAdjChanged) {
                teamsOrAdjChanged = false;

                // Calculate unpaid items

                adjUnpaid = registration.AdjudicatorsGranted - adjudicatorsPaid;
                teamsUnpaid = registration.TeamsGranted - teamsPaid;

                // Prefer adjudicators

                var adjNeeded = teamsPaid - registration.Tournament.AdjucatorSubtract;

                if (adjUnpaid > 0                   // unpaid adjudicators?
                    && paymentSum >= avgAdjPrice    // payments still sufficient
                    && (
                        adjNeeded >= adjudicatorsPaid   // apply adj rule
                        || teamsUnpaid == 0             // or all teams already paid
                    )) {

                    adjudicatorsPaid++;
                    teamsOrAdjChanged = true;
                    paymentSum -= avgAdjPrice;
                }

                // teams
                if (teamsUnpaid > 0
                    && paymentSum >= avgTeamPrice) {

                    teamsPaid++;
                    teamsOrAdjChanged = true;
                    paymentSum -= avgTeamPrice;
                }
            }

            // already confirmed slots cannot be withdrawn

            if (adjudicatorsPaid > registration.AdjudicatorsPaid) {
                result.Adjudicators = adjudicatorsPaid;
            }

            if (teamsPaid > registration.TeamsPaid) {
                result.Teams = teamsPaid;
            }

            return result;

        }

        #endregion

        private void SendAssignmentNotification(TournamentOrganizationRegistration registration, String paymentPageUrl, User user) {

            if (registration != null
                && registration.Tournament != null) {

                var balance = String.Format("{0} {1}",
                    registration.Tournament.Currency.Symbol,
                    bookingManager.GetBalance(registration.OrganizationId, registration.TournamentId));



                // Create message

                EMailMessage message = new EMailMessage();
                message.To = new List<string>();
                var delegates = from ua in registration.Organization.UserAssociations
                                where ua.Role == OrganizationRole.Delegate
                                select ua.User;
#if DEBUG
                message.To.Add(mailService.FromAddress);

#else
                foreach (var deleg in delegates) {
                    message.To.Add(deleg.Email);
                }
#endif
                message.Bcc = new List<string>();
                message.Bcc.Add(mailService.FromAddress);
                message.Subject = String.Format(
                    Resources.Facade.PaymentManager.Strings.ConfirmationNotificationSubject,
                    registration.Tournament.Name);

                Object[] messageVariables = new Object[] {
                    registration.Tournament.Name,               // {0}
                    registration.Organization.Name,             // {1}
                    registration.TeamsPaid,                  // {2}
                    registration.AdjudicatorsPaid,           // {3}
                    balance,                                    // {4}
                    String.Format(paymentPageUrl,               // {5}
                        registration.TournamentId,
                        registration.OrganizationId)            
                };
                message.Body = String.Format(
                    Resources.Facade.PaymentManager.Strings.ConfirmationNotificationBody,
                    messageVariables);



#if DEBUG
                mailService.Send(message);
#endif
                message.HTMLBody = String.Format(
                    Resources.Facade.PaymentManager.Strings.ConfirmationNotificationHTMLBody,
                    messageVariables);

                Guid messageId = Guid.NewGuid();
                sentMessages.Add(messageId, new SlotAssignmentStatusMessage {
                    Registration = registration,
                    MailMessage = message,
                    User = user
                });

                mailService.Send(message, messageId);
            }

        }

        private void ConfigureMailService(SMTPHostConfiguration smtpHostConfiguration) {
            mailService.Host = smtpHostConfiguration.Host;
            mailService.Port = smtpHostConfiguration.Port;
            mailService.SSL = smtpHostConfiguration.SSL;

            mailService.Username = smtpHostConfiguration.Username;
            mailService.Password = smtpHostConfiguration.Password;

            mailService.FromAddress = smtpHostConfiguration.FromAddress;

            mailService.SendCompleted += mailService_SendCompleted;
        }

        private void mailService_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e) {
            if (e.UserState != null) {
                Guid messageId = (Guid)e.UserState;

                if (sentMessages.ContainsKey(messageId)) {
                    EMailMessage message = sentMessages[messageId].MailMessage;
                    TournamentOrganizationRegistration registration = sentMessages[messageId].Registration;
                    User user = sentMessages[messageId].User;

                    if (e.Error == null) {
                        // TODO: Positive logging?
                    }
                    else {
                        // TODO: Log error
                    }
                    sentMessages.Remove(messageId);
                }
            }
        }

        public PaymentManager(ITournamentRegistrationsManager registrationsManager, IBookingManager bookingManager, IEMailService mailService) {
            this.registrationsManager = registrationsManager;
            this.bookingManager = bookingManager;
            this.mailService = mailService;
        }
    }
}
