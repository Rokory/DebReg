using DebReg.Models;
using System;
using System.Threading.Tasks;

namespace DebRegComponents {
    public interface IOrganizationManager {
        Organization GetOrganization(Guid id);
        void AddBankAccount(BankAccount account, User user);
        void UpdateBankAccount(BankAccount account, User user);
        void DeleteBankAccount(Guid accountId, User user);

        Task<CreateOrUpdateOrganizationResult> CreateOrganizationAsync(Organization organization, User user);
        Task<CreateOrUpdateOrganizationResult> UpdateOrganizationAsync(Organization organization, User user);
        Task DeleteOrganizationAsync(Guid organizationId, User user);
    }

    public enum CreateOrUpdateOrganizationResult {
        Success,
        DuplicateName,
        DuplicateAbbreviation,
        DuplicateNameOnLinkedOrganization,
        DuplicateAbbreviationOnLinkedOrganization
    }
}
