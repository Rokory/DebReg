using DebReg.Data;
using DebReg.Mocks;
using DebReg.Security;
using DebRegCommunication;
using DebRegComponents;
using DebReg.Models;
using DebRegOrchestration;
using DebReg.Web.Areas.TournamentManagement.Controllers;
using DebReg.Web.Areas.TournamentManagement.Models;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace DebReg.WebUI.Tests
{

    [TestClass]
    public class FinanceControllerTests
    {
        FinanceController financeController;

        ITournamentRegistrationsManager registrationManager;

        HTTPMocks httpMocks = new HTTPMocks();

        User financeManagerUser = new User { Id = Guid.NewGuid().ToString(), UserName = "manager@org.com" };
        Tournament tournament;

        Organization hostingOrganization = new Organization { Id = Guid.NewGuid() };


        [TestInitialize]
        public void Init()
        {
            // Create user for finance and tournament management

            DebRegDataMocks dataMocks = new DebRegDataMocks();
            DebRegUserManager userManager = dataMocks.UserManager;
            {
                var task = userManager.CreateAsync(financeManagerUser);
                if (!task.IsCompleted)
                {
                    task.Wait();
                }
            }

            // Create hosting organization

            SecurityMocks securityMocks = new SecurityMocks();
            IAuthenticationManager authManager = securityMocks.AuthManager;
            ISecurityManager securityManager = new SecurityManager(userManager, authManager);

            IUnitOfWork unitOfWork = dataMocks.UnitOfWork;
            IOrganizationManager organizationManager = new OrganizationManager(unitOfWork, userManager);
            {
                var task = organizationManager.CreateOrganizationAsync(hostingOrganization, financeManagerUser);
                if (!task.IsCompleted)
                {
                    task.Wait();
                }
            }

            // Make user tournament manager

            OrganizationUser organizationUser = new OrganizationUser
            {
                Organization = hostingOrganization,
                OrganizationId = hostingOrganization.Id,
                User = financeManagerUser,
                UserId = financeManagerUser.Id,
                Role = OrganizationRole.OrganizationTournamentManager
            };
            financeManagerUser.OrganizationAssociations.Add(organizationUser);
            hostingOrganization.UserAssociations.Add(organizationUser);
            financeManagerUser.OrganizationAssociations.Add(organizationUser);
            unitOfWork.GetRepository<OrganizationUser>().Insert(organizationUser);
            unitOfWork.Save();

            // Create tournament

            ITournamentManager tournamentManager = new TournamentManager(unitOfWork, userManager);

            tournament = new Tournament
            {
                Id = Guid.NewGuid(),
                HostingOrganization = hostingOrganization,
                HostingOrganizationID = hostingOrganization.Id
            };
            {
                var task = tournamentManager.AddTournamentAsync(tournament, financeManagerUser);
                if (!task.IsCompleted)
                {
                    task.Wait();
                }
            }

            // Create Controller

            DebRegCommunicationMocks communicationMocks = new DebRegCommunicationMocks();
            IEMailService mailService = communicationMocks.EMailService;

            registrationManager = new TournamentRegistrationsManager(unitOfWork, mailService, userManager);
            IBookingManager bookingManager = new BookingManager(unitOfWork);
            IPaymentManager paymentManager = new PaymentManager(registrationManager, bookingManager, mailService);

            financeController = new FinanceController(tournamentManager, registrationManager, paymentManager, bookingManager, userManager);

            // Set controller context
            httpMocks.UserId = financeManagerUser.Id;
            httpMocks.UserName = financeManagerUser.UserName;

            financeController.ControllerContext = httpMocks.ControllerContext;
        }

        [TestMethod]
        public void ConfirmSlots_WithValidData_ShouldReturnViewWithChanges()
        {

            #region ARRANGE

            // Arrange

            List<RegistrationFinancialViewModel> registrations = new List<RegistrationFinancialViewModel>();

            var teamsPaid = 2;
            var adjudicatorsPaid = 1;
            Guid organizationId = Guid.NewGuid();


            {
                // Create Organization

                Organization organization = new Organization { Id = organizationId };
                Organization billedOrganization = organization;

                // Create registration

                Guid billedOrganizationId = organizationId;
                int teamsWanted = 2;
                int adjudicatorsWanted = 1;
                String notes = "";
                String userId = Guid.NewGuid().ToString();
                User user = new User { Id = userId };
                registrationManager.AddRegistration(tournament.Id, organizationId, billedOrganizationId, teamsWanted, adjudicatorsWanted, notes, user);

                // Assign slots

                registrationManager.SetTeamsAndAdjudicatorsGranted(tournament.Id, organizationId, 2, 1, user);

                // Create form data


                RegistrationFinancialViewModel registrationViewModel = new RegistrationFinancialViewModel
                {
                    TournamentId = tournament.Id,
                    OrganizationId = organizationId,
                    OrganizationName = organization.Name,
                    TeamsPaid = teamsPaid,
                    AdjudicatorsPaid = adjudicatorsPaid
                };

                registrations.Add(registrationViewModel);
            }
            #endregion


            // Act

            var task = financeController.ConfirmSlots(registrations);
            if (!task.IsCompleted)
            {
                task.Wait();
            }

            // Assert

            var actionResult = task.Result;
            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));

            ViewResult viewResult = (ViewResult)actionResult;

            List<PaymentViewModel> changes = (List<PaymentViewModel>)viewResult.Model;
            Assert.AreEqual(1, changes.Count);
            Assert.AreEqual(adjudicatorsPaid, changes[0].AdjudicatorsPaid);
            Assert.AreEqual(teamsPaid, changes[0].TeamsPaid);

        }

        [TestMethod]
        public void UpdateConfirmedSlots_WithValidData_ShouldUpdatesConfimedSlots()
        {
            #region Arrange

            // Arrange

            List<PaymentViewModel> payments = new List<PaymentViewModel>();

            var teamsPaid = 2;
            var adjudicatorsPaid = 1;
            Guid organizationId = Guid.NewGuid();


            {

                // Create Organization

                Organization organization = new Organization { Id = organizationId };
                Organization billedOrganization = organization;

                // Create registration

                Guid billedOrganizationId = organizationId;
                int teamsWanted = 2;
                int adjudicatorsWanted = 1;
                String notes = "";
                String userId = Guid.NewGuid().ToString();
                User user = new User { Id = userId };
                registrationManager.AddRegistration(tournament.Id, organizationId, billedOrganizationId, teamsWanted, adjudicatorsWanted, notes, user);

                // Assign slots

                registrationManager.SetTeamsAndAdjudicatorsGranted(tournament.Id, organizationId, 2, 1, user);

                // Create form data

                payments.Add(new PaymentViewModel
                {
                    OrganizationId = organizationId,
                    TeamsPaid = teamsPaid,
                    AdjudicatorsPaid = adjudicatorsPaid
                });

            }



            #endregion

            // Act

            var task = financeController.UpdateConfirmedSlots(payments);
            if (!task.IsCompleted)
            {
                task.Wait();
            }

            // Assert

            ActionResult actionResult = task.Result;
            Assert.IsInstanceOfType(actionResult, typeof(RedirectToRouteResult));
            var registration = registrationManager.GetRegistration(tournament.Id, organizationId);
            Assert.AreEqual(teamsPaid, registration.TeamsPaid);
            Assert.AreEqual(adjudicatorsPaid, registration.AdjudicatorsPaid);

        }
    }
}
