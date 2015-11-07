using DebReg.Security;
using DebReg.Web.APIModels;
using DebReg.Web.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace DebReg.Web.APIControllers
{
    [Authorize]
    public class UserController : ApiController
    {
        private DebRegUserManager userManager;

        [HttpGet]
        public async Task<FindUsersResult> FindUsers(String searchTerm)
        {
            FindUsersResult result = new FindUsersResult();

            // Search for users and store results and results count

            result.results = from user in userManager.Find(searchTerm)
                             select new User(user);
            result.totalResults = result.results.Count();

            // Now cut the results to the first 10

            result.results = result.results.Take(10);

            return result;

        }

        [HttpGet]
        public async Task<User> GetUser(String id)
        {
            var user = await userManager.FindByIdAsync(id);
            return new User(user);
        }

        public IEnumerable<User> GetAllUsers()
        {
            return new List<User>();
        }


        public UserController(DebRegUserManager userManager)
        {
            this.userManager = userManager;
        }
    }
}
