using DebReg.Data;
using DebReg.Models;
using DebReg.Security;
using DebRegCommunication;
using DebRegCommunication.Models;
using DebRegComponents;
using DebReg.Web.Areas.TournamentManagement.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DebReg.Web.Areas.TournamentManagement.Controllers
{

    [Authorize]
    public class OrganizationController : Controller
    {
        private Dictionary<Guid, OrganizationStatusMessage> sentMessages = new Dictionary<Guid, OrganizationStatusMessage>();


        private IUnitOfWork unitOfWork;
        private IEMailService mailService;
        private ISecurityManager securityManager;
        private ITournamentRegistrationsManager tournamentRegistrationsManager;
        private DebRegUserManager userManager;


        // GET: TournamentManagement/TournamentRegistration
        public async Task<ActionResult> Index(SortField sort = SortField.Name, Boolean descending = false)
        {

            // Find currentTournament

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var currentTournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            if (tournament == null)
            {
                return RedirectToAction("SelectTournament", "Home");
            }

            // Check if user has current tournament and is in correct role

            if (tournament == null
                || tournament.UserRoles.FirstOrDefault(
                        r => r.UserId == ident.GetUserId()
                            && r.Role == TournamentRole.OrganizationApprover
                    ) == null)
            {
                return RedirectToAction("SelectTournament", "Home");
            }


            // Get all registered organizations

            var registeredOrganizations = unitOfWork.GetRepository<TournamentOrganizationRegistration>().Get(
                r => r.TournamentId == tournament.Id);

            // Set default status, if necessary

            foreach (var registeredOrganization in registeredOrganizations.Where(
                r => r.OrganizationStatus == OrganizationStatus.Unknown))
            {
                registeredOrganization.OrganizationStatus = registeredOrganization.Organization.Status;
                registeredOrganization.OrganizationStatusDraft = true;
            }

            registeredOrganizations = SortOrganizations(registeredOrganizations, sort, descending);
            ViewBag.Sort = sort;
            ViewBag.Descending = descending;
            return View(registeredOrganizations.ToList());
        }

        // POST: TournamentManagement/TournamentRegistration
        [HttpPost, Authorize(Roles = "OrganizationApprover")]
        public async Task<ActionResult> Index(List<TournamentOrganizationRegistration> registrations, SortField sort = SortField.Name, Boolean descending = false, String action = "")
        {
            // Find currentTournament

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var currentTournamentId = userManager.GetCurrentTournamentId(ident) ?? Guid.Empty;
            var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());

            // Check if user is found

            if (user == null)
            {
                return RedirectToAction("Login", "User", new { Area = "" });
            }

            // Check if tournament is found

            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            if (tournament == null)
            {
                return RedirectToAction("SelectTournament", "Home");
            }

            // Save stati

            if (action == "Save" || action == "Draft")
            {
                if (ModelState.IsValid)
                {

                    foreach (var item in registrations)
                    {
                        // find saved registration

                        var savedRegistration = tournamentRegistrationsManager.GetRegistration(currentTournamentId, item.OrganizationId);

                        if (savedRegistration != null)
                        {

                            // update registration in db, if status has changed

                            if (savedRegistration.OrganizationStatus != item.OrganizationStatus)
                            {
                                if (item.OrganizationStatus == OrganizationStatus.Unknown)
                                {
                                    ModelState.AddModelError("[" + registrations.IndexOf(item) + "].OrganizationStatus", Resources.TournamentManagement.Organization.Strings.ErrorStatusCannotBeChangedToUnknown);
                                    item.OrganizationStatus = savedRegistration.OrganizationStatus;
                                }
                            }

                            if (ModelState.IsValid && (
                                savedRegistration.OrganizationStatus != item.OrganizationStatus
                                || savedRegistration.OrganizationStatusNote != item.OrganizationStatusNote))
                            {
                                tournamentRegistrationsManager.SetOrganizationStatusAndNote(
                                    currentTournamentId,
                                    item.OrganizationId,
                                    item.OrganizationStatus,
                                    item.OrganizationStatusNote,
                                    true,
                                    user
                                );
                            }
                        }
                    }
                }
            }

            if (ModelState.IsValid
                && action == "Save")
            {
                return RedirectToAction("ConfirmSave");
            }

            registrations = SortOrganizations(
                tournamentRegistrationsManager.GetRegistrationsByTournamentId(currentTournamentId).ToList(),
                sort,
                descending).ToList();
            ViewBag.Sort = sort;
            ViewBag.Descending = descending;
            return View(registrations);
        }

        public async Task<ActionResult> ConfirmSave(SortField sort = SortField.Name, Boolean descending = false)
        {
            // Find currentTournament

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var currentTournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            // Check if user has current tournament and is in correct role

            if (tournament == null
                || tournament.UserRoles.FirstOrDefault(
                        r => r.UserId == ident.GetUserId()
                            && r.Role == TournamentRole.OrganizationApprover
                    ) == null)
            {
                return RedirectToAction("SelectTournament", "Home");
            }


            // Get all registered organizations

            var registeredOrganizations = unitOfWork.GetRepository<TournamentOrganizationRegistration>().Get(
                r => r.TournamentId == tournament.Id && r.OrganizationStatusDraft && r.OrganizationStatus != OrganizationStatus.Unknown);


            registeredOrganizations = SortOrganizations(registeredOrganizations, sort, descending);
            ViewBag.Sort = sort;
            ViewBag.Descending = descending;
            return View(registeredOrganizations.ToList());
        }

        [HttpPost]
        [Authorize(Roles = "OrganizationApprover")]
        public async Task<ActionResult> ConfirmSave(Boolean confirmed)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var currentTournamentId = userManager.GetCurrentTournamentId(ident);
            var user = unitOfWork.GetRepository<User>().GetById(ident.GetUserId());

            // Check if user is found

            if (user == null)
            {
                return RedirectToAction("Login", "User", new { Area = "" });
            }

            // Check if tournament is found

            Tournament tournament = unitOfWork.GetRepository<Tournament>().GetById(currentTournamentId);

            if (tournament == null)
            {
                return RedirectToAction("SelectTournament", "Home");
            }


            if (confirmed
                && tournament.HostingOrganization != null
                && tournament.HostingOrganization.SMTPHostConfiguration != null)
            {
                // Configure mailService
                mailService.FromAddress = tournament.HostingOrganization.SMTPHostConfiguration.FromAddress;
                mailService.Host = tournament.HostingOrganization.SMTPHostConfiguration.Host;
                mailService.Password = tournament.HostingOrganization.SMTPHostConfiguration.Password;
                mailService.Port = tournament.HostingOrganization.SMTPHostConfiguration.Port;
                mailService.SSL = tournament.HostingOrganization.SMTPHostConfiguration.SSL;
                mailService.Username = tournament.HostingOrganization.SMTPHostConfiguration.Username;
                mailService.SendCompleted += mailService_SendCompleted;


                // Save and send e-mail notifications

                foreach (var savedRegistration in tournamentRegistrationsManager.GetRegistrationsByTournamentId((Guid)currentTournamentId)
                    .Where(r => r.OrganizationStatusDraft && r.OrganizationStatus != OrganizationStatus.Unknown))
                {
                    tournamentRegistrationsManager.PublishOrganizationStatusAndNote((Guid)currentTournamentId, savedRegistration.OrganizationId, user);
                    EMailMessage message = new EMailMessage();

#if DEBUG
                    message.To.Add(mailService.FromAddress);
#else

                    foreach (var recipient in savedRegistration.Organization.UserAssociations.Where(
                        a => a.Role == OrganizationRole.Delegate)) {
                        message.To.Add(recipient.User.Email);
                    }
#endif

                    if (message.To.Count > 0)
                    {

                        message.Subject = Resources.TournamentManagement.Organization.Strings.StatusChangeEMailSubject;

                        message.Bcc.Add(tournament.HostingOrganization.SMTPHostConfiguration.FromAddress);

                        // Compose body

                        var body = String.Format(
                            Resources.TournamentManagement.Organization.Strings.StatusChangeEMailBody,
                            savedRegistration.Organization.Name,
                            tournament.Name,
                            savedRegistration.OrganizationStatus);
                        var htmlBody = String.Format(
                            Resources.TournamentManagement.Organization.Strings.StatusChangeEMailHTMLBody,
                            savedRegistration.Organization.Name,
                            tournament.Name,
                            savedRegistration.OrganizationStatus);
                        if (!String.IsNullOrWhiteSpace(savedRegistration.OrganizationStatusNote))
                        {
                            body += String.Format(
                                Resources.TournamentManagement.Organization.Strings.StatusChangeEMailBodyAdditionalNotes,
                                savedRegistration.OrganizationStatusNote);
                            htmlBody += String.Format(
                                Resources.TournamentManagement.Organization.Strings.StatusChangeEMailBodyAdditionalNotesHTML,
                                savedRegistration.OrganizationStatusNote);
                        }
                        if (savedRegistration.OrganizationStatus == OrganizationStatus.Dropped)
                        {
                            body += Resources.TournamentManagement.Organization.Strings.StatusChangeEMailBodyDropped;
                            htmlBody += Resources.TournamentManagement.Organization.Strings.StatusChangeEMailBodyDroppedHTML;
                        }
                        else if (savedRegistration.OrganizationStatus == OrganizationStatus.Approved)
                        {
                            body += Resources.TournamentManagement.Organization.Strings.StatusChangeEMailBodyApproved;
                            htmlBody += Resources.TournamentManagement.Organization.Strings.StatusChangeEMailBodyApprovedHTML;
                        }


                        // Send mail

                        Guid messageId = Guid.NewGuid();

                        sentMessages.Add(messageId, new OrganizationStatusMessage
                        {
                            Registration = savedRegistration,
                            MailMessage = message,
                            User = user
                        });

                        message.Body = body;
#if DEBUG
                        mailService.Send(message);
#endif
                        message.HTMLBody = htmlBody;
                        mailService.Send(message, messageId);
                    }
                }

                return View("OrganizationStatusSaved");
            }
            return RedirectToAction("Index");
        }


        void mailService_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            if (e.UserState != null)
            {
                Guid messageId = (Guid)e.UserState;

                if (sentMessages.ContainsKey(messageId))
                {
                    EMailMessage message = sentMessages[messageId].MailMessage;
                    TournamentOrganizationRegistration registration = sentMessages[messageId].Registration;
                    User user = sentMessages[messageId].User;

                    if (e.Error == null)
                    {
                        // TODO: Positive logging?

                        // Change status
                    }
                    else
                    {
                        // TODO: Log error
                    }
                    sentMessages.Remove(messageId);
                }
            }
        }

        String GetInnerExeceptionMessage(Exception e)
        {
            if (e.InnerException == null)
            {
                return e.Message;
            }
            else
            {
                return GetInnerExeceptionMessage(e.InnerException);
            }
        }


        private IEnumerable<TournamentOrganizationRegistration> FilterOrganizations(OrganizationFilterSort filterSort, IEnumerable<TournamentOrganizationRegistration> registrations)
        {
            if (filterSort != null)
            {
                var registeredOrganizations = registrations.Where(
                    o => FilterOrganization(o.Organization, filterSort));
                return registeredOrganizations;
            }
            else
            {
                return registrations;
            }
        }

        private IList<TournamentOrganizationRegistration> SortOrganizations(IList<TournamentOrganizationRegistration> registrations, SortField sort, Boolean descending = false)
        {
            if (sort != null)
            {
                IList<TournamentOrganizationRegistration> registeredOrganizations;
                if (descending)
                {
                    switch (sort)
                    {
                        case SortField.Name:
                            registeredOrganizations = registrations.OrderByDescending(o => o.Organization.Name).ToList();
                            break;
                        case SortField.Abbreviation:
                            registeredOrganizations = registrations.OrderByDescending(o => o.Organization.Abbreviation).ToList();
                            break;
                        case SortField.Universitary:
                            registeredOrganizations = registrations.OrderByDescending(o => o.Organization.Universitary).ToList();
                            break;
                        case SortField.Status:
                            registeredOrganizations = registrations.OrderByDescending(o => o.OrganizationStatus).ToList();
                            break;
                        case SortField.Country:
                            registeredOrganizations = registrations.OrderByDescending(o => o.Organization.Address != null ? o.Organization.Address.Country : String.Empty).ToList();
                            break;
                        case SortField.Region:
                            registeredOrganizations = registrations.OrderByDescending(o => o.Organization.Address != null ? o.Organization.Address.Region : String.Empty).ToList();
                            break;
                        case SortField.City:
                            registeredOrganizations = registrations.OrderByDescending(o => o.Organization.Address != null ? o.Organization.Address.City : String.Empty).ToList();
                            break;
                        case SortField.Draft:
                            registeredOrganizations = registrations.OrderByDescending(o => o.OrganizationStatusDraft).ToList();
                            break;
                        default:
                            registeredOrganizations = registrations;
                            break;
                    }

                }
                else
                {
                    switch (sort)
                    {
                        case SortField.Name:
                            registeredOrganizations = registrations.OrderBy(o => o.Organization.Name).ToList();
                            break;
                        case SortField.Abbreviation:
                            registeredOrganizations = registrations.OrderBy(o => o.Organization.Abbreviation).ToList();
                            break;
                        case SortField.Universitary:
                            registeredOrganizations = registrations.OrderBy(o => o.Organization.Universitary).ToList();
                            break;
                        case SortField.Status:
                            registeredOrganizations = registrations.OrderBy(o => o.OrganizationStatus).ToList();
                            break;
                        case SortField.Country:
                            registeredOrganizations = registrations.OrderBy(o => o.Organization.Address != null ? o.Organization.Address.Country : String.Empty).ToList();
                            break;
                        case SortField.Region:
                            registeredOrganizations = registrations.OrderBy(o => o.Organization.Address != null ? o.Organization.Address.Region : String.Empty).ToList();
                            break;
                        case SortField.City:
                            registeredOrganizations = registrations.OrderBy(o => o.Organization.Address != null ? o.Organization.Address.City : String.Empty).ToList();
                            break;
                        case SortField.Draft:
                            registeredOrganizations = registrations.OrderBy(o => o.OrganizationStatusDraft).ToList();
                            break;
                        default:
                            registeredOrganizations = registrations;
                            break;
                    }
                }
                return registeredOrganizations;
            }
            else
            {
                return registrations;
            }
        }

        public OrganizationController(IUnitOfWork unitOfWork, IEMailService mailService, ISecurityManager securityManager, ITournamentRegistrationsManager tournamentRegistrationsManager, DebRegUserManager userManager)
        {
            this.unitOfWork = unitOfWork;
            this.mailService = mailService;
            this.securityManager = securityManager;
            this.tournamentRegistrationsManager = tournamentRegistrationsManager;
            this.userManager = userManager;
        }

        #region FUNCTIONS
        private bool FilterOrganization(Organization organization, OrganizationFilterSort filter)
        {
            Boolean result = true;
            result = result && FilterMultipleSelection(organization.Address.City, filter.City);
            result = result && FilterMultipleSelection(organization.Address.Country, filter.Country);
            result = result && FilterMultipleSelection(organization.Address.Region, filter.Region);
            result = result && FilterMultipleSelection(organization.Status, filter.Status);
            if (filter.Universitary != null)
            {
                result = result && organization.Universitary == filter.Universitary;
            }
            return result;
        }

        private bool FilterMultipleSelection<T>(T item, IEnumerable<T> filter)
        {
            if (filter != null
                && filter.Count() > 0)
            {
                return filter.Where(f => f.Equals(item)).Count() > 0;
            }
            else
            {
                return true;
            }
        }

        #endregion
    }
}