
using DebReg.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace DebRegComponents
{
    public interface ITournamentManager
    {
        Tournament GetTournament(Guid tournamentId);
        IEnumerable<Tournament> GetTournaments();
        Task AddTournamentAsync(Tournament tournament, User user);
        void UpdateTournament(Tournament tournament, User user);
        void DeleteTournament(Guid tournamentId, User user);
        IEnumerable<UserTournamentProperty> GetUserTournamentProperties(Guid tournamentId);
        UserTournamentProperty GetUserTournamentProperty(Guid tournamentId, Guid userPropertyId);

        Task SetUserTournamentPropertyValueAsync(String userId, Guid propertyId, Guid tournamentId, String value);

        Task SetUserTournamentPropertyValueAsync(User user, UserProperty property, Tournament tournament, String value);
        UserTournamentPropertyValue GetUserTournamentPropertyValue(String userId, Guid propertyId, Guid tournamentId);

    }
}
