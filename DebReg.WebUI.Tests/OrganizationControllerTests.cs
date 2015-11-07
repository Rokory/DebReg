using DebReg.Data;
using DebReg.Mocks;
using DebReg.Security;
using DebRegComponents;
using DebReg.Models;
using DebReg.Web.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace DebReg.WebUI.Tests
{
    [TestClass]
    public class OrganizationControllerTests
    {
        private IUnitOfWork unitOfWork;
        private OrganizationController organizationController;
        private DebRegUserManager userManager;
        private HTTPMocks httpMocks;
        private IOrganizationManager organizationManager;

        [TestInitialize]
        public void Init()
        {
            var debRegDataMocks = new DebRegDataMocks();
            unitOfWork = debRegDataMocks.UnitOfWork;
            userManager = debRegDataMocks.UserManager;
            organizationManager = new OrganizationManager(unitOfWork, userManager);

            var securityMocks = new SecurityMocks();
            var securityManager = securityMocks.SecurityManager;
            organizationManager = new OrganizationManager(unitOfWork, userManager);


            organizationController = new OrganizationController(organizationManager, unitOfWork, securityManager, userManager);

            httpMocks = new HTTPMocks();
            organizationController.ControllerContext = httpMocks.ControllerContext;

        }

        #region Register with or without Id
        [TestMethod]
        public void Register_WithoutOrganizationId_ShouldReturnViewWithEmptyOrganization()
        {
            // Act

            ViewResult result = (ViewResult)organizationController.Register();

            // Assert

            Organization organization = (Organization)result.Model;
            Assert.AreEqual(Guid.Empty, organization.Id);
        }

        [TestMethod]
        public void Register_WithOrganizationIdAndAuthorizedUser_ShouldReturnViewWithOrganization()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString(), UserName = "roman.korecky@debattierklubwien.at" };

            var task = userManager.CreateAsync(user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }

            Guid organizationId = Guid.NewGuid();
            Organization organization = new Organization { Id = organizationId };
            unitOfWork.GetRepository<Organization>().Insert(organization);
            unitOfWork.Save();

            unitOfWork.GetRepository<OrganizationUser>().Insert(new OrganizationUser
            {
                OrganizationId = organization.Id,
                UserId = user.Id,
                Role = OrganizationRole.Delegate
            });
            unitOfWork.Save();

            httpMocks.UserId = user.Id;
            httpMocks.UserName = user.UserName;



            // Act

            ViewResult result = (ViewResult)organizationController.Register(organization.Id);

            // Assert

            organization = (Organization)result.Model;
            Assert.AreEqual(organizationId, organization.Id);

        }

        [TestMethod]
        public void Register_WithInvalidOrganizationId_ShouldReturnEmptyOrganization()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString(), UserName = "roman.korecky@debattierklubwien.at" };

            var task = userManager.CreateAsync(user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }
            httpMocks.UserId = user.Id;
            httpMocks.UserName = user.UserName;

            // Act

            ViewResult result = (ViewResult)organizationController.Register(Guid.NewGuid());

            // Assert

            Organization organization = (Organization)result.Model;
            Assert.AreEqual(Guid.Empty, organization.Id);

        }

        [TestMethod]
        public void Register_WithOrganizationIdAndInvalidUser_ShouldRedirectToHome()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString(), UserName = "roman.korecky@debattierklubwien.at" };

            var task = userManager.CreateAsync(user);
            if (!task.IsCompleted)
            {
                task.Wait();
            }

            Guid organizationId = Guid.NewGuid();
            Organization organization = new Organization { Id = organizationId };
            unitOfWork.GetRepository<Organization>().Insert(organization);
            unitOfWork.Save();

            unitOfWork.GetRepository<OrganizationUser>().Insert(new OrganizationUser
            {
                OrganizationId = organization.Id,
                UserId = user.Id,
                Role = OrganizationRole.Delegate
            });
            unitOfWork.Save();

            httpMocks.UserId = Guid.NewGuid().ToString();
            httpMocks.UserName = "anonymous@internet.org";



            // Act

            var result = organizationController.Register(organization.Id);

            // Assert

            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }
        #endregion

        #region Register with organization object
        [TestMethod]
        public void Register_WithAddButtonClicked_ShouldReturnOrganizationWithAdditionalLinkedOrganization()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString(), UserName = "test@test.com" };
            var userCreateTask = userManager.CreateAsync(user);
            if (!userCreateTask.IsCompleted) { userCreateTask.Wait(); }

            Organization organization = new Organization();
            httpMocks.UserName = user.UserName;
            httpMocks.UserId = user.Id;

            // Act

            var organizationRegisterTask = organizationController.Register(organization, "Add");
            if (!organizationRegisterTask.IsCompleted) { organizationRegisterTask.Wait(); }
            var result = organizationRegisterTask.Result;

            // Assert

            var model = (result as ViewResult).Model as Organization;
            Assert.AreEqual(1, model.LinkedOrganizations.Count);

        }

        [TestMethod]
        public void Register_WithRemoveButtonClicked_ShouldReturnOrganizationWithLinkedOrganizationRemoved()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString(), UserName = "test@test.com" };
            var userCreateTask = userManager.CreateAsync(user);
            if (!userCreateTask.IsCompleted) { userCreateTask.Wait(); }

            Organization organization = new Organization();
            httpMocks.UserName = user.UserName;
            httpMocks.UserId = user.Id;

            Organization[] linkedOrganizations = new Organization[3];

            for (int i = 0; i < linkedOrganizations.Length; i++)
            {
                linkedOrganizations[i] = new Organization { Id = Guid.NewGuid() };
            }

            foreach (var linkedOrganization in linkedOrganizations)
            {
                organization.LinkedOrganizations.Add(linkedOrganization);
            }

            // Act

            var organizationRegisterTask = organizationController.Register(organization, "Remove", 1);
            if (!organizationRegisterTask.IsCompleted) { organizationRegisterTask.Wait(); }
            var result = organizationRegisterTask.Result;

            // Assert

            var model = (result as ViewResult).Model as Organization;
            Assert.AreEqual(2, model.LinkedOrganizations.Count);
            Assert.AreEqual(linkedOrganizations[0], model.LinkedOrganizations[0]);
            Assert.AreEqual(linkedOrganizations[2], model.LinkedOrganizations[1]);

        }

        [TestMethod]
        public void Register_WithDuplicateOrganizationName_ShouldReturnModelError()
        {
            // Arrange

            User user = CreateUserInRepository();

            Organization organization = new Organization { Id = Guid.NewGuid(), Name = "Organization" };
            var orgCreateTask = organizationManager.CreateOrganizationAsync(organization, user);
            if (!orgCreateTask.IsCompleted)
            {
                orgCreateTask.Wait();
            }

            organization = new Organization { Id = Guid.NewGuid(), Name = "Organization" };


            // Act

            var organizationRegisterTask = organizationController.Register(organization);
            if (!organizationRegisterTask.IsCompleted) { organizationRegisterTask.Wait(); }
            var result = organizationRegisterTask.Result;

            // Assert

            var view = result as ViewResult;
            var errorValues = view.ViewData.ModelState.Values
                .Where(v => v.Errors.Count >= 1);
            Assert.AreEqual(1, errorValues.Count());
            var errorValue = errorValues.FirstOrDefault();
            Assert.AreEqual(1, errorValue.Errors.Count);
            Assert.AreEqual(Resources.Organization.Strings.ErrorDuplicateOrganization, errorValue.Errors[0].ErrorMessage);
        }


        [TestMethod]
        public void Register_WithDuplicateLinkedOrganizationName_ShouldReturnModelError()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString(), UserName = "test@test.com" };
            var userCreateTask = userManager.CreateAsync(user);
            if (!userCreateTask.IsCompleted) { userCreateTask.Wait(); }
            httpMocks.UserName = user.UserName;
            httpMocks.UserId = user.Id;

            String orgName = "Organization";

            Organization organization = new Organization { Id = Guid.NewGuid(), Name = orgName, Abbreviation = "Org" };
            var orgCreateTask = organizationManager.CreateOrganizationAsync(organization, user);
            if (!orgCreateTask.IsCompleted)
            {
                orgCreateTask.Wait();
            }

            organization = new Organization
            {
                Id = Guid.NewGuid(),
                Name = "Organization 2",
                Abbreviation = "Org2",
                LinkedOrganizations = new List<Organization> { 
                    new Organization {Id = Guid.NewGuid(), Name = orgName}
                }
            };


            // Act

            var organizationRegisterTask = organizationController.Register(organization);
            if (!organizationRegisterTask.IsCompleted) { organizationRegisterTask.Wait(); }
            var result = organizationRegisterTask.Result;

            // Assert

            var view = result as ViewResult;
            var errorValues = view.ViewData.ModelState.Values
                .Where(v => v.Errors.Count >= 1);
            Assert.AreEqual(1, errorValues.Count());
            var errorValue = errorValues.FirstOrDefault();
            Assert.AreEqual(1, errorValue.Errors.Count);
            Assert.AreEqual(Resources.Organization.Strings.ErrorDuplicateLinkedOrganization, errorValue.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void Register_WithDuplicateOrganizationAbbreviation_ShouldReturnModelError()
        {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString(), UserName = "test@test.com" };
            var userCreateTask = userManager.CreateAsync(user);
            if (!userCreateTask.IsCompleted) { userCreateTask.Wait(); }
            httpMocks.UserName = user.UserName;
            httpMocks.UserId = user.Id;

            Organization organization = new Organization { Id = Guid.NewGuid(), Name = "Organization", Abbreviation = "Org" };
            var orgCreateTask = organizationManager.CreateOrganizationAsync(organization, user);
            if (!orgCreateTask.IsCompleted)
            {
                orgCreateTask.Wait();
            }

            organization = new Organization { Id = Guid.NewGuid(), Name = "Organization2", Abbreviation = "Org" };


            // Act

            var organizationRegisterTask = organizationController.Register(organization);
            if (!organizationRegisterTask.IsCompleted) { organizationRegisterTask.Wait(); }
            var result = organizationRegisterTask.Result;

            // Assert

            var view = result as ViewResult;
            var errorValues = view.ViewData.ModelState.Values
                .Where(v => v.Errors.Count >= 1);
            Assert.AreEqual(1, errorValues.Count());
            var errorValue = errorValues.FirstOrDefault();
            Assert.AreEqual(1, errorValue.Errors.Count);
            Assert.AreEqual(Resources.Organization.Strings.ErrorDuplicateOrganizationAbbreviation, errorValue.Errors[0].ErrorMessage);
        }

        [TestMethod]
        public void Register_WithNewOrganizationAndNoReturnUrl_ShouldSaveOrganization()
        {
            // Arrange

            User user = CreateUserInRepository();
            var organization = CreateOrganizationObject();

            String linkedOrganizationName = "University 1";
            organization.LinkedOrganizations.Add(new Organization { Name = linkedOrganizationName });
            String returnUrl = "http://returnurl";
            // Act

            var task = organizationController.Register(organization, "", -1, returnUrl);
            if (!task.IsCompleted)
            {
                task.Wait();
            }

            var actionResult = task.Result;

            // Assert

            var savedOrganization = unitOfWork.GetRepository<Organization>().Get(o =>
                o.Name == organization.Name).FirstOrDefault();
            Assert.IsNotNull(savedOrganization);
            Assert.AreEqual(organization.Abbreviation, savedOrganization.Abbreviation);
            Assert.AreEqual(organization.Address.City, savedOrganization.Address.City);
            Assert.AreEqual(organization.Address.Country, savedOrganization.Address.Country);
            Assert.AreEqual(organization.Address.PostalCode, savedOrganization.Address.PostalCode);
            Assert.AreEqual(organization.Address.Region, savedOrganization.Address.Region);
            Assert.AreEqual(organization.Address.StreetAddress1, savedOrganization.Address.StreetAddress1);
            Assert.AreEqual(organization.Address.StreetAddress2, savedOrganization.Address.StreetAddress2);
            Assert.AreEqual(organization.Name, savedOrganization.Name);
            Assert.AreEqual(organization.University, savedOrganization.University);
            Assert.AreEqual(organization.VatId, savedOrganization.VatId);
            Assert.AreEqual(OrganizationStatus.Unknown, savedOrganization.Status);
            Assert.AreEqual(1, savedOrganization.LinkedOrganizations.Count);
            Assert.AreEqual(linkedOrganizationName, savedOrganization.LinkedOrganizations.FirstOrDefault().Name);
            Assert.IsInstanceOfType(actionResult, typeof(RedirectResult));
            var redirectUrl = ((RedirectResult)actionResult).Url;
            Assert.AreEqual(returnUrl, redirectUrl);
        }

        [TestMethod]
        public void Register_WithNewOrganizationAndReturnUrl_ShouldSaveOrganization()
        {
            // Arrange

            User user = CreateUserInRepository();
            var organization = CreateOrganizationObject();

            // Act

            var task = organizationController.Register(organization);
            if (!task.IsCompleted)
            {
                task.Wait();
            }

            var actionResult = task.Result;

            // Assert

            var savedOrganization = unitOfWork.GetRepository<Organization>().Get(o =>
                o.Name == organization.Name).FirstOrDefault();
            Assert.IsNotNull(savedOrganization);
            Assert.IsInstanceOfType(actionResult, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void Register_WithExistingOrganization_ShouldSaveOrganization()
        {
            #region Arrange
            // Arrange

            Organization organization;
            Organization linkedOrganization;
            Guid organizationId;
            String linkedOrganizationName;

            {
                User user = CreateUserInRepository();

                // Create organization with one linked organization

                organization = new Organization { Id = Guid.NewGuid(), Name = "Organization A" };
                linkedOrganization = new Organization { Id = Guid.NewGuid(), Name = "University A" };
                organization.LinkedOrganizations.Add(linkedOrganization);


                var organizationCreateTask = organizationManager.CreateOrganizationAsync(organization, user);
                if (!organizationCreateTask.IsCompleted)
                {
                    organizationCreateTask.Wait();
                }


                organizationId = organization.Id;
                var linkedOrganizationId = linkedOrganization.LinkedOrganizationId;

                // Create a new organization object with same id and a new linked Organization to update the existing one

                organization = CreateOrganizationObject();
                organization.Id = organizationId;

                linkedOrganizationName = "University 1";
                organization.LinkedOrganizations.Add(new Organization { Id = Guid.NewGuid(), Name = linkedOrganizationName });

                httpMocks.UserId = user.Id;
                httpMocks.UserName = user.UserName;
            }

            #endregion

            // Act

            var task = organizationController.Register(organization);
            if (!task.IsCompleted)
            {
                task.Wait();
            }

            var actionResult = task.Result;

            // Assert

            var savedOrganization = unitOfWork.GetRepository<Organization>().GetById(organizationId);
            Assert.IsNotNull(savedOrganization);
            Assert.AreEqual(organization.Abbreviation, savedOrganization.Abbreviation);
            Assert.AreEqual(organization.Address.City, savedOrganization.Address.City);
            Assert.AreEqual(organization.Address.Country, savedOrganization.Address.Country);
            Assert.AreEqual(organization.Address.PostalCode, savedOrganization.Address.PostalCode);
            Assert.AreEqual(organization.Address.Region, savedOrganization.Address.Region);
            Assert.AreEqual(organization.Address.StreetAddress1, savedOrganization.Address.StreetAddress1);
            Assert.AreEqual(organization.Address.StreetAddress2, savedOrganization.Address.StreetAddress2);
            Assert.AreEqual(organization.Name, savedOrganization.Name);
            Assert.AreEqual(organization.University, savedOrganization.University);
            Assert.AreEqual(organization.VatId, savedOrganization.VatId);
            Assert.AreEqual(1, savedOrganization.LinkedOrganizations.Count);
            Assert.AreEqual(linkedOrganizationName, savedOrganization.LinkedOrganizations.FirstOrDefault().Name);
        }

        [TestMethod]
        public void Register_WithExistingOrganizationAndInvalidUser_ShouldReturnRedirect()
        {
            // Arrange

            User user = CreateUserInRepository();
            Organization organization = new Organization { Id = Guid.NewGuid() };
            var organizationCreateTask = organizationManager.CreateOrganizationAsync(organization, user);
            if (!organizationCreateTask.IsCompleted)
            {
                organizationCreateTask.Wait();
            }
            var organizationId = organization.Id;

            organization = CreateOrganizationObject();
            organization.Id = organizationId;

            String linkedOrganizationName = "University 1";
            organization.LinkedOrganizations.Add(new Organization { Name = linkedOrganizationName });

            httpMocks.UserId = Guid.NewGuid().ToString();
            httpMocks.UserName = "invalid";

            // Act

            var task = organizationController.Register(organization);
            if (!task.IsCompleted)
            {
                task.Wait();
            }

            var actionResult = task.Result;

            // Assert

            Assert.IsInstanceOfType(actionResult, typeof(RedirectToRouteResult));
        }

        #endregion
        private Organization CreateOrganizationObject()
        {
            String abbreviation = " Org ";
            String city = " City ";
            String country = " Country ";
            String postalCode = " PostalCode ";
            String region = " Region ";
            String streetAddress1 = " Street Address 1 ";
            String streetAddress2 = " Street Address 2 ";
            String name = " Organization Name ";
            Boolean university = false;
            String vatId = " ATU12345678 ";
            SMTPHostConfiguration smtpHostConfiguration = new SMTPHostConfiguration();

            Organization organization = new Organization
            {
                Abbreviation = abbreviation,
                Address = new Address
                {
                    City = city,
                    Country = country,
                    PostalCode = postalCode,
                    Region = region,
                    StreetAddress1 = streetAddress1,
                    StreetAddress2 = streetAddress2,
                },
                Name = name,
                SMTPHostConfiguration = smtpHostConfiguration,
                Status = OrganizationStatus.Approved,
                University = university,
                VatId = vatId
            };

            return organization;
        }

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
