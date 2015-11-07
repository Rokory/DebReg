using DebRegComponents;
using DebReg.Data;
using DebReg.Models;
using DebRegOrchestration;
using DebReg.Web.Areas.TournamentManagement.Models;
using DebReg.Web.Controllers;
using DebReg.Web.Infrastructure;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using System.Threading.Tasks;
using DebReg.Security;

namespace DebReg.Web.Areas.TournamentManagement.Controllers
{
    [Authorize(Roles = "SlotManager")]
    public class SlotController : BaseController
    {
        private IUnitOfWork unitOfWork;
        // private IClaimsManager claimsManager;
        private ITournamentRegistrationsManager tournamentRegistrationsManager;
        private ISlotAssignmentManager slotAssignmentManager;
        private ISlotManager slotManager;
        private ITournamentManager tournamentManager;
        private IOrganizationManager organizationManager;
        private DebRegUserManager userManager;


        // GET: TournamentManagement/Slot
        public async Task<ActionResult> Index()
        {
            // Find currentTournament

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);
            var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());
            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            if (tournament == null)
            {
                return RedirectToAction("SelectTournament", "Home");
            }

            var assignmentsViewModel = GetSlotAssignmentViewModels(currentTournamentId, user);
            SlotViewModel viewModel = new SlotViewModel
            {
                SlotAssignments = assignmentsViewModel,
                TeamWaitlist = slotManager.GetTeamWaitlist(currentTournamentId, user),
                AdjudicatorWaitlist = slotManager.GetAdjudicatorWaitlist(currentTournamentId, user)
            };
            return View(viewModel);
        }

        private IEnumerable<SlotAssignmentViewModel> GetSlotAssignmentViewModels(Guid tournamentId, User user)
        {
            // Build wait list
            var waitList = tournamentRegistrationsManager.GetRegistrationsSortedByRank(tournamentId, user);

            // Get latest version
            var latestVersion = slotAssignmentManager.GetLatestVersion(tournamentId);
            if (latestVersion == null)
            {
                latestVersion = slotAssignmentManager.CreateVersion(tournamentId);
            }

            // Get assignments
            var assignments = slotManager.GetSlotAssignments(tournamentId, latestVersion, user);

            // Build view model
            List<SlotAssignmentViewModel> assignmentsViewModel = new List<SlotAssignmentViewModel>();
            foreach (var registration in waitList)
            {
                var assignment = assignments.FirstOrDefault(
                    a => a.OrganizationId == registration.OrganizationId);
                if (assignment == null)
                {
                    assignment = new SlotAssignment();
                }
                assignmentsViewModel.Add(new SlotAssignmentViewModel
                {
                    Assignment = assignment,
                    Registration = registration
                });
            }
            return assignmentsViewModel;
        }


        // GET: TournamentManagement/Slot/UpdateRanks
        public async Task<ActionResult> UpdateRanks()
        {
            // Find currentTournament

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);
            var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());
            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            if (tournament == null)
            {
                return RedirectToAction("SelectTournament", "Home");
            }


            var registrations = tournamentRegistrationsManager.GetRegistrationsSortedByRank(currentTournamentId, user);

            return View(registrations
                .OrderByDescending(r => r.Rank)
                .ThenBy(r => r.Organization.Name)
                .ToList());
        }

        // POST: TournamentManagement/Slot/UpdateRanks
        [HttpPost]
        public async Task<ActionResult> UpdateRanks(IEnumerable<OrganizationRank> organizationRanks)
        {
            // Find currentTournament

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var user = userManager.FindById(ident.GetUserId());
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);
            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            if (tournament == null)
            {
                return RedirectToAction("SelectTournament", "Home");
            }

            if (ModelState.IsValid)
            {
                foreach (var organizationRank in organizationRanks)
                {
                    try
                    {
                        tournamentRegistrationsManager.SetRank(currentTournamentId, organizationRank.OrganizationId, organizationRank.Rank, user);
                    }
                    catch (DataException e)
                    {
                        ModelState.AddModelError("", Resources.Strings.ErrorSaveChanges);
                        return View(organizationRanks);
                    }
                }
            }
            return RedirectToAction("Index");
        }

        // GET: TournamentManagement/Slot/UpdateSlots
        public async Task<ActionResult> UpdateSlots()
        {
            // Find currentTournament

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var currentTournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());
            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            var viewModel = GetSlotAssignmentViewModels(tournament.Id, user).ToList();

            return View(viewModel);
        }

        // POST: TournamentManagement/Slot/UpdateSlots
        [HttpPost]
        public async Task<ActionResult> UpdateSlots(IList<SlotAssignmentViewModel> slotAssignments)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);
            var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());

            var registrations = tournamentRegistrationsManager.GetRegistrationsSortedByRank(currentTournamentId, user).ToList();

            // Check if slots granted is at least slots confirmed
            //foreach (var assignment in slotAssignments) {
            //    var registration = registrations.First(r => r.OrganizationId == assignment.Registration.OrganizationId);
            //    if (registration.TeamsPaid > assignment.Assignment.TeamsGranted) {
            //        ModelState.AddModelError(
            //            String.Format("[{0}].Assignment.TeamsGranted", slotAssignments.IndexOf(assignment)),
            //            Resources.TournamentManagement.Slot.UpdateSlots.Strings.SlotsGrantedLowerThanSlotsPaid);
            //    }
            //    if (registration.AdjudicatorsPaid > assignment.Assignment.AdjucatorsGranted) {
            //        ModelState.AddModelError(
            //            String.Format("[{0}].Assignment.AdjucatorsGranted", slotAssignments.IndexOf(assignment)),
            //            Resources.TournamentManagement.Slot.UpdateSlots.Strings.SlotsGrantedLowerThanSlotsPaid);
            //    }
            //}

            if (ModelState.IsValid)
            {
                // Find currentTournament

                Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

                var lastVersion = slotAssignmentManager.GetLatestVersion(currentTournamentId);
                if (lastVersion.Status != VersionStatus.Draft)
                {
                    lastVersion = slotAssignmentManager.CreateVersion(currentTournamentId);
                }

                foreach (var assignment in slotAssignments)
                {
                    var registration = registrations.First(r => r.OrganizationId == assignment.Registration.OrganizationId);

                    slotAssignmentManager.AssignSlots(currentTournamentId, assignment.Registration.OrganizationId, lastVersion.Id, assignment.Assignment.TeamsGranted, assignment.Assignment.AdjucatorsGranted, user);

                    // update LockAutoAssign if needed
                    if (registration.LockAutoAssign != assignment.Registration.LockAutoAssign)
                    {
                        tournamentRegistrationsManager.SetLockAutoAssign(currentTournamentId, assignment.Registration.OrganizationId, assignment.Registration.LockAutoAssign, user);
                    }
                }
                return RedirectToAction("Index");
            }

            foreach (var assignment in slotAssignments)
            {
                assignment.Registration.Organization = organizationManager.GetOrganization(assignment.Registration.OrganizationId);
                assignment.Registration.Tournament = tournamentManager.GetTournament(assignment.Registration.TournamentId);
            }
            return View(slotAssignments);

        }

        // GET: TournamentManagement/Slots/AutoAssign
        public async Task<ActionResult> AutoAssign()
        {
            // Find currentTournament

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);
            var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());
            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            slotManager.AssignTeamSlots(currentTournamentId, user);
            slotManager.AssignAdjudicatorSlots(currentTournamentId, user);

            return RedirectToAction("Index");
        }

        // GET: TournamentManagement/Slots/PublishResults
        public async Task<ActionResult> PublishAssignments()
        {
            // Find currentTournament

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var currentTournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());
            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            if (tournament == null)
            {
                return RedirectToAction("Index", "Home");
            }

            PublishSlotAssignmentsViewModel viewModel = new PublishSlotAssignmentsViewModel
            {
                SlotAssignments = GetSlotAssignmentChanges(tournament, user),
                PaymentsDueDate = null,
                Publish = false
            };

            return View(viewModel);
        }

        private IEnumerable<SlotAssignmentViewModel> GetSlotAssignmentChanges(Tournament tournament, User user)
        {
            return GetSlotAssignmentViewModels(tournament.Id, user).Where(
            a => a.Assignment.TeamsGranted != a.Registration.TeamsGranted
                || a.Assignment.AdjucatorsGranted != a.Registration.AdjudicatorsGranted);
        }

        // GET: TournamentManagement/Slots/PublishResults
        [HttpPost]
        public async Task<ActionResult> PublishAssignments(PublishSlotAssignmentsViewModel model)
        {
            if (model.Publish)
            {
                var ident = HttpContext.User.Identity as ClaimsIdentity;
                var currentTournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
                var user = userManager.FindById(ident.GetUserId());
                Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

                if (ModelState.IsValid && model.PaymentsDueDate != null)
                {
                    // Find currentTournament


                    if (tournament == null)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                    String paymentPageUrl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) +
                        Url.Action("Display", "TournamentRegistration", new
                        {
                            Area = "",
                            tournamentId = "{0}",   // will be filled by slotManager
                            organizationId = "{1}",  // will be filled by slotManager
                            tab = "account",
                        });
                    paymentPageUrl = HttpContext.Server.UrlDecode(paymentPageUrl);
                    slotManager.PublishAssignments(tournament.Id, (DateTime)model.PaymentsDueDate, paymentPageUrl, user);
                }
                else
                {
                    model.SlotAssignments = GetSlotAssignmentChanges(tournament, user);
                    return View(model);
                }
            }
            return RedirectToAction("Index");
        }

        //public ActionResult SendAssignmentNotifications() {
        //    // Find currentTournament

        //    var ident = HttpContext.User.Identity as ClaimsIdentity;
        //    var currentTournamentId = claimsManager.GetCurrentTournamentId(ident);
        //    var user = userManager.FindById(ident.GetUserId());

        //    String paymentPageUrl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority) +
        //        Url.Action("Display", "TournamentRegistration", new {
        //            Area = "",
        //            tournamentId = "{0}",   // will be filled by slotManager
        //            organizationId = "{1}",  // will be filled by slotManager
        //            tab = "account",
        //        });
        //    paymentPageUrl = HttpContext.Server.UrlDecode(paymentPageUrl);
        //    slotManager.SendAssignmentNotifications(currentTournamentId, paymentPageUrl, user);
        //    return Redirect("Index");
        //}
        public SlotController(
            IUnitOfWork unitOfWork,
            ITournamentRegistrationsManager tournamentRegistrationsManager,
            ISlotAssignmentManager slotAssignmentManager,
            ISlotManager slotManager,
            IOrganizationManager organizationManager,
            ITournamentManager tournamentManager,
            DebRegUserManager userManager)
        {
            this.unitOfWork = unitOfWork;
            // this.claimsManager = claimsManager;
            this.tournamentRegistrationsManager = tournamentRegistrationsManager;
            this.slotAssignmentManager = slotAssignmentManager;
            this.slotManager = slotManager;
            this.organizationManager = organizationManager;
            this.tournamentManager = tournamentManager;
            this.userManager = userManager;
        }
    }
}