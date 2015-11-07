using DebReg.Data;
using DebReg.Mocks;
using DebReg.Security;
using DebRegComponents;
using DebReg.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DebReg.Components.Tests
{
    [TestClass]
    public class TournamentManagerTests
    {

        Organization organization;
        TournamentManager tournamentManager;
        DebRegUserManager userManager;
        IUnitOfWork unitOfWork;


        [TestInitialize]
        public void Init()
        {
            var debRegDataMocks = new DebRegDataMocks();
            organization = new Organization { Id = Guid.NewGuid() };


            unitOfWork = debRegDataMocks.UnitOfWork;
            userManager = debRegDataMocks.UserManager;
            tournamentManager = new TournamentManager(unitOfWork, userManager);
        }

        #region TEST METHODS
        [TestMethod]
        public void AddTournament_WithValidUser_ShouldAddTournament()
        {
            // arrange
            var tournament = CreateTournament();
            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.OrganizationTournamentManager);

            // act
            #region ACT

            {
                var task = tournamentManager.AddTournamentAsync(tournament, user);
                if (!task.IsCompleted)
                {
                    task.Wait();
                }
            }

            #endregion

            // assert

            #region ASSERT
            var savedTournament = unitOfWork.GetRepository<Tournament>().GetById(tournament.Id);
            Assert.AreEqual(tournament.Id, savedTournament.Id);
            Assert.AreEqual(tournament.HostingOrganizationID, savedTournament.HostingOrganizationID);
            Assert.AreEqual(user.Id, savedTournament.CreatedById);
            Assert.AreEqual(user.Id, savedTournament.ModifiedById);
            Assert.IsNotNull(savedTournament.Created);
            Assert.IsNotNull(savedTournament.Modified);
            #endregion
        }

        [TestMethod]
        public void AddTournament_UserMember_ShouldThrowUnauthorizedAccess()
        {
            // arrange
            #region ARRANGE
            var tournament = CreateTournament();
            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.Member);

            #endregion

            // Act

            #region ACT

            Task task = tournamentManager.AddTournamentAsync(tournament, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            #endregion

            // Assert 

            #region ASSERT

            Assert.AreEqual(TaskStatus.Faulted, task.Status);
            var exception = task.Exception.InnerException;
            Assert.IsInstanceOfType(exception, typeof(UnauthorizedAccessException));
            Assert.AreEqual(TournamentManager.UserNotAuthorizedMessage, exception.Message);
            #endregion
        }

        [TestMethod]
        public void AddTournament_UserDelegate_ShouldThrowUnauthorizedAccess()
        {
            // arrange
            #region ARRANGE
            var tournament = CreateTournament();
            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.Delegate);

            #endregion

            // act
            #region ACT

            Task task = tournamentManager.AddTournamentAsync(tournament, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            #endregion

            // Assert 

            #region ASSERT

            Assert.AreEqual(TaskStatus.Faulted, task.Status);
            var exception = task.Exception.InnerException;
            Assert.IsInstanceOfType(exception, typeof(UnauthorizedAccessException));
            Assert.AreEqual(TournamentManager.UserNotAuthorizedMessage, exception.Message);
            #endregion

        }

        [TestMethod]
        public void AddTournament_UserNone_ShouldThrowUnauthorizedAccess()
        {
            // Arrange

            #region ARRANGE
            var tournament = CreateTournament();
            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.None);

            #endregion

            // Act

            #region ACT

            Task task = tournamentManager.AddTournamentAsync(tournament, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            #endregion

            // Assert 

            #region ASSERT

            Assert.AreEqual(TaskStatus.Faulted, task.Status);
            var exception = task.Exception.InnerException;
            Assert.IsInstanceOfType(exception, typeof(UnauthorizedAccessException));
            Assert.AreEqual(TournamentManager.UserNotAuthorizedMessage, exception.Message);
            #endregion
        }

        [TestMethod]
        public void AddTournament_WrongHostingOrganization_ShouldThrowUnauthorizedAccess()
        {
            // Arrange
            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.OrganizationTournamentManager);
            var tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                HostingOrganization = new Organization { Id = Guid.NewGuid() }
            };

            tournament.HostingOrganizationID = tournament.HostingOrganization.Id;

            // Act

            #region ACT

            Task task = tournamentManager.AddTournamentAsync(tournament, user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            #endregion

            // Assert 

            #region ASSERT

            Assert.AreEqual(TaskStatus.Faulted, task.Status);
            var exception = task.Exception.InnerException;
            Assert.IsInstanceOfType(exception, typeof(UnauthorizedAccessException));
            Assert.AreEqual(TournamentManager.UserNotAuthorizedMessage, exception.Message);
            #endregion
        }

        [TestMethod]
        public void UpdateTournament_WithValidUser_ShouldUpdateTournament()
        {
            // Arrange
            #region ARRANGE

            #region CREATE ORIGINAL TOURNAMENT

            var tournament = CreateTournament();

            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();
            #endregion

            #region CREATE UPDATED TOURNAMENT

            string tournamentName = "test tournament";
            string tournamentLocation = "test location";
            DateTime tournamentStart = DateTime.Today.AddDays(30);
            DateTime tournamentEnd = tournamentStart.AddDays(2);
            DateTime tournamentRegistrationStart = tournamentStart.AddDays(-30);
            DateTime tournamentRegistrationEnd = tournamentRegistrationStart.AddDays(7);
            int? tournamentAdjudicatorSubtract = 2;
            int tournamentTeamSize = 2;
            int tournamentTeamCap = 48;
            int tournamentAdjudicatorCap = 48;
            Product teamProduct = new Product
            {
                Id = Guid.NewGuid()
            };
            Product adjudicatorProduct = new Product
            {
                Id = Guid.NewGuid()
            };
            bool tournamentUniversityRequired = false;
            Currency tournamentCurrency = new Currency
            {
                Id = Guid.NewGuid()
            };
            string tournamentFinanceEmail = "finance@tournament.org";
            string tournamentTC = "Terms & Conditions";
            string tournamentTCLink = "http://tournamnet.org/tc.html";
            string tournamentMoneyTransferLinkCaption = "Money transfer";
            string tournamentMondeyTransferLink = "http://moneytransfer.com";
            string tournamentPaymentReference = "Payment reference";
            BankAccount bankAccount = new BankAccount
            {
                Id = Guid.NewGuid()
            };

            tournament = new Tournament
            {
                Id = tournament.Id,
                HostingOrganization = organization,
                Name = tournamentName,
                Location = tournamentLocation,
                Start = tournamentStart,
                End = tournamentEnd,
                RegistrationStart = tournamentRegistrationStart,
                RegistrationEnd = tournamentRegistrationEnd,
                AdjucatorSubtract = tournamentAdjudicatorSubtract,
                TeamSize = tournamentTeamSize,
                TeamCap = tournamentTeamCap,
                AdjudicatorCap = tournamentAdjudicatorCap,
                TeamProduct = teamProduct,
                AdjudicatorProduct = adjudicatorProduct,
                UniversityRequired = tournamentUniversityRequired,
                Currency = tournamentCurrency,
                FinanceEMail = tournamentFinanceEmail,
                TermsConditions = tournamentTC,
                TermsConditionsLink = tournamentTCLink,
                MoneyTransferLinkCaption = tournamentMoneyTransferLinkCaption,
                MoneyTransferLink = tournamentMondeyTransferLink,
                PaymentReference = tournamentPaymentReference,
                BankAccount = bankAccount
            };
            #endregion

            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.OrganizationTournamentManager);

            #endregion

            // Act

            tournamentManager.UpdateTournament(tournament, user);

            // Assert

            #region ASSERT

            var savedTournament = unitOfWork.GetRepository<Tournament>().GetById(tournament.Id);
            //Assert.AreNotEqual(tournament, savedTournament);
            Assert.AreEqual(tournament.Id, savedTournament.Id);
            Assert.AreEqual(tournament.HostingOrganization, savedTournament.HostingOrganization);
            Assert.AreEqual(tournament.Name, savedTournament.Name);
            Assert.AreEqual(tournament.Location, savedTournament.Location);
            Assert.AreEqual(tournament.Start, savedTournament.Start);
            Assert.AreEqual(tournament.End, savedTournament.End);
            Assert.AreEqual(tournament.RegistrationStart, savedTournament.RegistrationStart);
            Assert.AreEqual(tournament.RegistrationEnd, savedTournament.RegistrationEnd);
            Assert.AreEqual(tournament.AdjucatorSubtract, savedTournament.AdjucatorSubtract);
            Assert.AreEqual(tournament.TeamSize, savedTournament.TeamSize);
            Assert.AreEqual(tournament.TeamCap, savedTournament.TeamCap);
            Assert.AreEqual(tournament.AdjudicatorCap, savedTournament.AdjudicatorCap);
            Assert.AreEqual(tournament.TeamProduct, savedTournament.TeamProduct);
            Assert.AreEqual(tournament.AdjudicatorProduct, savedTournament.AdjudicatorProduct);
            Assert.AreEqual(tournament.UniversityRequired, savedTournament.UniversityRequired);
            Assert.AreEqual(tournament.Currency, savedTournament.Currency);
            Assert.AreEqual(tournament.FinanceEMail, savedTournament.FinanceEMail);
            Assert.AreEqual(tournament.TermsConditions, savedTournament.TermsConditions);
            Assert.AreEqual(tournament.TermsConditionsLink, savedTournament.TermsConditionsLink);
            Assert.AreEqual(tournament.MoneyTransferLinkCaption, savedTournament.MoneyTransferLinkCaption);
            Assert.AreEqual(tournament.MoneyTransferLink, savedTournament.MoneyTransferLink);
            Assert.AreEqual(tournament.PaymentReference, savedTournament.PaymentReference);
            Assert.AreEqual(tournament.BankAccount, savedTournament.BankAccount);

            Assert.AreEqual(savedTournament.ModifiedBy, user);
            Assert.IsNotNull(savedTournament.Modified);

            #endregion
        }


        [TestMethod]
        public void UpdateTournament_UserNone_ShouldThrowUnauthorizedAccess()
        {
            // arrange
            #region ARRANGE
            var tournament = CreateTournament();
            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.None);

            tournament = new Tournament
            {
                Id = tournament.Id
            };

            #endregion

            // act
            #region ACT
            try
            {
                tournamentManager.UpdateTournament(tournament, user);
            }
            #endregion

            // assert
            #region ASSERT
            catch (UnauthorizedAccessException e)
            {
                StringAssert.Contains(e.Message, TournamentManager.UserNotAuthorizedMessage);
                return;
            }
            Assert.Fail("No exception was thrown.");
            #endregion

        }

        [TestMethod]
        public void UpdateTournament_UserMember_ShouldThrowUnauthorizedAccess()
        {
            // arrange
            #region ARRANGE
            var tournament = CreateTournament();
            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.Member);

            tournament = new Tournament
            {
                Id = tournament.Id
            };

            #endregion

            // act
            #region ACT
            try
            {
                tournamentManager.UpdateTournament(tournament, user);
            }
            #endregion

            // assert
            #region ASSERT
            catch (UnauthorizedAccessException e)
            {
                StringAssert.Contains(e.Message, TournamentManager.UserNotAuthorizedMessage);
                return;
            }
            Assert.Fail("No exception was thrown.");
            #endregion


        }
        [TestMethod]
        public void UpdateTournament_UserDelegate_ShouldThrowUnauthorizedAccess()
        {
            // arrange
            #region ARRANGE
            var tournament = CreateTournament();
            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.Delegate);

            tournament = new Tournament
            {
                Id = tournament.Id
            };

            #endregion

            // act
            #region ACT
            try
            {
                tournamentManager.UpdateTournament(tournament, user);
            }
            #endregion

            // assert
            #region ASSERT
            catch (UnauthorizedAccessException e)
            {
                StringAssert.Contains(e.Message, TournamentManager.UserNotAuthorizedMessage);
                return;
            }
            Assert.Fail("No exception was thrown.");
            #endregion


        }

        [TestMethod]
        public void UpdateTournament_InvalidTournament_ShouldThrowInvalidArgument()
        {
            // arrange
            Tournament tournament = CreateTournament();
            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.OrganizationTournamentManager);

            // act
            try
            {
                tournamentManager.UpdateTournament(tournament, user);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, TournamentManager.TournamentNotFoundMessage);
                return;
            }
            Assert.Fail("No exeception was thrown.");
        }

        [TestMethod]
        public void UpdateTournament_WrongHostingOrganization_ShouldThrowUnauthorizedAccess()
        {
            // Arrange
            Tournament tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                HostingOrganization = new Organization { Id = Guid.NewGuid() }
            };
            tournament.HostingOrganizationID = tournament.HostingOrganization.Id;
            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.OrganizationTournamentManager);
            tournament = new Tournament
            {
                Id = tournament.Id,
                HostingOrganization = organization,
                HostingOrganizationID = organization.Id
            };


            try
            {
                // Act
                tournamentManager.UpdateTournament(tournament, user);
            }
            catch (UnauthorizedAccessException e)
            {
                // Assert
                StringAssert.Equals(e.Message, TournamentManager.UserNotAuthorizedMessage);
                return;
            }
            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void DeleteTournament_WithValidUser_ShouldDeleteTournamentAndRegistrations()
        {
            // Arrange
            #region ARRANGE

            #region CREATE ORIGINAL TOURNAMENT

            var tournament = CreateTournament();

            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();
            #endregion

            #region CREATE REGISTRATIONS
            for (int i = 0; i < 10; i++)
            {
                TournamentOrganizationRegistration registration = new TournamentOrganizationRegistration
                {
                    TournamentId = tournament.Id,
                    OrganizationId = Guid.NewGuid()
                };
                unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
                unitOfWork.Save();
                tournament.Registrations.Add(registration);
            }
            #endregion


            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.OrganizationTournamentManager);

            #endregion

            // Act

            tournamentManager.DeleteTournament(tournament.Id, user);

            // Assert

            #region ASSERT

            var savedTournament = unitOfWork.GetRepository<Tournament>().GetById(tournament.Id);
            Assert.IsTrue(savedTournament.Deleted);
            Assert.AreEqual(user.Id, savedTournament.ModifiedBy.Id);
            Assert.IsNotNull(savedTournament.Modified);
            foreach (var registration in savedTournament.Registrations)
            {
                Assert.IsTrue(registration.Deleted);
                Assert.AreEqual(user.Id, registration.ModifiedBy.Id);
                Assert.IsNotNull(registration.Modified);
            }

            #endregion
        }

        [TestMethod]
        public void DeleteTournament_UserNone_ShouldThrowUnauthorizedAccess()
        {
            // arrange
            #region ARRANGE
            var tournament = CreateTournament();
            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.None);

            #endregion

            // act
            #region ACT
            try
            {
                tournamentManager.DeleteTournament(tournament.Id, user);
            }
            #endregion

            // assert
            #region ASSERT
            catch (UnauthorizedAccessException e)
            {
                StringAssert.Contains(e.Message, TournamentManager.UserNotAuthorizedMessage);
                return;
            }
            Assert.Fail("No exception was thrown.");
            #endregion

        }

        [TestMethod]
        public void DeleteTournament_UserMember_ShouldThrowUnauthorizedAccess()
        {
            // arrange
            #region ARRANGE
            var tournament = CreateTournament();
            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.Member);

            #endregion

            // act
            #region ACT
            try
            {
                tournamentManager.DeleteTournament(tournament.Id, user);
            }
            #endregion

            // assert
            #region ASSERT
            catch (UnauthorizedAccessException e)
            {
                StringAssert.Contains(e.Message, TournamentManager.UserNotAuthorizedMessage);
                return;
            }
            Assert.Fail("No exception was thrown.");
            #endregion


        }
        [TestMethod]
        public void DeleteTournament_UserDelegate_ShouldThrowUnauthorizedAccess()
        {
            // arrange
            #region ARRANGE
            var tournament = CreateTournament();
            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.Delegate);

            #endregion

            // act
            #region ACT
            try
            {
                tournamentManager.DeleteTournament(tournament.Id, user);
            }
            #endregion

            // assert
            #region ASSERT
            catch (UnauthorizedAccessException e)
            {
                StringAssert.Contains(e.Message, TournamentManager.UserNotAuthorizedMessage);
                return;
            }
            Assert.Fail("No exception was thrown.");
            #endregion


        }

        [TestMethod]
        public void DeleteTournament_InvalidTournament_ShouldThrowInvalidArgument()
        {
            // arrange
            Tournament tournament = CreateTournament();
            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.OrganizationTournamentManager);

            // act
            try
            {
                tournamentManager.DeleteTournament(tournament.Id, user);
            }
            catch (ArgumentException e)
            {
                StringAssert.Contains(e.Message, TournamentManager.TournamentNotFoundMessage);
                return;
            }
            Assert.Fail("No exeception was thrown.");
        }

        [TestMethod]
        public void DeleteTournament_WrongHostingOrganization_ShouldThrowUnauthorizedAccess()
        {
            // Arrange
            Tournament tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                HostingOrganization = new Organization { Id = Guid.NewGuid() }
            };
            tournament.HostingOrganizationID = tournament.HostingOrganization.Id;
            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            var user = CreateUserWithOrganizationRole(organization, OrganizationRole.OrganizationTournamentManager);

            try
            {
                // Act
                tournamentManager.DeleteTournament(tournament.Id, user);
            }
            catch (UnauthorizedAccessException e)
            {
                // Assert
                StringAssert.Equals(e.Message, TournamentManager.UserNotAuthorizedMessage);
                return;
            }
            Assert.Fail("No exception thrown.");
        }

        [TestMethod]
        public void GetTournaments_ShouldReturnOnlyTournamentsNotDeleted()
        {
            // Arrange
            var tournaments = unitOfWork.GetRepository<Tournament>().Get();
            int numberOfTournaments = tournaments.Count();

            Tournament tournament;

            if (numberOfTournaments == 0)
            {
                for (int i = 0; i < 10; i++)
                {
                    tournament = CreateTournament();
                    unitOfWork.GetRepository<Tournament>().Insert(tournament);
                    unitOfWork.Save();
                }
                numberOfTournaments = unitOfWork.GetRepository<Tournament>().Get().Count();
            }

            tournament = unitOfWork.GetRepository<Tournament>().Get().First();
            tournament.Deleted = true;
            unitOfWork.GetRepository<Tournament>().Update(tournament);
            unitOfWork.Save();

            // Act
            tournaments = tournamentManager.GetTournaments().ToList();

            // Assert
            Assert.AreEqual(numberOfTournaments - 1, tournaments.Count());
        }

        [TestMethod]
        public void GetTournament_ShouldReturnTournamentWithValidId()
        {
            // Arrange

            var tournament = CreateTournament();
            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();
            var id = tournament.Id;
            tournament = null;

            // Act

            tournament = tournamentManager.GetTournament(id);

            // Assert

            Assert.IsNotNull(tournament);
            Assert.AreEqual(tournament.Id, id);
        }

        [TestMethod]
        public void GetTournament_ShouldReturnNullWithInvalidId()
        {
            // Arrange
            var id = Guid.Empty;


            // Act

            var tournament = tournamentManager.GetTournament(id);

            // Assert
            Assert.IsNull(tournament);
        }

        #endregion

        private Tournament CreateTournament()
        {
            Tournament tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                HostingOrganization = organization,
                HostingOrganizationID = organization.Id
            };
            return tournament;
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

    }
}
