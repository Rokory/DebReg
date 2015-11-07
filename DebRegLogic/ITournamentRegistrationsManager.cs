using DebReg.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DebRegComponents
{
    public interface ITournamentRegistrationsManager
    {
        TournamentOrganizationRegistration AddRegistration(Guid tournamentId, Guid organizationId, Guid BilledOrganizationId, int TeamsWanted, int AdjudicatorsWanted, String notes, User user);
        IEnumerable<TournamentOrganizationRegistration> GetApprovedRegistrations(Guid tournamentId);
        IEnumerable<TournamentOrganizationRegistration> GetRegistrationsSortedByRank(Guid tournamentId, User user);
        TournamentOrganizationRegistration GetRegistration(Guid tournamentId, Guid organizationId);
        TournamentOrganizationRegistration GetRegistration(String bookingCode, Guid tournamentId);
        TournamentOrganizationRegistration FindRegistrationByOrganizationName(Guid tournamentId, String searchTerm);
        TournamentOrganizationRegistration FindRegistrationByOrganization(Guid tournamentId, Organization organization);
        IEnumerable<TournamentOrganizationRegistration> GetRegistrationsByOrganizationId(Guid organizationId);
        IEnumerable<TournamentOrganizationRegistration> GetRegistrationsByTournamentId(Guid tournamentId);
        void SetRank(Guid tournamentId, Guid organizationId, Decimal rank, User user);
        void SetTeamsAndAdjudicatorsGranted(Guid tournamentId, Guid organizationId, int teamsGranted, int adjudicatorsGranted, User user);

        void SetOrganizationStatusAndNote
        (
            Guid tournamentId,
            Guid organizationId,
            OrganizationStatus organizationStatus,
            String organizationStatusNote,
            Boolean draft,
            User user
        );

        void PublishOrganizationStatusAndNote(Guid tournamentId, Guid organizationId, User user);

        TournamentOrganizationRegistration SetTeamsAndAdjudicatorsPaid(Guid tournamentId, Guid organizationId, int teamsPaid, int adjudicatorsPaid, User user);

        void SetRandomRank(Guid tournamentId, Guid organizationId, int randomRank, User user);
        void SetLockAutoAssign(Guid tournamentId, Guid organizationId, Boolean lockAutoAssign, User user);
        IEnumerable<Team> GetTeams(Guid tournamentId, Guid organizationId);
        IEnumerable<Team> GetTeams(Guid tournamentId);
        IEnumerable<Team> GetTeams(String userId);
        Team GetTeam(Guid teamId);
        SetTeamOrAdjudicatorResult SetTeam(Team team, User user);
        Team DeleteTeam(Guid teamId, User user);

        IEnumerable<Adjudicator> GetAdjudicators(Guid tournamentId, Guid organizationId);
        IEnumerable<Adjudicator> GetAdjudicators(Guid tournamentId);
        Adjudicator GetAdjudicator(Guid tournamentId, String userId);
        IEnumerable<Adjudicator> GetAdjudicators(String userId);
        Task<SetTeamOrAdjudicatorResult> AddAdjudicatorAsync(Guid tournamentId, Guid organizationId, String userId, User user);
        SetTeamOrAdjudicatorResult RemoveAdjudicator(Guid tournamentId, String userId, User user);

        Task<SetTeamOrAdjudicatorResult> AddSpeakerAsync(Guid teamId, String speakerId, User user);
        SetTeamOrAdjudicatorResult RemoveSpeaker(Guid teamId, String speakerId, User user);
        String GenerateTeamName(Guid organizationId, Guid tournamentId);
        String GenerateTeamName(Guid organizationId, Guid tournamentId, IEnumerable<Team> teams);
        String GenerateAutosuffix(Guid organizationId, Guid tournamentId);
        String GenerateAutosuffix(Guid organizationId, Guid tournamentId, IEnumerable<Team> teams);


    }

    public enum SetTeamOrAdjudicatorResult
    {
        TeamAdded = 201,
        AdjudicatorAdded = 202,
        TeamUpdated = 205,
        AdjudicatorUpdated = 206,
        NotAuthorized = 401,
        RegistrationNotFound = 440,
        TeamNotFound = 441,
        UserNotFound = 442,
        AdjudicatorNotFound = 443,
        TooManyTeams = 450,
        TooManySpeakers = 451,
        TooManyAdjudicators = 452,
        SpeakerAlreadyInOtherTeam = 460,
        SpeakerAlreadyInTeam = 461,
        SpeakerAlreadyAdjudicator = 462,
        AdjudicatorAlreadyRegisteredWithOtherOrganization = 463,
        AdjudicatorAlreadyRegistered = 464,
        AdjudicatorAlreadySpeaker = 465
    }



}
