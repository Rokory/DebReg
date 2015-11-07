using DebReg.Models;
using System;
using System.Collections.Generic;

namespace DebRegComponents {
    public interface ISlotAssignmentManager {
        DebReg.Models.Version GetLatestVersion(Guid tournamentId);
        DebReg.Models.Version GetLatestPublishedVersion(Guid tournamentId);
        void PublishLatestVersion(Guid tournamentId);
        SlotAssignment GetSlotAssignment(Guid tournamentId, Guid organizationId, Guid versionId);
        SlotAssignment GetSlotAssignment(Guid tournamentId, Guid organizationId);
        IEnumerable<SlotAssignment> GetSlotAssignments(Guid tournamentId, Guid versionId);
        int GetTeamSlotsGranted(Guid tournamentId, Guid versionId);
        int GetAdjudicatorsGranted(Guid tournamentId, Guid versionId);
        DebReg.Models.Version CreateVersion(Guid tournamentId);
        SlotAssignment AssignTeamSlots(Guid tournamentId, Guid organizationId, int teamSlots, User user);
        SlotAssignment AssignTeamSlots(Guid tournamentId, Guid organizationId, Guid versionId, int teamSlots, User user);
        SlotAssignment AssignAdjudicatorSlots(Guid tournamentId, Guid organizationId, int adjudicatorSlots, User user);
        SlotAssignment AssignAdjudicatorSlots(Guid tournamentId, Guid organizationId, Guid versionId, int adjudicatorSlots, User user);
        SlotAssignment AssignSlots(Guid tournamentId, Guid organizationId, int teamSlots, int adjudicatorSlots, User user);
        SlotAssignment AssignSlots(Guid tournamentId, Guid organizationId, Guid versionId, int teamSlots, int adjudicatorSlots, User user);
    }
}
