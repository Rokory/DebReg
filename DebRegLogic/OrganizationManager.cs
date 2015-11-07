using DebReg.Data;
using DebReg.Security;
using DebReg.Models;
using EasyOn.Utilities;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace DebRegComponents
{
    public class OrganizationManager : BaseManager, IOrganizationManager
    {
        public const string UserNotAuthorizedMessage = "User is not authorized to manage this organization.";
        public const string OrganizationNotFoundMessage = "An organization with this id was not found.";

        private DebRegUserManager userManager;

        public OrganizationManager(IUnitOfWork unitOfWork, DebRegUserManager userManager)
            : base(unitOfWork)
        {
            this.userManager = userManager;
        }

        #region IOrganizationManager Members

        public async Task DeleteOrganizationAsync(Guid organizationId, User user)
        {
            // Check permissions

            if (!userManager.HasOrganizationRole(user.Id, organizationId, OrganizationRole.Delegate))
            {
                throw new UnauthorizedAccessException(UserNotAuthorizedMessage);
            }

            // Set deleted flags


            var organization = GetOrganization(organizationId);
            foreach (var linkedOrganization in organization.LinkedOrganizations)
            {
                linkedOrganization.LinkedOrganization = null;
                linkedOrganization.Deleted = true;
                linkedOrganization.UpdateTrackingData(user);
                await unitOfWork.SaveAsync();
            }

            organization.LinkedOrganization = null;
            organization.Deleted = true;
            organization.UpdateTrackingData(user);
            await unitOfWork.SaveAsync();

        }


        public Organization GetOrganization(Guid id)
        {
            return unitOfWork.GetRepository<Organization>().GetById(id);
        }

        public void AddBankAccount(BankAccount account, User user)
        {
            throw new NotImplementedException();
        }
        public void UpdateBankAccount(BankAccount account, User user)
        {
            throw new NotImplementedException();
        }

        public void DeleteBankAccount(Guid accountId, User user)
        {
            throw new NotImplementedException();
        }

        public async Task<CreateOrUpdateOrganizationResult> CreateOrganizationAsync(Organization organization, User user)
        {
            var organizationRepository = unitOfWork.GetRepository<Organization>();

            RemoveWhiteSpaceFromStrings(organization);

            // Create id

            GenerateIds(organization);


            //// Check for permissions on LinkedOrganization

            //CheckLinkedOrganizationPermissions(organization, user);

            // Check for duplicates

            var duplicateResult = CheckForDuplicates(organization);
            if (duplicateResult != CreateOrUpdateOrganizationResult.Success)
            {
                return duplicateResult;
            }

            foreach (var linkedOrganization in organization.LinkedOrganizations)
            {
                RemoveWhiteSpaceFromStrings(linkedOrganization);
                linkedOrganization.LinkedOrganization = organization;
                var linkedOrganizationDuplicateResult = CheckForDuplicates(linkedOrganization);
                switch (linkedOrganizationDuplicateResult)
                {
                    case CreateOrUpdateOrganizationResult.Success:
                        break;
                    case CreateOrUpdateOrganizationResult.DuplicateName:
                        return CreateOrUpdateOrganizationResult.DuplicateNameOnLinkedOrganization;
                    default:
                        return linkedOrganizationDuplicateResult;
                }

                if (linkedOrganization.Name.ToLower() == organization.Name.ToLower())
                {
                    return CreateOrUpdateOrganizationResult.DuplicateNameOnLinkedOrganization;
                }
            }

            SetOrganizationDefaults(organization);

            // Add organization

            organization.UpdateTrackingData(user);
            organizationRepository.Insert(organization);
            await unitOfWork.SaveAsync();

            // Make user delegate of the organization and all linked Organizations

            await AddUserAsDelegateForOrganization(organization, user);

            foreach (var linkedOrganization in organization.LinkedOrganizations)
            {
                await AddUserAsDelegateForOrganization(linkedOrganization, user);
            }

            //// Make organization the current organization of the user

            //if (organization.LinkedOrganization == null && organization.LinkedOrganizationId == null)
            //{
            //    await securityManager.SetCurrentOrganizationAsync(user, organization.Id, userManager);
            //}


            return CreateOrUpdateOrganizationResult.Success;
        }

        private void SetOrganizationDefaults(Organization organization)
        {
            foreach (var linkedOrganization in organization.LinkedOrganizations)
            {
                SetOrganizationDefaults(linkedOrganization);
            }

            // Remove linked objects

            organization.UserAssociations.Clear();
            organization.TournamentRegistrations.Clear();

            // Set status

            organization.Status = OrganizationStatus.Unknown;
        }



        public async Task<CreateOrUpdateOrganizationResult> UpdateOrganizationAsync(Organization organization, User user)
        {

            // get saved organization

            var savedOrganization = GetOrganization(organization.Id);

            // Check permissions

            if (savedOrganization.LinkedOrganization == null
                && !userManager.HasOrganizationRole(user.Id, organization.Id, OrganizationRole.Delegate))
            {
                throw new UnauthorizedAccessException(UserNotAuthorizedMessage);
            }

            if (savedOrganization.LinkedOrganization != null
                && ! userManager.HasOrganizationRole(user.Id, (Guid) savedOrganization.LinkedOrganizationId, OrganizationRole.Delegate))
            {
                throw new UnauthorizedAccessException(UserNotAuthorizedMessage);
            }

            // Clean strings

            RemoveWhiteSpaceFromStrings(organization);

            // Check for duplicates

            var duplicateResult = CheckForDuplicates(organization);

            if (duplicateResult != CreateOrUpdateOrganizationResult.Success)
            {
                return duplicateResult;
            }



            // Save linked organizations

            foreach (var linkedOrganization in organization.LinkedOrganizations)
            {
                // Is this a new linkedOrganization?
                var savedLinkedOrganization = GetOrganization(linkedOrganization.Id);

                CreateOrUpdateOrganizationResult result;
                if (savedLinkedOrganization == null)
                {
                    linkedOrganization.LinkedOrganization = savedOrganization;
                    linkedOrganization.LinkedOrganizationId = savedOrganization.Id;
                    result = await CreateOrganizationAsync(linkedOrganization, user);
                    savedLinkedOrganization = GetOrganization(linkedOrganization.Id);
                }
                else
                {
                    result = await UpdateOrganizationAsync(linkedOrganization, user);
                }


                if (result != CreateOrUpdateOrganizationResult.Success)
                {
                    return result;
                }

                if (!savedOrganization.LinkedOrganizations.Any(o => o.Id == linkedOrganization.Id))
                {
                    savedOrganization.LinkedOrganizations.Add(savedLinkedOrganization);
                }
            }



            // remove linked organizations, if necessary

            for (int i = savedOrganization.LinkedOrganizations.Count - 1; i > -1; i--)
            {
                if (!organization.LinkedOrganizations.Any(o => o.Id == savedOrganization.LinkedOrganizations[i].Id))
                {
                    var linkedOrganization = savedOrganization.LinkedOrganizations[i];
                    savedOrganization.LinkedOrganizations.RemoveAt(i);
                    await DeleteOrganizationAsync(linkedOrganization.Id, user);
                }
            }

            // Save updates

            if (organization.Address != null)
            {
                if (savedOrganization.Address == null)
                {
                    savedOrganization.Address = new Address();
                }
                organization.Address.CopyProperties(savedOrganization.Address);
                organization.AddressId = savedOrganization.AddressId;
            }

            if (organization.SMTPHostConfiguration != null)
            {
                if (savedOrganization.SMTPHostConfiguration == null)
                {
                    savedOrganization.SMTPHostConfiguration = new SMTPHostConfiguration();
                }
                organization.SMTPHostConfiguration.CopyProperties(savedOrganization.SMTPHostConfiguration);
            }

            organization.CopyProperties(savedOrganization);


            unitOfWork.GetRepository<Organization>().Update(savedOrganization);
            await unitOfWork.SaveAsync();
            organization.Id = savedOrganization.Id;

            return CreateOrUpdateOrganizationResult.Success;
        }


        #endregion

        private void RemoveWhiteSpaceFromStrings(Organization organization)
        {
            organization.Name = TrimIfNotNull(organization.Name);
            organization.Abbreviation = TrimIfNotNull(organization.Abbreviation);

            if (organization.Address != null)
            {
                organization.Address.City = TrimIfNotNull(organization.Address.City);
                organization.Address.Country = TrimIfNotNull(organization.Address.Country);
                organization.Address.PostalCode = TrimIfNotNull(organization.Address.PostalCode);
                organization.Address.Region = TrimIfNotNull(organization.Address.Region);
                organization.Address.StreetAddress1 = TrimIfNotNull(organization.Address.StreetAddress1);
                organization.Address.StreetAddress2 = TrimIfNotNull(organization.Address.StreetAddress2);

            }
            organization.VatId = TrimIfNotNull(organization.VatId);
        }

        private CreateOrUpdateOrganizationResult CheckForDuplicates(Organization organization)
        {

            // Check linked organizations

            foreach (var linkedOrganization in organization.LinkedOrganizations)
            {
                var duplicateResult = CheckForDuplicates(linkedOrganization);
                if (duplicateResult == CreateOrUpdateOrganizationResult.DuplicateName)
                {
                    duplicateResult = CreateOrUpdateOrganizationResult.DuplicateNameOnLinkedOrganization;
                }
                //if (duplicateResult == CreateOrUpdateOrganizationResult.DuplicateAbbreviation)
                //{
                //    duplicateResult = CreateOrUpdateOrganizationResult.DuplicateAbbreviationOnLinkedOrganization;
                //}
                //if (duplicateResult != CreateOrUpdateOrganizationResult.Success)
                //{
                //    return duplicateResult;
                //}
            }

            // Check organization itself

            var organizationRepository = unitOfWork.GetRepository<Organization>();
            if (organizationRepository.Get(o =>
                o.Name.Trim().ToLower() == organization.Name.ToLower()
                && o.Id != organization.Id
                && !o.Deleted)
                .Count > 0)
            {
                return CreateOrUpdateOrganizationResult.DuplicateName;
            }

            if (organization.LinkedOrganizationId == null && organization.LinkedOrganization == null)
            {
                if (organizationRepository.Get(o =>
                    o.Abbreviation.Trim().ToLower() == organization.Abbreviation.ToLower()
                    && o.Id != organization.Id
                    && !o.Deleted)
                    .Count > 0)
                {
                    return CreateOrUpdateOrganizationResult.DuplicateAbbreviation;
                }
            }

            return CreateOrUpdateOrganizationResult.Success;

        }

        private async Task AddUserAsDelegateForOrganization(Organization organization, User user)
        {
            OrganizationUser orgUser = new OrganizationUser
            {
                OrganizationId = organization.Id,
                Organization = organization,
                UserId = user.Id,
                User = user,
                Role = OrganizationRole.Delegate
            };
            orgUser.UpdateTrackingData(user);

            unitOfWork.GetRepository<OrganizationUser>().Insert(orgUser);
            await unitOfWork.SaveAsync();
        }


        private String TrimIfNotNull(String s)
        {
            if (s != null)
            {
                return s.Trim();
            }
            return null;
        }
        private void GenerateIds(Organization organization)
        {
            foreach (var linkedOrganization in organization.LinkedOrganizations)
            {
                GenerateIds(linkedOrganization);
            }

            organization.Id = Guid.NewGuid();
            if (organization.Address != null)
            {
                organization.Address.Id = Guid.NewGuid();
                organization.AddressId = organization.Address.Id;
            }

            if (organization.SMTPHostConfiguration != null)
            {
                organization.SMTPHostConfiguration.Id = Guid.NewGuid();
            }

        }



    }
}
