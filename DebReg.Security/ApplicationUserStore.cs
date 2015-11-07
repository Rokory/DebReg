using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;

namespace DebReg.Security
{
    public class ApplicationUserStore<TUser> : UserStore<TUser> where TUser : global::Microsoft.AspNet.Identity.EntityFramework.IdentityUser
    {
        public DbContext DbContext { get; private set; }
        public ApplicationUserStore(DbContext context)
            : base(context)
        {
            DbContext = context;
        }
    }
}
