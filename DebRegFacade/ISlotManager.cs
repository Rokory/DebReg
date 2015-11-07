using DebReg.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DebRegOrchestration
{
    public interface ISlotManager
    {
        IList<SlotAssignment> GetSlotAssignments(Guid tournamentId, DebReg.Models.Version version, User user);
        int GetFreeTeamSlots(Guid tournamentId, Guid versionId);
        int GetFreeAdjudicatorSlots(Guid tournamentId, Guid versionId);

        IEnumerable<TournamentOrganizationRegistration> GetTeamWaitlist(Guid tournamentId, User user);
        IEnumerable<TournamentOrganizationRegistration> GetAdjudicatorWaitlist(Guid tournamentId, User user);

        void AssignTeamSlots(Guid tournamentId, User user);
        void AssignAdjudicatorSlots(Guid tournamentId, User user);
        void PublishAssignments(Guid tournamentId, DateTime paymentsDueDate, String paymentPageUrl, User user);
        // void SendAssignmentNotifications(Guid tournamentId, String paymentPageUrl, User user);
        Task<IEnumerable<User>> GetUsersWithIncompleteDataAsync(Guid organizationId, Guid tournamentId);
        Task<IEnumerable<User>> GetUsersWithIncompleteDataAsync(Guid tournamentId);
        Task<IEnumerable<UserProperty>> GetIncompletePropertiesAsync(String userId, Guid tournamentId);
    }
}
