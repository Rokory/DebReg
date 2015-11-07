using DebReg.Models;
using System;
using System.Threading.Tasks;

namespace DebReg.Security
{
    public interface ISecurityManager
    {
        Task<Boolean> LoginAsync(String userName, String password);
        Task LoginAsync(User user);
        //Guid? GetCurrentTournamentId(ClaimsIdentity ident);
        //Guid? GetCurrentOrganizationId(ClaimsIdentity ident);
        void Logout();
    }
}
