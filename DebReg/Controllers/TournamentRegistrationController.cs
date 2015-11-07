using DebReg.Data;
using DebReg.Models;
using DebReg.Security;
using DebReg.Web.Models;
using DebRegComponents;
using DebRegOrchestration;
using Microsoft.AspNet.Identity;
using System;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DebReg.Web.Controllers
{
    [Authorize]
    public class TournamentRegistrationController : BaseController
    {
        private IUnitOfWork unitOfWork;
        private ITournamentRegistrationsManager tournamentRegistrationsManager;
        private IBookingManager bookingManager;
        private DebRegUserManager userManager;
        private ISlotManager slotManager;
        private ISecurityManager securityManager;



        // GET: TournamentRegistration/Index

        /// <summary>
        /// Displays the Tournament Registration view
        /// </summary>
        /// <returns>Redirect to login, if the user is not logged in.
        /// Redirect to organization selection, if there is no current organization.
        /// View of tournaments the current organization is registed for
        /// and tournaments open for registration, if the user is delegate of organization.
        /// Redirect to Home.</returns>
        public ActionResult Index()
        {

            ClaimsIdentity ident = HttpContext.User.Identity as ClaimsIdentity;
            var user = userManager.FindById(ident.GetUserId());
            if (user == null)
            {
                return RedirectToAction("Logout", "User");
            }
            var organizationId = user.CurrentOrganizationId;
            var currentOrganizationId = (Guid)(organizationId == null ? Guid.Empty : organizationId);
            // var currentOrganizationId = claimsManager.GetCurrentOrganizationId(ident);

            // Check if user has selected organization
            if (currentOrganizationId == Guid.Empty)
            {
                return RedirectToAction("Select", "Organization", new { returnUrl = HttpContext.Request.RawUrl });
            }

            //if (!userManager.HasOrganizationRole(ident.GetUserId(), currentOrganizationId, OrganizationRole.Delegate)) {
            //    return RedirectToAction("Select", "Organization");
            //}

            return View();
        }

        /// <summary>
        /// Displays tournaments open for registration
        /// </summary>
        /// <returns>null, if the user is not logged in, or if there is no current organization.
        /// Partial View of tournaments open for registration.</returns>
        [ChildActionOnly]
        public ActionResult GetFutureTournamentsPartial()
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var organizationId = userManager.FindById(ident.GetUserId()).CurrentOrganizationId;
            var currentOrganizationId = (Guid)(organizationId == null ? Guid.Empty : organizationId);

            //var currentOrganizationId = claimsManager.GetCurrentOrganizationId(ident);

            // Check if user has selected organization
            if (currentOrganizationId == Guid.Empty)
            {
                return null;
            }

            // Check if user is delegate of selected organization

            // var user = userManager.FindByName(ident.Name);
            if (userManager.HasOrganizationRole(ident.GetUserId(), currentOrganizationId, OrganizationRole.Delegate))
            {

                // Find registrations of organization
                var registrations = tournamentRegistrationsManager.GetRegistrationsByOrganizationId(currentOrganizationId);

                // Get Tournaments available for registrations

                var tournaments = unitOfWork.GetRepository<Tournament>().Get(
                    filter: t => t.RegistrationEnd > DateTime.UtcNow,
                    orderBy: q => q.OrderBy(t => t.Start)).ToList();

                // Remove tournaments already registered for
                var tournamentsToRemove = (from t in tournaments
                                           join r in registrations
                                           on t.Id equals r.TournamentId
                                           select t)
                                           .ToList();
                foreach (var tournament in tournamentsToRemove)
                {
                    tournaments.Remove(tournament);
                }

                return PartialView(tournaments);

            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Displays tournaments the organization is registered for.
        /// </summary>
        /// <returns>Null, if the user is not logged in or there is no current organization.
        /// Partial View of tournaments the organization is registered for.</returns>
        [ChildActionOnly]
        public async Task<ActionResult> GetRegistrationsPartial()
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var organizationId = userManager.GetCurrentOrganizationId(ident);
            var currentOrganizationId = (Guid)(organizationId == null ? Guid.Empty : organizationId);
            //var currentOrganizationId = claimsManager.GetCurrentOrganizationId(ident);

            // Check if user has selected organization
            if (currentOrganizationId == Guid.Empty)
            {
                return null;
            }

            // var user = userManager.FindByName(ident.Name);
            if (!userManager.HasOrganizationRole(ident.GetUserId(), currentOrganizationId, OrganizationRole.Delegate))
            {
                return null;
            }

            var registrations = tournamentRegistrationsManager.GetRegistrationsByOrganizationId(currentOrganizationId);


            var viewModel = (from r in registrations
                             select new TournamentOrganizationRegistrationViewModel
                             {
                                 Registration = r,
                                 Bookings = bookingManager.GetBookings(r.OrganizationId, r.TournamentId),
                                 Teams = tournamentRegistrationsManager
                                     .GetTeams(r.TournamentId, r.OrganizationId)
                                     .Select(t => new APIModels.Team(t))
                                     .ToList(),
                                 Adjudicators = tournamentRegistrationsManager
                                     .GetAdjudicators(r.TournamentId, r.OrganizationId)
                                     .Select(a => new APIModels.Adjudicator(a))
                                     .ToList(),
                                 TeamsCompleted = tournamentRegistrationsManager.GetTeams(r.TournamentId, r.OrganizationId)
                                    .Where(t => t.Speaker.Count >= r.Tournament.TeamSize).Count(),
                                 AdjudicatorsCompleted = tournamentRegistrationsManager.GetAdjudicators(r.TournamentId, r.OrganizationId).Count()
                             }).ToList();


            foreach (var item in viewModel)
            {
                var inclompleteUsers = await slotManager.GetUsersWithIncompleteDataAsync(currentOrganizationId, item.Registration.TournamentId);
                item.IncompleteUsers = inclompleteUsers.ToList();
            }

            return PartialView(viewModel);
        }

        [ChildActionOnly]
        public async Task<ActionResult> GetSpeakerRegistrationsPartial()
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var userId = ident.GetUserId();

            var teams = tournamentRegistrationsManager.GetTeams(userId).Where(t => t.Tournament.End >= DateTime.UtcNow);
            var speakerRegistrations = teams.Select(t => new SpeakerRegistrationViewModel { Team = t }).ToList();
            foreach (var speakerRegistration in speakerRegistrations)
            {
                var missingProperties = await slotManager.GetIncompletePropertiesAsync(userId, speakerRegistration.Team.TournamentId);
                speakerRegistration.PersonalDataComplete = missingProperties.Count() == 0;
            }
            return PartialView(speakerRegistrations);
        }

        [ChildActionOnly]
        public async Task<ActionResult> GetAdjudicatorRegistrationsPartial()
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var userId = ident.GetUserId();

            var adjudicatorRegistrations = tournamentRegistrationsManager.GetAdjudicators(userId)
                .Where(a => a.Tournament.End >= DateTime.UtcNow)
                .Select(a => new AdjudicatorRegistrationViewModel { Adjudicator = a })
                .ToList();

            foreach (var adjudicatorRegistration in adjudicatorRegistrations)
            {
                var missingProperties = await slotManager.GetIncompletePropertiesAsync(userId, adjudicatorRegistration.Adjudicator.TournamentId);
                adjudicatorRegistration.PersonalDataComplete = missingProperties.Count() == 0;
            }

            return PartialView(adjudicatorRegistrations);
        }


        // GET: TournamentRegistration/Register
        /// <summary>
        /// Displays a view for the organization to register for a tournament.
        /// </summary>
        /// <param name="tournamentId">The id of the tournament, the organization should register for.</param>
        /// <returns>Redirect to login, if the user is not logged in.
        /// Redirect to organization selection, if there is no current organization.
        /// Redirect to Home, if the user is not a delegate of the organization.
        /// Tournament registration view.</returns>
        public ActionResult Register(Guid tournamentId)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var organizationId = userManager.FindById(ident.GetUserId()).CurrentOrganizationId;
            var currentOrganizationId = (Guid)(organizationId == null ? Guid.Empty : organizationId);
            //var currentOrganizationId = claimsManager.GetCurrentOrganizationId(ident);

            if (currentOrganizationId == Guid.Empty)
            {
                return RedirectToAction("Select", "Organization", new { returnUrl = HttpContext.Request.RawUrl });
            }

            // var user = userManager.FindByName(ident.Name);
            if (!userManager.HasOrganizationRole(ident.GetUserId(), currentOrganizationId, OrganizationRole.Delegate))
            {
                return RedirectToAction("Index", "Home");
            }

            var tournament = unitOfWork.GetRepository<Tournament>().GetById(tournamentId);

            if (tournament != null
                && IsOpenForRegistration(tournament))
            {
                TournamentOrganizationRegistration registration = unitOfWork.GetRepository<TournamentOrganizationRegistration>().GetById(tournamentId, currentOrganizationId);

                if (registration == null)
                {

                    // check if university is required
                    var organization = unitOfWork.GetRepository<Organization>().GetById(currentOrganizationId);

                    if (tournament.UniversityRequired)
                    {
                        if (organization == null || !organization.Universitary)
                        {
                            ViewBag.returnUrl = HttpContext.Request.Url;
                            return View("UniversityRequired", new TournamentOrganizationRegistration
                            {
                                OrganizationId = currentOrganizationId,
                                TournamentId = tournament.Id
                            });
                        }
                    }


                    registration = new TournamentOrganizationRegistration
                    {
                        OrganizationId = currentOrganizationId,
                        Organization = organization,
                        TournamentId = tournamentId,
                        TeamsWanted = 1
                    };
                    if (tournament.AdjucatorSubtract != null)
                    {
                        registration.AdjudicatorsWanted = registration.TeamsWanted - (int)tournament.AdjucatorSubtract;
                    }
                }
                return View(registration);
            }

            return RedirectToAction("Index");
        }

        // POST: TournamentRegistration/Register
        /// <summary>
        /// Registers an organization for a tournament
        /// </summary>
        /// <param name="registration">Object containing registration data.</param>
        /// <returns>Redirect to login, if the user is not logged in.
        /// Redirect to organization selection, if there is no current organization.
        /// Redirect to Home, if the user is not a delegate of the organization.
        /// Redirects to Index, if the tournament is not open for registration.
        /// Redirects to Display, if the registration is successful.
        /// Registration form view in all other cases.</returns>
        [HttpPost]
        public ActionResult Register(TournamentOrganizationRegistration registration)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var organizationId = userManager.FindById(ident.GetUserId()).CurrentOrganizationId;
            var currentOrganizationId = (Guid)(organizationId == null ? Guid.Empty : organizationId);
            if (currentOrganizationId == Guid.Empty)
            {
                return RedirectToAction("Select", "Organization", new { returnUrl = HttpContext.Request.RawUrl });
            }

            var organization = unitOfWork.GetRepository<Organization>().GetById(registration.OrganizationId);

            var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());
            //if (user == null) {
            //    return RedirectToAction("Login", "User");
            //}


            if (userManager.HasOrganizationRole(user.Id, currentOrganizationId, OrganizationRole.Delegate))
            {
                var tournament = unitOfWork.GetRepository<Tournament>().GetById(registration.TournamentId);

                if (tournament != null
                    && IsOpenForRegistration(tournament))
                {


                    // check if university is required

                    if (tournament.UniversityRequired
                        && !IsUniversity(organization))
                    {
                        return View("UniversityRequired");
                    }

                    // check number of teams and adjudicators
                    if (registration.TeamsWanted > tournament.TeamCap)
                    {
                        ModelState.AddModelError("", Resources.TournamentRegistration.Strings.ErrorTeamsWantedExceedTeamCap);
                    }

                    if (registration.AdjudicatorsWanted > tournament.AdjudicatorCap)
                    {
                        ModelState.AddModelError("", Resources.TournamentRegistration.Strings.ErrorAdjWantedExceedAdjCap);
                    }

                    if (tournament.AdjucatorSubtract != null
                        && registration.AdjudicatorsWanted < registration.TeamsWanted - (int)tournament.AdjucatorSubtract)
                    {

                        ModelState.AddModelError("", Resources.TournamentRegistration.Strings.ErrorAdjWantedBelowPolicy);
                    }

                    // check if organization can be billed
                    if (organization.Id != registration.BilledOrganizationId
                        && organization.LinkedOrganizations.FirstOrDefault(
                            o => o.Id == registration.BilledOrganizationId) == null)
                    {

                        ModelState.AddModelError("BilledOrganizationId", Resources.TournamentRegistration.Strings.ErrorOrganizationCannotBeBilled);
                    }


                    // Everything okay, so register
                    if (ModelState.IsValid)
                    {

                        try
                        {
                            var registrationSaved = unitOfWork.GetRepository<TournamentOrganizationRegistration>().GetById(registration.TournamentId, registration.OrganizationId);

                            // if old registration found, update it
                            if (registrationSaved != null)
                            {
                                registrationSaved.BilledOrganizationId = registration.BilledOrganizationId;
                                registrationSaved.TeamsWanted = registration.TeamsWanted;
                                registrationSaved.AdjudicatorsWanted = registration.AdjudicatorsWanted;
                                registrationSaved.Notes = registration.Notes;
                                registrationSaved.UpdateTrackingData(user);
                            }

                            // otherwise create a new one
                            else
                            {
                                registration.TeamsGranted = 0;
                                registration.TeamsPaid = 0;
                                registration.AdjudicatorsGranted = 0;
                                registration.AdjudicatorsPaid = 0;
                                registration.UpdateTrackingData(user);
                                unitOfWork.GetRepository<TournamentOrganizationRegistration>().Insert(registration);
                            }
                            unitOfWork.Save();
                            return RedirectToAction("Display", new { organizationId = registration.OrganizationId, tournamentId = registration.TournamentId });
                        }
                        catch (DataException)
                        {
                            ModelState.AddModelError("", Resources.Strings.ErrorSaveChanges);
                        }
                    }
                    else
                    {
                        registration.OrganizationId = currentOrganizationId;
                        registration.Organization = unitOfWork.GetRepository<Organization>().GetById(currentOrganizationId);
                        // On errors, display registration form again
                        return View(registration);
                    }


                }
                else
                {
                    // tournament not found or not open for registration
                    return RedirectToAction("Index");
                }
            }

            // if user is not authorized, redirect to home
            return RedirectToAction("Home", "Index");
        }

        // GET: TournamentRegistration/Display
        /// <summary>
        /// Displays the registration of an organization for a tournament.
        /// </summary>
        /// <param name="tournamentId">ID of tournament</param>
        /// <returns>Redirect to login, if the user is not logged in.
        /// Redirect to organization selection, if there is no current organization.
        /// Redirect to Home, if the user is not a delegate of the organization.
        /// View to display registration information.</returns>
        public async Task<ActionResult> Display(Guid tournamentId, Guid? organizationId = null, String tab = "")
        {

            #region Checks
            // Get user

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var userId = ident.GetUserId();
            var user = await userManager.FindByIdAsync(userId);

            Guid currentOrganizationId = Guid.Empty;

            // Get organizationId parameter

            if (organizationId != null)
            {
                currentOrganizationId = (Guid)organizationId;
            }

            // If we do not have an organizationId parameter, get the organizationId from the user object

            if (currentOrganizationId == Guid.Empty)
            {
                var currentOrganizationIdNullable = user.CurrentOrganizationId;
                currentOrganizationId = (Guid)(currentOrganizationIdNullable == null ? Guid.Empty : currentOrganizationIdNullable);
            }

            // If still do not have an organizationId, let the user select an organization

            if (currentOrganizationId == Guid.Empty)
            {
                String returnUrl = null;

                if (HttpContext != null
                    && HttpContext.Request != null)
                {
                    returnUrl = HttpContext.Request.RawUrl;
                }

                return RedirectToAction("Select", "Organization", new { returnUrl = returnUrl });
            }


            // Check user permissions: Either organization delegate or slot manager for tournament

            if (!userManager.HasOrganizationRole(userId, currentOrganizationId, OrganizationRole.Delegate)
                && !userManager.HasTournamentRole(userId, tournamentId, TournamentRole.SlotManager))
            {
                return RedirectToAction("Index", "Home");
            }

            #endregion
            // Everything okay, get the registration

            var registration = tournamentRegistrationsManager.GetRegistration(tournamentId, currentOrganizationId);

            // If we found a registration, build the view model

            if (registration != null)
            {
                TournamentOrganizationRegistrationViewModel viewModel = new TournamentOrganizationRegistrationViewModel
                {
                    Registration = registration,
                    Bookings = bookingManager.GetBookings(registration.OrganizationId, registration.TournamentId)
                };

                #region Teams

                var teams = tournamentRegistrationsManager
                    .GetTeams(registration.TournamentId, registration.OrganizationId)
                    .OrderBy(t => t.Created)
                    .ToList(); ;
                //viewModel.TeamsCompleted = teams.Count();

                // Generate Autosuffix if necessary

                foreach (Team team in teams)
                {
                    if (String.IsNullOrWhiteSpace(team.AutoSuffix))
                    {
                        team.AutoSuffix = tournamentRegistrationsManager.GenerateAutosuffix(registration.OrganizationId, registration.TournamentId, teams);
                        tournamentRegistrationsManager.SetTeam(team, user);
                    }
                }


                // Add teams to view model

                foreach (var team in teams)
                {
                    viewModel.Teams.Add(new APIModels.Team(team));
                }

                // Calculate completed teams

                viewModel.TeamsCompleted = teams.Where(t => t.Speaker.Count >= t.Tournament.TeamSize).Count();

                // if we do not get as many teams as confirmed, add empty teams

                var missingTeams = registration.TeamsPaid - viewModel.Teams.Count;

                for (int i = 0; i < missingTeams; i++)
                {
                    var team = new Team
                    {
                        Id = Guid.NewGuid(),
                        OrganizationId = registration.OrganizationId,
                        Organization = registration.Organization,
                        TournamentId = registration.TournamentId,
                        Tournament = registration.Tournament,
                        AutoSuffix = tournamentRegistrationsManager.GenerateAutosuffix(currentOrganizationId, tournamentId, teams),
                        Name = tournamentRegistrationsManager.GenerateTeamName(currentOrganizationId, tournamentId, teams)
                    };

                    tournamentRegistrationsManager.SetTeam(team, user);
                    viewModel.Teams.Add(new APIModels.Team(team));

                    teams = tournamentRegistrationsManager
                        .GetTeams(registration.TournamentId, registration.OrganizationId)
                        .OrderBy(t => t.Created)
                        .ToList(); ;
                }



                // add empty speakers to view model, if necessary

                foreach (var team in viewModel.Teams)
                {
                    var missingSpeakers = registration.Tournament.TeamSize - team.speakers.Count;
                    for (int i = 0; i < missingSpeakers; i++)
                    {
                        var speaker = new APIModels.User
                        {
                            firstname = Resources.TournamentRegistration.Display.Strings.DefaultFirstName,
                            lastname = Resources.TournamentRegistration.Display.Strings.DefaultLastName
                        };
                        team.speakers.Add(speaker);
                    }

                }
                #endregion



                #region Adjudicators

                // Adjudicators

                var adjudicators = tournamentRegistrationsManager
                    .GetAdjudicators(registration.TournamentId, registration.OrganizationId)
                    .OrderBy(t => t.Created)
                    .ToList();

                viewModel.AdjudicatorsCompleted = adjudicators.Count();

                // if we do not get as many adjudicators as confirmed, add empty adjudicators to view model


                // Add teams to view model

                foreach (var adjudicator in adjudicators)
                {
                    viewModel.Adjudicators.Add(new APIModels.Adjudicator(adjudicator));
                }

                var missingAdjudicators = registration.AdjudicatorsPaid - viewModel.Adjudicators.Count;

                for (int i = 0; i < missingAdjudicators; i++)
                {
                    viewModel.Adjudicators.Add(new APIModels.Adjudicator
                    {
                        organizationId = organizationId,
                        tournamentId = tournamentId,
                        user = new APIModels.User
                        {
                            firstname = Resources.TournamentRegistration.Display.Strings.DefaultFirstName,
                            lastname = Resources.TournamentRegistration.Display.Strings.DefaultLastName
                        }
                    });
                }



                #endregion
                ViewBag.Tab = tab;
                return View(viewModel);
            }

            // if we did not find the registration, display the organization instead

            return RedirectToAction("Display", "Organization");
        }

        // GET: TournamentRegistration/Delete
        /// <summary>
        /// Displays a view to confirm the registration deletion.
        /// </summary>
        /// <param name="tournamentId">ID of tournament</param>
        /// <returns>Redirect to login, if the user is not logged in.
        /// Redirect to organization selection, if there is no current organization.
        /// Redirect to Home, if the user is not a delegate of the organization.
        /// Redirect to Display, if the tournament is not open for registration.
        /// View to confirm deletion.</returns>
        public async Task<ActionResult> Delete(Guid tournamentId)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var oganizationId = (await userManager.FindByIdAsync(ident.GetUserId())).CurrentOrganizationId;
            var currentOrganizationId = (Guid)(oganizationId == null ? Guid.Empty : oganizationId);

            if (currentOrganizationId == Guid.Empty)
            {
                return RedirectToAction("Select", "Organization", new { returnUrl = HttpContext.Request.RawUrl });
            }

            // var user = userManager.FindByName(ident.Name);
            if (!userManager.HasOrganizationRole(ident.GetUserId(), currentOrganizationId, OrganizationRole.Delegate))
            {
                return RedirectToAction("Index", "Home");
            }

            var tournament = unitOfWork.GetRepository<Tournament>().GetById(tournamentId);

            if (IsOpenForRegistration(tournament))
            {
                var registration = unitOfWork.GetRepository<TournamentOrganizationRegistration>().GetById(tournamentId, currentOrganizationId);
                if (registration != null)
                {
                    return View(registration);
                }
            }
            return RedirectToAction("Display", new { id = tournamentId });
        }

        // POST: TournamentRegistration/Delete
        /// <summary>
        /// Deletes the tournament registration of an organization.
        /// </summary>
        /// <param name="tournamentId">ID of tournament</param>
        /// <returns>Redirect to login, if the user is not logged in.
        /// Redirect to organization selection, if there is no current organization.
        /// Redirect to Home, if the user is not a delegate of the organization.
        /// Redirect to Index.
        /// </returns>
        [HttpPost]
        public ActionResult Delete(TournamentOrganizationRegistration registration)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var organizationId = userManager.FindById(ident.GetUserId()).CurrentOrganizationId;
            var currentOrganizationId = (Guid)(organizationId == null ? Guid.Empty : organizationId);

            if (currentOrganizationId == Guid.Empty)
            {
                return RedirectToAction("Select", "Organization", new { returnUrl = HttpContext.Request.RawUrl });
            }

            //var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());
            if (!userManager.HasOrganizationRole(ident.GetUserId(), currentOrganizationId, OrganizationRole.Delegate))
            {
                return RedirectToAction("Index", "Home");
            }

            var tournament = unitOfWork.GetRepository<Tournament>().GetById(registration.TournamentId);
            if (IsOpenForRegistration(tournament))
            {
                if (registration != null)
                {
                    unitOfWork.GetRepository<TournamentOrganizationRegistration>().Delete(registration.TournamentId, registration.OrganizationId);
                    try
                    {
                        unitOfWork.Save();
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // POST: TournamentRegistration/PrintAccountStatement
        [HttpGet]
        public async Task<ActionResult> PrintAccountStatement(Guid tournamentId, Guid? organizationId)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var currentOrganizationIdNullable = (await userManager.FindByIdAsync(ident.GetUserId())).CurrentOrganizationId;
            var currentOrganizationId = (Guid)(currentOrganizationIdNullable == null ? Guid.Empty : currentOrganizationIdNullable);

            if (currentOrganizationId == Guid.Empty)
            {
                return RedirectToAction("Select", "Organization", new { returnUrl = HttpContext.Request.RawUrl });
            }

            var user = userManager.FindByName(ident.Name);

            TournamentUserRole tournamentRole = null;
            if (organizationId != null && user != null)
            {
                tournamentRole = unitOfWork.GetRepository<TournamentUserRole>().Get(
                                          r => r.TournamentId == tournamentId
                                              && r.UserId == user.Id
                                              && r.Role == TournamentRole.SlotManager)
                                              .FirstOrDefault();
                if (tournamentRole != null)
                {
                    currentOrganizationId = (Guid)organizationId;
                }
            }


            if (!userManager.HasOrganizationRole(user.Id, currentOrganizationId, OrganizationRole.Delegate)
                && tournamentRole == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var registration = unitOfWork.GetRepository<TournamentOrganizationRegistration>().GetById(tournamentId, currentOrganizationId);

            if (registration != null)
            {
                TournamentOrganizationRegistrationViewModel viewModel = new TournamentOrganizationRegistrationViewModel
                {
                    Registration = registration,
                    Bookings = bookingManager.GetBookings(registration.OrganizationId, registration.TournamentId)
                };
                return View(viewModel);
            }

            return RedirectToAction("Display", "Organization");
        }

        private bool IsOpenForRegistration(Tournament tournament)
        {
            if (tournament != null)
            {
                return tournament.RegistrationStart < DateTime.UtcNow
                    && tournament.RegistrationEnd > DateTime.UtcNow
                    && tournament.Start > DateTime.UtcNow;

            }
            return false;
        }

        public bool IsUniversity(Organization organization)
        {
            if (organization.University)
            {
                return true;
            }
            var linkedUniversitities = from o in organization.LinkedOrganizations
                                       where o.University
                                       select o;
            return linkedUniversitities.Count() > 0;
        }

        //public TournamentRegistrationController(IUnitOfWork unitOfWork, ITournamentRegistrationsManager tournamentRegistrationsManager, IBookingManager bookingManager, ISlotManager slotManager, ISecurityManager securityManager)
        //{
        //    this.unitOfWork = unitOfWork;
        //    this.tournamentRegistrationsManager = tournamentRegistrationsManager;
        //    this.bookingManager = bookingManager;
        //    userManager = UserManager;
        //    this.slotManager = slotManager;
        //    this.securityManager = securityManager;
        //}

        public TournamentRegistrationController(IUnitOfWork unitOfWork, ITournamentRegistrationsManager tournamentRegistrationsManager, IBookingManager bookingManager, DebRegUserManager userManager, ISlotManager slotManager, ISecurityManager securityManager)
        {
            this.unitOfWork = unitOfWork;
            this.tournamentRegistrationsManager = tournamentRegistrationsManager;
            this.bookingManager = bookingManager;
            this.slotManager = slotManager;
            this.securityManager = securityManager;

            this.userManager = userManager;
        }


    }
}