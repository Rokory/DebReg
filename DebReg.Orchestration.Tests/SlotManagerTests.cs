using DebReg.Data;
using DebReg.Mocks;
using DebReg.Models;
using DebReg.Security;
using DebRegComponents;
using DebRegOrchestration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DebReg.Orchestration.Tests
{
    [TestClass]
    public class SlotManagerTests
    {
        SlotManager slotManager;
        ISlotAssignmentManager slotAssignmentManager;
        ITournamentManager tournamentManager;
        ITournamentRegistrationsManager tournamentRegistrationsManager;
        IBookingManager bookingManager;
        Tournament tournament;
        IUnitOfWork unitOfWork;
        DebRegUserManager userManager;

        [TestInitialize]
        public void Init()
        {
            // Create mocks
            DebRegDataMocks dataMocks = new DebRegDataMocks();
            DebRegCommunicationMocks communicationMocks = new DebRegCommunicationMocks();
            userManager = dataMocks.UserManager;

            unitOfWork = dataMocks.UnitOfWork;
            // Create Managers
            slotAssignmentManager = new SlotAssignmentManager(unitOfWork);
            tournamentManager = new TournamentManager(unitOfWork, userManager);
            tournamentRegistrationsManager = new TournamentRegistrationsManager(unitOfWork, communicationMocks.EMailService, userManager);
            bookingManager = new BookingManager(unitOfWork);

            slotManager = new SlotManager(slotAssignmentManager, tournamentRegistrationsManager, tournamentManager, bookingManager, communicationMocks.EMailService, userManager);

            // Create basic objects

            Organization hostingOrganization = new Organization { Id = Guid.NewGuid() };
            Random rand = new Random();

            User user = CreateUserWithOrganizationRole(hostingOrganization, OrganizationRole.OrganizationTournamentManager);

            // Create tournament

            tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                HostingOrganization = hostingOrganization,
                HostingOrganizationID = hostingOrganization.Id,
                TeamProduct = new Product
                {
                    Id = Guid.NewGuid(),
                    Price = 40
                },
                AdjudicatorProduct = new Product
                {
                    Id = Guid.NewGuid(),
                    Price = 20
                }
            };
            {
                var task = tournamentManager.AddTournamentAsync(tournament, user);
                if (!task.IsCompleted)
                {
                    task.Wait();
                }
            }

            // Create registrations
            Guid[] organizationIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };

            for (int i = 0; i < organizationIds.Length; i++)
            {
                int teamsWanted = i + 1;
                int adjudicatorsWanted = i;
                var registration = tournamentRegistrationsManager.AddRegistration(
                    tournament.Id,
                    organizationIds[i],
                    organizationIds[i],
                    teamsWanted,
                    adjudicatorsWanted,
                    null,
                    user
                );

                registration.OrganizationStatus = OrganizationStatus.Approved;
                registration.OrganizationStatusDraft = false;
            }

        }

        private User CreateUserWithOrganizationRole(Organization organization, OrganizationRole role)
        {
            User user = new User
            {
                Id = Guid.NewGuid().ToString()
            };

            user.OrganizationAssociations.Add(new OrganizationUser
            {
                User = user,
                UserId = user.Id,
                Organization = organization,
                OrganizationId = organization.Id,
                Role = role
            });

            var task = userManager.CreateAsync(user);
            while (!task.IsCompleted)
            {

            }
            return user;
        }

        private Dictionary<Guid, Decimal> CreateDraftVersion(User user)
        {
            Dictionary<Guid, Decimal> expectedBalances = new Dictionary<Guid, Decimal>();

            DebReg.Models.Version version = slotAssignmentManager.CreateVersion(tournament.Id);

            foreach (var registration in tournamentRegistrationsManager.GetApprovedRegistrations(tournament.Id))
            {
                int teamsGranted = registration.TeamsWanted;

                int adjudicatorsGranted = registration.AdjudicatorsWanted;

                slotAssignmentManager.AssignSlots(
                    registration.TournamentId,
                    registration.OrganizationId,
                    teamsGranted,
                    adjudicatorsGranted,
                    user
                );
                var oldBalance = bookingManager.GetBalance(registration.OrganizationId, registration.TournamentId);
                var expectedBalance =
                    (registration.TeamsGranted - teamsGranted) * tournament.TeamProduct.Price +
                    (registration.AdjudicatorsGranted - adjudicatorsGranted) * tournament.AdjudicatorProduct.Price;
                expectedBalances.Add(registration.OrganizationId, expectedBalance);
            }

            return expectedBalances;
        }


        [TestMethod]
        public void PublishAssignments_FirstVersion_ShouldPublishAssignedSlots()
        {
            // Arrange
            #region ARRANGE

            // Basic parameters and objects

            User user = new User { Id = Guid.NewGuid().ToString() };
            DateTime paymentsDueDate = DateTime.Today.AddDays(14);
            Dictionary<Guid, Decimal> expectedBalances;



            // Create new draft version

            expectedBalances = CreateDraftVersion(user);


            #endregion

            // Act

            slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);


            // Assert

            var latestVersion = slotAssignmentManager.GetLatestVersion(tournament.Id);
            Assert.AreEqual(latestVersion.Status, VersionStatus.Public);

            foreach (var registration in tournamentRegistrationsManager.GetApprovedRegistrations(tournament.Id))
            {
                var assignment = slotAssignmentManager.GetSlotAssignment(tournament.Id, registration.OrganizationId);
                Assert.AreEqual(registration.AdjudicatorsGranted, assignment.AdjucatorsGranted);
                Assert.AreEqual(registration.TeamsGranted, assignment.TeamsGranted);
                Decimal expectedBalance;
                expectedBalances.TryGetValue(registration.OrganizationId, out expectedBalance);
                Decimal actualBalance = bookingManager.GetBalance(registration.OrganizationId, tournament.Id);
                Assert.AreEqual(expectedBalance, actualBalance);
            }

            #region ASSERT
            #endregion
        }


        [TestMethod]
        public void PublishAssignments_WithNoVersion_ShouldDoNothing()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };
            DateTime paymentsDueDate = DateTime.Today.AddDays(14);
            var bookingsCountOld = bookingManager.GetBookings(tournament.Id).Count;

            // Act

            slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);

            // Assert

            var latestVersion = slotAssignmentManager.GetLatestVersion(tournament.Id);
            Assert.IsNull(latestVersion);
            Assert.AreEqual(bookingsCountOld, bookingManager.GetBookings(tournament.Id).Count);
        }

        [TestMethod]
        public void PublishAssignments_WithAlreadyPublishedVersion_ShouldDoNothing()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };
            DateTime paymentsDueDate = DateTime.Today.AddDays(14);

            {
                // Create first published version

                CreateDraftVersion(user);
                slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);
            }

            var latestVersionOld = slotAssignmentManager.GetLatestVersion(tournament.Id);
            var bookingsCountOld = bookingManager.GetBookings(tournament.Id).Count;

            // Act

            slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);

            // Assert

            var latestVersionNew = slotAssignmentManager.GetLatestVersion(tournament.Id);
            Assert.AreEqual(latestVersionOld.Id, latestVersionNew.Id);
            Assert.AreEqual(bookingsCountOld, bookingManager.GetBookings(tournament.Id).Count);
        }

        [TestMethod]
        public void PublishAssignments_WithSMTPHostConfiguration()
        {
            // Arrange
            tournament.HostingOrganization.SMTPHostConfiguration = new SMTPHostConfiguration();
            tournament.HostingOrganization.SMTPHostConfiguration.Password = "abc";
            User user = new User { Id = Guid.NewGuid().ToString() };
            DateTime paymentsDueDate = DateTime.Today.AddDays(14);
            CreateDraftVersion(user);

            // Act

            slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);

            // Assert
        }


        [TestMethod]
        public void PublishAssignments_WithNewDraftVersion_ShouldInvalidateOldPublishedVersion()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };
            DateTime paymentsDueDate = DateTime.Today.AddDays(14);

            {
                // Create first published version

                CreateDraftVersion(user);
                slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);
            }

            var latestVersionOld = slotAssignmentManager.GetLatestVersion(tournament.Id);
            Dictionary<Guid, Decimal> expectedBalances = new Dictionary<Guid, Decimal>();

            slotAssignmentManager.CreateVersion(tournament.Id);

            foreach (var assignment in slotAssignmentManager.GetSlotAssignments(tournament.Id, latestVersionOld.Id))
            {

                var teamsGranted = assignment.TeamsGranted;
                if (assignment.TeamsGranted > 2)
                {
                    teamsGranted--;
                }

                if (assignment.TeamsGranted < 2)
                {
                    teamsGranted++;
                }

                var adjudicatorsGranted = assignment.AdjucatorsGranted;
                if (assignment.AdjucatorsGranted > 2)
                {
                    adjudicatorsGranted--;
                }

                if (assignment.AdjucatorsGranted < 2)
                {
                    adjudicatorsGranted++;
                }

                slotAssignmentManager.AssignSlots(assignment.TournamentId, assignment.OrganizationId, teamsGranted, adjudicatorsGranted, user);

                var oldBalance = bookingManager.GetBalance(assignment.OrganizationId, assignment.TournamentId);
                var expectedBalance =
                    oldBalance +
                    (assignment.TeamsGranted - teamsGranted) * tournament.TeamProduct.Price +
                    (assignment.AdjucatorsGranted - adjudicatorsGranted) * tournament.AdjudicatorProduct.Price;
                expectedBalances.Add(assignment.OrganizationId, expectedBalance);
            }


            // Act

            slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);

            // Assert

            Assert.AreEqual(VersionStatus.Outdated, latestVersionOld.Status);

            var latestVersionNew = slotAssignmentManager.GetLatestVersion(tournament.Id);
            Assert.AreEqual(VersionStatus.Public, latestVersionNew.Status);

            foreach (var assignment in slotAssignmentManager.GetSlotAssignments(tournament.Id, latestVersionNew.Id))
            {
                Decimal expectedBalance;
                expectedBalances.TryGetValue(assignment.OrganizationId, out expectedBalance);
                var actualBalance = bookingManager.GetBalance(assignment.OrganizationId, assignment.TournamentId);
                Assert.AreEqual(expectedBalance, actualBalance);
            }
        }

        [TestMethod]
        public void PublishAssignments_WithInvalidTournamentId_ShouldDoNothing()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };
            DateTime paymentsDueDate = DateTime.Today.AddDays(14);

            // Create new draft version

            CreateDraftVersion(user);


            // Act

            var oldBookingsCount = bookingManager.GetBookings(tournament.Id).Count;
            slotManager.PublishAssignments(Guid.Empty, paymentsDueDate, null, user);

            // Assert

            var latestVersion = slotAssignmentManager.GetLatestVersion(tournament.Id);
            Assert.AreEqual(VersionStatus.Draft, latestVersion.Status);
            var newBookingsCount = bookingManager.GetBookings(tournament.Id).Count;
            Assert.AreEqual(oldBookingsCount, newBookingsCount);

        }

        [TestMethod]
        public void PublishAssignments_WithEmptySMTPConfiguration()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };
            DateTime paymentsDueDate = DateTime.Today.AddDays(14);

            // Create new draft version

            CreateDraftVersion(user);


            // Act

            var oldBookingsCount = bookingManager.GetBookings(tournament.Id).Count;
            slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);

            // Assert

        }

        [TestMethod]
        public void PublishAssignments_WithFewerSlotsThanPaid_ShouldDecreaseSlotsPaid()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };
            DateTime paymentsDueDate = DateTime.Today.AddDays(14);

            {
                // Create first published version

                CreateDraftVersion(user);
                slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);
            }

            var latestVersionOld = slotAssignmentManager.GetLatestVersion(tournament.Id);
            Dictionary<Guid, Decimal> expectedBalances = new Dictionary<Guid, Decimal>();

            slotAssignmentManager.CreateVersion(tournament.Id);

            foreach (var assignment in slotAssignmentManager.GetSlotAssignments(tournament.Id, latestVersionOld.Id))
            {

                var registration = tournamentRegistrationsManager.GetRegistration(tournament.Id, assignment.OrganizationId);
                registration.TeamsPaid = registration.TeamsGranted;
                registration.AdjudicatorsPaid = registration.AdjudicatorsWanted;

                var teamsGranted = assignment.TeamsGranted;
                if (assignment.TeamsGranted > 1)
                {
                    teamsGranted--;
                }

                if (assignment.TeamsGranted < 2)
                {
                    teamsGranted++;
                }

                var adjudicatorsGranted = assignment.AdjucatorsGranted;
                if (assignment.AdjucatorsGranted > 1)
                {
                    adjudicatorsGranted--;
                }

                if (assignment.AdjucatorsGranted < 2)
                {
                    adjudicatorsGranted++;
                }

                slotAssignmentManager.AssignSlots(assignment.TournamentId, assignment.OrganizationId, teamsGranted, adjudicatorsGranted, user);

                var oldBalance = bookingManager.GetBalance(assignment.OrganizationId, assignment.TournamentId);
                var expectedBalance =
                    oldBalance +
                    (assignment.TeamsGranted - teamsGranted) * tournament.TeamProduct.Price +
                    (assignment.AdjucatorsGranted - adjudicatorsGranted) * tournament.AdjudicatorProduct.Price;
                expectedBalances.Add(assignment.OrganizationId, expectedBalance);
            }


            // Act

            slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);

            // Assert


            var latestVersionNew = slotAssignmentManager.GetLatestVersion(tournament.Id);

            foreach (var registration in tournamentRegistrationsManager.GetApprovedRegistrations(tournament.Id))
            {
                Assert.IsTrue(registration.TeamsPaid <= registration.TeamsGranted);
            }
        }

        [TestMethod]
        public void PublishAssignments_WithMissingProductLinks_ShouldPublishAssignedSlotsWithNoBookings()
        {
            // Arrange
            #region ARRANGE

            // Basic parameters and objects

            User user = new User { Id = Guid.NewGuid().ToString() };
            DateTime paymentsDueDate = DateTime.Today.AddDays(14);

            // Create new draft version

            CreateDraftVersion(user);

            // Remove Product Links

            var teamProduct = tournament.TeamProduct;
            var adjudicatorProduct = tournament.AdjudicatorProduct;
            tournament.TeamProduct = null;
            tournament.AdjudicatorProduct = null;

            #endregion

            // Act
            var oldBookingsCount = bookingManager.GetBookings(tournament.Id).Count;
            slotManager.PublishAssignments(tournament.Id, paymentsDueDate, null, user);

            // Restore product links
            tournament.TeamProduct = teamProduct;
            tournament.AdjudicatorProduct = adjudicatorProduct;

            // Assert

            var latestVersion = slotAssignmentManager.GetLatestVersion(tournament.Id);
            Assert.AreEqual(latestVersion.Status, VersionStatus.Public);

            foreach (var registration in tournamentRegistrationsManager.GetApprovedRegistrations(tournament.Id))
            {
                var assignment = slotAssignmentManager.GetSlotAssignment(tournament.Id, registration.OrganizationId);
                Assert.AreEqual(registration.AdjudicatorsGranted, assignment.AdjucatorsGranted);
                Assert.AreEqual(registration.TeamsGranted, assignment.TeamsGranted);
            }

            var newbookingsCount = bookingManager.GetBookings(tournament.Id).Count;
            Assert.AreEqual(oldBookingsCount, newbookingsCount);

            #region ASSERT
            #endregion
        }

    }
}
