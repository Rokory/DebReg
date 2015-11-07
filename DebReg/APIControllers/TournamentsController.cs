using DebReg.Security;
using DebReg.Web.APIModels;
using DebReg.Web.Filters;
using DebRegComponents;
using Microsoft.OData.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.OData;
using System.Web.OData.Query;

namespace DebReg.Web.APIControllers
{
    /*
    The WebApiConfig class may require additional changes to add a route for this controller. Merge these statements into the Register method of the WebApiConfig class as applicable. Note that OData URLs are case sensitive.

    using System.Web.Http.OData.Builder;
    using System.Web.Http.OData.Extensions;
    using DebReg.Web.APIModels;
    ODataConventionModelBuilder builder = new ODataConventionModelBuilder();
    builder.EntitySet<Tournament>("Tournaments");
    config.Routes.MapODataServiceRoute("odata", "odata", builder.GetEdmModel());
    */
    [Authorize]
    [IdentityBasicAuthentication]
    public class TournamentsController : ODataController
    {
        private static ODataValidationSettings _validationSettings = new ODataValidationSettings();
        private ITournamentManager tournamentManager;
        private DebRegUserManager userManager;
        private ITournamentRegistrationsManager tournamentRegistrationsManager;

        public TournamentsController(ITournamentManager tournamentManager, ITournamentRegistrationsManager tournamentRegistrationsManager, DebRegUserManager userManager)
        {
            this.tournamentManager = tournamentManager;
            this.userManager = userManager;
            this.tournamentRegistrationsManager = tournamentRegistrationsManager;
        }

        // GET: odata/Tournaments
        [Queryable]
        public async Task<IHttpActionResult> GetTournaments(ODataQueryOptions<Tournament> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            DebReg.Models.User user = await GetUser();

            if (user == null)
            {
                return Unauthorized();
            }

            var tournaments = tournamentManager.GetTournaments().ToList();
            var result = tournaments.Select(t => new Tournament(t, user)).AsQueryable();
            return Ok(result);
        }

        private async Task<DebReg.Models.User> GetUser()
        {
            DebReg.Models.User user = null;

            if (RequestContext != null
                && RequestContext.Principal != null)
            {
                var userName = RequestContext.Principal.Identity.Name;
                user = await userManager.FindByNameAsync(userName);
            }
            return user;
        }

        // GET: odata/Tournaments(5)
        [Queryable]
        [Authorize(Roles = "ReportViewer,OrganizationApprover,SlotManager,FinanceManager")]
        public async Task<IHttpActionResult> GetTournament([FromODataUri] System.Guid key, ODataQueryOptions<Tournament> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            DebReg.Models.User user = await GetUser();

            if (user == null)
            {
                return Unauthorized();
            }

            var tournament = tournamentManager.GetTournament(key);
            var result = new List<Tournament>();
            result.Add(new Tournament(tournament, user));
            return Ok(result.AsQueryable());
        }

        // GET: odata/Tournaments(5)/attendees
        [Queryable]
        public async Task<IHttpActionResult> GetAttendees([FromODataUri] System.Guid key, ODataQueryOptions<Tournament> queryOptions)
        {
            // validate the query.
            try
            {
                queryOptions.Validate(_validationSettings);
            }
            catch (ODataException ex)
            {
                return BadRequest(ex.Message);
            }

            DebReg.Models.User user = await GetUser();

            if (user == null)
            {
                return Unauthorized();
            }

            // Get team speakers

            var teams = tournamentRegistrationsManager.GetTeams(key);
            var speakers = from t in teams
                           from s in t.Speaker
                           select s;

            // Get adjudicators

            var adjudicators = tournamentRegistrationsManager.GetAdjudicators(key).Select(a => a.User);

            // Join attendees

            var attendees = speakers.Union(adjudicators);

            // Generate result model

            var result = attendees.Select(a => new User(a, key));
            return Ok(result.AsQueryable());
        }

        // PUT: odata/Tournaments(5)
        public async Task<IHttpActionResult> Put([FromODataUri] System.Guid key, Delta<Tournament> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Put(tournament);

            // TODO: Save the patched entity.

            // return Updated(tournament);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // POST: odata/Tournaments
        public async Task<IHttpActionResult> Post(Tournament tournament)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Add create logic here.

            // return Created(tournament);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // PATCH: odata/Tournaments(5)
        [AcceptVerbs("PATCH", "MERGE")]
        public async Task<IHttpActionResult> Patch([FromODataUri] System.Guid key, Delta<Tournament> delta)
        {
            Validate(delta.GetEntity());

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // TODO: Get the entity here.

            // delta.Patch(tournament);

            // TODO: Save the patched entity.

            // return Updated(tournament);
            return StatusCode(HttpStatusCode.NotImplemented);
        }

        // DELETE: odata/Tournaments(5)
        public async Task<IHttpActionResult> Delete([FromODataUri] System.Guid key)
        {
            // TODO: Add delete logic here.

            // return StatusCode(HttpStatusCode.NoContent);
            return StatusCode(HttpStatusCode.NotImplemented);
        }
    }
}
