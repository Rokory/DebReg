using DebReg.Data;
using DebReg.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DebRegComponents
{
    public class SlotAssignmentManager : BaseManager, ISlotAssignmentManager
    {

        public SlotAssignmentManager(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }


        #region ISlotAssignmentManager Members


        public int GetTeamSlotsGranted(Guid tournamentId, Guid versionId)
        {
            return unitOfWork.GetRepository<SlotAssignment>().Get(
                a => a.TournamentId == tournamentId
                    && a.VersionId == versionId)
                    .Sum(a => a.TeamsGranted);
        }

        public int GetAdjudicatorsGranted(Guid tournamentId, Guid versionId)
        {
            return unitOfWork.GetRepository<SlotAssignment>().Get(
                a => a.TournamentId == tournamentId
                    && a.VersionId == versionId)
                    .Sum(a => a.AdjucatorsGranted);
        }

        public DebReg.Models.Version CreateVersion(Guid tournamentId)
        {
            var latestVersion = GetLatestVersion(tournamentId);

            // copy assignments of last version to new version, if last version found

            int latestVersionNumber = latestVersion == null ? 0 : latestVersion.Number;
            var newVersion = CreateVersion(latestVersionNumber + 1);
            unitOfWork.GetRepository<DebReg.Models.Version>().Insert(newVersion);
            unitOfWork.Save();

            if (latestVersion != null)
            {
                var assignments = GetSlotAssignments(tournamentId, latestVersion.Id);
                foreach (var assignment in assignments)
                {
                    var newAssignment = assignment.Clone();
                    newAssignment.Version = newVersion;
                    newAssignment.VersionId = newVersion.Id;
                    unitOfWork.GetRepository<SlotAssignment>().Insert(newAssignment);
                    unitOfWork.Save();
                }
            }

            return newVersion;
        }

        private DebReg.Models.Version CreateVersion(int number)
        {

            return new DebReg.Models.Version
            {
                Id = Guid.NewGuid(),
                Number = number,
                Status = VersionStatus.Draft
            };
        }

        public DebReg.Models.Version GetLatestVersion(Guid tournamentId)
        {
            var allAssignments = unitOfWork.GetRepository<SlotAssignment>().Get(
                a => a.TournamentId == tournamentId);

            DebReg.Models.Version version;

            if (allAssignments.Count() > 0)
            {
                int latestVersionNumber = allAssignments.Max(a => a.Version.Number);
                version = allAssignments.FirstOrDefault(a => a.Version.Number == latestVersionNumber).Version;
            }
            else
            {
                return null;
            }
            return version;
        }

        public DebReg.Models.Version GetLatestPublishedVersion(Guid tournamentId)
        {
            var assignmentsOfPublishedVersions = unitOfWork.GetRepository<SlotAssignment>().Get(
                a => a.TournamentId == tournamentId && a.Version != null && a.Version.Status == VersionStatus.Public);

            DebReg.Models.Version version = null;
            if (assignmentsOfPublishedVersions.Count() > 0)
            {
                var latestVersionNumber = assignmentsOfPublishedVersions.Max(a => a.Version.Number);
                version = assignmentsOfPublishedVersions.FirstOrDefault(a => a.Version.Number == latestVersionNumber).Version;
            }
            return version;
        }

        public void PublishLatestVersion(Guid tournamentId)
        {
            var latestPublishedVersion = GetLatestPublishedVersion(tournamentId);
            var latestVersion = GetLatestVersion(tournamentId);

            if (latestPublishedVersion != null && latestVersion != null && latestPublishedVersion.Id != latestVersion.Id)
            {
                latestPublishedVersion.Status = VersionStatus.Outdated;
                unitOfWork.Save();
            }

            if (latestVersion.Status != VersionStatus.Public)
            {
                latestVersion.Status = VersionStatus.Public;
            }
            unitOfWork.Save();
        }

        public SlotAssignment GetSlotAssignment(Guid tournamentId, Guid organizationId, Guid versionId)
        {
            return unitOfWork.GetRepository<SlotAssignment>().GetById(
                    tournamentId,
                    organizationId,
                    versionId);
        }

        public SlotAssignment GetSlotAssignment(Guid tournamentId, Guid organizationId)
        {
            var latestVersion = GetLatestVersion(tournamentId);

            if (latestVersion != null)
            {
                return unitOfWork.GetRepository<SlotAssignment>().GetById(
                tournamentId,
                organizationId,
                latestVersion.Id);

            }

            return null;
        }

        public IEnumerable<SlotAssignment> GetSlotAssignments(Guid tournamentId, Guid versionId)
        {
            return unitOfWork.GetRepository<SlotAssignment>().Get(
                a => tournamentId == a.TournamentId && versionId == a.VersionId);
        }


        public SlotAssignment AssignTeamSlots(Guid tournamentId, Guid organizationId, int teamSlots, User user)
        {
            var savedAssignment = GetOrCreateAssignment(tournamentId, organizationId);
            savedAssignment = AssignTeamSlots(savedAssignment, teamSlots, user);

            return savedAssignment;
        }
        public SlotAssignment AssignTeamSlots(Guid tournamentId, Guid organizationId, Guid versionId, int teamSlots, User user)
        {
            var savedAssignment = GetOrCreateAssignment(tournamentId, organizationId, versionId);
            savedAssignment = AssignTeamSlots(savedAssignment, teamSlots, user);

            return savedAssignment;
        }

        private SlotAssignment AssignTeamSlots(SlotAssignment assignment, int teamSlots, User user)
        {
            if (assignment.TeamsGranted != teamSlots)
            {
                assignment.TeamsGranted = teamSlots;
                assignment.UpdateTrackingData(user);

                unitOfWork.GetRepository<SlotAssignment>().Update(assignment);
                unitOfWork.Save();
            }

            return assignment;
        }

        private SlotAssignment GetOrCreateAssignment(Guid tournamentId, Guid organizationId)
        {
            var savedAssignment = GetSlotAssignment(tournamentId, organizationId);

            if (savedAssignment == null)
            {
                DebReg.Models.Version version = GetLatestVersion(tournamentId);
                if (version == null || version.Status != VersionStatus.Draft)
                {
                    version = CreateVersion(tournamentId);
                }

                savedAssignment = CreateAssignment(tournamentId, organizationId, version.Id);
                savedAssignment.Version = version;
            }
            return savedAssignment;
        }

        private SlotAssignment GetOrCreateAssignment(Guid tournamentId, Guid organizationId, Guid versionId)
        {
            var savedAssignment = GetSlotAssignment(tournamentId, organizationId, versionId);

            if (savedAssignment == null)
            {
                savedAssignment = CreateAssignment(tournamentId, organizationId, versionId);
            }
            return savedAssignment;
        }

        private SlotAssignment CreateAssignment(Guid tournamentId, Guid organizationId, Guid versionId)
        {
            var savedAssignment = new SlotAssignment
            {
                TournamentId = tournamentId,
                OrganizationId = organizationId,
                VersionId = versionId,
                TeamsGranted = 0,
                AdjucatorsGranted = 0
            };
            unitOfWork.GetRepository<SlotAssignment>().Insert(savedAssignment);
            unitOfWork.Save();
            return savedAssignment;
        }


        public SlotAssignment AssignSlots(Guid tournamentId, Guid organizationId, int teamSlots, int adjudicatorSlots, User user)
        {
            var savedAssignment = GetOrCreateAssignment(tournamentId, organizationId);
            savedAssignment = AssignSlots(savedAssignment, teamSlots, adjudicatorSlots, user);
            return savedAssignment;
        }

        public SlotAssignment AssignSlots(Guid tournamentId, Guid organizationId, Guid versionId, int teamSlots, int adjudicatorSlots, User user)
        {
            var savedAssignment = GetOrCreateAssignment(tournamentId, organizationId, versionId);
            savedAssignment = AssignSlots(savedAssignment, teamSlots, adjudicatorSlots, user);
            return savedAssignment;
        }

        private SlotAssignment AssignSlots(SlotAssignment assignment, int teamSlots, int adjudicatorSlots, User user)
        {
            if (assignment.TeamsGranted != teamSlots || assignment.AdjucatorsGranted != adjudicatorSlots)
            {
                assignment.TeamsGranted = teamSlots;
                assignment.AdjucatorsGranted = adjudicatorSlots;
                assignment.UpdateTrackingData(user);

                unitOfWork.GetRepository<SlotAssignment>().Update(assignment);
                unitOfWork.Save();
            }

            return assignment;
        }


        public SlotAssignment AssignAdjudicatorSlots(Guid tournamentId, Guid organizationId, int adjudicatorSlots, User user)
        {
            var savedAssignment = GetOrCreateAssignment(tournamentId, organizationId);
            savedAssignment = AssignAdjudicatorSlots(savedAssignment, adjudicatorSlots, user);
            return savedAssignment;
        }

        public SlotAssignment AssignAdjudicatorSlots(Guid tournamentId, Guid organizationId, Guid versionId, int adjudicatorSlots, User user)
        {
            var savedAssignment = GetOrCreateAssignment(tournamentId, organizationId, versionId);
            savedAssignment = AssignAdjudicatorSlots(savedAssignment, adjudicatorSlots, user);
            return savedAssignment;
        }

        private SlotAssignment AssignAdjudicatorSlots(SlotAssignment assignment, int adjudicatorSlots, User user)
        {
            if (assignment.AdjucatorsGranted != adjudicatorSlots)
            {
                assignment.AdjucatorsGranted = adjudicatorSlots;
                assignment.UpdateTrackingData(user);

                unitOfWork.GetRepository<SlotAssignment>().Update(assignment);
                unitOfWork.Save();
            }

            return assignment;
        }

        #endregion

    }
}
