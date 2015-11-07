using DebReg.Models;
using DebReg.Security;
using DebRegCommunication;
using DebRegCommunication.Models;
using DebRegComponents;
using DebRegOrchestration.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebRegOrchestration
{


    public class SlotManager : ISlotManager
    {
        private ISlotAssignmentManager slotAssignmentManager;
        private ITournamentRegistrationsManager tournamentregistrationsManager;
        private ITournamentManager tournamentManager;
        private IBookingManager bookingManager;
        private IEMailService mailService;
        private Boolean mailServiceConfigured = false;
        private Dictionary<Guid, SlotAssignmentStatusMessage> sentMessages = new Dictionary<Guid, SlotAssignmentStatusMessage>();
        private DebRegUserManager userManager;
        //private SlotAssignment CreateSlotAssignment(TournamentOrganizationRegistration registration, DebReg.Models.Version version) {
        //    return new SlotAssignment {
        //        AdjucatorsGranted = registration.AdjudicatorsGranted,
        //        OrganizationId = registration.OrganizationId,
        //        TeamsGranted = registration.TeamsGranted,
        //        TournamentId = registration.TournamentId,
        //        Version = version,
        //    };
        //}

        private void PublishAssignment(SlotAssignment assignment, DateTime paymentsDueDate, String paymentPageUrl, User user)
        {
            // Get Tournament and registration

            var tournament = tournamentManager.GetTournament(assignment.TournamentId);
            if (tournament == null) { return; }

            var registration = tournamentregistrationsManager.GetRegistration(assignment.TournamentId, assignment.OrganizationId);
            if (registration == null) { return; }

            // Compare assignments with previously published assignments

            var teamsDifference = assignment.TeamsGranted - registration.TeamsGranted;
            var adjudicatorsDifference = assignment.AdjucatorsGranted - registration.AdjudicatorsGranted;

            // Create bookings in balance

            if (tournament.TeamProduct != null && teamsDifference != 0)
            {
                bookingManager.AddBooking(
                    DateTime.UtcNow,
                    assignment.OrganizationId,
                    assignment.TournamentId,
                    tournament.TeamProduct,
                    teamsDifference,
                    false,                                     // Debit
                    teamsDifference > 0 ? (DateTime?)paymentsDueDate : null,    // PaymentsDueDate
                    user);
            }

            if (tournament.AdjudicatorProduct != null && adjudicatorsDifference != 0)
            {
                bookingManager.AddBooking(
                    DateTime.UtcNow,
                    assignment.OrganizationId,
                    assignment.TournamentId,
                    tournament.AdjudicatorProduct,
                    adjudicatorsDifference,
                    false,                                  // Debit
                    adjudicatorsDifference > 0 ? (DateTime?)paymentsDueDate : null, // PaymentsDueDate
                    user);
            }

            // If assigned slots are lower than confirmed, decrease confirmed slots

            var adjudicatorsPaid = registration.AdjudicatorsPaid;

            if (adjudicatorsPaid > assignment.AdjucatorsGranted)
            {
                adjudicatorsPaid = assignment.AdjucatorsGranted;
            }

            var teamsPaid = registration.TeamsPaid;
            if (teamsPaid > assignment.TeamsGranted)
            {
                teamsPaid = assignment.TeamsGranted;
            }

            if (teamsPaid != registration.TeamsPaid || adjudicatorsPaid != registration.AdjudicatorsPaid)
            {
                tournamentregistrationsManager.SetTeamsAndAdjudicatorsPaid(assignment.TournamentId, assignment.OrganizationId, teamsPaid, adjudicatorsPaid, user);
            }

            // Publish assignment in registration


            tournamentregistrationsManager.SetTeamsAndAdjudicatorsGranted(assignment.TournamentId, assignment.OrganizationId, assignment.TeamsGranted, assignment.AdjucatorsGranted, user);


            // Send mail to organization

            if (teamsDifference != 0 || adjudicatorsDifference != 0)
            {
                SendAssignmentNotification(registration, paymentsDueDate, paymentPageUrl, user);
            }
        }

        private void SendAssignmentNotification(TournamentOrganizationRegistration registration, DateTime paymentsDueDate, String paymentPageUrl, User user)
        {

            if (registration != null
                && registration.Tournament != null)
            {

                var balance = String.Format("{0} {1}",
                    registration.Tournament.Currency.Symbol,
                    bookingManager.GetBalance(registration.OrganizationId, registration.TournamentId));
                var dueDate = paymentsDueDate.ToString("d");



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
                    Resources.Facade.SlotManager.Strings.AssignmentNotificationSubject,
                    registration.Tournament.Name);

                Object[] messageVariables = new Object[] {
                    registration.Tournament.Name,               // {0}
                    registration.Organization.Name,             // {1}
                    registration.TeamsGranted,                  // {2}
                    registration.AdjudicatorsGranted,           // {3}
                    balance,                                    // {4}
                    dueDate,    // {5}
                    String.Format(paymentPageUrl,               // {6}
                        registration.TournamentId,
                        registration.OrganizationId)            
                };
                message.Body = String.Format(
                    Resources.Facade.SlotManager.Strings.AssignmentNotificationBody,
                    messageVariables);



#if DEBUG
                mailService.Send(message);
#endif
                message.HTMLBody = String.Format(
                    Resources.Facade.SlotManager.Strings.AssignmentNotificationHTMLBody,
                    messageVariables);

                Guid messageId = Guid.NewGuid();
                sentMessages.Add(messageId, new SlotAssignmentStatusMessage
                {
                    Registration = registration,
                    MailMessage = message,
                    User = user
                });

                mailService.Send(message, messageId);
            }

        }

        void mailService_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.UserState != null)
            {
                Guid messageId = (Guid)e.UserState;

                if (sentMessages.ContainsKey(messageId))
                {
                    EMailMessage message = sentMessages[messageId].MailMessage;
                    TournamentOrganizationRegistration registration = sentMessages[messageId].Registration;
                    User user = sentMessages[messageId].User;

                    if (e.Error == null)
                    {
                        // TODO: Positive logging?
                    }
                    else
                    {
                        // TODO: Log error
                    }
                    sentMessages.Remove(messageId);
                }
            }
        }

        #region ISlotManager Members

        public async Task<IEnumerable<UserProperty>> GetIncompletePropertiesAsync(String userId, Guid tournamentId)
        {
            //List<UserProperty> incompleteProperties = new List<UserProperty>();

            // Get all required properties

            var requiredProperties = GetRequiredProperties(tournamentId);

            // Get user and user properties

            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return null;
            }


            var userPropertyValues = user.PropertyValues
                .Select(v => new { UserPropertyId = v.UserPropertyId, Value = v.Value })
                .Union
                (
                    user.TournamentPropertyValues
                    .Where(p => p.TournamentId == tournamentId)
                    .Select(p => new { UserPropertyId = p.UserPropertyId, Value = p.Value })
                )
                .Where(v => !String.IsNullOrWhiteSpace(v.Value));

            var incompleteProperties = from p in requiredProperties
                                       join v in userPropertyValues
                                       on p.Id equals v.UserPropertyId
                                       into pv
                                       from v in pv.DefaultIfEmpty()
                                       where v == null
                                       select p;




            //var requiredUserProperties = userManager.GetUserProperties().Where(p => p.Required);
            //var requiredTournamentProperties = tournamentManager.GetUserTournamentProperties(tournamentId).Where(p => p.Required);

            //// Check user properties

            //foreach (var userProperty in requiredUserProperties)
            //{
            //    var userPropertyValue = await userManager.GetUserPropertyValueAsync(userId, userProperty.Id);
            //    if (userPropertyValue == null || String.IsNullOrWhiteSpace(userPropertyValue))
            //    {
            //        incompleteProperties.Add(userProperty);
            //    }
            //}

            //foreach (var userTournamentProperty in requiredTournamentProperties)
            //{

            //    String value = null;
            //    if (userTournamentProperty.UserProperty.TournamentSpecific)
            //    {
            //        var userTournamentPropertyValue = tournamentManager.GetUserTournamentPropertyValue(userId, userTournamentProperty.UserPropertyId, tournamentId);
            //        if (userTournamentPropertyValue != null)
            //        {
            //            value = userTournamentPropertyValue.Value;
            //        }
            //    }
            //    else
            //    {
            //        value = await userManager.GetUserPropertyValueAsync(userId, userTournamentProperty.UserPropertyId);
            //    }
            //    if (value == null || String.IsNullOrWhiteSpace(value))
            //    {
            //        incompleteProperties.Add(userTournamentProperty.UserProperty);
            //    }
            //}

            return incompleteProperties;

        }

        private IEnumerable<UserProperty> GetRequiredProperties(Guid tournamentId)
        {
            var requiredProperties = userManager.GetUserProperties()
                .Where(p => p.Required)
                .Union
                (
                    tournamentManager.GetUserTournamentProperties(tournamentId)
                    .Where(p => p.Required)
                    .Select(p => p.UserProperty)
                );

            return requiredProperties;
        }

        public async Task<IEnumerable<User>> GetUsersWithIncompleteDataAsync(Guid organizationId, Guid tournamentId)
        {
            // Get all users for tournament and organization

            var attendees = from t in tournamentregistrationsManager.GetTeams(tournamentId, organizationId)
                            from s in t.Speaker
                            select s;
            attendees = attendees.Union(tournamentregistrationsManager.GetAdjudicators(tournamentId, organizationId).Select(a => a.User));

            List<User> incompleteUsers = await GetUsersWithIncompleteDataAsync(tournamentId, attendees);
            return incompleteUsers;
        }

        private async Task<List<User>> GetUsersWithIncompleteDataAsync(Guid tournamentId, IEnumerable<User> attendees)
        {
            var requiredProperties = GetRequiredProperties(tournamentId);

            var usersWithProperties = attendees
                .Select
                (
                    a => new
                    {
                        User = a,
                        PropertyValues = a.PropertyValues
                            .Select(v => new { UserPropertyId = v.UserPropertyId, Value = v.Value })
                            .Union
                            (
                                a.TournamentPropertyValues
                                .Where(p => p.TournamentId == tournamentId)
                                .Select(p => new { UserPropertyId = p.UserPropertyId, Value = p.Value })
                            )
                            .Where(v => !String.IsNullOrWhiteSpace(v.Value))
                    }
                );

            var usersWithRequiredPropertyValues = from u in usersWithProperties
                                                  select new
                                                  {
                                                      User = u.User,
                                                      RequiredPropertyValues =
                                                      (
                                                        from pv in u.PropertyValues
                                                        join rp in requiredProperties
                                                        on pv.UserPropertyId equals rp.Id
                                                        select pv
                                                      )
                                                  };

            var requiredPropertiesCount = requiredProperties.Count();

            var incompleteUsers = (from u in usersWithRequiredPropertyValues
                                   where u.RequiredPropertyValues.Count() < requiredPropertiesCount
                                   select u.User)
                                  .Distinct()
                                  .ToList();


            //var incompleteUsers = (from u in usersWithProperties
            //from pv in u.PropertyValues
            //join rp in requiredProperties
            //on pv.UserPropertyId equals rp.Id
            //                       into joinedProperties
            //                       where joinedProperties.Count() < requiredProperties.Count()
            //                       select u.User)
            //                      .Distinct()
            //                      .ToList();




            // Check users for incomplete properties

            //List<User> incompleteUsers = new List<User>();

            //foreach (var user in attendees)
            //{
            //    var incompleteProperties = await GetIncompletePropertiesAsync(user.Id, tournamentId);
            //    if (incompleteProperties.Count() > 0)
            //    {
            //        incompleteUsers.Add(user);
            //    }
            //}
            return incompleteUsers;
        }

        public async Task<IEnumerable<User>> GetUsersWithIncompleteDataAsync(Guid tournamentId)
        {
            var attendees = from t in tournamentregistrationsManager.GetTeams(tournamentId)
                            from s in t.Speaker
                            select s;
            attendees = attendees.Union(tournamentregistrationsManager.GetAdjudicators(tournamentId).Select(a => a.User));

            List<User> incompleteUsers = await GetUsersWithIncompleteDataAsync(tournamentId, attendees);
            return incompleteUsers;
        }

        private void SendAssignmentNotifications(Guid tournamentId, DateTime paymentsDueDate, String paymentPageUrl, User user)
        {
            if (mailServiceConfigured)
            {
                var tournament = tournamentManager.GetTournament(tournamentId);
                if (tournament != null)
                {
                    if (tournament.HostingOrganization != null
                        && tournament.HostingOrganization.SMTPHostConfiguration != null)
                    {

                        ConfigureMailService(tournament.HostingOrganization.SMTPHostConfiguration);

                        foreach (var registration in tournamentregistrationsManager.GetRegistrationsSortedByRank(tournamentId, user))
                        {
                            SendAssignmentNotification(registration, paymentsDueDate, paymentPageUrl, user);
                        }

                    }

                }

            }
        }

        public IList<SlotAssignment> GetSlotAssignments(Guid tournamentId, DebReg.Models.Version version, User user)
        {
            var waitList = tournamentregistrationsManager.GetRegistrationsSortedByRank(tournamentId, user);
            // List<SlotAssignment> assignments = new List<SlotAssignment>();

            var assignments = slotAssignmentManager.GetSlotAssignments(tournamentId, version.Id).ToList();
            IList<SlotAssignment> result = new List<SlotAssignment>();

            // Create missing assignments

            foreach (var registration in waitList)
            {
                var currentAssignment = assignments.FirstOrDefault(a => a.OrganizationId == registration.OrganizationId);

                if (currentAssignment == null)
                {
                    currentAssignment = new SlotAssignment
                    {
                        Organization = registration.Organization,
                        OrganizationId = registration.OrganizationId,
                        TournamentId = registration.TournamentId,
                        TeamsGranted = 0,
                        AdjucatorsGranted = 0,
                        Version = version
                    };
                }
                result.Add(currentAssignment);
            }
            return assignments;
        }

        public int GetFreeTeamSlots(Guid tournamentId, Guid versionId)
        {
            var tournament = tournamentManager.GetTournament(tournamentId);
            var teamSlotsGranted = slotAssignmentManager.GetTeamSlotsGranted(tournamentId, versionId);
            return tournament.TeamCap - teamSlotsGranted;
        }

        public int GetFreeAdjudicatorSlots(Guid tournamentId, Guid versionId)
        {
            var tournament = tournamentManager.GetTournament(tournamentId);
            var adjudicatorSlotsGranted = slotAssignmentManager.GetAdjudicatorsGranted(tournamentId, versionId);
            return tournament.AdjudicatorCap - adjudicatorSlotsGranted;
        }

        public IEnumerable<TournamentOrganizationRegistration> GetTeamWaitlist(Guid tournamentId, User user)
        {
            var latestVersion = slotAssignmentManager.GetLatestVersion(tournamentId);
            var registrations = tournamentregistrationsManager.GetRegistrationsSortedByRank(tournamentId, user);
            if (latestVersion == null)
            {
                latestVersion = slotAssignmentManager.CreateVersion(tournamentId);
            }
            var assignments = slotAssignmentManager.GetSlotAssignments(tournamentId, latestVersion.Id);

            // Left join registrations with assignments
            // order result by whether the organization has teams waiting, then by TeamsGranted, then by ranks

            registrations = from Registration in registrations
                            where !Registration.LockAutoAssign
                            join a in assignments
                            on new { Registration.TournamentId, Registration.OrganizationId } equals new { a.TournamentId, a.OrganizationId }
                            into ar // builds collection of collections grouped by r
                            from Assignment in ar.DefaultIfEmpty() // returns a default, if the collection is empty
                            orderby
                                Registration.LockAutoAssign,
                                Registration.TeamsWanted - (Assignment != null ? Assignment.TeamsGranted : 0) > 0 descending, // assignments with waiting slots first
                                Assignment != null ? Assignment.TeamsGranted : 0, // assignments with less teams granted first
                                Registration.Rank descending,
                                Registration.RandomRank
                            select Registration;

            return registrations;
        }
        public IEnumerable<TournamentOrganizationRegistration> GetAdjudicatorWaitlist(Guid tournamentId, User user)
        {
            var latestVersion = slotAssignmentManager.GetLatestVersion(tournamentId);
            var registrations = tournamentregistrationsManager.GetRegistrationsSortedByRank(tournamentId, user);
            if (latestVersion == null)
            {
                latestVersion = slotAssignmentManager.CreateVersion(tournamentId);
            }

            var assignments = slotAssignmentManager.GetSlotAssignments(tournamentId, latestVersion.Id);

            // Left join registrations with assignments
            // order result by whether the organization has teams waiting, then by TeamsGranted, then by ranks

            registrations = from r in registrations
                            where !r.LockAutoAssign
                            join assignment in assignments
                            on new { r.TournamentId, r.OrganizationId } equals new { assignment.TournamentId, assignment.OrganizationId }
                            into ar // builds collection of collections grouped by r
                            from a in ar.DefaultIfEmpty() // returns a default, if the collection is empty
                            orderby (r.AdjudicatorsWanted - (a == null ? 0 : a.AdjucatorsGranted)) > 0 descending, // organizations without items on wait list are at the end of the list
                                    a == null ? 0 : a.AdjucatorsGranted,
                                    r.Rank descending,
                                    r.RandomRank
                            select r;
            return registrations;
        }
        public void AssignTeamSlots(Guid tournamentId, User user)
        {
            // Get latest version
            var latestVersion = slotAssignmentManager.GetLatestVersion(tournamentId);

            if (latestVersion == null || latestVersion.Status != VersionStatus.Draft)
            {
                latestVersion = slotAssignmentManager.CreateVersion(tournamentId);
            }


            // Assign slots
            var assignments = slotAssignmentManager.GetSlotAssignments(tournamentId, latestVersion.Id);
            var registrations = GetTeamWaitlist(tournamentId, user).Where(r => !r.LockAutoAssign);
            int freeSlots = GetFreeTeamSlots(tournamentId, latestVersion.Id);
            Boolean slotsAssigned = true;   // indicates whether slots were assigned to avoid endless loop


            while (freeSlots > 0 && slotsAssigned)
            {
                slotsAssigned = false;
                foreach (var registration in registrations)
                {
                    // get or create assignment
                    var assignment = slotAssignmentManager.GetSlotAssignment(tournamentId, registration.OrganizationId, latestVersion.Id);

                    int teamsAssigned;
                    if (assignment == null)
                    {
                        //assignment = CreateSlotAssignment(registration, latestVersion);
                        teamsAssigned = registration.TeamsGranted;
                    }
                    else
                    {
                        teamsAssigned = assignment.TeamsGranted;
                    }

                    // Calculate number of teams on wait list for the organization
                    var teamsWaiting = registration.TeamsWanted - teamsAssigned;

                    if (teamsWaiting > 0)
                    {
                        // assign one team
                        slotAssignmentManager.AssignTeamSlots(registration.TournamentId, registration.OrganizationId, latestVersion.Id, teamsAssigned + 1, user);
                        freeSlots--;
                        slotsAssigned = true;
                    }
                }
            }

        }



        public void AssignAdjudicatorSlots(Guid tournamentId, User user)
        {
            // Get latest version
            var latestVersion = slotAssignmentManager.GetLatestVersion(tournamentId);
            var currentVersion = latestVersion;

            if (latestVersion.Status != VersionStatus.Draft)
            {
                currentVersion = slotAssignmentManager.CreateVersion(tournamentId);
            }

            // Assign slots
            var assignments = slotAssignmentManager.GetSlotAssignments(tournamentId, latestVersion.Id);
            var registrations = GetAdjudicatorWaitlist(tournamentId, user).Where(r => !r.LockAutoAssign);
            int freeSlots = GetFreeAdjudicatorSlots(tournamentId, latestVersion.Id);
            Boolean slotsAssigned = true;   // indicates whether slots were assigned to avoid endless loop

            while (freeSlots > 0 && slotsAssigned)
            {
                slotsAssigned = false;
                foreach (var registration in registrations)
                {

                    // get or create assignment
                    var assignment = slotAssignmentManager.GetSlotAssignment(tournamentId, registration.OrganizationId, latestVersion.Id);

                    int adjudicatorsAssigned;
                    if (assignment == null)
                    {
                        //assignment = CreateSlotAssignment(registration, latestVersion);
                        adjudicatorsAssigned = registration.AdjudicatorsGranted;
                    }
                    else
                    {
                        adjudicatorsAssigned = assignment.AdjucatorsGranted;
                    }

                    // Calculate number of adjudicators on wait list for the organization
                    var adjudicatorsWaiting = registration.AdjudicatorsWanted - adjudicatorsAssigned;

                    if (adjudicatorsWaiting > 0)
                    {
                        // assign one adjudicator
                        slotAssignmentManager.AssignAdjudicatorSlots(registration.TournamentId, registration.OrganizationId, currentVersion.Id, adjudicatorsAssigned + 1, user);
                        freeSlots--;
                        slotsAssigned = true;
                    }
                }
            }
        }



        public void PublishAssignments(Guid tournamentId, DateTime paymentsDueDate, String paymentPageUrl, User user)
        {
            // Get latest version

            var latestVersion = slotAssignmentManager.GetLatestVersion(tournamentId);

            // Check if latest version is not published

            if (latestVersion == null || latestVersion.Status != VersionStatus.Draft)
            {
                return;
            }

            // Configure mail service

            var tournament = tournamentManager.GetTournament(tournamentId);

            if (tournament.HostingOrganization != null)
            {
                if (tournament.HostingOrganization.SMTPHostConfiguration != null)
                {

                    var smtpHostConfiguration = tournament.HostingOrganization.SMTPHostConfiguration;
                    ConfigureMailService(smtpHostConfiguration);
                }
            }

            // Get assignments for version

            var assignments = slotAssignmentManager.GetSlotAssignments(tournamentId, latestVersion.Id);

            // Publish assignments



            foreach (var assignment in assignments)
            {
                PublishAssignment(assignment, paymentsDueDate, paymentPageUrl, user);
            }

            // Publish version
            slotAssignmentManager.PublishLatestVersion(tournamentId);

        }


        #endregion

        private void ConfigureMailService(SMTPHostConfiguration smtpHostConfiguration)
        {
            mailService.Host = smtpHostConfiguration.Host;
            mailService.Port = smtpHostConfiguration.Port;
            mailService.SSL = smtpHostConfiguration.SSL;

            mailService.Username = smtpHostConfiguration.Username;
            mailService.Password = smtpHostConfiguration.Password;

            mailService.FromAddress = smtpHostConfiguration.FromAddress;

            mailService.SendCompleted += mailService_SendCompleted;
            mailServiceConfigured = true;
        }


        public SlotManager(ISlotAssignmentManager slotAssignmentManager, ITournamentRegistrationsManager tournamentRegistrationsManager, ITournamentManager tournamentManager, IBookingManager bookingManager, IEMailService mailService, DebRegUserManager userManager)
        {
            this.slotAssignmentManager = slotAssignmentManager;
            this.tournamentregistrationsManager = tournamentRegistrationsManager;
            this.tournamentManager = tournamentManager;
            this.bookingManager = bookingManager;
            this.mailService = mailService;
            this.userManager = userManager;
        }

    }
}
