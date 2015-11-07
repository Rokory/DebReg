using DebReg.Data;
using DebReg.Mocks;
using DebReg.Models;
using DebReg.Security;
using DebRegComponents;
using DebRegOrchestration;
using DebReg.Web.Controllers;
using DebReg.Web.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using System.Web.Mvc;

namespace DebReg.WebUI.Tests
{
    [TestClass]
    public class TournamentRegistrationControllerTests
    {
        TournamentRegistrationController tournamentRegistrationController;
        HTTPMocks httpMocks;
        DebRegUserManager userManager;
        IUnitOfWork unitOfWork;
        ITournamentRegistrationsManager tournamentRegistrationsManager;
        IBookingManager bookingManager;

        [TestInitialize]
        public void Init()
        {
            var securityMocks = new SecurityMocks();
            httpMocks = new HTTPMocks();


            var debRegDataMocks = new DebRegDataMocks();
            unitOfWork = debRegDataMocks.UnitOfWork;
            userManager = debRegDataMocks.UserManager;


            var communicationMocks = new DebRegCommunicationMocks();

            tournamentRegistrationsManager = new TournamentRegistrationsManager(unitOfWork, communicationMocks.EMailService, userManager);
            bookingManager = new BookingManager(unitOfWork);

            ISlotManager slotManager = null;
            ISecurityManager securityManager = null;

            tournamentRegistrationController = new TournamentRegistrationController(
                unitOfWork,
                tournamentRegistrationsManager,
                bookingManager,
                userManager,
                slotManager,
                securityManager
            );
            tournamentRegistrationController.ControllerContext = httpMocks.ControllerContext;
        }

        #region Display
        [TestMethod]
        public void Display_WithValidTournamentId_ShouldReturnView()
        {
            // Arrange

            Guid tournamentId = Guid.NewGuid();
            Guid organizationId = Guid.NewGuid();
            Guid billedOrganizationId = organizationId;
            User user = CreateUserInRepository();
            unitOfWork.GetRepository<OrganizationUser>().Insert(new OrganizationUser
            {
                UserId = user.Id,
                OrganizationId = organizationId,
                Role = OrganizationRole.Delegate
            });
            unitOfWork.Save();

            user.CurrentOrganizationId = organizationId;
            tournamentRegistrationsManager.AddRegistration(tournamentId, organizationId, billedOrganizationId, 2, 1, "", user);
            bookingManager.AddBooking(DateTime.Now, organizationId, tournamentId, 40, false, user);

            // Act

            var task = tournamentRegistrationController.Display(tournamentId);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            ActionResult actionResult = task.Result;


            // Assert

            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)actionResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(TournamentOrganizationRegistrationViewModel));
            TournamentOrganizationRegistrationViewModel viewModel = (TournamentOrganizationRegistrationViewModel)viewResult.Model;
            Assert.AreEqual(tournamentId, viewModel.Registration.TournamentId);
            Assert.AreEqual(organizationId, viewModel.Registration.OrganizationId);
            Assert.IsTrue(viewModel.Bookings.Count() > 0);
        }

        [TestMethod]
        public void Display_WithMissingCurrentOrganizationId_ShouldReturnRedirect()
        {
            // Arrange

            Guid tournamentId = Guid.NewGuid();
            Guid organizationId = Guid.NewGuid();
            User user = CreateUserInRepository();
            unitOfWork.GetRepository<OrganizationUser>().Insert(new OrganizationUser
            {
                UserId = user.Id,
                OrganizationId = organizationId,
                Role = OrganizationRole.Delegate
            });

            // Act

            var task = tournamentRegistrationController.Display(tournamentId);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            ActionResult actionResult = task.Result;


            // Assert

            Assert.IsInstanceOfType(actionResult, typeof(RedirectToRouteResult));
            RedirectToRouteResult redirectToRouteResult = (RedirectToRouteResult)actionResult;
        }

        [TestMethod]
        public void Display_WithValidTournamentIdAndOrganizationId_ShouldReturnView()
        {
            // Arrange

            Guid tournamentId = Guid.NewGuid();
            Guid organizationId = Guid.NewGuid();
            Guid billedOrganizationId = organizationId;
            User user = CreateUserInRepository();
            unitOfWork.GetRepository<TournamentUserRole>().Insert(new TournamentUserRole
            {
                UserId = user.Id,
                TournamentId = tournamentId,
                Role = TournamentRole.SlotManager
            });
            unitOfWork.Save();

            tournamentRegistrationsManager.AddRegistration(tournamentId, organizationId, billedOrganizationId, 2, 1, "", user);
            bookingManager.AddBooking(DateTime.Now, organizationId, tournamentId, 40, false, user);

            // Act

            var task = tournamentRegistrationController.Display(tournamentId, organizationId);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            ActionResult actionResult = task.Result;


            // Assert

            Assert.IsInstanceOfType(actionResult, typeof(ViewResult));
            ViewResult viewResult = (ViewResult)actionResult;
            Assert.IsInstanceOfType(viewResult.Model, typeof(TournamentOrganizationRegistrationViewModel));
            TournamentOrganizationRegistrationViewModel viewModel = (TournamentOrganizationRegistrationViewModel)viewResult.Model;
            Assert.AreEqual(tournamentId, viewModel.Registration.TournamentId);
            Assert.AreEqual(organizationId, viewModel.Registration.OrganizationId);
            Assert.IsTrue(viewModel.Bookings.Count() > 0);
        }

        [TestMethod]
        public void Display_WithUnauthorizedUser_ShouldReturnRedirect()
        {
            // Arrange

            Guid tournamentId = Guid.NewGuid();
            Guid organizationId = Guid.NewGuid();
            User user = CreateUserInRepository();
            user.CurrentOrganizationId = organizationId;

            // Act

            var task = tournamentRegistrationController.Display(tournamentId);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            ActionResult actionResult = task.Result;

            // Assert

            Assert.IsInstanceOfType(actionResult, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void Display_WithInValidTournamentId_ShouldReturnRedirect()
        {
            // Arrange

            Guid tournamentId = Guid.NewGuid();
            Guid organizationId = Guid.NewGuid();
            User user = CreateUserInRepository();
            unitOfWork.GetRepository<OrganizationUser>().Insert(new OrganizationUser
            {
                UserId = user.Id,
                OrganizationId = organizationId,
                Role = OrganizationRole.Delegate
            });
            unitOfWork.Save();

            user.CurrentOrganizationId = organizationId;

            // Act

            var task = tournamentRegistrationController.Display(tournamentId);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            ActionResult actionResult = task.Result;


            // Assert

            Assert.IsInstanceOfType(actionResult, typeof(RedirectToRouteResult));
        }

        #endregion

        private User CreateUserInRepository()
        {
            User user = new User { Id = Guid.NewGuid().ToString(), UserName = "test@test.com" };
            var userCreateTask = userManager.CreateAsync(user);
            if (!userCreateTask.IsCompleted) { userCreateTask.Wait(); }
            httpMocks.UserName = user.UserName;
            httpMocks.UserId = user.Id;
            return user;
        }


    }
}
