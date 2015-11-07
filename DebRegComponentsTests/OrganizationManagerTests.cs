using DebReg.Data;
using DebReg.Mocks;
using DebReg.Security;
using DebRegComponents;
using DebReg.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace DebReg.Components.Tests {
    [TestClass]
    public class OrganizationManagerTests {
        private IUnitOfWork unitOfWork;
        private IOrganizationManager organizationManager;
        private DebRegUserManager userManager;

        [TestInitialize]
        public void Init() {
            var debRegDataMocks = new DebRegDataMocks();
            unitOfWork = debRegDataMocks.UnitOfWork;
            userManager = debRegDataMocks.UserManager;
            organizationManager = new OrganizationManager(unitOfWork, userManager);
        }

        #region GetOrganization
        [TestMethod]
        public void GetOrganization_WithValidId_ShouldReturnOrganization() {
            // Arrange
            Guid id = Guid.NewGuid();
            var organization = new Organization();
            organization.Id = id;
            unitOfWork.GetRepository<Organization>().Insert(organization);
            unitOfWork.Save();

            // Act
            var savedOrganization = organizationManager.GetOrganization(id);

            // Assert
            Assert.AreEqual(id, savedOrganization.Id);
        }

        [TestMethod]
        public void GetOrganization_WithInValidId_ShouldReturnNull() {
            // Arrange
            Guid id = Guid.NewGuid();
            var organization = new Organization();
            organization.Id = id;
            unitOfWork.GetRepository<Organization>().Insert(organization);
            unitOfWork.Save();

            // Act
            var savedOrganization = organizationManager.GetOrganization(Guid.NewGuid());

            // Assert
            Assert.IsNull(savedOrganization);
        }

        #endregion

        #region CreateOrganizationAsync
        [TestMethod]
        public void CreateOrganizationAsync_WithValidOrganization_ShouldCreateOrganization() {
            #region Arrange
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };

            String abbreviation = " Org ";
            String city = " City ";
            String country = " Country ";
            String postalCode = " PostalCode ";
            String region = " Region ";
            String streetAddress1 = " Street Address 1 ";
            String streetAddress2 = " Street Address 2 ";
            List<Organization> linkedOrganizations = new List<Organization>();
            linkedOrganizations.Add(new Organization());
            List<OrganizationUser> userAssociations = new List<OrganizationUser>();
            userAssociations.Add(new OrganizationUser());
            List<TournamentOrganizationRegistration> tournamentRegistrations = new List<TournamentOrganizationRegistration>();
            tournamentRegistrations.Add(new TournamentOrganizationRegistration());
            String name = " Organization Name ";
            Boolean university = false;
            String vatId = " ATU12345678 ";
            SMTPHostConfiguration smtpHostConfiguration = new SMTPHostConfiguration();

            Organization organization = new Organization {
                Abbreviation = abbreviation,
                Address = new Address {
                    City = city,
                    Country = country,
                    PostalCode = postalCode,
                    Region = region,
                    StreetAddress1 = streetAddress1,
                    StreetAddress2 = streetAddress2,
                },
                LinkedOrganizations = linkedOrganizations,
                UserAssociations = userAssociations,
                TournamentRegistrations = tournamentRegistrations,
                Name = name,
                SMTPHostConfiguration = smtpHostConfiguration,
                Status = OrganizationStatus.Approved,
                University = university,
                VatId = vatId
            };
            #endregion

            // Act

            var task = organizationManager.CreateOrganizationAsync(organization, user);

            if (!task.IsCompleted) { task.Wait(); }

            var result = task.Result;

            #region Assert
            // Assert

            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);

            var savedOrganization = organizationManager.GetOrganization(organization.Id);

            Assert.AreEqual(abbreviation.Trim(), savedOrganization.Abbreviation);

            Assert.AreNotEqual(Guid.Empty, savedOrganization.Address.Id);
            Assert.AreEqual(city.Trim(), savedOrganization.Address.City);
            Assert.AreEqual(country.Trim(), savedOrganization.Address.Country);
            Assert.AreEqual(postalCode.Trim(), savedOrganization.Address.PostalCode);
            Assert.AreEqual(region.Trim(), savedOrganization.Address.Region);
            Assert.AreEqual(streetAddress1.Trim(), savedOrganization.Address.StreetAddress1);
            Assert.AreEqual(streetAddress2.Trim(), savedOrganization.Address.StreetAddress2);

            Assert.AreEqual(0, savedOrganization.UserAssociations.Count);

            var organizationUser = unitOfWork.GetRepository<OrganizationUser>().GetById(organization.Id, user.Id, OrganizationRole.Delegate);
            Assert.IsNotNull(organizationUser);

            Assert.AreEqual(0, savedOrganization.TournamentRegistrations.Count);

            Assert.AreEqual(name.Trim(), savedOrganization.Name);
            Assert.AreNotEqual(Guid.Empty, savedOrganization.SMTPHostConfiguration.Id);
            Assert.AreEqual(OrganizationStatus.Unknown, savedOrganization.Status);
            Assert.AreEqual(university, savedOrganization.University);
            Assert.AreEqual(vatId.Trim(), savedOrganization.VatId);

            Assert.IsNotNull(savedOrganization.ModifiedBy);
            Assert.IsNotNull(savedOrganization.Modified);
            Assert.IsNotNull(savedOrganization.CreatedBy);
            Assert.IsNotNull(savedOrganization.Created);
            #endregion
        }

        [TestMethod]
        public void CreateOrganizationAsync_WithDuplicateOrganizationName_ShouldReturnDuplicateName() {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };
            String name = " Organization ";
            Organization organization = new Organization { Name = name };

            var task = organizationManager.CreateOrganizationAsync(organization, user);
            if (!task.IsCompleted) { task.Wait(); }
            var result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);

            organization = new Organization { Name = name };

            // Act

            task = organizationManager.CreateOrganizationAsync(organization, user);
            if (!task.IsCompleted) { task.Wait(); }
            result = task.Result;

            // Assert

            Assert.AreEqual(CreateOrUpdateOrganizationResult.DuplicateName, result);


        }

        [TestMethod]
        public void CreateOrganizationAsync_WithDuplicateAbbreviation_ShouldReturnDuplicateAbbreviation() {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };
            String name = " Organization ";
            String abbreviation = " Org ";
            Organization organization = new Organization { Name = name, Abbreviation = abbreviation };

            var task = organizationManager.CreateOrganizationAsync(organization, user);
            if (!task.IsCompleted) { task.Wait(); }
            var result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);

            name = " Organization2 ";
            organization = new Organization { Name = name, Abbreviation = abbreviation };

            // Act

            task = organizationManager.CreateOrganizationAsync(organization, user);
            if (!task.IsCompleted) { task.Wait(); }
            result = task.Result;

            // Assert

            Assert.AreEqual(CreateOrUpdateOrganizationResult.DuplicateAbbreviation, result);
        }


        #endregion

        #region UpdateOrganizationAsync
        [TestMethod]
        public void UpdateOrganizationAsync_WithCorrectOrganizationAndUser_ShouldUpdateOrganization() {
            // Arrange

            var user = new User { Id = Guid.NewGuid().ToString() };
            Organization organization = new Organization();
            var task = organizationManager.CreateOrganizationAsync(organization, user);
            if (!task.IsCompleted) {
                task.Wait();
            }

            var result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);
            organization = organizationManager.GetOrganization(organization.Id);

            String name = " Organization ";
            organization.Name = name;


            // Act

            task = organizationManager.UpdateOrganizationAsync(organization, user);
            if (!task.IsCompleted) {
                task.Wait();
            }

            // Assert

            result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);
            organization = organizationManager.GetOrganization(organization.Id);
            Assert.AreEqual(name.Trim(), organization.Name);

        }

        [TestMethod]
        public void UpdateOrganizationAsync_WithUnauthorizedUser_ShouldThrowException() {
            // Arrange

            var user = new User { Id = Guid.NewGuid().ToString() };
            Organization organization = new Organization();

            var task = organizationManager.CreateOrganizationAsync(organization, user);
            if (!task.IsCompleted) {
                task.Wait();
            }
            var result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);

            organization = organizationManager.GetOrganization(organization.Id);
            String name = " Organization ";
            organization.Name = name;

            user = new User { Id = Guid.NewGuid().ToString() };


            // Act

            try {
                task = organizationManager.UpdateOrganizationAsync(organization, user);
                if (!task.IsCompleted) {
                    task.Wait();
                }

            }

            // Assert


            catch (UnauthorizedAccessException e) {
                Assert.AreEqual(OrganizationManager.UserNotAuthorizedMessage, e.Message);
            }
        }

        [TestMethod]
        public void UpdateOrganizationAsync_WithUnauthorizedUserForLinkedOrganization_ShouldThrowException() {
            // Arrange

            var user = new User { Id = Guid.NewGuid().ToString() };
            Organization organization = new Organization();

            var task = organizationManager.CreateOrganizationAsync(organization, user);
            if (!task.IsCompleted) {
                task.Wait();
            }
            var result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);

            Organization linkedOrganization = new Organization();
            linkedOrganization.Name = "Organization";
            linkedOrganization.Abbreviation = "Org";

            task = organizationManager.CreateOrganizationAsync(linkedOrganization, new User { Id = Guid.NewGuid().ToString() });
            if (!task.IsCompleted) {
                task.Wait();
            }
            result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);


            organization = organizationManager.GetOrganization(organization.Id);
            organization.LinkedOrganization = linkedOrganization;


            // Act

            try {
                task = organizationManager.UpdateOrganizationAsync(organization, user);
                if (!task.IsCompleted) {
                    task.Wait();
                }
            }

            // Assert


            catch (UnauthorizedAccessException e) {
                Assert.AreEqual(OrganizationManager.UserNotAuthorizedMessage, e.Message);
            }
        }

        [TestMethod]
        public void UpdateOrganizationAsync_WithNonExistingOrganization_ShouldThrowException() {
            // Arrange

            var user = new User { Id = Guid.NewGuid().ToString() };
            Organization organization = new Organization { Id = Guid.NewGuid() };

            // Act

            try {
                var task = organizationManager.UpdateOrganizationAsync(organization, user);
                if (!task.IsCompleted) {
                    task.Wait();
                }
            }

            // Assert


            catch (ArgumentException e) {
                Assert.AreEqual(OrganizationManager.OrganizationNotFoundMessage, e.Message);
            }
        }

        [TestMethod]
        public void UpdateOrganizationAsync_WithDuplicateName_ShouldReturnDuplicateNameResult() {
            // Arrange

            var user = new User { Id = Guid.NewGuid().ToString() };
            String name = " Organization ";
            String abbreviation = " Org ";
            Organization organization = new Organization { Name = name, Abbreviation = abbreviation };
            var task = organizationManager.CreateOrganizationAsync(organization, user);
            if (!task.IsCompleted) {
                task.Wait();
            }
            var result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);

            name = " Organization2 ";
            abbreviation = " Org2 ";
            Organization organization2 = new Organization { Name = name, Abbreviation = abbreviation };
            task = organizationManager.CreateOrganizationAsync(organization2, user);
            if (!task.IsCompleted) {
                task.Wait();
            }
            result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);


            organization2 = organizationManager.GetOrganization(organization2.Id);

            name = "Organization";
            abbreviation = "Org2a";
            organization2.Name = name;
            organization2.Abbreviation = abbreviation;



            // Act

            task = organizationManager.UpdateOrganizationAsync(organization, user);
            if (!task.IsCompleted) {
                task.Wait();
            }

            // Assert

            result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.DuplicateName, result);
        }

        [TestMethod]
        public void UpdateOrganizationAsync_WithDuplicateAbbreviation_ShouldReturnDuplicateAbbreviationResult() {
            // Arrange

            var user = new User { Id = Guid.NewGuid().ToString() };
            String name = " Organization ";
            String abbreviation = " Org ";
            Organization organization = new Organization { Name = name, Abbreviation = abbreviation };
            var task = organizationManager.CreateOrganizationAsync(organization, user);
            if (!task.IsCompleted) {
                task.Wait();
            }
            var result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);

            name = " Organization2 ";
            abbreviation = " Org2 ";
            Organization organization2 = new Organization { Name = name, Abbreviation = abbreviation };
            task = organizationManager.CreateOrganizationAsync(organization2, user);
            if (!task.IsCompleted) {
                task.Wait();
            }
            result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.Success, result);


            organization2 = organizationManager.GetOrganization(organization2.Id);

            name = "Organization2a";
            abbreviation = "Org";
            organization2.Name = name;
            organization2.Abbreviation = abbreviation;

            // Act

            task = organizationManager.UpdateOrganizationAsync(organization2, user);
            if (!task.IsCompleted) {
                task.Wait();
            }

            // Assert

            result = task.Result;
            Assert.AreEqual(CreateOrUpdateOrganizationResult.DuplicateAbbreviation, result);
        }



        #endregion

        #region DeleteOrganizationAsync

        [TestMethod]
        public void DeleteOrganization_WithValidUser_ShouldDeleteOrganizationAndLinkedOrganizations() {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };

            Organization organization = new Organization {
                Id = Guid.NewGuid(),
                LinkedOrganizations = new List<Organization> {
                    new Organization { Id = Guid.NewGuid() }
                }
            };

            unitOfWork.GetRepository<Organization>().Insert(organization);
            unitOfWork.Save();

            unitOfWork.GetRepository<OrganizationUser>().Insert(new OrganizationUser {
                UserId = user.Id,
                OrganizationId = organization.Id,
                Role = OrganizationRole.Delegate
            });
            unitOfWork.Save();

            foreach (var linkedOrganization in organization.LinkedOrganizations) {
                unitOfWork.GetRepository<OrganizationUser>().Insert(new OrganizationUser {
                    UserId = user.Id,
                    OrganizationId = linkedOrganization.Id,
                    Role = OrganizationRole.Delegate
                });
                unitOfWork.Save();
            }

            var linkedOrganizations = organization.LinkedOrganizations;
            var organizationId = organization.Id;

            // Act

            var task = organizationManager.DeleteOrganizationAsync(organizationId, user);
            if (!task.IsCompleted) {
                task.Wait();
            }

            // Assert

            var savedOrganization = organizationManager.GetOrganization(organizationId);
            Assert.IsTrue(savedOrganization.Deleted);
            foreach (var linkedOrganization in linkedOrganizations) {
                Assert.IsTrue(linkedOrganization.Deleted);
                Assert.IsNull(linkedOrganization.LinkedOrganization);
            }

        }

        [TestMethod]
        public void DeleteOrganization_WithInvalidUser_ShouldThrowException() {
            // Arrange

            User user = new User { Id = Guid.NewGuid().ToString() };

            Organization organization = new Organization {
                Id = Guid.NewGuid(),
            };

            unitOfWork.GetRepository<Organization>().Insert(organization);
            unitOfWork.Save();
            var organizationId = organization.Id;


            user = new User { Id = Guid.NewGuid().ToString() };

            // Act

            try {
                var task = organizationManager.DeleteOrganizationAsync(organizationId, user);
                if (!task.IsCompleted) {
                    task.Wait();
                }

            }

            // Assert
            catch (UnauthorizedAccessException e) {
                Assert.AreEqual(OrganizationManager.UserNotAuthorizedMessage, e.Message);
            }
        }
        #endregion
    }
}
