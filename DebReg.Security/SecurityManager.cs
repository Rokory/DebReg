using DebReg.Models;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DebReg.Security
{
    public class SecurityManager : ISecurityManager
    {


        //// Constants for claims

        //private readonly String currentOrganizatonIdType = "CurrentOrganizationId";
        //private readonly String currentTournamentIdType = "CurrentTournamentId";


        private DebRegUserManager userManager;
        private IAuthenticationManager authManager;



        #region ISecurityManager Members

        public async Task<Boolean> LoginAsync(string userName, string password)
        {
            User user = await userManager.FindAsync(userName, password);

            if (user == null)
            {
                return false;
            }

            await LoginAsync(user);
            return true;
        }

        public async Task LoginAsync(User user)
        {
            ClaimsIdentity ident = await userManager.CreateIdentityAsync(user, CookieAuthenticationDefaults.AuthenticationType);


            // Sign in

            authManager.SignOut();
            authManager.SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
        }


        public void Logout()
        {
            authManager.SignOut();
        }


        public SecurityManager(DebRegUserManager userManager, IAuthenticationManager authManager)
        {
            this.userManager = userManager;
            this.authManager = authManager;

        }
        #endregion



        //private void ComposeClaims(ClaimsIdentity identity, User user)
        //{
        //    if (identity != null)
        //    {

        //        RemoveClaims(identity, currentOrganizatonIdType);
        //        RemoveClaims(identity, currentTournamentIdType);

        //        if (user == null)
        //        {
        //            return;
        //        }
        //        Guid currentOrganizationId = user.CurrentOrganizationId ?? Guid.Empty;
        //        Guid currentTournamentId = user.CurrentTournamentId ?? Guid.Empty;
        //        AddClaim(identity, currentOrganizatonIdType, currentOrganizationId.ToString());
        //        AddClaim(identity, currentTournamentIdType, currentTournamentId.ToString());

        //        //// Add tournament to manage

        //        //RemoveClaims(identity, tournamentManagerType);

        //        //foreach (var tournamentRole in user.TournamentRoles)
        //        //{
        //        //    AddClaim(identity, tournamentManagerType, tournamentRole.TournamentId.ToString());
        //        //}
        //    }
        //}

        //public Guid? GetCurrentTournamentId(ClaimsIdentity ident)
        //{
        //    return GetGuidClaim(ident, currentTournamentIdType);
        //}
        //public Guid? GetCurrentOrganizationId(ClaimsIdentity ident)
        //{
        //    return GetGuidClaim(ident, currentOrganizatonIdType);
        //}

        //private Guid? GetGuidClaim(ClaimsIdentity ident, string type)
        //{
        //    var claims = GetClaims(ident, type);
        //    Claim resultClaim = claims.FirstOrDefault();
        //    if (resultClaim != null)
        //    {
        //        String resultString = resultClaim.Value;
        //        Guid resultGuid;
        //        if (Guid.TryParse(resultString, out resultGuid))
        //        {
        //            return resultGuid;
        //        }
        //    }
        //    return null;
        //}

        //private IEnumerable<Claim> GetClaims(ClaimsIdentity ident, string type)
        //{
        //    return ident.Claims.Where(c => c.Type == type);
        //}

        //private void addclaim(claimsidentity identity, string type, string value)
        //{
        //    if (identity != null)
        //    {
        //        claim claim = new claim(type, value);
        //        identity.addclaim(claim);
        //        // await getusermanager(context).addclaimasync(identity.getuserid(), claim);
        //    }
        //}

        //private void removeclaims(claimsidentity identity, string type)
        //{
        //    var claims = getclaims(identity, type);
        //    foreach (var claim in claims)
        //    {
        //        identity.removeclaim(claim);
        //    }
        //}

    }
}
