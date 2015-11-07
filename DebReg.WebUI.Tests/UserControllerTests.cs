using DebReg.Mocks;
using DebReg.Security;
using DebRegCommunication;
using DebRegComponents;
using DebReg.Web.Controllers;
using DebReg.Web.Models;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;

namespace DebReg.WebUI.Tests
{
    [TestClass]
    public class UserControllerTests
    {
        UserController userController;

        [TestInitialize]
        public void Init()
        {
            var dataMocks = new DebRegDataMocks();
            DebRegUserManager userManager = dataMocks.UserManager;
            SecurityMocks securityMocks = new SecurityMocks();
            IAuthenticationManager authManager = securityMocks.AuthManager;
            ISecurityManager securityManager = new SecurityManager(userManager, authManager);

            var unitOfWork = dataMocks.UnitOfWork;

            ICountryManager countryManager = new CountryManager(unitOfWork);
            ITournamentManager tournamentManager = new TournamentManager(unitOfWork, userManager);

            var communicationMocks = new DebRegCommunicationMocks();

            userController = new UserController(
                dataMocks.UnitOfWork,
                new TournamentRegistrationsManager(dataMocks.UnitOfWork, communicationMocks.EMailService, dataMocks.UserManager),
                new SendMail(communicationMocks.EMailService),
                new OrganizationManager(dataMocks.UnitOfWork, dataMocks.UserManager),
                userManager,
                securityManager,
                countryManager,
                tournamentManager
            );

        }
        [TestMethod]
        public void Find_WithoutSearchTerm_ShouldReturnNoResults()
        {
            // Arrange

            var searchTerm = " ";

            // Act

            ViewResult actionResult = (ViewResult)userController.Find(searchTerm);

            // Assert

            var model = (FindUserViewModel)actionResult.Model;
            Assert.AreEqual(0, model.Results.Count);
            Assert.AreEqual(searchTerm.Trim(), model.SearchTerm);
            Assert.IsFalse(model.DisplayNewUserLink);

        }

        [TestMethod]
        public void Find_WithValidSearchTerm_ShouldReturnResults()
        {
            // Arrange

            var searchTerm = "susi meier hans@moser.at";

            // Act

            ViewResult actionResult = (ViewResult)userController.Find(searchTerm);

            // Assert

            var model = (FindUserViewModel)actionResult.Model;
            Assert.AreEqual(3, model.Results.Count);
            Assert.AreEqual(searchTerm, model.SearchTerm);
            Assert.IsTrue(model.DisplayNewUserLink);

        }

        [TestMethod]
        public void Find_WithInValidSearchTerm_ShouldReturnNoResultsButNewUserLink()
        {
            // Arrange

            var searchTerm = "@@@@";

            // Act

            ViewResult actionResult = (ViewResult)userController.Find(searchTerm);

            // Assert

            var model = (FindUserViewModel)actionResult.Model;
            Assert.AreEqual(0, model.Results.Count);
            Assert.AreEqual(searchTerm, model.SearchTerm);
            Assert.IsTrue(model.DisplayNewUserLink);

        }

        [TestMethod]
        public void Find_WithShortSearchTerm_ShouldReturnModelError()
        {
            // Arrange

            var searchTerm = "@";

            // Act

            ViewResult actionResult = (ViewResult)userController.Find(searchTerm);

            // Assert

            var model = (FindUserViewModel)actionResult.Model;
            Assert.AreEqual(0, model.Results.Count);
            Assert.AreEqual(searchTerm, model.SearchTerm);

            ModelState modelStateError;
            Assert.IsTrue(actionResult.ViewData.ModelState.TryGetValue("searchTerm", out modelStateError));
            Assert.AreEqual(1, modelStateError.Errors.Count);
            Assert.AreEqual(Resources.Shared.FindUser.Strings.SearchTermMinimumCharactersErrorMessage, modelStateError.Errors[0].ErrorMessage);

            Assert.IsFalse(model.DisplayNewUserLink);

        }
    }
}
