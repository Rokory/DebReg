using Microsoft.Owin.Security.OAuth;
using System;
using System.Threading.Tasks;

namespace DebReg.Security
{
    // Inspired by: http://leastprivilege.com/2013/10/31/retrieving-bearer-tokens-from-alternative-locations-in-katanaowin/

    /// <summary>
    /// OAuthBearerProvider that accepts the authentication token in a query string parameter.
    /// </summary>
    public class QueryStringOAuthBearerProvider : OAuthBearerAuthenticationProvider
    {
        readonly string _name;  // name of query string parameter containing the token

        /// <summary>
        /// Configures the name of the query string parameter for the token.
        /// </summary>
        /// <param name="name">
        /// Name of query string parameter for the token.
        /// </param>
        public QueryStringOAuthBearerProvider(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Handles processing OAuth bearer token.
        /// First tries to find the bearer token in then query string parameter.
        /// If it is not in the query string parameter, fall back to the default header parameter.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public override async Task RequestToken(OAuthRequestTokenContext context)
        {
            // Get the token from the query string parameter

            var value = context.Request.Query.Get(_name);

            if (!String.IsNullOrEmpty(value))
            {
                context.Token = value;
            }

            // If not in query string parameter fall back to default

            if (String.IsNullOrEmpty(value))
            {
                await base.RequestToken(context);
            }
        }
    }
}
