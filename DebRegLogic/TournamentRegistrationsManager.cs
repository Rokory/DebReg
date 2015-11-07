using DebReg.Data;
using DebReg.Models;
using DebReg.Models.Comparers;
using DebReg.Security;
using DebRegCommunication;
using DebRegComponents.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DebRegComponents
{
    public class TournamentRegistrationsManager : BaseManager, ITournamentRegistrationsManager
    {
        private IEMailService mailService;
        private DebRegUserManager userManager;
        public TournamentRegistrationsManager(IUnitOfWork unitOfWork, IEMailService mailService, DebRegUserManager userManager)
            : base(unitOfWork)
        {
            this.mailService = mailService;
            this.userManager = userManager;
        }

        #region ISlotManager Members

        public IEnumerable<TournamentOrganizationRegistration> GetRegistrationsByTournamentId(Guid tournamentId)
        {
            return unitOfWork.GetRepository<TournamentOrganizationRegistration>().Get(r => r.TournamentId == tournamentId && !r.Deleted);
        }
        public TournamentOrganizationRegistration AddRegistration(Guid tournamentId, Guid organizationId, Guid billedOrganizationId, int teamsWanted, int adjudicatorsWanted, String notes, User user)
        {
            if (tournamentId != Guid.Empty
                && organizationId != Guid.Empty
                && billedOrganizationId != Guid.Empty
                && (teamsWanted > 0 || adjudicatorsWanted > 0))
            {

                TournamentOrganizationRegistration registration = new TournamentOrganizationRegistration
                {
                    TournamentId = tournamentId,
                    OrganizationId = organizationId,
                    BilledOrganizationId = billedOrganizationId,
                    TeamsWanted = teamsWanted,
                    AdjudicatorsWanted = adjudicatorsWanted,
                    TeamsGranted = 0,
                    TeamsPaid = 0,
                    AdjudicatorsGranted = 0,
                    AdjudicatorsPaid = 0,
                    LockAutoAssign = false,
                    OrganizationStatusDraft = true,
                    Notes = notes
                };
                registration.UpdateTrackingData(user);
                SetBookingCode(registration);
                unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
                unitOfWork.Save();
                return registration;
            }
            return null;
        }
        public IEnumerable<TournamentOrganizationRegistration> GetRegistrationsSortedByRank(Guid tournamentId, User user)
        {

            // Get registrations, where either the status is approved and not draft,
            // or slots have been assigned or paid
            var registrations = unitOfWork.GetRepository<TournamentOrganizationRegistration>().Get(
                r => (r.TournamentId == tournamentId
                    && (
                        (
                            r.OrganizationStatus == OrganizationStatus.Approved
                            && !r.OrganizationStatusDraft
                        )
                        || r.TeamsGranted > 0
                        || r.TeamsPaid > 0
                        || r.AdjudicatorsGranted > 0
                        || r.AdjudicatorsPaid > 0
                    )
                )
            );

            // Create seed for random

            Random rand = new Random();

            // Build a random ranking, if needed

            Boolean duplicateRandomRanks;
            do
            {
                duplicateRandomRanks = false;

                // Get groups of equal random rank

                var groupedByRandomRank = from r in registrations
                                          group r by r.RandomRank into grp
                                          select grp;


                foreach (var registrationsGroup in groupedByRandomRank)
                {

                    // if there is more than one registration in a group, randomize group
                    if (registrationsGroup.Count() > 1)
                    {
                        foreach (var registration in registrationsGroup)
                        {
                            SetRandomRank(registration.TournamentId, registration.OrganizationId, rand.Next(), user);
                        }
                        duplicateRandomRanks = true;
                    }
                }
            } while (duplicateRandomRanks);

            var result = registrations.OrderByDescending(r => r.Rank).ThenBy(r => r.RandomRank);

            return result;
        }

        public IEnumerable<TournamentOrganizationRegistration> GetApprovedRegistrations(Guid tournamentId)
        {
            var registrations = unitOfWork.GetRepository<TournamentOrganizationRegistration>().Get(
                r => r.TournamentId == tournamentId
                    && r.OrganizationStatus == OrganizationStatus.Approved
                    && !r.OrganizationStatusDraft);
            return registrations;
        }
        public TournamentOrganizationRegistration GetRegistration(Guid tournamentId, Guid organizationId)
        {
            return unitOfWork.GetRepository<TournamentOrganizationRegistration>().GetById(tournamentId, organizationId);
        }
        public TournamentOrganizationRegistration FindRegistrationByOrganizationName(Guid tournamentId, String searchTerm)
        {
            TournamentOrganizationRegistration registration = null;

            // Try to find organization in abbreviation
            var organizations = unitOfWork.GetRepository<Organization>().Get(
                o => o.Abbreviation.ToLower().Contains(searchTerm.ToLower()));

            registration = FindRegistrationByOrganizations(tournamentId, organizations);

            // if registration was not found, try to find organization and registration in name

            if (registration == null)
            {
                organizations = unitOfWork.GetRepository<Organization>().Get(
                    o => o.Name.ToLower().Contains(searchTerm.ToLower()));

                registration = FindRegistrationByOrganizations(tournamentId, organizations);
            }

            return registration;
        }

        private TournamentOrganizationRegistration FindRegistrationByOrganizations(Guid tournamentId, IEnumerable<Organization> organizations)
        {
            TournamentOrganizationRegistration registration = null;
            foreach (var organization in organizations)
            {
                registration = FindRegistrationByOrganization(tournamentId, organization);
            }

            return registration;
        }

        public TournamentOrganizationRegistration FindRegistrationByOrganization(Guid tournamentId, Organization organization)
        {
            TournamentOrganizationRegistration registration = unitOfWork.GetRepository<TournamentOrganizationRegistration>().GetById(tournamentId, organization.Id);

            // If no registration was found, find registration for linked organization
            if (registration == null)
            {
                registration = unitOfWork.GetRepository<TournamentOrganizationRegistration>().GetById(tournamentId, organization.LinkedOrganizationId);

                // If still no registration was found, find first registration for all linked organizations
                if (registration == null)
                {
                    foreach (var linkedOrganization in organization.LinkedOrganizations)
                    {
                        registration = unitOfWork.GetRepository<TournamentOrganizationRegistration>().GetById(tournamentId, linkedOrganization.Id);
                        if (registration != null)
                        {
                            break;
                        }
                    }
                }
            }

            return registration;
        }

        public TournamentOrganizationRegistration GetRegistration(String bookingCode, Guid tournamentId)
        {
            return unitOfWork.GetRepository<TournamentOrganizationRegistration>().Get(
                r => r.BookingCode == bookingCode && r.TournamentId == tournamentId).FirstOrDefault();

        }

        public IEnumerable<TournamentOrganizationRegistration> GetRegistrationsByOrganizationId(Guid organizationId)
        {
            return unitOfWork.GetRepository<TournamentOrganizationRegistration>().Get(r => r.OrganizationId == organizationId && !r.Deleted);
        }
        public void SetRank(Guid tournamentId, Guid organizationId, Decimal rank, User user)
        {
            var registration = GetRegistration(tournamentId, organizationId);
            SetBookingCode(registration);
            if (registration != null)
            {
                registration.Rank = rank;
                unitOfWork.GetRepository<TournamentOrganizationRegistration>().Update(registration);
                unitOfWork.Save();
            }
        }

        public void SetTeamsAndAdjudicatorsGranted(Guid tournamentId, Guid organizationId, int teamsGranted, int adjudicatorsGranted, User user)
        {
            var registration = GetRegistration(tournamentId, organizationId);
            SetBookingCode(registration);
            registration.TeamsGranted = teamsGranted;
            registration.AdjudicatorsGranted = adjudicatorsGranted;
            registration.UpdateTrackingData(user);
            unitOfWork.Save();
        }

        public void SetOrganizationStatusAndNote
        (
            Guid tournamentId,
            Guid organizationId,
            OrganizationStatus organizationStatus,
            String organizationStatusNote,
            Boolean draft,
            User user
        )
        {
            var registration = GetRegistration(tournamentId, organizationId);
            SetBookingCode(registration);
            registration.OrganizationStatus = organizationStatus;
            registration.OrganizationStatusNote = organizationStatusNote;
            registration.OrganizationStatusDraft = draft;
            registration.UpdateTrackingData(user);
            unitOfWork.Save();
        }

        public void PublishOrganizationStatusAndNote(Guid tournamentId, Guid organizationId, User user)
        {
            var registration = GetRegistration(tournamentId, organizationId);
            SetBookingCode(registration);
            registration.OrganizationStatusDraft = false;
            registration.Organization.Status = registration.OrganizationStatus;
            registration.UpdateTrackingData(user);
            registration.Organization.UpdateTrackingData(user);
            unitOfWork.Save();
        }

        public TournamentOrganizationRegistration SetTeamsAndAdjudicatorsPaid(Guid tournamentId, Guid organizationId, int teamsPaid, int adjudicatorsPaid, User user)
        {
            var registration = GetRegistration(tournamentId, organizationId);
            SetBookingCode(registration);
            registration.TeamsPaid = teamsPaid;
            registration.AdjudicatorsPaid = adjudicatorsPaid;

            registration.UpdateTrackingData(user);
            unitOfWork.Save();
            return registration;
        }

        public void SetRandomRank(Guid tournamentId, Guid organizationId, int randomRank, User user)
        {
            var registration = GetRegistration(tournamentId, organizationId);
            if (registration != null)
            {
                SetBookingCode(registration);
                registration.RandomRank = randomRank;
                unitOfWork.Save();
            }
        }
        public void SetLockAutoAssign(Guid tournamentId, Guid organizationId, Boolean lockAutoAssign, User user)
        {
            var registration = GetRegistration(tournamentId, organizationId);
            if (registration != null)
            {
                SetBookingCode(registration);
                registration.LockAutoAssign = lockAutoAssign;
                unitOfWork.Save();
            }

        }

        public IEnumerable<Team> GetTeams(Guid tournamentId, Guid organizationId)
        {
            return unitOfWork.GetRepository<Team>().Get(t =>
                t.OrganizationId == organizationId
                && t.TournamentId == tournamentId
                && !t.Deleted);
        }

        public IEnumerable<Team> GetTeams(Guid tournamentId)
        {
            return unitOfWork.GetRepository<Team>().Get(t =>
                t.TournamentId == tournamentId
                    && !t.Deleted);
        }

        public IEnumerable<Team> GetTeams(String userId)
        {
            var teams = from t in unitOfWork.GetRepository<Team>().Get()
                        from sp in t.Speaker
                        where sp.Id == userId
                        select t;
            return teams;
        }

        public Team GetTeam(Guid teamId)
        {
            return unitOfWork.GetRepository<Team>().GetById(teamId);
        }

        public async Task<SetTeamOrAdjudicatorResult> AddSpeakerAsync(Guid teamId, String speakerId, User user)
        {
            var team = GetTeam(teamId);

            // Get and check team
            if (team == null)
            {
                return SetTeamOrAdjudicatorResult.TeamNotFound;
            }

            // Check maximum number of speakers

            if (team.Speaker.Count >= team.Tournament.TeamSize)
            {
                return SetTeamOrAdjudicatorResult.TooManySpeakers;
            }

            // check if speaker is already in team

            if (team.Speaker.Any(sp => sp.Id == speakerId))
            {
                return SetTeamOrAdjudicatorResult.SpeakerAlreadyInTeam;
            }

            // Get and check user

            var speaker = await userManager.FindByIdAsync(speakerId);

            if (speaker == null)
            {
                return SetTeamOrAdjudicatorResult.UserNotFound;
            }

            // check if speaker is already in other team

            var teams = GetTeams(team.TournamentId);
            var tournamentSpeakers = GetSpeakers(team.TournamentId, teamId);
            if (tournamentSpeakers.Any(tsp => tsp.Id == speakerId))
            {
                return SetTeamOrAdjudicatorResult.SpeakerAlreadyInOtherTeam;
            }

            // check if speaker is already adjudicator

            var adjudicators = GetAdjudicators(team.TournamentId);
            if (adjudicators.Any(adj => adj.UserId == speakerId))
            {
                return SetTeamOrAdjudicatorResult.SpeakerAlreadyAdjudicator;
            }

            // add user to organization if needed

            if (!speaker.OrganizationAssociations.Any(oa => oa.OrganizationId == team.OrganizationId))
            {
                OrganizationUser orgUser = new OrganizationUser
                {
                    OrganizationId = team.Organization.Id,
                    Organization = team.Organization,
                    User = speaker,
                    UserId = speaker.Id,
                    Role = OrganizationRole.Member
                };

                unitOfWork.GetRepository<OrganizationUser>().Insert(orgUser);
                await unitOfWork.SaveAsync();
            }

            // add user to team

            team.Speaker.Add(speaker);
            team.UpdateTrackingData(user);
            unitOfWork.GetRepository<Team>().Update(team);
            await unitOfWork.SaveAsync();
            return SetTeamOrAdjudicatorResult.TeamUpdated;
        }

        public SetTeamOrAdjudicatorResult RemoveSpeaker(Guid teamId, String speakerId, User user)
        {
            // get team

            var team = GetTeam(teamId);

            if (team == null)
            {
                return SetTeamOrAdjudicatorResult.TeamNotFound;
            }

            // find speaker

            var speaker = team.Speaker.FirstOrDefault(sp => sp.Id == speakerId);
            if (speaker == null)
            {
                return SetTeamOrAdjudicatorResult.UserNotFound;
            }

            // remove speaker
            team.Speaker.Remove(speaker);
            team.UpdateTrackingData(user);
            unitOfWork.GetRepository<Team>().Update(team);
            unitOfWork.Save();
            return SetTeamOrAdjudicatorResult.TeamUpdated;
        }
        public SetTeamOrAdjudicatorResult SetTeam(Team team, User user)
        {

            var registration = GetRegistration(team.TournamentId, team.OrganizationId);

            if (registration == null)
            {
                return SetTeamOrAdjudicatorResult.RegistrationNotFound;
            }

            if (team.Speaker.Count > 0)
            {
                // Check maximum number of speakers

                if (team.Speaker.Count > team.Tournament.TeamSize)
                {
                    return SetTeamOrAdjudicatorResult.TooManySpeakers;
                }

                // check for duplicate speakers in team
                var distinctSpeakers = team.Speaker.Distinct(new UserComparer());
                if (distinctSpeakers.Count() != team.Speaker.Count)
                {
                    return SetTeamOrAdjudicatorResult.SpeakerAlreadyInTeam;
                }

                // check if speakers are already in other team or adjudicator

                var adjudicators = GetAdjudicators(team.TournamentId);

                var tournamentSpeakers = GetSpeakers(team.TournamentId, team.Id);

                foreach (var speaker in team.Speaker)
                {
                    if (tournamentSpeakers.Any(tsp => tsp.Id == speaker.Id))
                    {
                        return SetTeamOrAdjudicatorResult.SpeakerAlreadyInOtherTeam;
                    }

                    if (adjudicators.Any(adj => adj.UserId == speaker.Id))
                    {
                        return SetTeamOrAdjudicatorResult.SpeakerAlreadyAdjudicator;
                    }
                }


            }

            team.UpdateTrackingData(user);

            // check if team already exists

            var savedTeam = GetTeam(team.Id);

            if (savedTeam == null)
            {
                // check if organization has too many teams

                var teamCountForOrganization = unitOfWork.GetRepository<Team>().Get(t =>
                    t.TournamentId == team.TournamentId && t.OrganizationId == team.OrganizationId).Count();

                if (teamCountForOrganization >= registration.TeamsPaid)
                {
                    return SetTeamOrAdjudicatorResult.TooManyTeams;
                }

                // Insert new team

                unitOfWork.GetRepository<Team>().Insert(team);
                unitOfWork.Save();

                return SetTeamOrAdjudicatorResult.TeamAdded;
            }

            unitOfWork.Detach(savedTeam);
            unitOfWork.GetRepository<Team>().Update(team);
            unitOfWork.Save();

            return SetTeamOrAdjudicatorResult.TeamUpdated;
        }

        public Team DeleteTeam(Guid teamId, User user)
        {
            throw new NotImplementedException();
        }

        public String GenerateTeamName(Guid organizationId, Guid tournamentId)
        {
            var teams = GetTeams(tournamentId);
            return GenerateTeamName(organizationId, tournamentId, teams);
        }
        public String GenerateTeamName(Guid organizationId, Guid tournamentId, IEnumerable<Team> teams)
        {
            var suffix = GenerateAutosuffix(organizationId, tournamentId, teams);
            var registration = GetRegistration(tournamentId, organizationId);
            if (registration != null)
            {
                return String.Format("{0} {1}", registration.Organization.Abbreviation, suffix);
            }
            return String.Empty;
        }

        public String GenerateAutosuffix(Guid organizationId, Guid tournamentId)
        {
            var teams = GetTeams(tournamentId, organizationId);
            return GenerateAutosuffix(organizationId, tournamentId, teams);
        }
        public String GenerateAutosuffix(Guid organizationId, Guid tournamentId, IEnumerable<Team> teams)
        {
            // Initialize first character
            List<Char> suffixes = new List<char>();
            suffixes.Add('A');
            int characterIndex = 0;
            suffixes[characterIndex]--;

            String suffix;

            do
            {
                // next character in alphabet
                suffixes[characterIndex]++;


                #region Logic when Z is reached
                // check, if alphabet is through
                if (suffixes[characterIndex] > 'Z')
                {
                    // Find first previous character, that is not Z
                    var additionalCharacterNeeded = true;
                    for (int i = 0; i < suffixes.Count - 1; i++)
                    {
                        if (suffixes[i] < 'Z')
                        {
                            suffixes[i]++;
                            suffixes[characterIndex] = 'A';
                            additionalCharacterNeeded = false;
                            break;
                        }
                    }

                    // Do we need an additional character?
                    if (additionalCharacterNeeded)
                    {
                        // Reset all characters to A
                        for (int i = 0; i < suffixes.Count; i++)
                        {
                            suffixes[i] = 'A';
                        }

                        // Add an additional character
                        suffixes.Add('A');
                        characterIndex++;
                    }
                }

                #endregion

                // Build team name
                StringBuilder suffixBuilder = new StringBuilder();
                foreach (var character in suffixes)
                {
                    suffixBuilder.Append(character);
                }
                suffix = suffixBuilder.ToString();

            } while (teams.Any(t =>
                t.AutoSuffix != null
                && t.AutoSuffix.ToLower() == suffix.ToLower()));

            return suffix;
        }

        public IEnumerable<Adjudicator> GetAdjudicators(Guid tournamentId, Guid organizationId)
        {
            return unitOfWork.GetRepository<Adjudicator>().Get(a =>
                a.TournamentId == tournamentId
                && a.OrganizationId == organizationId
                && !a.Deleted
                );
        }

        public IEnumerable<Adjudicator> GetAdjudicators(Guid tournamentId)
        {
            return unitOfWork.GetRepository<Adjudicator>().Get(a =>
                a.TournamentId == tournamentId
                && !a.Deleted
                );
        }
        public IEnumerable<Adjudicator> GetAdjudicators(String userId)
        {
            return unitOfWork.GetRepository<Adjudicator>().Get(a => a.UserId == userId && !a.Deleted);
        }

        public Adjudicator GetAdjudicator(Guid tournamentId, string userId)
        {
            return unitOfWork.GetRepository<Adjudicator>().Get(a =>
                a.UserId == userId
                ).FirstOrDefault();
        }

        public async Task<SetTeamOrAdjudicatorResult> AddAdjudicatorAsync(Guid tournamentId, Guid organizationId, string userId, User user)
        {
            // Get registration

            var registration = GetRegistration(tournamentId, organizationId);


            // If max number of adj. is reached, return error result

            if (registration.AdjudicatorsPaid <= GetAdjudicators(tournamentId, organizationId).Count())
            {
                return SetTeamOrAdjudicatorResult.TooManyAdjudicators;
            }

            // check if already registered as speaker

            var tournamentSpeakers = GetSpeakers(tournamentId);

            if (tournamentSpeakers.Any(tsp => tsp.Id == userId))
            {
                return SetTeamOrAdjudicatorResult.AdjudicatorAlreadySpeaker;
            }

            // Try to find adjudicator

            var adjudicator = GetAdjudicator(tournamentId, userId);

            if (adjudicator != null)
            {

                // If it is deleted, undelete it and set organization

                if (adjudicator.Deleted)
                {
                    await AssociateUser(organizationId, userId, user);
                    adjudicator.OrganizationId = organizationId;
                    adjudicator.Deleted = false;
                    adjudicator.UpdateTrackingData(user);
                    unitOfWork.GetRepository<Adjudicator>().Update(adjudicator);
                    await unitOfWork.SaveAsync();

                    return SetTeamOrAdjudicatorResult.AdjudicatorUpdated;
                }

                // If it is in another organization, return error result

                if (adjudicator.OrganizationId != organizationId)
                {
                    return SetTeamOrAdjudicatorResult.AdjudicatorAlreadyRegisteredWithOtherOrganization;
                }

                // if it is not deleted and in same organization, do nothing

                return SetTeamOrAdjudicatorResult.AdjudicatorAlreadyRegistered;
            }


            // Otherwise add adjudicator

            await AssociateUser(organizationId, userId, user);

            adjudicator = new Adjudicator
            {
                OrganizationId = organizationId,
                TournamentId = tournamentId,
                UserId = userId
            };
            adjudicator.UpdateTrackingData(user);
            unitOfWork.GetRepository<Adjudicator>().Insert(adjudicator);
            await unitOfWork.SaveAsync();
            return SetTeamOrAdjudicatorResult.AdjudicatorAdded;
        }



        public SetTeamOrAdjudicatorResult RemoveAdjudicator(Guid tournamentId, string userId, User user)
        {
            var adjudicator = GetAdjudicator(tournamentId, userId);
            if (adjudicator == null)
            {
                return SetTeamOrAdjudicatorResult.AdjudicatorNotFound;
            }

            adjudicator.Deleted = true;
            adjudicator.UpdateTrackingData(user);
            unitOfWork.GetRepository<Adjudicator>().Update(adjudicator);
            unitOfWork.Save();
            return SetTeamOrAdjudicatorResult.AdjudicatorUpdated;
        }



        #endregion
        private async Task AssociateUser(Guid organizationId, string userId, User user)
        {
            var assocUser = await userManager.FindByIdAsync(userId);

            if (!assocUser.OrganizationAssociations.Any(oa => oa.OrganizationId == organizationId))
            {
                OrganizationUser orgUser = new OrganizationUser
                {
                    OrganizationId = organizationId,
                    UserId = userId,
                    Role = OrganizationRole.Member
                };
                orgUser.UpdateTrackingData(user);
                assocUser.OrganizationAssociations.Add(orgUser);
                await userManager.UpdateAsync(assocUser);
            }
        }
        public void SetBookingCode(TournamentOrganizationRegistration registration)
        {

            // if registration does not have a booking code, create one

            if (String.IsNullOrEmpty(registration.BookingCode))
            {
                // Generate tournament unique booking code
                String bookingCode;
                do
                {
                    bookingCode = BookingCodeHelper.GuidToBookingCode(Guid.NewGuid(), 6);
                } while (GetRegistration(bookingCode, registration.TournamentId) != null);
                registration.BookingCode = bookingCode;
            }
        }

        private IEnumerable<User> GetSpeakers(Guid tournamentId, Guid? excludeTeamid = null)
        {
            var teams = GetTeams(tournamentId);
            if (excludeTeamid != null)
            {
                teams = teams.Where(t => t.Id != excludeTeamid);
            }

            return from t in teams
                   from sp in t.Speaker
                   select sp;
        }

    }
}
