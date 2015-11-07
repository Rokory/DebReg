using DebReg.Security;
using DebReg.Web.Areas.TournamentManagement.Models;
using DebReg.Web.Controllers;
using DebReg.Web.Models;
using DebRegComponents;
using DebRegOrchestration;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace DebReg.Web.Areas.TournamentManagement.Controllers
{
    [Authorize(Roles = "FinanceManager")]
    public class FinanceController : BaseController
    {
        private ITournamentRegistrationsManager registrationManager;
        private IPaymentManager paymentManager;
        // private IClaimsManager claimsManager;
        private ITournamentManager tournamentManager;
        private IBookingManager bookingManager;
        private DebRegUserManager userManager;

        // GET: TournamentManagement/Finance
        public async Task<ActionResult> Index()
        {

            return View(await GetRegistrationFinancialViewModelsAsync());
        }

        private async Task<IEnumerable<RegistrationFinancialViewModel>> GetRegistrationFinancialViewModelsAsync()
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await userManager.FindByIdAsync(ident.GetUserId())).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);
            var userId = ident.GetUserId();
            var user = await userManager.FindByIdAsync(userId);

            var registrations = registrationManager.GetRegistrationsSortedByRank(currentTournamentId, user);

            var registrationsFinancial = from r in registrations
                                         select new RegistrationFinancialViewModel
                                         {
                                             TournamentId = r.TournamentId,
                                             OrganizationId = r.OrganizationId,
                                             OrganizationName = r.Organization.Name,
                                             BookingCode = r.BookingCode,
                                             Balance = bookingManager.GetBalance(r.OrganizationId, r.TournamentId),
                                             TeamsGranted = r.TeamsGranted,
                                             TeamsPaid = r.TeamsPaid,
                                             AdjudicatorsGranted = r.AdjudicatorsGranted,
                                             AdjudicatorsPaid = r.AdjudicatorsPaid
                                         };

            registrationsFinancial = registrationsFinancial.OrderBy(r => r.OrganizationName);
            return registrationsFinancial.ToList();
        }

        // GET: TournamentManagement/Finance/ConfirmSlots
        public async Task<ActionResult> ConfirmSlots()
        {
            return View(await GetRegistrationFinancialViewModelsAsync());
        }

        // POST: TournamentManagement/Finance/ConfirmSlots
        [HttpPost]
        public async Task<ActionResult> ConfirmSlots(List<RegistrationFinancialViewModel> registrations)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var userId = ident.GetUserId();
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                String returnUrl = null;
                if (HttpContext.Request != null)
                {
                    returnUrl = HttpContext.Request.RawUrl;
                }
                return RedirectToAction("Login", "User", new { Area = "", returnUrl = returnUrl });
            }
            var tournamentId = user.CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);

            List<PaymentViewModel> changes = new List<PaymentViewModel>();

            // Check Model
            foreach (var registration in registrations)
            {
                var oldRegistration = registrationManager.GetRegistration(currentTournamentId, registration.OrganizationId);

                if (oldRegistration.AdjudicatorsPaid != registration.AdjudicatorsPaid
                    || oldRegistration.TeamsPaid != registration.TeamsPaid)
                {

                    if (oldRegistration.TeamsPaid > registration.TeamsPaid)
                    {
                        ModelState.AddModelError("[" + registrations.IndexOf(registration) + "].TeamsPaid", Resources.TournamentManagement.Finance.Strings.ErrorTeamsPaidConfirmedNewBelowOld);
                    }


                    if (oldRegistration.TeamsGranted < registration.TeamsPaid)
                    {
                        ModelState.AddModelError("[" + registrations.IndexOf(registration) + "].TeamsPaid", Resources.TournamentManagement.Finance.Strings.ErrorTeamsPaidConfirmedOverGranted);
                    }

                    if (oldRegistration.AdjudicatorsPaid > registration.AdjudicatorsPaid)
                    {
                        ModelState.AddModelError("[" + registrations.IndexOf(registration) + "].AdjudicatorsPaid", Resources.TournamentManagement.Finance.Strings.ErrorAdjudicatorsPaidNewBelowOld);
                    }

                    if (oldRegistration.AdjudicatorsGranted < registration.AdjudicatorsPaid)
                    {
                        ModelState.AddModelError("[" + registrations.IndexOf(registration) + "].AdjudicatorsPaid", Resources.TournamentManagement.Finance.Strings.ErrorAdjudicatorsPaidConfirmedOverGranted);
                    }

                    if (ModelState.IsValid)
                    {
                        PaymentViewModel paymentViewModel = new PaymentViewModel
                        {
                            OrganizationId = oldRegistration.OrganizationId,
                            Balance = bookingManager.GetBalance(oldRegistration.OrganizationId, oldRegistration.TournamentId),
                            TeamsGranted = oldRegistration.TeamsGranted,
                            TeamsPaidOld = oldRegistration.TeamsPaid,
                            TeamsPaid = registration.TeamsPaid,
                            AdjudicatorsGranted = oldRegistration.AdjudicatorsGranted,
                            AdjudicatorsPaidOld = oldRegistration.AdjudicatorsPaid,
                            AdjudicatorsPaid = registration.AdjudicatorsPaid
                        };

                        if (oldRegistration.Organization != null)
                        {
                            paymentViewModel.OrganizationName = oldRegistration.Organization.Name;
                        }

                        changes.Add(paymentViewModel);
                    }

                }
            }

            if (ModelState.IsValid && changes.Count > 0)
            {
                ModelState.Clear();
                return View("UpdateConfirmedSlots", changes);
            }
            ModelState.AddModelError("", Resources.Strings.ValidationErrorCheckBelow);
            return View(registrations);
        }

        // POST: TournamentManagement/Finance/UpdateConfirmedSlots
        [HttpPost]
        public async Task<ActionResult> UpdateConfirmedSlots(List<PaymentViewModel> payments)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);
            var user = await userManager.FindByNameAsync(ident.Name);

            foreach (var payment in payments)
            {
                paymentManager.ConfirmSlots(
                    currentTournamentId,
                    payment.OrganizationId,
                    payment.TeamsPaid,
                    payment.AdjudicatorsPaid,
                    GetPaymentPageUrl(),
                    user);
            }

            return RedirectToAction("Index");
        }

        // GET: TournamentManagement/Finance/OrganizationDetails
        [HttpGet]
        public async Task<ActionResult> OrganizationDetails(Guid id)
        {

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);

            var registration = registrationManager.GetRegistration(currentTournamentId, id);

            TournamentOrganizationRegistrationViewModel registrationViewModel = null;
            if (registration != null)
            {
                registrationViewModel = new TournamentOrganizationRegistrationViewModel
                {
                    Registration = registration,
                    Bookings = bookingManager.GetBookings(id, currentTournamentId)
                };
            }



            return View(registrationViewModel);
        }


        // GET: TournamentManagement/Finance/PostPayments
        [HttpGet]
        public ActionResult PostPayments()
        {
            PostPaymentsViewModel vm = new PostPaymentsViewModel
            {
                Date = DateTime.Today,
                Value = 0,
                SearchTerm = string.Empty,
                Note = Resources.TournamentManagement.Finance.Strings.PaymentReceivedNote
            };
            return View(vm);
        }


        // POST: TournamentManagement/Finance/BookPayments
        [HttpPost]
        public async Task<ActionResult> PostPayments(PostPaymentsViewModel paymentsVM, Guid? paymentToDelete = null, Boolean validate = false, Boolean post = false)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);

            // model validation

            if (paymentsVM.Payments != null)
            {
                foreach (var payment in paymentsVM.Payments)
                {
                    //if (String.IsNullOrWhiteSpace(payment.SearchTerm)) {
                    //    ModelState.AddModelError("[" + payments.IndexOf(payment) + "].SearchTerm", Resources.TournamentManagement.Finance.Strings.ErrorSearchTermMissing);
                    //}



                    if (payment.OrganizationId != null && payment.OrganizationId != Guid.Empty)
                    {
                        var registration = registrationManager.GetRegistration(currentTournamentId, payment.OrganizationId);

                        if (registration != null)
                        {


                            payment.TeamsPaidOld = registration.TeamsPaid;

                            if (payment.TeamsPaid < payment.TeamsPaidOld)
                            {
                                ModelState.AddModelError("Payments[" + paymentsVM.Payments.IndexOf(payment) + "].TeamsPaid", Resources.TournamentManagement.Finance.Strings.ErrorTeamsPaidConfirmedNewBelowOld);
                            }

                            payment.TeamsGranted = registration.TeamsGranted;

                            if (payment.TeamsPaid > payment.TeamsGranted)
                            {
                                ModelState.AddModelError("Payments[" + paymentsVM.Payments.IndexOf(payment) + "].TeamsPaid", Resources.TournamentManagement.Finance.Strings.ErrorTeamsPaidConfirmedOverGranted);
                            }

                            payment.AdjudicatorsPaidOld = registration.AdjudicatorsPaid;

                            if (payment.AdjudicatorsPaid < payment.AdjudicatorsPaidOld)
                            {
                                ModelState.AddModelError("Payments[" + paymentsVM.Payments.IndexOf(payment) + "].AdjudicatorsPaid", Resources.TournamentManagement.Finance.Strings.ErrorTeamsPaidConfirmedNewBelowOld);
                            }

                            payment.AdjudicatorsGranted = registration.AdjudicatorsGranted;

                            if (payment.AdjudicatorsPaid > payment.AdjudicatorsGranted)
                            {
                                ModelState.AddModelError("Payments[" + paymentsVM.Payments.IndexOf(payment) + "].AdjudicatorsPaid", Resources.TournamentManagement.Finance.Strings.ErrorAdjudicatorsPaidConfirmedOverGranted);
                            }

                            payment.OrganizationId = registration.OrganizationId;
                            payment.OrganizationName = registration.Organization.Name;
                            payment.Balance = bookingManager.GetBalance(registration.OrganizationId, currentTournamentId);
                        }
                    }
                }
            }

            if (paymentToDelete != null)
            {
                var payment = paymentsVM.Payments.FirstOrDefault(
                        p => p.OrganizationId == paymentToDelete
                    );
                if (payment != null)
                {
                    paymentsVM.Payments.Remove(payment);
                }

            }

            if (ModelState.IsValid)
            {
                if (validate)
                {
                    await ValidatePayment(paymentsVM);
                }
                else if (post)
                {
                    await PostPaymentsAsync(paymentsVM.Payments);
                    return RedirectToAction("Index");
                }

            }

            return View(paymentsVM);
        }

        // GET: TournamentManagement/Finance/Journal
        public async Task<ActionResult> Journal()
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);

            var bookings = bookingManager.GetBookings(currentTournamentId);

            return View(bookings);
        }

        private async Task<List<PaymentViewModel>> PostPaymentsAsync(List<PaymentViewModel> payments)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);
            var user = await userManager.FindByNameAsync(ident.Name);

            foreach (var payment in payments)
            {
                if (payment.OrganizationId != null && payment.OrganizationId != Guid.Empty)
                {
                    bookingManager.AddBooking(
                        payment.Date,
                        payment.OrganizationId,
                        currentTournamentId,
                        payment.Value,
                        true,
                        payment.Note,
                        user);

                    String paymentPageUrl = GetPaymentPageUrl();
                    paymentManager.ConfirmSlots(currentTournamentId, payment.OrganizationId, payment.TeamsPaid, payment.AdjudicatorsPaid, paymentPageUrl, user);
                }

            }
            return payments;
        }

        private string GetPaymentPageUrl()
        {
            String paymentPageUrl = String.Empty;
            if
            (
                HttpContext.Request != null
                && HttpContext.Request.Url != null
            )
            {
                paymentPageUrl = HttpContext.Request.Url.GetLeftPart(UriPartial.Authority);
            }

            if (Url != null)
            {
                paymentPageUrl +=
                        Url.Action("Display", "TournamentRegistration", new
                        {
                            Area = "",
                            tournamentId = "{0}",   // will be filled by paymentManager
                            organizationId = "{1}",  // will be filled by paymentManager
                            tab = "account",
                        });
            }

            if (HttpContext.Server != null)
            {
                paymentPageUrl = HttpContext.Server.UrlDecode(paymentPageUrl);
            }
            return paymentPageUrl;
        }

        private async Task ValidatePayment(PostPaymentsViewModel payments)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var tournamentId = (await (userManager.FindByIdAsync(ident.GetUserId()))).CurrentTournamentId;
            var currentTournamentId = (Guid)(tournamentId == null ? Guid.Empty : tournamentId);

            // Check for empty search term

            if (String.IsNullOrWhiteSpace(payments.SearchTerm))
            {
                ModelState.AddModelError("SearchTerm", Resources.TournamentManagement.Finance.Strings.ErrorSearchTermMissing);
                return;
            }

            // Find registration by booking code or organization name

            var registration = registrationManager.GetRegistration(payments.SearchTerm, currentTournamentId);
            if (!String.IsNullOrWhiteSpace(payments.SearchTerm) && registration == null)
            {
                registration = registrationManager.FindRegistrationByOrganizationName(currentTournamentId, payments.SearchTerm);
            }

            // Registration found

            if (registration != null)
            {

                // Check for duplicate

                var duplicate = payments.Payments.FirstOrDefault(p => p.OrganizationId == registration.OrganizationId);

                if (duplicate != null)
                {
                    ModelState.AddModelError(
                        "SearchTerm",
                        String.Format(
                            Resources.TournamentManagement.Finance.Strings.ErrorDuplicatePayment,
                            registration.Organization.Name
                        )
                    );
                    return;
                }

                // Calculate paid slots

                var slotsPaidResult = paymentManager.CalculatePaidSlots(currentTournamentId, registration.OrganizationId, payments.Value);


                // Build view model and add it to the to the main view model

                var viewModel = new PaymentViewModel
                {
                    AdjudicatorsPaid = slotsPaidResult.Adjudicators,
                    Balance = bookingManager.GetBalance(registration.OrganizationId, currentTournamentId),
                    Date = payments.Date,
                    Note = payments.Note,
                    Value = payments.Value,
                    OrganizationId = registration.OrganizationId,
                    OrganizationName = registration.Organization.Name,
                    TeamsPaidOld = registration.TeamsPaid,
                    TeamsPaid = slotsPaidResult.Teams,
                    TeamsGranted = registration.TeamsGranted,
                    AdjudicatorsGranted = registration.AdjudicatorsGranted
                };

                if (payments.Payments == null)
                {
                    payments.Payments = new List<PaymentViewModel>();
                }
                payments.Payments.Add(viewModel);

                // Reset input fields

                ModelState.Clear();
                payments.SearchTerm = String.Empty;
                payments.Value = 0;
                payments.Note = Resources.TournamentManagement.Finance.Strings.PaymentReceivedNote;

                //payments.Payments.Insert(0, viewModel);


                //// renumber model state entries

                //Regex rgxIndexPrefix = new Regex(@"^Payments\[\d+\]\.", RegexOptions.IgnoreCase);
                //Regex rgxDigits = new Regex(@"\d+");

                //ModelStateDictionary newModelState = new ModelStateDictionary();
                //foreach (var item in ModelState) {
                //    var indexPrefix = rgxIndexPrefix.Match(item.Key);
                //    String newKey = item.Key;
                //    if (indexPrefix.Success) {
                //        var indexString = rgxDigits.Match(indexPrefix.Value).Value;
                //        var index = int.Parse(indexString);
                //        index++;
                //        newKey = rgxIndexPrefix.Replace(item.Key, "Payments[" + index + "].");
                //    }
                //    newModelState.Add(newKey, item.Value);
                //}
                //ModelState.Clear();

                //foreach (var item in newModelState) {
                //    ModelState.Add(item);
                //}
            }
            else
            {
                ModelState.AddModelError("SearchTerm", Resources.TournamentManagement.Finance.Strings.ErrorSearchTermNotFound);
            }
        }


        //public FinanceController(ITournamentManager tournamentManager, ITournamentRegistrationsManager registrationManager, IPaymentManager paymentManager, IBookingManager bookingManager)
        //{
        //    this.tournamentManager = tournamentManager;
        //    this.registrationManager = registrationManager;
        //    this.paymentManager = paymentManager;
        //    // this.claimsManager = claimsManager;
        //    this.bookingManager = bookingManager;
        //    this.userManager = UserManager;
        //}

        public FinanceController(ITournamentManager tournamentManager, ITournamentRegistrationsManager registrationManager, IPaymentManager paymentManager, IBookingManager bookingManager, DebRegUserManager userManager)
        {
            this.tournamentManager = tournamentManager;
            this.registrationManager = registrationManager;
            this.paymentManager = paymentManager;
            this.bookingManager = bookingManager;

            this.userManager = userManager;
        }
    }
}