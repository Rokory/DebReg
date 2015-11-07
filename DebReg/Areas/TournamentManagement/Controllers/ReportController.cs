using DebReg.Models;
using DebReg.Security;
using DebReg.Web.Areas.TournamentManagement.Models;
using DebReg.Web.Models;
using DebRegComponents;
using DebRegOrchestration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DebReg.Web.Areas.TournamentManagement.Controllers
{
    [Authorize(Roles = "ReportViewer,OrganizationApprover,SlotManager,FinanceManager")]
    public class ReportController : Controller
    {
        private ITournamentRegistrationsManager tournamentRegistrationsManager;
        private ISecurityManager securityManager;
        private DebRegUserManager userManager;
        private ITournamentManager tournamentManager;
        private ICountryManager countryManager;
        private ISlotManager slotManager;

        // GET: TournamentManagement/Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MissingRegistrations()
        {
            var currentTournamentId = userManager.GetCurrentTournamentId(HttpContext.User.Identity as ClaimsIdentity);
            if (currentTournamentId == null)
            {
                return RedirectToAction("SelectTournament", "Home", new { returnUrl = HttpContext.Request.RawUrl });
            }

            // Get all registrations with confirmed slots

            var organizationAttendeesViewModels = (from r in tournamentRegistrationsManager.GetRegistrationsByTournamentId((Guid)currentTournamentId)
                                                   where r.TeamsPaid > 0 || r.AdjudicatorsPaid > 0
                                                   select new OrganizationAttendeesViewModel
                                                   {
                                                       Registration = r
                                                   })
                                                  .ToList();

            // Get all teams

            var teams = tournamentRegistrationsManager.GetTeams((Guid)currentTournamentId);

            // Get all adjudicators

            var adjudicators = tournamentRegistrationsManager.GetAdjudicators((Guid)currentTournamentId);

            // Fill view model with teams, adjudicators, and delegates

            foreach (var organizationAttendeesViewModel in organizationAttendeesViewModels)
            {
                organizationAttendeesViewModel.Teams = (from t in teams
                                                        where t.OrganizationId == organizationAttendeesViewModel.Registration.OrganizationId
                                                        select t)
                                                       .ToList();
                organizationAttendeesViewModel.Adjudicators = (from a in adjudicators
                                                               where a.OrganizationId == organizationAttendeesViewModel.Registration.OrganizationId
                                                               select a)
                                                              .ToList();
                organizationAttendeesViewModel.Delegates = (from ou in organizationAttendeesViewModel.Registration.Organization.UserAssociations
                                                            where ou.Role == OrganizationRole.Delegate
                                                            select ou.User)
                                                           .ToList();
            }

            // Get tournament

            var tournament = tournamentManager.GetTournament((Guid)currentTournamentId);

            // Filter view models for organizations with incomplete registrations

            for (int i = organizationAttendeesViewModels.Count() - 1; i >= 0; i--)
            {
                var item = organizationAttendeesViewModels[i];
                if (item.Teams.Count >= item.Registration.TeamsPaid
                    && !item.Teams.Any(t => t.Speaker.Count < tournament.TeamSize)
                    && item.Adjudicators.Count >= item.Registration.AdjudicatorsPaid)
                {
                    organizationAttendeesViewModels.RemoveAt(i);
                }
            }

            organizationAttendeesViewModels = organizationAttendeesViewModels.OrderBy(vm => vm.Registration.Organization.Name).ToList();

            return View(organizationAttendeesViewModels);

        }

        public async Task<ActionResult> MissingPersonalData()
        {
            var currentTournamentId = userManager.GetCurrentTournamentId(HttpContext.User.Identity as ClaimsIdentity);
            if (currentTournamentId == null)
            {
                return RedirectToAction("SelectTournament", "Home", new { returnUrl = HttpContext.Request.RawUrl });
            }

            var attendeesWithMissingData = await slotManager.GetUsersWithIncompleteDataAsync((Guid)currentTournamentId);
            attendeesWithMissingData = attendeesWithMissingData.OrderBy(a => a.LastName).ThenBy(a => a.FirstName);



            // Find organizations of users and build view model
            List<MissingPersonalDataViewModel> viewModel = new List<MissingPersonalDataViewModel>();

            var teams = tournamentRegistrationsManager.GetTeams((Guid)currentTournamentId).ToList();
            var adjudicators = tournamentRegistrationsManager.GetAdjudicators((Guid)currentTournamentId).ToList();

            foreach (var attendee in attendeesWithMissingData)
            {
                // find in adjudicators first

                Organization organization = null;
                var adjudicator = adjudicators.FirstOrDefault(a => a.UserId == attendee.Id);
                if (adjudicator != null)
                {
                    organization = adjudicator.Organization;
                }

                // if no adjudicator found, find speaker
                if (organization == null)
                {
                    var team = (from t in teams
                                from speaker in t.Speaker
                                where speaker.Id == attendee.Id
                                select t)
                               .FirstOrDefault();
                    if (team != null)
                    {
                        organization = team.Organization;
                    }

                }

                // Add to view model

                viewModel.Add(new MissingPersonalDataViewModel { User = attendee, Organization = organization });

            }

            return View(viewModel);
        }

        public ActionResult Teams()
        {
            var currentTournamentId = userManager.GetCurrentTournamentId(HttpContext.User.Identity as ClaimsIdentity);

            if (currentTournamentId == null)
            {
                return RedirectToAction("SelectTournament", "Home", new { returnUrl = HttpContext.Request.RawUrl });
            }
            var teams = from t in tournamentRegistrationsManager.GetTeams((Guid)currentTournamentId)
                        where t.Speaker.Count > 0
                        orderby t.Organization.Name, t.Name
                        select t;
            return View(teams);
        }

        public ActionResult Attendees()
        {
            // Get currentTournamentId

            var currentTournamentId = userManager.GetCurrentTournamentId(HttpContext.User.Identity as ClaimsIdentity);

            if (currentTournamentId == null)
            {
                return RedirectToAction("SelectTournament", "Home", new { returnUrl = HttpContext.Request.RawUrl });
            }

            // Build view model

            AttendeeListViewModel viewModel = new AttendeeListViewModel();

            // Get all speakers and adjudicators

            var speakers = from t in tournamentRegistrationsManager.GetTeams((Guid)currentTournamentId)
                           from s in t.Speaker
                           select new UserViewModel(s, (Guid)currentTournamentId, t.Organization);

            var adjudicators = from a in tournamentRegistrationsManager.GetAdjudicators((Guid)currentTournamentId)
                               select new UserViewModel(a.User, (Guid)currentTournamentId, a.Organization);

            viewModel.Users = speakers.Union(adjudicators).ToList();

            // Fill countries

            IEnumerable<Country> countries = null;

            foreach (var user in viewModel.Users)
            {
                foreach (var property in user.UserProperties)
                {
                    if (property.Type == DebReg.Models.PropertyType.Country)
                    {
                        if (countries == null)
                        {
                            countries = countryManager.GetCountries();
                        }
                        property.CreateCountryList(countries);
                    }
                }
            }

            // Sort

            viewModel.Users = viewModel.Users
                .OrderBy(u => u.User.LastName)
                .ThenBy(u => u.User.FirstName)
                .ToList();

            // Add user properties to view model

            var userProperties = userManager.GetUserProperties();
            var tournamentUserProperties = tournamentManager.GetUserTournamentProperties((Guid)currentTournamentId);

            foreach (var property in userProperties)
            {
                viewModel.UserProperties.Add(property);
            }

            foreach (var property in tournamentUserProperties)
            {
                if (!viewModel.UserProperties.Any(p => p.Id == property.UserPropertyId))
                {
                    viewModel.UserProperties.Add(property.UserProperty);
                }
            }

            // Ready

            return View(viewModel);


        }

        public ActionResult Adjudicators()
        {
            // Get currentTournamentId

            var currentTournamentId = userManager.GetCurrentTournamentId(HttpContext.User.Identity as ClaimsIdentity);

            if (currentTournamentId == null)
            {
                return RedirectToAction("SelectTournament", "Home", new { returnUrl = HttpContext.Request.RawUrl });
            }

            // Get adjudicators

            var adjudicators = from a in tournamentRegistrationsManager.GetAdjudicators((Guid)currentTournamentId)
                               orderby a.Organization.Name, a.User.LastName, a.User.FirstName
                               select a;
            return View(adjudicators);
        }

        // GET: TournamentManagement/Report/BillingAddresses
        public ActionResult BillingAddresses() {
            // Get currentTournamentId

            var currentTournamentId = userManager.GetCurrentTournamentId(HttpContext.User.Identity as ClaimsIdentity);

            if (currentTournamentId == null)
            {
                return RedirectToAction("SelectTournament", "Home", new { returnUrl = HttpContext.Request.RawUrl });
            }

            // Get tournament registrations
            return View(tournamentRegistrationsManager.GetRegistrationsByTournamentId((Guid) currentTournamentId));
        }
        public ReportController(ITournamentRegistrationsManager tournamentRegistrationsManager, ITournamentManager tournamentManager, ICountryManager countryManager, DebRegUserManager userManager, ISecurityManager securityManager, ISlotManager slotManager)
        {
            this.tournamentRegistrationsManager = tournamentRegistrationsManager;
            this.tournamentManager = tournamentManager;
            this.countryManager = countryManager;
            this.securityManager = securityManager;
            this.userManager = userManager;
            this.slotManager = slotManager;
        }

    }
}