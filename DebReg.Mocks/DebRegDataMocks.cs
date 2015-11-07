using DebReg.Data;
using DebReg.Models;
using DebReg.Security;
using Microsoft.AspNet.Identity;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DebReg.Mocks
{
    public class DebRegDataMocks
    {

        #region UnitOfWork Mock

        // UnitOfWork Mock

        #region Repositories

        private List<Tournament> tournaments = new List<Tournament>();
        private List<Tournament> newTournaments = new List<Tournament>();
        private List<Tournament> updatedTournaments = new List<Tournament>();

        private List<TournamentOrganizationRegistration> tournamentOrganizationRegistrations = new List<TournamentOrganizationRegistration>();
        private List<TournamentOrganizationRegistration> newTournamentOrganizationRegistrations = new List<TournamentOrganizationRegistration>();
        private List<TournamentOrganizationRegistration> updatedTournamentOrganizationRegistrations = new List<TournamentOrganizationRegistration>();

        private List<Organization> organizations = new List<Organization>();
        private List<Organization> newOrganizations = new List<Organization>();
        private List<Organization> updatedOrganizations = new List<Organization>();

        private List<BookingRecord> bookingRecords = new List<BookingRecord>();
        private List<BookingRecord> newBookingRecords = new List<BookingRecord>();
        private List<BookingRecord> updatedBookingRecords = new List<BookingRecord>();

        private List<SlotAssignment> slotAssignments = new List<SlotAssignment>();
        private List<SlotAssignment> newSlotAssignments = new List<SlotAssignment>();
        private List<SlotAssignment> updatedSlotAssignments = new List<SlotAssignment>();

        private List<User> users = new List<User>();
        private List<User> newUsers = new List<User>();
        private List<User> updatedUsers = new List<User>();

        private List<OrganizationUser> organizationUsers = new List<OrganizationUser>();
        private List<OrganizationUser> newOrganizationUsers = new List<OrganizationUser>();
        private List<OrganizationUser> updatedOrganizationUsers = new List<OrganizationUser>();

        private List<TournamentUserRole> tournamentUserRoles = new List<TournamentUserRole>();
        private List<TournamentUserRole> newTournamentUserRoles = new List<TournamentUserRole>();
        private List<TournamentUserRole> updatedTournamentUserRoles = new List<TournamentUserRole>();

        private List<Team> teams = new List<Team>();
        private List<Team> newTeams = new List<Team>();
        private List<Team> updatedTeams = new List<Team>();

        private List<DebReg.Models.Version> versions = new List<DebReg.Models.Version>();
        private List<DebReg.Models.Version> newVersions = new List<DebReg.Models.Version>();
        private List<DebReg.Models.Version> updatedVersions = new List<DebReg.Models.Version>();

        private List<Adjudicator> adjudicators = new List<Adjudicator>();
        private List<Adjudicator> newAdjudicators = new List<Adjudicator>();
        private List<Adjudicator> updatedAdjudicators = new List<Adjudicator>();

        #endregion

        private IUnitOfWork _unitOfWork;



        public IUnitOfWork UnitOfWork
        {
            get
            {
                if (_unitOfWork == null)
                {
                    var mockUnitOfWork = new Mock<IUnitOfWork>();


                    #region GetRepository<TournamentOrganizationRegistration> Mock
                    mockUnitOfWork.Setup(m => m.GetRepository<TournamentOrganizationRegistration>())
                        .Returns(() =>
                        {

                            var mockTournamentOrganizationRegistrationRepository = new Mock<IRepository<TournamentOrganizationRegistration>>();

                            #region GetById Mock
                            mockTournamentOrganizationRegistrationRepository.Setup(m =>
                                m.GetById(It.IsAny<Guid>(), It.IsAny<Guid>())
                            ).Returns<Object[]>(p =>
                                tournamentOrganizationRegistrations.FirstOrDefault(r =>
                                    r.TournamentId == ((Guid)p[0]) && r.OrganizationId == ((Guid)p[1])
                                )
                            );
                            #endregion

                            SetupGetMock(mockTournamentOrganizationRegistrationRepository, tournamentOrganizationRegistrations);
                            SetupInsertMock(mockTournamentOrganizationRegistrationRepository, newTournamentOrganizationRegistrations);
                            SetupUpdateMock(mockTournamentOrganizationRegistrationRepository, updatedTournamentOrganizationRegistrations);

                            return mockTournamentOrganizationRegistrationRepository.Object;

                        }
                    );
                    #endregion

                    #region GetRepository<Tournament> Mock
                    mockUnitOfWork.Setup(m => m.GetRepository<Tournament>())
                        .Returns(() =>
                        {

                            var mockTournamentRepository = new Mock<IRepository<Tournament>>();

                            #region GetById Mock
                            mockTournamentRepository.Setup(m =>
                                m.GetById(It.IsAny<Guid>())
                            ).Returns<Object[]>(p =>
                            {
                                var tournament = tournaments.FirstOrDefault(t =>
                                    t.Id == ((Guid)p.First()));
                                if (tournament != null)
                                {
                                    tournament.Registrations = tournamentOrganizationRegistrations.Where(
                                        r => r.TournamentId == tournament.Id)
                                        .ToList();
                                }
                                return tournament;
                            }
                            );
                            #endregion

                            mockTournamentRepository.Setup(m => m.Get(
                                            It.IsAny<Expression<Func<Tournament, bool>>>(),
                                            It.Is<Func<IQueryable<Tournament>, IOrderedQueryable<Tournament>>>(v => v == null),
                                            It.Is<String>(v => v == "")))
                                            .Returns<
                                                Expression<Func<Tournament, bool>>,
                                                Func<IQueryable<Tournament>, IOrderedQueryable<Tournament>>,
                                                String
                                            >((filter, sort, properties) =>
                                            {
                                                var result = tournaments.Where(filter.Compile()).ToList();
                                                FillTournamentRegistrations(result);
                                                return result;
                                            }
                                        );

                            mockTournamentRepository.Setup(m => m.Get(
                                It.Is<Expression<Func<Tournament, bool>>>(v => v == null),
                                It.Is<Func<IQueryable<Tournament>, IOrderedQueryable<Tournament>>>(v => v == null),
                                It.Is<String>(v => v == "")))
                                .Returns<
                                    Expression<Func<Tournament, bool>>,
                                    Func<IQueryable<Tournament>, IOrderedQueryable<Tournament>>,
                                    String
                                >((filter, sort, properties) =>
                                {
                                    var result = tournaments;
                                    FillTournamentRegistrations(result);
                                    return result;
                                }
                            );

                            SetupInsertMock(mockTournamentRepository, newTournaments);
                            SetupUpdateMock(mockTournamentRepository, updatedTournaments);

                            return mockTournamentRepository.Object;

                        }
                    );
                    #endregion

                    #region GetRepository<Organization> Mock
                    mockUnitOfWork.Setup(m => m.GetRepository<Organization>())
                        .Returns(() =>
                        {

                            var mockOrganizationRepository = new Mock<IRepository<Organization>>();

                            #region GetById Mock
                            mockOrganizationRepository.Setup(m =>
                                m.GetById(It.IsAny<Guid>())
                            ).Returns<Object[]>(p =>
                                organizations.FirstOrDefault(t =>
                                    t.Id == ((Guid)p.First()))
                            );
                            #endregion

                            SetupGetMock(mockOrganizationRepository, organizations);
                            mockOrganizationRepository.Setup(m =>
                                m.Insert(It.IsAny<Organization>()))
                                .Callback((Organization o) =>
                                {
                                    newOrganizations.Add(o);
                                    foreach (var linkedOrganization in o.LinkedOrganizations)
                                    {
                                        if (!organizations.Any(lo => lo.Id == linkedOrganization.Id)
                                            && !newOrganizations.Any(lo => lo.Id == linkedOrganization.Id))
                                        {
                                            linkedOrganization.LinkedOrganization = o;
                                            newOrganizations.Add(linkedOrganization);
                                        }
                                    }
                                })
                                .Verifiable();
                            SetupUpdateMock(mockOrganizationRepository, updatedOrganizations);

                            return mockOrganizationRepository.Object;

                        }
                    );
                    #endregion

                    #region GetRepository<BookingRecord> Mock
                    mockUnitOfWork.Setup(m => m.GetRepository<BookingRecord>())
                        .Returns(() =>
                        {

                            var mockRepository = new Mock<IRepository<BookingRecord>>();

                            #region GetById Mock
                            mockRepository.Setup(m =>
                                m.GetById(It.IsAny<Guid>())
                            ).Returns<Object[]>(p =>
                                bookingRecords.FirstOrDefault(t =>
                                    t.Id == ((Guid)p.First()))
                            );
                            #endregion

                            SetupGetMock(mockRepository, bookingRecords);
                            SetupInsertMock(mockRepository, newBookingRecords);
                            SetupUpdateMock(mockRepository, updatedBookingRecords);

                            return mockRepository.Object;

                        }
                    );
                    #endregion

                    #region GetRepository<SlotAssignment> Mock

                    // GetRepository<SlotAssignment>

                    mockUnitOfWork.Setup(m => m.GetRepository<SlotAssignment>())
                        .Returns(() =>
                        {

                            var mockRepository = new Mock<IRepository<SlotAssignment>>();

                            #region GetById Mock
                            mockRepository.Setup(m =>
                                m.GetById(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))

                                .Returns<Object[]>((p) =>
                                slotAssignments.FirstOrDefault(a =>
                                    a.TournamentId == (Guid)p[0]
                                    && a.OrganizationId == (Guid)p[1]
                                    && a.VersionId == (Guid)p[2])
                            );
                            #endregion

                            SetupGetMock(mockRepository, slotAssignments);
                            SetupInsertMock(mockRepository, newSlotAssignments);
                            SetupUpdateMock(mockRepository, updatedSlotAssignments);

                            return mockRepository.Object;

                        }
                    );
                    #endregion

                    #region GetRepository<User> Mock

                    // GetRepository<User>

                    if (users == null)
                    {
                        users = new List<User>();
                        FillUsers();
                    }

                    mockUnitOfWork.Setup(m => m.GetRepository<User>())
                        .Returns(() =>
                        {

                            var mockRepository = new Mock<IRepository<User>>();

                            #region GetById Mock
                            mockRepository.Setup(m =>
                                m.GetById(It.IsAny<String>()))

                                .Returns<String>((id) =>
                                {
                                    return users.FirstOrDefault(u =>
                                        u.Id == id);
                                });
                            #endregion

                            SetupGetMock(mockRepository, users);
                            SetupInsertMock(mockRepository, newUsers);
                            SetupUpdateMock(mockRepository, updatedUsers);

                            return mockRepository.Object;

                        }
                    );
                    #endregion

                    #region GetRepository<OrganizationUser> Mock

                    // GetRepository<OrganizationUser>

                    mockUnitOfWork.Setup(m => m.GetRepository<OrganizationUser>())
                        .Returns(() =>
                        {

                            var mockRepository = new Mock<IRepository<OrganizationUser>>();

                            #region GetById Mock
                            mockRepository.Setup(m =>
                                m.GetById(It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<OrganizationRole>()))

                                .Returns<Object[]>((p) =>
                                organizationUsers.FirstOrDefault(ou =>
                                    ou.OrganizationId == (Guid)p[0]
                                    && ou.UserId == (String)p[1]
                                    && ou.Role == (OrganizationRole)p[2])
                            );
                            #endregion

                            SetupGetMock(mockRepository, organizationUsers);
                            SetupInsertMock(mockRepository, newOrganizationUsers);
                            SetupUpdateMock(mockRepository, updatedOrganizationUsers);

                            return mockRepository.Object;

                        }
                    );
                    #endregion

                    #region GetRepository<TournamentUserRole> Mock

                    // GetRepository<TournamentUserRole>

                    mockUnitOfWork.Setup(m => m.GetRepository<TournamentUserRole>())
                        .Returns(() =>
                        {

                            var mockRepository = new Mock<IRepository<TournamentUserRole>>();

                            #region GetById Mock
                            mockRepository.Setup(m =>
                                m.GetById(It.IsAny<Guid>(), It.IsAny<String>(), It.IsAny<OrganizationRole>()))

                                .Returns<Object[]>((p) =>
                                tournamentUserRoles.FirstOrDefault(tr =>
                                    tr.TournamentId == (Guid)p[0]
                                    && tr.UserId == (String)p[1]
                                    && tr.Role == (TournamentRole)p[2])
                            );
                            #endregion

                            SetupGetMock(mockRepository, tournamentUserRoles);
                            SetupInsertMock(mockRepository, newTournamentUserRoles);
                            SetupUpdateMock(mockRepository, updatedTournamentUserRoles);

                            return mockRepository.Object;

                        }
                    );
                    #endregion

                    #region GetRepository<Team> Mock
                    mockUnitOfWork.Setup(m => m.GetRepository<Team>())
                        .Returns(() =>
                        {

                            var mockRepository = new Mock<IRepository<Team>>();

                            #region GetById Mock
                            mockRepository.Setup(m =>
                                m.GetById(It.IsAny<Guid>())
                            ).Returns<Object[]>(p =>
                                teams.FirstOrDefault(t =>
                                    t.Id == ((Guid)p.First()))
                            );
                            #endregion

                            SetupGetMock(mockRepository, teams);
                            SetupInsertMock(mockRepository, newTeams);
                            SetupUpdateMock(mockRepository, updatedTeams);

                            return mockRepository.Object;

                        }
                    );
                    #endregion

                    #region GetRepository<Adjudicator> Mock
                    mockUnitOfWork.Setup(m => m.GetRepository<Adjudicator>())
                        .Returns(() =>
                        {

                            var mockRepository = new Mock<IRepository<Adjudicator>>();

                            #region GetById Mock
                            mockRepository.Setup
                            (
                                m =>
                                    m.GetById(It.IsAny<Guid>())
                            )
                            .Returns<Object[]>
                            (
                                p =>
                                    adjudicators.FirstOrDefault
                                    (
                                        a =>
                                            a.TournamentId == ((Guid)p[0])
                                            && a.UserId == ((String)p[1])
                                    )
                            );
                            #endregion

                            SetupGetMock(mockRepository, adjudicators);
                            SetupInsertMock(mockRepository, newAdjudicators);
                            SetupUpdateMock(mockRepository, updatedAdjudicators);

                            return mockRepository.Object;

                        }
                    );
                    #endregion


                    #region GetRepository<DebReg.Models.Version> Mock
                    mockUnitOfWork.Setup(m => m.GetRepository<DebReg.Models.Version>())
                        .Returns(() =>
                        {

                            var mockRepository = new Mock<IRepository<DebReg.Models.Version>>();

                            #region GetById Mock
                            mockRepository.Setup(m =>
                                m.GetById(It.IsAny<Guid>())
                            ).Returns<Object[]>(p =>
                                versions.FirstOrDefault(t =>
                                    t.Id == ((Guid)p.First()))
                            );
                            #endregion

                            SetupGetMock(mockRepository, versions);
                            SetupInsertMock(mockRepository, newVersions);
                            SetupUpdateMock(mockRepository, updatedVersions);

                            return mockRepository.Object;

                        }
                    );
                    #endregion


                    #region Save Mock

                    mockUnitOfWork.Setup(m => m.Save())
                        .Callback(() =>
                        {
                            SaveRepositories();

                        });

                    mockUnitOfWork.Setup(m => m.SaveAsync())
                        .Returns(() => SaveRepositoriesAsync());

                    #endregion

                    #region Detach Mock
                    mockUnitOfWork.Setup(m =>
                        m.Detach(It.IsAny<Object>())).Callback(() => { }
                    );
                    #endregion

                    _unitOfWork = mockUnitOfWork.Object;

                }
                return _unitOfWork;
            }
        }

        private async Task SaveRepositoriesAsync()
        {
            SaveRepositories();
        }

        private void SaveRepositories()
        {
            SaveRepository(
                tournaments,
                newTournaments,
                updatedTournaments,
                (t1, t2) => t1.Id == t2.Id);
            SaveRepository(
                tournamentOrganizationRegistrations,
                newTournamentOrganizationRegistrations,
                updatedTournamentOrganizationRegistrations,
                (r1, r2) => r1.TournamentId == r2.TournamentId && r1.OrganizationId == r2.OrganizationId);
            SaveRepository(
                organizations,
                newOrganizations,
                updatedOrganizations,
                (o1, o2) => o1.Id == o2.Id);
            SaveRepository(
                bookingRecords,
                newBookingRecords,
                updatedBookingRecords,
                (r1, r2) => r1.Id == r2.Id);
            SaveRepository(
                slotAssignments,
                newSlotAssignments,
                updatedSlotAssignments,
                (a1, a2) =>
                    a1.TournamentId == a2.TournamentId
                    && a1.OrganizationId == a2.OrganizationId
                    && a1.VersionId == a2.VersionId
            );
            SaveRepository(
                users,
                newUsers,
                updatedUsers,
                (a1, a2) => a1.Id == a2.Id);
            SaveRepository(
                organizationUsers,
                newOrganizationUsers,
                updatedOrganizationUsers,
                (a1, a2) =>
                    a1.OrganizationId == a2.OrganizationId
                    && a1.UserId == a2.UserId
                    && a1.Role == a2.Role
            );
            SaveRepository(
                tournamentUserRoles,
                newTournamentUserRoles,
                updatedTournamentUserRoles,
                (t1, t2) =>
                    t1.TournamentId == t2.TournamentId
                    && t1.UserId == t2.UserId
                    && t1.Role == t2.Role
            );
            SaveRepository(
                teams,
                newTeams,
                updatedTeams,
                (t1, t2) => t1.Id == t2.Id
            );
            SaveRepository(
                versions,
                newVersions,
                updatedVersions,
                (t1, t2) => t1.Id == t2.Id
            );
            SaveRepository(
                adjudicators,
                newAdjudicators,
                updatedAdjudicators,
                (t1, t2) =>
                    t1.TournamentId == t2.TournamentId && t1.UserId == t2.UserId);
        }


        private void FillUsers()
        {
            User[] users = new User[] { 
                new User {
                    FirstName = "Max",
                    LastName = "Mustermann",
                    Email = "max@mustermann.at"
                },
                new User {
                    FirstName = "Susi",
                    LastName = "Sorglos",
                    Email = "Susi@Sorglos.at"
                },
                new User {
                    FirstName = "Hubsi",
                    LastName = "Meier",
                    Email = "hubsi@Maier.at"
                },

                new User {
                    FirstName = "Hans",
                    LastName = "Moser",
                    Email = "hans@moser.at"
                }
            };

            foreach (var user in users)
            {
                this.users.Add(user);
            }
        }

        private void FillTournamentRegistrations(List<Tournament> tournaments)
        {
            foreach (var tournament in tournaments)
            {
                tournament.Registrations = tournamentOrganizationRegistrations.Where(
                    r => r.TournamentId == tournament.Id).ToList();
            }
        }

        private void SaveRepository<T>(List<T> repository, List<T> newItems, List<T> updatedItems, Func<T, T, bool> matchFunction)
        {
            for (int i = newItems.Count - 1; i >= 0; i--)
            {
                repository.Add(newItems[i]);
                newItems.RemoveAt(i);
            }
            for (int i = updatedItems.Count - 1; i >= 0; i--)
            {
                var oldItem = repository.FirstOrDefault(item =>
                    matchFunction(item, updatedItems[i]));
                if (oldItem != null)
                {
                    var index = repository.IndexOf(oldItem);
                    repository.RemoveAt(index);
                    repository.Insert(index, updatedItems[i]);
                    updatedItems.RemoveAt(i);
                }
            }
        }

        private void SetupUpdateMock<T>(Mock<IRepository<T>> mockRepository, List<T> updatedItems)
        {
            mockRepository.Setup(m =>
                m.Update(It.IsAny<T>()))
                .Callback((T i) => updatedItems.Add(i))
                .Verifiable();
        }
        private void SetupInsertMock<T>(Mock<IRepository<T>> mockRepository, List<T> newItems)
        {
            mockRepository.Setup(m =>
                m.Insert(It.IsAny<T>()))
                .Callback((T i) => newItems.Add(i))
                .Verifiable();
        }

        private void SetupGetMock<T>(Mock<IRepository<T>> mockRepository, List<T> repository)
        {
            mockRepository.Setup(m => m.Get(
                It.IsAny<Expression<Func<T, bool>>>(),
                It.Is<Func<IQueryable<T>, IOrderedQueryable<T>>>(v => v == null),
                It.Is<String>(v => v == "")))
                .Returns<
                    Expression<Func<T, bool>>,
                    Func<IQueryable<T>, IOrderedQueryable<T>>,
                    String
                >((filter, sort, properties) =>
                {

                    var predicate = filter.Compile();

                    var result = repository.Where(predicate).ToList();
                    return result;
                }
            );
            mockRepository.Setup(m => m.Get(
                It.Is<Expression<Func<T, bool>>>(v => v == null),
                It.Is<Func<IQueryable<T>, IOrderedQueryable<T>>>(v => v == null),
                It.Is<String>(v => v == "")))
                .Returns<
                    Expression<Func<T, bool>>,
                    Func<IQueryable<T>, IOrderedQueryable<T>>,
                    String
                >((filter, sort, properties) => repository
            );
        }


        #endregion

        #region DebRegUserManager Mock

        // DebRegUserManager Mock

        private DebRegUserManager _userManager;

        public DebRegUserManager UserManager
        {
            get
            {
                if (_userManager == null)
                {
                    var mockUserManager = new Mock<DebRegUserManager>(UserStore);

                    #region FindByIdAsync

                    // FindByIdAsync

                    mockUserManager.Setup(m =>
                        m.FindByIdAsync(It.IsAny<String>()))
                        .Returns<String>(id =>
                            Task.FromResult(
                                users.FirstOrDefault(u => u.Id == id)
                            )
                        );

                    #endregion

                    #region CreateAsync

                    // CreateAsync

                    mockUserManager.Setup(m =>
                        m.CreateAsync(It.IsAny<User>()))
                        .Returns<User>(user =>
                            CreateAsync(user)
                        );


                    #endregion

                    #region HasOrganizationRole

                    // HasOrganizationRole

                    mockUserManager.Setup(m =>
                        m.HasOrganizationRole(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<OrganizationRole>()))
                        .Returns<String, Guid, OrganizationRole>((userId, organizationId, role) =>
                        {
                            var user = users.FirstOrDefault(u => u.Id == userId);

                            if (user != null)
                            {
                                var association = user.OrganizationAssociations.FirstOrDefault(oa =>
                                oa.OrganizationId == organizationId && oa.Role == role);
                                if (association != null)
                                {
                                    return true;
                                }
                            }


                            return organizationUsers.FirstOrDefault(ou =>
                                ou.UserId == userId
                                && ou.OrganizationId == organizationId
                                && ou.Role == role) != null;
                        });
                    #endregion

                    #region HasOrganizationRole

                    // HasTournamentRole

                    mockUserManager.Setup(m =>
                        m.HasTournamentRole(It.IsAny<String>(), It.IsAny<Guid>(), It.IsAny<TournamentRole>()))
                        .Returns<String, Guid, TournamentRole>((userId, tournamentId, role) =>
                        {
                            var user = users.FirstOrDefault(u => u.Id == userId);

                            if (user != null)
                            {
                                var tournamentRole = user.TournamentRoles.FirstOrDefault(tr =>
                                tr.TournamentId == tournamentId && tr.Role == role);
                                if (tournamentRole != null)
                                {
                                    return true;
                                }
                            }


                            return tournamentUserRoles.FirstOrDefault(tr =>
                                tr.UserId == userId
                                && tr.TournamentId == tournamentId
                                && tr.Role == role) != null;
                        });
                    #endregion


                    _userManager = mockUserManager.Object;
                }
                return _userManager;
            }
        }

        //private async Task<User> FindByIdAsync(String id) {
        //    var result = users.FirstOrDefault(u => u.Id == id);
        //    return result;

        //}

        private async Task<IdentityResult> CreateAsync(User user)
        {
            users.Add(user);
            return new IdentityResult();
        }

        #endregion

        #region IUserStore Mock

        // IUserStore Mock

        private IUserStore<User> _userStore;

        public IUserStore<User> UserStore
        {
            get
            {
                if (_userStore == null)
                {
                    var mockUserStore = new Mock<IUserStore<User>>();
                    _userStore = mockUserStore.Object;
                }
                return _userStore;
            }
        }

        #endregion
    }
}
