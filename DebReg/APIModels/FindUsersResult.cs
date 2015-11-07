using System.Collections.Generic;

namespace DebReg.Web.APIModels
{
    public class FindUsersResult
    {
        /// <summary>
        /// Contains the actual results returned to the client.
        /// </summary>
        public IEnumerable<User> results { get; set; }

        /// <summary>
        /// Contains the number of results found.
        /// If greater than number of results, results are hidden because of privacy.
        /// </summary>
        public int totalResults { get; set; }
    }
}