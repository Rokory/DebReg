
using DebReg.Data;
using DebReg.Models;
using DebReg.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DebRegComponents
{
    public class TournamentManager : BaseManager, ITournamentManager
    {
        public const string UserNotAuthorizedMessage = "User is not authorized to manage tournaments for this organization.";
        public const string TournamentNotFoundMessage = "A tournament with this id was not found.";

        private DebRegUserManager userManager;
        public TournamentManager(IUnitOfWork unitOfWork, DebRegUserManager userManager)
            : base(unitOfWork)
        {
            this.userManager = userManager;
        }

        #region ITournamentManager Members

        public UserTournamentProperty GetUserTournamentProperty(Guid tournamentId, Guid userPropertyId)
        {
            var repository = unitOfWork.GetRepository<UserTournamentProperty>();
            var userTournamentProperty = repository.GetById(userPropertyId, tournamentId);
            return userTournamentProperty;
        }

        public UserTournamentPropertyValue GetUserTournamentPropertyValue(String userId, Guid propertyId, Guid tournamentId)
        {
            var repository = unitOfWork.GetRepository<UserTournamentPropertyValue>();
            var userTournamentPropertyValue = repository.GetById(userId, propertyId, tournamentId);
            return userTournamentPropertyValue;
        }
        public virtual async Task SetUserTournamentPropertyValueAsync(User user, UserProperty property, Tournament tournament, String value)
        {
            var repository = unitOfWork.GetRepository<UserTournamentPropertyValue>();
            if (user != null && property != null && tournament != null)
            {
                var userTournamentPropertyValue = repository.GetById(user.Id, property.Id, tournament.Id);
                if (userTournamentPropertyValue == null)
                {
                    userTournamentPropertyValue = new UserTournamentPropertyValue
                    {
                        User = user,
                        Tournament = tournament,
                        UserProperty = property,
                        Value = value
                    };
                    repository.Insert(userTournamentPropertyValue);
                }
                else
                {
                    userTournamentPropertyValue.Value = value;
                    repository.Update(userTournamentPropertyValue);
                }
                await unitOfWork.SaveAsync();
            }

        }

        public virtual async Task SetUserTournamentPropertyValueAsync(String userId, Guid propertyId, Guid tournamentId, String value)
        {
            var user = await userManager.FindByIdAsync(userId);
            var userProperty = unitOfWork.GetRepository<UserProperty>().GetById(propertyId);
            var tournament = GetTournament(tournamentId);
            await SetUserTournamentPropertyValueAsync(user, userProperty, tournament, value);
        }

        public IEnumerable<UserTournamentProperty> GetUserTournamentProperties(Guid tournamentId)
        {
            return unitOfWork
                .GetRepository<UserTournamentProperty>()
                .Get(utp => utp.TournamentId == tournamentId)
                .OrderBy(utp => utp.Order)
                .ThenBy(utp => utp.UserProperty.Order);
        }
        public Tournament GetTournament(System.Guid tournamentId)
        {
            return unitOfWork.GetRepository<Tournament>().GetById(tournamentId);
        }

        public IEnumerable<Tournament> GetTournaments()
        {
            return unitOfWork.GetRepository<Tournament>().Get(t => !t.Deleted);
        }

        public async Task AddTournamentAsync(Tournament tournament, User user)
        {
            // Check if user is authorized
            if (!userManager.HasOrganizationRole(user.Id, tournament.HostingOrganizationID, OrganizationRole.OrganizationTournamentManager))
            {
                throw new UnauthorizedAccessException(UserNotAuthorizedMessage);
            }

            tournament.UpdateTrackingData(user);


            unitOfWork.GetRepository<Tournament>().Insert(tournament);
            unitOfWork.Save();

            // Give user all management roles

            await AssignTournamentUserRoleAsync(tournament, user, TournamentRole.OrganizationApprover);
            await AssignTournamentUserRoleAsync(tournament, user, TournamentRole.SlotManager);
            await AssignTournamentUserRoleAsync(tournament, user, TournamentRole.FinanceManager);

            // make tournament the current tournament for user

            user.CurrentTournament = tournament; user.CurrentTournamentId = tournament.Id;
            await userManager.UpdateAsync(user);

            // TODO: Redesign layout, so we don't use claims anymore and the switch goes smoothly
        }

        public void UpdateTournament(Tournament tournament, User user)
        {
            var savedTournament = GetTournament(tournament.Id);

            if (savedTournament == null)
            {
                throw new ArgumentException(TournamentNotFoundMessage, "tournament.Id");
            }

            // Check if user is authorized
            if (!userManager.HasOrganizationRole(user.Id, savedTournament.HostingOrganizationID, OrganizationRole.OrganizationTournamentManager))
            {
                throw new UnauthorizedAccessException(UserNotAuthorizedMessage);
            }

            tournament.HostingOrganization = savedTournament.HostingOrganization;

            unitOfWork.Detach(savedTournament);

            // tournament.CopyProperties(savedTournament);

            tournament.UpdateTrackingData(user);
            unitOfWork.GetRepository<Tournament>().Update(tournament);
            unitOfWork.Save();
        }

        public void DeleteTournament(Guid tournamentId, User user)
        {
            var tournament = GetTournament(tournamentId);

            if (tournament == null)
            {
                throw new ArgumentException(TournamentNotFoundMessage, "tournamentId");
            }

            // Check if user is authorized
            if (!userManager.HasOrganizationRole(user.Id, tournament.HostingOrganizationID, OrganizationRole.OrganizationTournamentManager))
            {
                throw new UnauthorizedAccessException(UserNotAuthorizedMessage);
            }

            tournament.Registrations.ForEach(
                delegate(TournamentOrganizationRegistration registration)
                {
                    registration.Deleted = true;
                    registration.UpdateTrackingData(user);
                    unitOfWork.GetRepository<TournamentOrganizationRegistration>().Update(registration);
                }
            );
            tournament.Deleted = true;
            tournament.UpdateTrackingData(user);
            unitOfWork.GetRepository<Tournament>().Update(tournament);
            unitOfWork.Save();
        }
        #endregion

        private async Task AssignTournamentUserRoleAsync(Tournament tournament, User user, TournamentRole role)
        {
            TournamentUserRole tournamentUserRole = new TournamentUserRole
            {
                Tournament = tournament,
                TournamentId = tournament.Id,
                User = user,
                UserId = user.Id,
                Role = TournamentRole.FinanceManager
            };

            var repository = unitOfWork.GetRepository<TournamentUserRole>();

            repository.Insert(tournamentUserRole);
            await unitOfWork.SaveAsync();
        }
    }
}
