using DebReg.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DebReg.Security
{
    public class DebRegUserManager : UserManager<User>
    {
        // Constants for claims

        private readonly String currentOrganizatonIdType = "CurrentOrganizationId";
        private readonly String currentTournamentIdType = "CurrentTournamentId";

        ApplicationUserStore<User> store;

        public DebRegUserManager(ApplicationUserStore<User> store)
            : base(store)
        {
            this.store = store;
            this.UserTokenProvider = new EmailTokenProvider<User>();
        }


        public static DebRegUserManager Create(
            IdentityFactoryOptions<DebRegUserManager> options, IOwinContext context)
        {
            DebRegUserManager manager = new DebRegUserManager(new ApplicationUserStore<User>(context.Get<IdentityDbContext<User>>()));

            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 8,
                RequireLowercase = true,
                RequireUppercase = true,
                RequireDigit = true
            };

            manager.UserValidator = new UserValidator<User>(manager)
            {
                RequireUniqueEmail = true,
                AllowOnlyAlphanumericUserNames = false
            };

            var dataProtectionProvider = options.DataProtectionProvider;


            //token life span is 3 hours
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider =
                   new DataProtectorTokenProvider<User>
                      (dataProtectionProvider.Create())
                   {
                       TokenLifespan = TimeSpan.FromDays(4)
                   };
            }

            ////defining email service
            //manager.EmailService = new EmailService();


            //manager.UserTokenProvider = new EmailTokenProvider<User>();

            return manager;
        }

        public virtual Boolean HasOrganizationRole(string userId, Guid organizationId, OrganizationRole role)
        {
            var user = this.FindById(userId);

            if (user != null)
            {
                return user.OrganizationAssociations.FirstOrDefault(a =>
                    a.OrganizationId == organizationId && a.Role == role) != null;
            }

            return false;
        }

        public virtual Boolean HasTournamentRole(string userId, Guid tournamentId, TournamentRole role)
        {
            var user = this.FindById(userId);

            if (user != null)
            {
                return user.TournamentRoles.FirstOrDefault(tr =>
                    tr.TournamentId == tournamentId && tr.Role == role) != null;
            }
            return false;
        }

        public virtual IEnumerable<UserProperty> GetUserProperties()
        {
            var properties = store.DbContext.Set<UserProperty>()
                .Where(p => !p.TournamentSpecific)
                .OrderBy(p => p.Order);
            return properties;
        }

        public virtual async Task<UserProperty> GetUserPropertyAsync(Guid id)
        {
            var repository = store.DbContext.Set<UserProperty>();
            var property = await repository.FindAsync(id);
            return property;
        }

        public virtual async Task SetUserPropertyValueAsync(String userId, Guid propertyId, String value)
        {
            User user = await FindByIdAsync(userId);
            UserProperty property = await GetUserPropertyAsync(propertyId);
            await SetUserPropertyValueAsync(user, property, value);
        }

        public virtual async Task SetUserPropertyValueAsync(User user, UserProperty property, String value)
        {
            var repository = store.DbContext.Set<UserPropertyValue>();
            var propertyValue = await repository.FindAsync(user.Id, property.Id);
            if (propertyValue == null)
            {
                propertyValue = new UserPropertyValue
                {
                    User = user,
                    UserProperty = property
                };
                repository.Add(propertyValue);
            }
            propertyValue.Value = value;
            await store.DbContext.SaveChangesAsync();
        }

        public virtual async Task<String> GetUserPropertyValueAsync(String userId, Guid propertyId)
        {
            var repo = store.DbContext.Set<UserPropertyValue>();
            var propertyValue = await repo.FindAsync(userId, propertyId);
            if (propertyValue != null)
            {
                return propertyValue.Value;
            }
            return null;

        }

        public IEnumerable<User> Find(String searchTerm)
        {
            Dictionary<String, SearchResult> searchResults = new Dictionary<string, SearchResult>();

            var searchTerms = searchTerm.Split(' ');


            foreach (var term in searchTerms)
            {
                // First search for searchTerms in eMail
                // Weight will be 4 for them

                var repo = store.DbContext.Set<User>();
                var users = repo.Where(u => u.Email.ToLower().Contains(term.ToLower()));
                AddToSearchResults(users, searchResults, 4);

                // Then search for searchTerms in Lastname
                // Weight will be 2 for them

                users = repo.Where(u => u.LastName.ToLower().Contains(term.ToLower()));
                AddToSearchResults(users, searchResults, 2);

                // Then search for searchTerms in Firstname
                // Weight will be 1 for them

                users = repo.Where(u => u.FirstName.ToLower().Contains(term.ToLower()));
                AddToSearchResults(users, searchResults, 1);
            }

            var sortedResults = searchResults
                .OrderByDescending(r => r.Value.Weight)
                .ThenBy(r => r.Value.User.LastName)
                .ThenBy(r => r.Value.User.FirstName);

            return from r in sortedResults
                   select r.Value.User;
        }

        private void AddToSearchResults(IEnumerable<User> users, Dictionary<String, SearchResult> searchResults, int weight)
        {
            foreach (var user in users)
            {
                SearchResult searchResult;
                if (!searchResults.TryGetValue(user.Id, out searchResult))
                {
                    searchResult = new SearchResult
                    {
                        User = user,
                        Weight = 0
                    };
                    searchResults.Add(user.Id, searchResult);
                }

                searchResult.Weight += weight;
            }
        }

        private class SearchResult
        {
            public User User { get; set; }
            public int Weight { get; set; }
        }

        private async Task ComposeRoles(User user)
        {
            if (user != null)
            {
                // Remove current roles
                user.Roles.Clear();

                // Add organization role
                if (user.CurrentOrganizationId != null && user.CurrentOrganizationId != Guid.Empty)
                {
                    var associations = user.OrganizationAssociations.Where(a =>
                        a.OrganizationId == user.CurrentOrganizationId);
                    foreach (var association in associations)
                    {
                        await AddToRoleAsync(
                            user.Id,
                            Enum.GetName(
                                typeof(OrganizationRole),
                                association.Role
                            )
                        );
                    }
                }

                // Add tournament roles
                if (user.CurrentTournamentId != null && user.CurrentTournamentId != Guid.Empty)
                {
                    var tournamentRoles = user.TournamentRoles.Where(t =>
                        t.TournamentId == user.CurrentTournamentId);
                    foreach (var tournamentRole in tournamentRoles)
                    {
                        await AddToRoleAsync(
                            user.Id,
                            Enum.GetName(
                                typeof(TournamentRole),
                                tournamentRole.Role
                            )
                        );
                    }
                }

                //await UpdateAsync(user);

                if (user.TournamentRoles.Any(r => r.Role != TournamentRole.NoTournamentRole))
                {
                    await AddToRoleAsync(
                        user.Id,
                        "TournamentManager"
                    );

                }

                //await UpdateAsync(user);

            }
        }


        public async Task<User> LoginAsync(string userName, string password, DebRegUserManager userManager)
        {
            if (userManager != null)
            {
                User user = await userManager.FindAsync(userName, password);

                // Add Roles
                if (user != null)
                {
                    await ComposeRoles(user);
                }
                return user;
            }
            else
            {
                return null;
            }
        }

        public async Task SetCurrentOrganizationAsync(User user, Guid organizationId, DebRegUserManager userManager)
        {
            user.CurrentOrganizationId = organizationId;

            await ComposeRoles(user);
        }

        public async Task SetCurrentTournamentAsync(User user, Guid tournamentId, DebRegUserManager userManager)
        {
            user.CurrentTournamentId = tournamentId;

            await ComposeRoles(user);
        }

        public override async Task<User> FindByEmailAsync(string email)
        {
            var user = await base.FindByEmailAsync(email);
            if (user == null)
            {
                var repo = store.DbContext.Set<User>();
                user = repo.FirstOrDefault(u => u.NewEMail.Trim().ToLower() == email.Trim().ToLower());
            }
            return user;
        }

        public override async Task<ClaimsIdentity> CreateIdentityAsync(User user, string authenticationType)
        {
            // Add Roles

            await ComposeRoles(user);
            //await ComposeRoles(user); // For some reason, I have to compose roles twice, so that the identity can be created correctly


            // Create identity

            ClaimsIdentity ident = await base.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
            ComposeClaims(ident, user);
            return ident;
        }

        public Guid? GetCurrentTournamentId(ClaimsIdentity ident)
        {
            return GetGuidClaim(ident, currentTournamentIdType);
        }
        public Guid? GetCurrentOrganizationId(ClaimsIdentity ident)
        {
            return GetGuidClaim(ident, currentOrganizatonIdType);
        }

        private Guid? GetGuidClaim(ClaimsIdentity ident, string type)
        {
            var claims = GetClaims(ident, type);
            Claim resultClaim = claims.FirstOrDefault();
            if (resultClaim != null)
            {
                String resultString = resultClaim.Value;
                Guid resultGuid;
                if (Guid.TryParse(resultString, out resultGuid))
                {
                    return resultGuid;
                }
            }
            return null;
        }


        private void ComposeClaims(ClaimsIdentity identity, User user)
        {
            if (identity != null)
            {

                RemoveClaims(identity, currentOrganizatonIdType);
                RemoveClaims(identity, currentTournamentIdType);

                if (user == null)
                {
                    return;
                }
                Guid currentOrganizationId = user.CurrentOrganizationId ?? Guid.Empty;
                Guid currentTournamentId = user.CurrentTournamentId ?? Guid.Empty;
                AddClaim(identity, currentOrganizatonIdType, currentOrganizationId.ToString());
                AddClaim(identity, currentTournamentIdType, currentTournamentId.ToString());

                //// Add tournament to manage

                //RemoveClaims(identity, tournamentManagerType);

                //foreach (var tournamentRole in user.TournamentRoles)
                //{
                //    AddClaim(identity, tournamentManagerType, tournamentRole.TournamentId.ToString());
                //}
            }
        }

        private IEnumerable<Claim> GetClaims(ClaimsIdentity ident, string type)
        {
            return ident.Claims.Where(c => c.Type == type);
        }


        private void AddClaim(ClaimsIdentity identity, string type, string value)
        {
            if (identity != null)
            {
                Claim claim = new Claim(type, value);
                identity.AddClaim(claim);
                // await GetUserManager(context).AddClaimAsync(identity.GetUserId(), claim);
            }
        }

        private void RemoveClaims(ClaimsIdentity identity, string type)
        {
            var claims = GetClaims(identity, type);
            foreach (var claim in claims)
            {
                identity.RemoveClaim(claim);
            }
        }


    }
}