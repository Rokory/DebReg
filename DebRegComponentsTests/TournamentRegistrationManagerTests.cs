using DebReg.Data;
using DebReg.Mocks;
using DebReg.Security;
using DebRegCommunication;
using DebRegComponents;
using DebReg.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DebReg.Components.Tests
{
    [TestClass]
    public class TournamentRegistrationManagerTests
    {
        IUnitOfWork unitOfWork;
        TournamentRegistrationsManager tournamentRegistrationsManager;
        DebRegUserManager userManager;
        IEMailService mailService;

        [TestInitialize]
        public void Init()
        {
            var dataMocks = new DebRegDataMocks();

            unitOfWork = dataMocks.UnitOfWork;
            mailService = new DebRegCommunicationMocks().EMailService;
            userManager = dataMocks.UserManager;
            tournamentRegistrationsManager = new TournamentRegistrationsManager(unitOfWork, mailService, userManager);
        }

        #region TEST METHODS

        #region GetRegistration

        [TestMethod]
        public void GetRegistration_WithValidBookingCodeAndValidTournamentId_ShouldReturnRegistration()
        {
            // Arrange

            var registration = AddRegistration();
            var bookingCode = registration.BookingCode;
            var tournamentId = registration.TournamentId;
            registration = null;

            // Act

            registration = tournamentRegistrationsManager.GetRegistration(bookingCode, tournamentId);

            // Assert

            Assert.IsFalse(String.IsNullOrWhiteSpace(bookingCode));
            Assert.IsNotNull(registration);
        }


        [TestMethod]
        public void GetRegistration_WithValidBookingCodeAndInvalidTournamentId_ShouldReturnNull()
        {
            // Arrange

            var registration = AddRegistration();
            var bookingCode = registration.BookingCode;
            var tournamentId = Guid.Empty;
            registration = null;

            // Act

            registration = tournamentRegistrationsManager.GetRegistration(bookingCode, tournamentId);

            // Assert

            Assert.IsFalse(String.IsNullOrWhiteSpace(bookingCode));
            Assert.IsNull(registration);

        }

        [TestMethod]
        public void GetRegistration_WithInvalidBookingCodeAndValidTournamentId_ShouldReturnNull()
        {
            // Arrange

            var registration = AddRegistration();
            var bookingCode = "abcdef";
            var tournamentId = registration.TournamentId;
            registration = null;

            // Act

            registration = tournamentRegistrationsManager.GetRegistration(bookingCode, tournamentId);

            // Assert

            Assert.IsFalse(String.IsNullOrWhiteSpace(bookingCode));
            Assert.IsNull(registration);
        }

        [TestMethod]
        public void GetRegistration_WithValidTournamentIdAndValidOrganizationId_ShouldReturnRegistration()
        {
            // Arrange

            var registration = AddRegistration();
            var tournamentId = registration.TournamentId;
            var organizationId = registration.OrganizationId;
            registration = null;

            // Act

            registration = tournamentRegistrationsManager.GetRegistration(tournamentId, organizationId);

            // Assert

            Assert.IsNotNull(registration);

        }

        [TestMethod]
        public void GetRegistration_WithValidTournamentIdAndInvalidOrganization_ShouldReturnNull()
        {
            // Arrange

            var registration = AddRegistration();
            var tournamentId = registration.TournamentId;
            var organizationId = Guid.Empty;
            registration = null;

            // Act

            registration = tournamentRegistrationsManager.GetRegistration(tournamentId, organizationId);

            // Assert

            Assert.IsNull(registration);
        }

        [TestMethod]
        public void GetRegistration_WithInvalidTournamentIdAndValidOrganization_ShouldReturnNull()
        {
            // Arrange

            var registration = AddRegistration();
            var tournamentId = Guid.Empty;
            var organizationId = registration.OrganizationId;
            registration = null;

            // Act

            registration = tournamentRegistrationsManager.GetRegistration(tournamentId, organizationId);

            // Assert

            Assert.IsNull(registration);
        }

        #endregion

        [TestMethod]
        public void SetTeamsAndAdjudicatorsPaid_ShouldUpdateRegistration()
        {
            // Arrange

            var user = new User { Id = Guid.NewGuid().ToString() };

            var registration = AddRegistration();
            var tournamentId = registration.TournamentId;
            var organizationId = registration.OrganizationId;
            registration = null;

            // Act

            Random random = new Random();
            int teamsPaid = random.Next(3);
            int adjudicatorsPaid = random.Next(3);
            tournamentRegistrationsManager.SetTeamsAndAdjudicatorsPaid(tournamentId, organizationId, teamsPaid, adjudicatorsPaid, user);

            // Assert

            registration = tournamentRegistrationsManager.GetRegistration(tournamentId, organizationId);
            Assert.AreEqual(teamsPaid, registration.TeamsPaid);
            Assert.AreEqual(adjudicatorsPaid, registration.AdjudicatorsPaid);
            Assert.AreEqual(user.Id, registration.ModifiedById);
            Assert.IsNotNull(registration.Modified);
        }

        [TestMethod]
        public void SetTeamsAndAdjudicatorsGranted_ShouldUpdateRegistration()
        {
            // Arrange

            var user = new User { Id = Guid.NewGuid().ToString() };

            var registration = AddRegistration();
            var tournamentId = registration.TournamentId;
            var organizationId = registration.OrganizationId;
            registration = null;

            // Act

            Random random = new Random();
            int teamsGranted = random.Next(3);
            int adjudicatorsGranted = random.Next(3);
            tournamentRegistrationsManager.SetTeamsAndAdjudicatorsGranted(tournamentId, organizationId, teamsGranted, adjudicatorsGranted, user);

            // Assert

            registration = tournamentRegistrationsManager.GetRegistration(tournamentId, organizationId);
            Assert.AreEqual(teamsGranted, registration.TeamsGranted);
            Assert.AreEqual(adjudicatorsGranted, registration.AdjudicatorsGranted);
            Assert.AreEqual(user.Id, registration.ModifiedById);
            Assert.IsNotNull(registration.Modified);
        }

        #region AddRegistation

        [TestMethod]
        public void AddRegistration_WithValidParameters_ShouldAddRegistration()
        {
            // Arrange
            Random rand = new Random();
            User user = new User { Id = Guid.NewGuid().ToString() };
            Guid tournamentId = Guid.NewGuid();

            Guid[] organizationIds = new Guid[] { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            int[] teamsWanted = new int[] { 0, 1, 3 };
            int[] adjudicatorsWanted = new int[] { 1, 0, 2 };
            String note = "Testnote";

            // Act

            for (int i = 0; i < organizationIds.Length; i++)
            {
                tournamentRegistrationsManager.AddRegistration(
                    tournamentId,
                    organizationIds[i],
                    organizationIds[i],
                    teamsWanted[i],
                    adjudicatorsWanted[i],
                    note,
                    user);
            }

            // Assert

            for (int i = 0; i < organizationIds.Length; i++)
            {
                var registration = tournamentRegistrationsManager.GetRegistration(tournamentId, organizationIds[i]);
                Assert.AreEqual(registration.BilledOrganizationId, organizationIds[i]);
                Assert.AreEqual(registration.TeamsWanted, teamsWanted[i]);
                Assert.AreEqual(registration.AdjudicatorsWanted, adjudicatorsWanted[i]);
                Assert.AreEqual(registration.AdjudicatorsGranted, 0);
                Assert.AreEqual(registration.AdjudicatorsPaid, 0);
                Assert.AreEqual(registration.TeamsGranted, 0);
                Assert.AreEqual(registration.TeamsPaid, 0);
                Assert.AreEqual(registration.Notes, note);
                Assert.IsFalse(String.IsNullOrWhiteSpace(registration.BookingCode));
                Assert.AreEqual(registration.OrganizationStatus, OrganizationStatus.Unknown);
                Assert.IsTrue(registration.OrganizationStatusDraft);
                Assert.IsTrue(string.IsNullOrWhiteSpace(registration.OrganizationStatusNote));
                Assert.IsFalse(registration.LockAutoAssign);
                Assert.AreEqual(registration.CreatedById, user.Id);
                Assert.IsNotNull(registration.Created);
                Assert.IsNotNull(registration.Modified);
                Assert.AreEqual(registration.ModifiedById, user.Id);
            }
        }

        [TestMethod]
        public void AddRegistration_WithEmptyTournamentId_ShouldDoNothing()
        {
            // Arrange
            Random rand = new Random();
            User user = new User { Id = Guid.NewGuid().ToString() };
            Guid tournamentId = Guid.Empty;
            Guid organizationId = Guid.NewGuid();
            int teamsWanted = 2;
            int adjudicatorsWanted = 1;
            String note = "Testnote";

            // Act

            var registration = tournamentRegistrationsManager.AddRegistration(
                tournamentId,
                organizationId,
                organizationId,
                teamsWanted,
                adjudicatorsWanted,
                note,
                user);

            // Assert

            Assert.IsNull(registration);
            registration = null;
            registration = tournamentRegistrationsManager.GetRegistration(tournamentId, organizationId);
            Assert.IsNull(registration);
        }

        [TestMethod]
        public void AddRegistration_WithEmptyOrganizationId_ShouldDoNothing()
        {
            // Arrange
            Random rand = new Random();
            User user = new User { Id = Guid.NewGuid().ToString() };
            Guid tournamentId = Guid.NewGuid();
            Guid organizationId = Guid.Empty;
            int teamsWanted = 2;
            int adjudicatorsWanted = 1;
            String note = "Testnote";

            // Act

            var registration = tournamentRegistrationsManager.AddRegistration(
                tournamentId,
                organizationId,
                organizationId,
                teamsWanted,
                adjudicatorsWanted,
                note,
                user);

            // Assert

            Assert.IsNull(registration);
            registration = null;
            registration = tournamentRegistrationsManager.GetRegistration(tournamentId, organizationId);
            Assert.IsNull(registration);
        }

        [TestMethod]
        public void AddRegistration_With0Wanted_ShouldDoNothing()
        {
            // Arrange
            Random rand = new Random();
            User user = new User { Id = Guid.NewGuid().ToString() };
            Guid tournamentId = Guid.NewGuid();
            Guid organizationId = Guid.NewGuid();
            int teamsWanted = 0;
            int adjudicatorsWanted = 0;
            String note = "Testnote";

            // Act

            var registration = tournamentRegistrationsManager.AddRegistration(
                tournamentId,
                organizationId,
                organizationId,
                teamsWanted,
                adjudicatorsWanted,
                note,
                user);

            // Assert

            Assert.IsNull(registration);
            registration = null;
            registration = tournamentRegistrationsManager.GetRegistration(tournamentId, organizationId);
            Assert.IsNull(registration);
        }

        #endregion

        #region GetTeams
        [TestMethod]
        public void GetTeams_WithValidOrganizationAndTournamentId_ShouldReturnTeams()
        {

            #region Arrange

            // Arrange

            Guid organizationId = Guid.NewGuid();
            Guid tournamentId = Guid.NewGuid();

            #region Team 1

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                OrganizationId = organizationId
            };

            unitOfWork.GetRepository<Team>().Insert(team);
            #endregion

            #region Team 2

            team = new Team
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                OrganizationId = organizationId
            };
            unitOfWork.GetRepository<Team>().Insert(team);
            #endregion

            #region Deleted Team
            team = new Team
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                OrganizationId = organizationId,
                Deleted = true
            };
            unitOfWork.GetRepository<Team>().Insert(team);
            #endregion

            unitOfWork.Save();

            #endregion

            // Act

            var teams = tournamentRegistrationsManager.GetTeams(tournamentId, organizationId);

            // Assert

            Assert.AreEqual(2, teams.Count());
        }

        [TestMethod]
        public void GetTeams_WithValidTournamentId_ShouldReturnTeams()
        {
            #region Arrange

            // Arrange

            Guid organizationId = Guid.NewGuid();
            Guid tournamentId = Guid.NewGuid();

            #region Team 1

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                OrganizationId = organizationId
            };

            unitOfWork.GetRepository<Team>().Insert(team);
            #endregion

            #region Team 2

            team = new Team
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                OrganizationId = organizationId
            };
            unitOfWork.GetRepository<Team>().Insert(team);
            #endregion

            #region Deleted Team
            team = new Team
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                OrganizationId = organizationId,
                Deleted = true
            };
            unitOfWork.GetRepository<Team>().Insert(team);
            #endregion

            unitOfWork.Save();

            #endregion

            // Act

            var teams = tournamentRegistrationsManager.GetTeams(tournamentId);

            // Assert

            Assert.AreEqual(2, teams.Count());

        }

        #endregion

        #region GetTeam

        [TestMethod]
        public void GetTeam_WithValidTeamId_ShouldReturnTeam()
        {
            // Arrange

            var id = Guid.NewGuid();
            Team team = new Team
            {
                Id = id,
                Name = "Team A"
            };
            unitOfWork.GetRepository<Team>().Insert(team);
            unitOfWork.Save();
            team = null;

            // Act

            team = tournamentRegistrationsManager.GetTeam(id);

            // Assert

            Assert.AreEqual(id, team.Id);

        }

        #endregion

        #region AddSpeakerAsync
        [TestMethod]
        public void AddSpeakerAsync_WithValidTeamIdAndValidSpeakerId_ShouldAddSpeaker()
        {
            // Arrange

            var tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                TeamSize = 2
            };

            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            Organization organization = new Organization
            {
                Id = Guid.NewGuid()
            };

            unitOfWork.GetRepository<Organization>().Insert(organization);
            unitOfWork.Save();

            var teamId = Guid.NewGuid();


            Team team = new Team
            {
                Id = teamId,
                Tournament = tournament,
                TournamentId = tournament.Id,
                Organization = organization,
                OrganizationId = organization.Id
            };

            unitOfWork.GetRepository<Team>().Insert(team);
            unitOfWork.Save();

            String speakerId = Guid.NewGuid().ToString();
            User speaker = new User { Id = speakerId };
            var userTask = userManager.CreateAsync(speaker);
            if (!userTask.IsCompleted) { userTask.Wait(); }

            User user = new User();

            // Act

            var task = tournamentRegistrationsManager.AddSpeakerAsync(teamId, speakerId, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            var result = task.Result;

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.TeamUpdated, result);
            team = tournamentRegistrationsManager.GetTeam(teamId);
            speaker = team.Speaker.FirstOrDefault(sp => sp.Id == speakerId);
            Assert.IsNotNull(speaker);

        }

        [TestMethod]
        public void AddSpeakerAsync_WithInvalidTeamId_ShouldReturnTeamNotFound()
        {
            // Arrange
            Guid teamId = Guid.NewGuid();
            String speakerId = Guid.NewGuid().ToString();
            User speaker = new User { Id = speakerId };
            var userTask = userManager.CreateAsync(speaker);
            if (!userTask.IsCompleted) { userTask.Wait(); }

            User user = new User();

            // Act

            var task = tournamentRegistrationsManager.AddSpeakerAsync(teamId, speakerId, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            var result = task.Result;

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.TeamNotFound, result);

        }

        [TestMethod]
        public void AddSpeakerAsync_ExceedingTeamSize_ShouldReturnTooManySpeakers()
        {
            // Arrange
            var tournament = new Tournament
            {
                TeamSize = 2
            };
            var teamId = Guid.NewGuid();

            Team team = new Team
            {
                Id = teamId,
                Tournament = tournament
            };

            for (int i = 0; i < 2; i++)
            {
                team.Speaker.Add(new User());
            }

            unitOfWork.GetRepository<Team>().Insert(team);
            unitOfWork.Save();

            String speakerId = Guid.NewGuid().ToString();
            User speaker = new User { Id = speakerId };
            var userTask = userManager.CreateAsync(speaker);
            if (!userTask.IsCompleted) { userTask.Wait(); }

            User user = new User();

            // Act

            var task = tournamentRegistrationsManager.AddSpeakerAsync(teamId, speakerId, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            var result = task.Result;

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.TooManySpeakers, result);

        }

        [TestMethod]
        public void AddSpeakerAsync_WithSpeakerAlreadyInTeam_ShouldReturnSpeakerAlreadyInTeam()
        {
            // Arrange
            String speakerId = Guid.NewGuid().ToString();
            User speaker = new User { Id = speakerId };
            var userTask = userManager.CreateAsync(speaker);
            if (!userTask.IsCompleted) { userTask.Wait(); }

            var tournament = new Tournament
            {
                TeamSize = 2
            };
            var teamId = Guid.NewGuid();

            Team team = new Team
            {
                Id = teamId,
                Tournament = tournament
            };

            team.Speaker.Add(speaker);


            unitOfWork.GetRepository<Team>().Insert(team);
            unitOfWork.Save();


            User user = new User();

            // Act

            var task = tournamentRegistrationsManager.AddSpeakerAsync(teamId, speakerId, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            var result = task.Result;

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.SpeakerAlreadyInTeam, result);

        }

        [TestMethod]
        public void AddSpeakerAsync_WithInvalidUserId_ShouldReturnUserNotFound()
        {
            // Arrange
            String speakerId = Guid.NewGuid().ToString();

            var tournament = new Tournament
            {
                TeamSize = 2
            };
            var teamId = Guid.NewGuid();

            Team team = new Team
            {
                Id = teamId,
                Tournament = tournament
            };

            unitOfWork.GetRepository<Team>().Insert(team);
            unitOfWork.Save();


            User user = new User();

            // Act

            var task = tournamentRegistrationsManager.AddSpeakerAsync(teamId, speakerId, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            var result = task.Result;

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.UserNotFound, result);

        }

        [TestMethod]
        public void AddSpeakerAsync_WithUserAlreadyInOtherTeam_ShouldReturnUserAlreadyInOtherTeam()
        {
            // Arrange
            String speakerId = Guid.NewGuid().ToString();
            User speaker = new User { Id = speakerId };
            var userTask = userManager.CreateAsync(speaker);
            if (!userTask.IsCompleted) { userTask.Wait(); }

            var tournament = new Tournament
            {
                TeamSize = 2
            };
            var teamId = Guid.NewGuid();

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                Tournament = tournament
            };

            team.Speaker.Add(speaker);
            unitOfWork.GetRepository<Team>().Insert(team);
            unitOfWork.Save();

            team = new Team
            {
                Id = teamId,
                Tournament = tournament
            };

            unitOfWork.GetRepository<Team>().Insert(team);
            unitOfWork.Save();


            User user = new User();

            // Act

            var task = tournamentRegistrationsManager.AddSpeakerAsync(teamId, speakerId, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            var result = task.Result;

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.SpeakerAlreadyInOtherTeam, result);

        }

        #endregion

        #region RemoveSpeaker
        [TestMethod]
        public void RemoveSpeaker_WithValidTeamIdAndSpeakerId_ShouldRemoveUser()
        {
            // Arrange

            String speakerId = Guid.NewGuid().ToString();
            User speaker = new User { Id = speakerId };
            var userTask = userManager.CreateAsync(speaker);
            if (!userTask.IsCompleted) { userTask.Wait(); }

            Guid teamId = Guid.NewGuid();
            Team team = new Team
            {
                Id = teamId
            };
            team.Speaker.Add(speaker);
            unitOfWork.GetRepository<Team>().Insert(team);
            unitOfWork.Save();

            User user = new User();

            // Act

            var result = tournamentRegistrationsManager.RemoveSpeaker(teamId, speakerId, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.TeamUpdated, result);
            team = tournamentRegistrationsManager.GetTeam(teamId);
            speaker = team.Speaker.FirstOrDefault(sp => sp.Id == speakerId);
            Assert.IsNull(speaker);
        }

        [TestMethod]
        public void RemoveSpeaker_WithInvalidTeamId_ShouldReturnTeamNotFound()
        {
            // Arrange

            String speakerId = Guid.NewGuid().ToString();
            Guid teamId = Guid.NewGuid();

            User user = new User();

            // Act

            var result = tournamentRegistrationsManager.RemoveSpeaker(teamId, speakerId, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.TeamNotFound, result);
        }

        [TestMethod]
        public void RemoveSpeaker_WithInvalidSpeakerId_ShouldReturnUserNotFound()
        {
            // Arrange

            String speakerId = Guid.NewGuid().ToString();

            Guid teamId = Guid.NewGuid();
            Team team = new Team
            {
                Id = teamId
            };
            unitOfWork.GetRepository<Team>().Insert(team);
            unitOfWork.Save();


            User user = new User();

            // Act

            var result = tournamentRegistrationsManager.RemoveSpeaker(teamId, speakerId, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.UserNotFound, result);
        }

        #endregion

        #region SetTeam

        [TestMethod]
        public void SetTeam_WithNewTeam_ShouldCreateNewTeam()
        {
            #region Arrange
            // Arrange

            Guid tournamentId = Guid.NewGuid();
            Tournament tournament = new Tournament
            {
                Id = tournamentId,
                TeamSize = 2
            };

            Guid organizationId = Guid.NewGuid();

            TournamentOrganizationRegistration registration = new TournamentOrganizationRegistration
            {
                TournamentId = tournamentId,
                Tournament = tournament,
                OrganizationId = organizationId,
                TeamsWanted = 2,
                TeamsGranted = 2,
                TeamsPaid = 2,
            };

            unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
            unitOfWork.Save();

            Team team = new Team
            {
                Id = Guid.NewGuid(),
                TournamentId = tournamentId,
                Tournament = tournament,
                OrganizationId = organizationId
            };

            for (int i = 0; i < tournament.TeamSize; i++)
            {
                var speaker = new User
                {
                    Id = Guid.NewGuid().ToString()
                };
                var task = userManager.CreateAsync(speaker);
                if (!task.IsCompleted) { task.Wait(); }
                team.Speaker.Add(speaker);
            }

            User user = new User();

            #endregion
            // Act

            var result = tournamentRegistrationsManager.SetTeam(team, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.TeamAdded, result);
            var teamId = team.Id;
            team = null;
            team = tournamentRegistrationsManager.GetTeam(teamId);
            Assert.AreEqual(teamId, team.Id);

        }

        [TestMethod]
        public void SetTeam_WithExistingTeam_ShouldUpdateTeam()
        {
            #region Arrange
            // Arrange

            Guid tournamentId;
            Tournament tournament;

            {
                #region Create tournament

                // Create tournament
                tournamentId = Guid.NewGuid();
                tournament = new Tournament
                {
                    Id = tournamentId,
                    TeamSize = 2
                };

                #endregion
            }

            Guid organizationId = Guid.NewGuid();
            TournamentOrganizationRegistration registration;
            {
                #region Create registration

                // Create registration

                registration = new TournamentOrganizationRegistration
                {
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId,
                    TeamsWanted = 2,
                    TeamsGranted = 2,
                    TeamsPaid = 2,
                };

                unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
                unitOfWork.Save();

                #endregion

            }

            Team team;
            {
                #region Create original team

                // Create original team

                team = new Team
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId
                };

                for (int i = 0; i < tournament.TeamSize; i++)
                {
                    var speaker = new User
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    var task = userManager.CreateAsync(speaker);
                    if (!task.IsCompleted) { task.Wait(); }
                    team.Speaker.Add(speaker);
                }

                unitOfWork.GetRepository<Team>().Insert(team);
                unitOfWork.Save();

                #endregion
            }

            Team updatedTeam;
            {
                #region Create updated team

                // Create updated Team

                updatedTeam = new Team
                {
                    Id = team.Id,
                    TournamentId = team.TournamentId,
                    Tournament = tournament,
                    OrganizationId = team.OrganizationId,
                };

                foreach (var speaker in team.Speaker)
                {
                    updatedTeam.Speaker.Add(speaker);
                }

                {
                    #region Replace first speaker

                    // Replace first speaker

                    User newSpeaker = new User
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    var task = userManager.CreateAsync(newSpeaker);
                    if (!task.IsCompleted) { task.Wait(); }

                    updatedTeam.Speaker[0] = newSpeaker;

                    #endregion
                }

                #endregion
            }
            User user = new User();

            #endregion

            // Act

            var result = tournamentRegistrationsManager.SetTeam(updatedTeam, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.TeamUpdated, result);
            var teamId = team.Id;
            team = null;
            team = tournamentRegistrationsManager.GetTeam(teamId);
            Assert.AreEqual(updatedTeam.Speaker[0].Id, team.Speaker[0].Id);


        }

        [TestMethod]
        public void SetTeam_WithoutRegistration_ShouldReturnRegistrationNotFound()
        {
            // Arrange

            Team team = new Team { };
            User user = new User();

            // Act

            var result = tournamentRegistrationsManager.SetTeam(team, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.RegistrationNotFound, result);

        }

        [TestMethod]
        public void SetTeam_WithTooManySpeakers_ShouldReturnTooManySpeakers()
        {
            #region Arrange
            // Arrange

            Guid tournamentId;
            Tournament tournament;

            {
                #region Create tournament

                // Create tournament
                tournamentId = Guid.NewGuid();
                tournament = new Tournament
                {
                    Id = tournamentId,
                    TeamSize = 2
                };

                #endregion
            }

            Guid organizationId = Guid.NewGuid();
            TournamentOrganizationRegistration registration;
            {
                #region Create registration

                // Create registration

                registration = new TournamentOrganizationRegistration
                {
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId,
                    TeamsWanted = 2,
                    TeamsGranted = 2,
                    TeamsPaid = 2,
                };

                unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
                unitOfWork.Save();

                #endregion

            }

            Team team;
            {
                #region Create original team

                // Create original team

                team = new Team
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId
                };

                for (int i = 0; i < tournament.TeamSize + 1; i++)
                {
                    var speaker = new User
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    var task = userManager.CreateAsync(speaker);
                    if (!task.IsCompleted) { task.Wait(); }
                    team.Speaker.Add(speaker);
                }

                unitOfWork.GetRepository<Team>().Insert(team);
                unitOfWork.Save();

                #endregion
            }

            User user = new User();

            #endregion

            // Act

            var result = tournamentRegistrationsManager.SetTeam(team, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.TooManySpeakers, result);
        }


        [TestMethod]
        public void SetTeam_WithDuplicateSpeaker_ShouldReturnSpeakerAlreadyInTeam()
        {
            #region Arrange
            // Arrange

            Guid tournamentId;
            Tournament tournament;

            {
                #region Create tournament

                // Create tournament
                tournamentId = Guid.NewGuid();
                tournament = new Tournament
                {
                    Id = tournamentId,
                    TeamSize = 2
                };

                #endregion
            }

            Guid organizationId = Guid.NewGuid();
            TournamentOrganizationRegistration registration;
            {
                #region Create registration

                // Create registration

                registration = new TournamentOrganizationRegistration
                {
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId,
                    TeamsWanted = 2,
                    TeamsGranted = 2,
                    TeamsPaid = 2,
                };

                unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
                unitOfWork.Save();

                #endregion

            }

            Team team;
            {
                #region Create original team

                // Create original team

                team = new Team
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId
                };

                for (int i = 0; i < tournament.TeamSize; i++)
                {
                    var speaker = new User
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    var task = userManager.CreateAsync(speaker);
                    if (!task.IsCompleted) { task.Wait(); }
                    team.Speaker.Add(speaker);
                }

                unitOfWork.GetRepository<Team>().Insert(team);
                unitOfWork.Save();

                #endregion
            }

            Team updatedTeam;
            {
                #region Create updated team

                // Create updated Team

                updatedTeam = new Team
                {
                    Id = team.Id,
                    TournamentId = team.TournamentId,
                    Tournament = tournament,
                    OrganizationId = team.OrganizationId,
                };

                foreach (var speaker in team.Speaker)
                {
                    updatedTeam.Speaker.Add(speaker);
                }

                {
                    #region Replace first speaker with second speaker

                    // Replace first speaker with second speaker


                    updatedTeam.Speaker[0] = updatedTeam.Speaker[1];

                    #endregion
                }

                #endregion
            }
            User user = new User();

            #endregion

            // Act

            var result = tournamentRegistrationsManager.SetTeam(updatedTeam, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.SpeakerAlreadyInTeam, result);

        }

        [TestMethod]
        public void SetTeam_WithSpeakerInOutherTeam_ShouldReturnSpeakerAlreadyInOtherTeam()
        {
            #region Arrange
            // Arrange

            Guid tournamentId;
            Tournament tournament;

            {
                #region Create tournament

                // Create tournament
                tournamentId = Guid.NewGuid();
                tournament = new Tournament
                {
                    Id = tournamentId,
                    TeamSize = 2
                };

                #endregion
            }

            Guid organizationId = Guid.NewGuid();
            TournamentOrganizationRegistration registration;
            {
                #region Create registration

                // Create registration

                registration = new TournamentOrganizationRegistration
                {
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId,
                    TeamsWanted = 2,
                    TeamsGranted = 2,
                    TeamsPaid = 2,
                };

                unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
                unitOfWork.Save();

                #endregion

            }

            Team team;
            {
                #region Create original team

                // Create original team

                team = new Team
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId
                };

                for (int i = 0; i < tournament.TeamSize; i++)
                {
                    var speaker = new User
                    {
                        Id = Guid.NewGuid().ToString()
                    };
                    var task = userManager.CreateAsync(speaker);
                    if (!task.IsCompleted) { task.Wait(); }
                    team.Speaker.Add(speaker);
                }

                unitOfWork.GetRepository<Team>().Insert(team);
                unitOfWork.Save();

                #endregion
            }

            Team newTeam;
            {
                #region Create new team

                // Create new Team

                newTeam = new Team
                {
                    Id = Guid.NewGuid(),
                    TournamentId = team.TournamentId,
                    Tournament = tournament,
                    OrganizationId = team.OrganizationId,
                };

                // Add new speaker as first speaker

                User newSpeaker = new User { Id = Guid.NewGuid().ToString() };
                var task = userManager.CreateAsync(newSpeaker);
                if (!task.IsCompleted) { task.Wait(); }
                newTeam.Speaker.Add(newSpeaker);

                // Add speaker of another team as second speaker

                newTeam.Speaker.Add(team.Speaker.First());

                #endregion
            }
            User user = new User();

            #endregion

            // Act

            var result = tournamentRegistrationsManager.SetTeam(newTeam, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.SpeakerAlreadyInOtherTeam, result);

        }

        [TestMethod]
        public void SetTeam_WithMaximumTeamsExceeded_ShouldReturnTooManyTeams()
        {
            #region Arrange
            // Arrange

            Guid tournamentId;
            Tournament tournament;

            {
                #region Create tournament

                // Create tournament
                tournamentId = Guid.NewGuid();
                tournament = new Tournament
                {
                    Id = tournamentId,
                    TeamSize = 2
                };

                #endregion
            }

            Guid organizationId = Guid.NewGuid();
            TournamentOrganizationRegistration registration;
            {
                #region Create registration

                // Create registration

                registration = new TournamentOrganizationRegistration
                {
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId,
                    TeamsWanted = 2,
                    TeamsGranted = 2,
                    TeamsPaid = 2,
                };

                unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
                unitOfWork.Save();

                #endregion

            }

            Team team;

            {
                #region Create maximum number of teams

                // Create maximum number of teams

                for (int i = 0; i < registration.TeamsPaid; i++)
                {
                    team = new Team
                    {
                        Id = Guid.NewGuid(),
                        TournamentId = tournamentId,
                        Tournament = tournament,
                        OrganizationId = organizationId
                    };

                    for (int s = 0; s < tournament.TeamSize; s++)
                    {
                        var speaker = new User
                        {
                            Id = Guid.NewGuid().ToString()
                        };
                        var task = userManager.CreateAsync(speaker);
                        if (!task.IsCompleted) { task.Wait(); }
                        team.Speaker.Add(speaker);
                    }

                    unitOfWork.GetRepository<Team>().Insert(team);
                    unitOfWork.Save();
                }

                #endregion
            }

            {
                #region Create new team

                // Create new Team

                team = new Team
                {
                    Id = Guid.NewGuid(),
                    TournamentId = tournamentId,
                    Tournament = tournament,
                    OrganizationId = organizationId,
                };

                #endregion
            }
            User user = new User();

            #endregion

            // Act

            var result = tournamentRegistrationsManager.SetTeam(team, user);

            // Assert

            Assert.AreEqual(SetTeamOrAdjudicatorResult.TooManyTeams, result);

        }

        #endregion

        #region DeleteTeam

        #endregion

        #region GenerateTeamName

        [TestMethod]
        public void GenerateAutoSuffix_For26Teams_ShouldReturnZAsSuffixForLastTeam()
        {
            // Arrange

            Guid organizationId = Guid.NewGuid();
            Guid tournamentId = Guid.NewGuid();
            String abbreviation = "Org";
            AddRegistrationWithOrganizationToRepository(organizationId, tournamentId, abbreviation);

            // Act

            for (int i = 0; i < 26; i++)
            {
                var autoSuffix = tournamentRegistrationsManager.GenerateAutosuffix(organizationId, tournamentId);
                Team team = new Team
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    TournamentId = tournamentId,
                    AutoSuffix = autoSuffix
                };
                unitOfWork.GetRepository<Team>().Insert(team);
                unitOfWork.Save();
            }

            // Assert

            var teams = tournamentRegistrationsManager.GetTeams(tournamentId, organizationId)
                .OrderBy(t => t.AutoSuffix);
            Assert.AreEqual("A", teams.First().AutoSuffix);
            Assert.AreEqual("Z", teams.Last().AutoSuffix);
        }

        [TestMethod]
        public void GenerateAutoSuffix_For52Teams_ShouldReturnAZAsSuffixFor27thTeam()
        {
            // Arrange

            Guid organizationId = Guid.NewGuid();
            Guid tournamentId = Guid.NewGuid();
            String abbreviation = "Org";
            AddRegistrationWithOrganizationToRepository(organizationId, tournamentId, abbreviation);

            // Act

            for (int i = 0; i < 52; i++)
            {
                var autoSuffix = tournamentRegistrationsManager.GenerateAutosuffix(organizationId, tournamentId);
                Team team = new Team
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    TournamentId = tournamentId,
                    AutoSuffix = autoSuffix
                };
                unitOfWork.GetRepository<Team>().Insert(team);
                unitOfWork.Save();
            }

            // Assert

            var teams = tournamentRegistrationsManager.GetTeams(tournamentId, organizationId)
                .OrderBy(t => t.AutoSuffix).ToList();
            Assert.AreEqual("A", teams.First().AutoSuffix);
            Assert.AreEqual("AZ", teams[26].AutoSuffix);
        }

        [TestMethod]
        public void GenerateAutoSuffix_For702Teams_ShouldReturnZZAsSuffixForLastTeam()
        {
            // Arrange

            Guid organizationId = Guid.NewGuid();
            Guid tournamentId = Guid.NewGuid();
            String abbreviation = "Org";
            AddRegistrationWithOrganizationToRepository(organizationId, tournamentId, abbreviation);

            // Act

            for (int i = 0; i < 702; i++)
            {
                var autoSuffix = tournamentRegistrationsManager.GenerateAutosuffix(organizationId, tournamentId);
                Team team = new Team
                {
                    Id = Guid.NewGuid(),
                    OrganizationId = organizationId,
                    TournamentId = tournamentId,
                    AutoSuffix = autoSuffix
                };
                unitOfWork.GetRepository<Team>().Insert(team);
                unitOfWork.Save();
            }

            // Assert

            var teams = tournamentRegistrationsManager.GetTeams(tournamentId, organizationId)
                .OrderBy(t => t.AutoSuffix).ToList();
            Assert.AreEqual("A", teams.First().AutoSuffix);
            Assert.AreEqual("ZZ", teams.Last().AutoSuffix);
        }

        #endregion


        #endregion


        private void AddRegistrationWithOrganizationToRepository(Guid organizationId, Guid tournamentId, String abbreviation)
        {
            Organization organization = new Organization
            {
                Id = organizationId,
                Abbreviation = abbreviation
            };

            TournamentOrganizationRegistration registration = new TournamentOrganizationRegistration
            {
                TournamentId = tournamentId,
                OrganizationId = organizationId,
                Organization = organization
            };

            unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
            unitOfWork.Save();
        }

        private TournamentOrganizationRegistration CreateTournamentOrganizationRegistration()
        {
            Organization organization = new Organization
            {
                Id = Guid.NewGuid(),
            };
            Organization linkedOrganization = new Organization
            {
                Id = Guid.NewGuid(),
                LinkedOrganization = organization,
                LinkedOrganizationId = organization.Id
            };
            organization.LinkedOrganizations.Add(linkedOrganization);

            TournamentOrganizationRegistration registration = new TournamentOrganizationRegistration
            {
                OrganizationId = organization.Id,
                Organization = organization,
                TournamentId = Guid.NewGuid(),
            };
            tournamentRegistrationsManager.SetBookingCode(registration);
            return registration;
        }

        private TournamentOrganizationRegistration AddRegistration()
        {
            TournamentOrganizationRegistration registration = CreateTournamentOrganizationRegistration();
            unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
            unitOfWork.Save();
            return registration;
        }

    }
}
