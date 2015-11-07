using DebReg.Data;
using DebReg.Models;
using DebReg.Security;
using DebRegCommunication;
using DebRegComponents;
using DebReg.Web.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace DebReg.Web.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        #region Fields

        private IUnitOfWork unitOfWork;
        private ISecurityManager securityManager;
        private TournamentRegistrationsManager tournamentRegistrationsManager;
        private ISendMail sendMail;
        private OrganizationManager organizationManager;
        private DebRegUserManager userManager;
        private ICountryManager countryManager;
        private ITournamentManager tournamentManager;

        #endregion


        #region Actions

        #region Display
        // GET: User/Display

        public async Task<ActionResult> Display(Guid? tournamentId = null)
        {
            return View(await CreateUserViewModel(tournamentId));
        }
        #endregion

        #region Login

        // GET: User/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl = "")
        {
            // retrieve TournamentId or SponsoringOrganizationId from returnUrl

            // retrieve querystring
            var queryStringStart = returnUrl.IndexOf('?') + 1;
            var queryString = returnUrl.Substring(queryStringStart);

            // parse parameters
            var parameters = HttpUtility.ParseQueryString(queryString);

            // retrieve sponsoringOrgnizationId from querystring if available
            String sponsorIdString = parameters["sponsoringOrganizationId"];
            Guid sponsorId = Guid.Empty;
            if (!Guid.TryParse(sponsorIdString, out sponsorId))
            {

                // if not sponsoringOrganizationId is available, check for tournamentId
                String tournamentIdString = parameters["tournamentId"];
                Guid tournamentId = Guid.Empty;
                if (Guid.TryParse(tournamentIdString, out tournamentId))
                {

                    // if tournamentId was found, retrieve hosting organization as sponsoring organization
                    if (unitOfWork != null)
                    {
                        var tournament = unitOfWork.GetRepository<Tournament>().GetById(tournamentId);
                        if (tournament != null)
                        {
                            sponsorId = tournament.HostingOrganizationID;
                        }
                    }
                }
            }


            return View(new UserCredentials { returnUrl = returnUrl, SponsoringOrganizationId = sponsorId });
        }
        // Post: User/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(UserCredentials credentials)
        {
            // Check model
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("", Resources.User.Strings.ErrorEMailPasswordIncorrect);
                credentials.Password = string.Empty;
                return View(credentials);
            }

            // Login user

            if (await securityManager.LoginAsync(credentials.EMail, credentials.Password))
            {
                // Redirect
                if (!string.IsNullOrEmpty(credentials.returnUrl))
                {
                    return Redirect(credentials.returnUrl);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }

            // Login failed

            credentials.Password = "";
            ModelState.AddModelError("", Resources.User.Strings.ErrorEMailPasswordIncorrect);
            return View(credentials);
        }


        #endregion

        #region Logout

        // GET: User/Logout
        public ActionResult Logout()
        {
            securityManager.Logout();
            return RedirectToAction("Index", "Home");
        }
        #endregion

        #region Register/Registered

        // Get: User/Register

        /// <summary>
        /// Displays the register user dialog
        /// </summary>
        /// <param name="sponsoringOrganizationId">Id of a sponsoring (i. e. hosting) organization used to send e-mails. If not given, either teamId or tournamentId must be given</param>
        /// <param name="returnUrl">Optional parameter of url to call after successful registration.</param>
        /// <param name="teamId">Used to register a user and add it to a team of speakers at the same time.</param>
        /// <param name="tournamentId">Used to register a user and add it to a tournament as adjudicator at the same time.</param>
        /// <param name="organizationId">Used to register a user and add it to a tournament as adjudicator for an organization at the same time. If used, tournamentId must also be passed.</param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<ActionResult> Register(Guid? sponsoringOrganizationId = null, String returnUrl = "", Guid? teamId = null, Guid? tournamentId = null, Guid? organizationId = null)
        {
            User user;

            // if no sponsoringOrganizationId is given, try to get it from team

            if (sponsoringOrganizationId == null)
            {
                if (teamId != null)
                {
                    var team = tournamentRegistrationsManager.GetTeam((Guid)teamId);
                    if (team != null
                        && team.Tournament != null)
                    {

                        sponsoringOrganizationId = team.Tournament.HostingOrganizationID;
                    }
                }

                if (tournamentId != null && organizationId != null)
                {
                    var registration = tournamentRegistrationsManager.GetRegistration((Guid)tournamentId, (Guid)organizationId);
                    if (registration != null
                        && registration.Tournament != null)
                    {
                    }

                    sponsoringOrganizationId = registration.Tournament.HostingOrganizationID;
                }

                // if an authenticated user registers a new user, copy the sponsoring organization


                var ident = HttpContext.User.Identity as ClaimsIdentity;

                if (ident != null)
                {
                    var userId = ident.GetUserId();
                    user = await userManager.FindByIdAsync(userId);

                    if (user != null)
                    {
                        sponsoringOrganizationId = user.SponsoringOrganizationId;
                    }
                }


            }

            // if we do not have a sponsoring organization id, redirect to home

            if (sponsoringOrganizationId == null)
            {
                return RedirectToAction("Index", "Home");
            }

            user = new User { SponsoringOrganizationId = (Guid)sponsoringOrganizationId };
            return View(user);
        }

        //Post: User/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateInput(false)]
        public async Task<ActionResult> Register(
            User user,
            Guid? teamId = null,
            Guid? organizationId = null,
            Guid? tournamentId = null,
            String replacesPerson = null,
            String returnUrl = "")
        {

            #region Check teamId and permissions
            // Check teamId, organizationId, tournamentId and user permissions

            Team team = null;
            User authenticatedUser = null;

            if (teamId != null || organizationId != null)
            {

                // Get user

                var ident = HttpContext.User.Identity as ClaimsIdentity;

                if (ident != null)
                {
                    var userId = ident.GetUserId();
                    authenticatedUser = await userManager.FindByIdAsync(userId);

                    if (authenticatedUser != null)
                    {

                        // Get organizationId from team, if necessary

                        if (organizationId == null)
                        {
                            team = tournamentRegistrationsManager.GetTeam((Guid)teamId);

                            if (team != null)
                            {
                                organizationId = team.OrganizationId;
                                tournamentId = team.TournamentId;
                            }


                        }

                        // Check user permissions

                        if (!userManager.HasOrganizationRole(userId, (Guid)organizationId, OrganizationRole.Delegate))
                        {
                            return RedirectToAction("Index", "Home");
                        }

                        // Associate user with organization

                        var organization = organizationManager.GetOrganization((Guid)organizationId);

                        OrganizationUser orgUser = new OrganizationUser
                        {
                            Organization = organization,
                            User = user,
                            Role = OrganizationRole.Member,
                        };

                        user.OrganizationAssociations.Add(orgUser);
                        user.CurrentOrganization = organization;
                    }
                }

                // authenticatedUser is only null, if something was wrong

                if (authenticatedUser == null)
                {
                    return RedirectToAction("Index", "Home");

                }
            }

            #endregion

            if (ModelState.IsValid)
            {
                // store e-mail lower case
                user.Email = user.Email.ToLower();

                // set Username to e-mail address
                user.UserName = user.Email;

                // Create user
                IdentityResult result = await userManager.CreateAsync(user);

                // Process result
                if (result.Succeeded)
                {
                    user = await userManager.FindByNameAsync(user.Email);

                    // Generate reset password token

                    var token = userManager.GeneratePasswordResetToken(user.Id);
                    var resetUrl = this.Url.Action(
                        "ResetPassword",
                        null,
                        new
                        {
                            userId = user.Id,
                            token = token,
                            returnUrl = returnUrl
                        },
                        Request.Url.Scheme
                    );

                    var sponsoringOrganization = organizationManager.GetOrganization(user.SponsoringOrganizationId);

                    sendMail.SponsoringOrganization = sponsoringOrganization;
                    sendMail.UserRegistered(user, resetUrl);

                    // user registration without team, tournamentId, organizationId

                    if (teamId == null && (tournamentId == null || organizationId == null))
                    {
                        // await Signin(user); //signin, so that Registered method has the user. Will be signed out in Registered.
                        return View("Registered", new UserViewModel
                        {
                            User = user
                        });
                    }

                    // user registration with team

                    if (teamId != null)
                    {
                        user = userManager.FindByName(user.Email);

                        if (replacesPerson != null)
                        {
                            tournamentRegistrationsManager.RemoveSpeaker((Guid)teamId, replacesPerson, authenticatedUser);

                        }

                        await tournamentRegistrationsManager.AddSpeakerAsync((Guid)teamId, user.Id, authenticatedUser);
                    }

                    // user registration as adjudidicator

                    else if (organizationId != null & tournamentId != null)
                    {
                        if (replacesPerson != null)
                        {
                            tournamentRegistrationsManager.RemoveAdjudicator((Guid)tournamentId, replacesPerson, authenticatedUser);
                        }

                        await tournamentRegistrationsManager.AddAdjudicatorAsync((Guid)tournamentId, (Guid)organizationId, user.Id, authenticatedUser);
                    }

                    // if we have a returnUrl, redirect to it

                    if (!String.IsNullOrWhiteSpace(returnUrl))
                    {
                        return Redirect(returnUrl);
                    }

                    return View("Registered", new UserViewModel
                    {
                        User = user
                    });

                }
                else
                {
                    AddErrorsFromResult(result);
                }
            }
            return View(user);
        }

        //Get: User/Update
        [HttpGet]
        public async Task<ActionResult> Update(Guid? tournamentId = null)
        {
            UserViewModel viewModel = await CreateUserViewModel(tournamentId);

            return View(viewModel);
        }

        private async Task<UserViewModel> CreateUserViewModel(Guid? tournamentId)
        {
            // Get user

            var ident = HttpContext.User.Identity as ClaimsIdentity;
            String userId = ident.GetUserId();
            User user = await userManager.FindByIdAsync(userId);

            // Get tournament

            Tournament tournament = null;

            if (tournamentId != null)
            {
                tournament = tournamentManager.GetTournament((Guid)tournamentId);
            }

            // Build basic view model

            if (String.IsNullOrEmpty(user.NewEMail))
            {
                user.NewEMail = user.Email;
            }

            UserViewModel viewModel = new UserViewModel
            {
                User = user,
                Tournament = tournament
            };

            // Get common user properties and add them to view model

            var userProperties = userManager.GetUserProperties().ToList();

            foreach (var userProperty in userProperties)
            {
                var userPropertyValue = user.PropertyValues.FirstOrDefault(p => p.UserPropertyId == userProperty.Id);
                var value = userPropertyValue != null ? userPropertyValue.Value : null;
                var userPropertyValueViewModel = CreateUserPropertyValueViewModel(userProperty, value, user);
                viewModel.UserProperties.Add(userPropertyValueViewModel);
            }

            // Tournament specific user properties

            if (tournamentId != null)
            {
                var userTournamentProperties = tournamentManager.GetUserTournamentProperties((Guid)tournamentId);

                foreach (var userTournamentProperty in userTournamentProperties)
                {
                    // If it is a common user property, only update the required property, if necessary

                    var userPropertyValueViewModel = viewModel.UserProperties.FirstOrDefault(pv =>
                        pv.UserPropertyId == userTournamentProperty.UserPropertyId);
                    if (userPropertyValueViewModel != null && userTournamentProperty.Required)
                    {
                        userPropertyValueViewModel.Required = true;
                    }

                    // If it is tournament specific property, add it to the view model

                    if (userPropertyValueViewModel == null)
                    {
                        var userPropertyValue = user.TournamentPropertyValues.FirstOrDefault(tp =>
                            tp.TournamentId == userTournamentProperty.TournamentId
                            && tp.UserPropertyId == userTournamentProperty.UserPropertyId);
                        var value = userPropertyValue != null ? userPropertyValue.Value : null;
                        userPropertyValueViewModel = CreateUserPropertyValueViewModel(userTournamentProperty.UserProperty, value, user);
                        viewModel.UserProperties.Add(userPropertyValueViewModel);
                    }
                }

            }
            return viewModel;
        }

        private UserPropertyValueViewModel CreateUserPropertyValueViewModel(UserProperty userProperty, String value, User user)
        {

            // If not value is provided, build an empty value

            var propertyValue = new UserPropertyValue
            {
                User = user,
                UserId = user.Id,
                UserProperty = userProperty,
                UserPropertyId = userProperty.Id,
                Value = value
            };

            // Build view model for property value

            var propertyViewModel = new UserPropertyValueViewModel(propertyValue);

            // If type is country, build country list

            if (userProperty.Type == PropertyType.Country)
            {
                propertyViewModel.CreateCountryList(countryManager.GetCountries());
            }
            return propertyViewModel;
        }

        //Post: User/Update
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Update(UserViewModel userViewModel, Guid? tournamentId = null)
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            String userId = ident.GetUserId();
            User user = await userManager.FindByIdAsync(userId);

            // Validate custom user properties


            for (int i = 0; i < userViewModel.UserProperties.Count; i++)
            {
                var propertyValueViewModel = userViewModel.UserProperties[i];

                var userProperty = await userManager.GetUserPropertyAsync(propertyValueViewModel.UserPropertyId);
                UserPropertyValue userPropertyValue = new UserPropertyValue
                {
                    User = user,
                    UserId = user.Id,
                    UserProperty = userProperty,
                    UserPropertyId = userProperty.Id,
                    Value = propertyValueViewModel.Value,
                };
                UserPropertyValueViewModel propertyViewModelValidated = new UserPropertyValueViewModel(userPropertyValue);

                // Check if property is required for tournament

                if (tournamentId != null)
                {
                    var userTournamentProperty = tournamentManager.GetUserTournamentProperty((Guid)tournamentId, userProperty.Id);
                    propertyViewModelValidated.Required = userTournamentProperty != null && userTournamentProperty.Required;
                }


                var validationResults = propertyViewModelValidated.Validate(null);
                if (propertyViewModelValidated.Type == PropertyType.Country)
                {
                    propertyViewModelValidated.CreateCountryList(countryManager.GetCountries());
                }
                userViewModel.UserProperties[i] = propertyViewModelValidated;
                foreach (var validationResult in validationResults)
                {
                    ModelState.AddModelError("", validationResult.ErrorMessage);
                }

            }
            #region Change e-mail address

            userViewModel.User.NewEMail = userViewModel.User.NewEMail.Trim().ToLower();

            if (String.IsNullOrWhiteSpace(user.NewEMail))
            {
                user.NewEMail = user.Email;
            }

            if (userViewModel.User.NewEMail != user.Email.Trim().ToLower()
                || userViewModel.User.NewEMail != user.NewEMail.Trim().ToLower())
            {

                // check for duplicate user.
                var duplicateUserEMail = await userManager.FindByEmailAsync(userViewModel.User.NewEMail);
                var duplicateUserName = await userManager.FindByNameAsync(userViewModel.User.NewEMail);
                if (
                    (
                        duplicateUserEMail != null
                        && duplicateUserEMail.Id != user.Id
                    )
                    ||
                    (
                        duplicateUserName != null
                        && duplicateUserName.Id != user.Id
                        )
                    )
                {
                    ModelState.AddModelError("User.NewEMail", Resources.User.Strings.ErrorDuplicateEMail);
                }
            }
            #endregion

            if (!ModelState.IsValid)
            {
                if (tournamentId != null)
                {
                    userViewModel.Tournament = tournamentManager.GetTournament((Guid)tournamentId);
                }
                return View(userViewModel);
            }

            // Everything okay, update data

            user.FirstName = userViewModel.User.FirstName.Trim();
            user.LastName = userViewModel.User.LastName.Trim();
            user.PhoneNumber = userViewModel.User.PhoneNumber;
            user.NewEMail = userViewModel.User.NewEMail;


            // Update user

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
                return View();
            }

            // Store user properties

            foreach (var propertyValue in userViewModel.UserProperties)
            {
                if (propertyValue.TournamentSpecific && tournamentId != null)
                {
                    await tournamentManager.SetUserTournamentPropertyValueAsync(userId, propertyValue.UserPropertyId, (Guid)tournamentId, propertyValue.Value);
                }
                if (!propertyValue.TournamentSpecific)
                {
                    await userManager.SetUserPropertyValueAsync(userId, propertyValue.UserPropertyId, propertyValue.Value);
                }
            }

            // Verify e-mail if needed

            if (user.Email != user.NewEMail)
            {
                // Send E-Mail Confirmation Message

                RedirectToAction("ConfirmEMail", new { userId = userId, tournamentId = tournamentId });
            }

            return RedirectToAction("Display", new { tournamentId = tournamentId });
        }

        public async Task<ActionResult> SendConfirmEMail(string userId, Guid? tournamentId = null)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user != null)
            {
                var token = userManager.GenerateEmailConfirmationToken(userId);

                sendMail.SponsoringOrganization = user.SponsoringOrganization;
                String confirmUrl = this.Url.Action
                (
                    "ConfirmEMail",
                    null,
                    new
                    {
                        userId = user.Id,
                        token = token
                    },
                    Request.Url.Scheme
                );
                sendMail.ConfirmEMailAddress(user, confirmUrl);
            }
            return RedirectToAction("Display", new { tournamentId = tournamentId });
        }

        public async Task<ActionResult> ConfirmEMail(String userId, String token)
        {
            User user = null;

            try
            {
                var result = await userManager.ConfirmEmailAsync(userId, token);
                if (result == IdentityResult.Success)
                {
                    user = await userManager.FindByIdAsync(userId);

                    // Check for duplicate e-mail again

                    var duplicateUser = await userManager.FindByEmailAsync(user.NewEMail);

                    if (duplicateUser != null)
                    {
                        ModelState.AddModelError("", Resources.User.Strings.ErrorDuplicateEMail);
                        user.NewEMail = user.Email;
                        await userManager.UpdateAsync(user);
                        return View((object)null);
                    }

                    if (duplicateUser == null)
                    {
                        AuthManager.SignOut();
                        user.Email = user.NewEMail;
                        user.UserName = user.NewEMail;
                    }
                    await userManager.UpdateAsync(user);
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

            }
            catch (InvalidOperationException e)
            {
                ModelState.AddModelError("", e.Message);
            }
            catch (Exception)
            {
                throw;
            }

            return View(user);
        }

        #endregion

        // Get: User/Find
        public ActionResult Find(String searchTerm = "")
        {
            searchTerm = searchTerm.Trim();
            var viewModel = new FindUserViewModel { SearchTerm = searchTerm, DisplayNewUserLink = false };

            if (searchTerm.Length > 0 && searchTerm.Length < 3)
            {
                ModelState.AddModelError("searchTerm", Resources.Shared.FindUser.Strings.SearchTermMinimumCharactersErrorMessage);
            }

            if (ModelState.IsValid && searchTerm.Length > 0)
            {
                viewModel.Results = userManager.Find(searchTerm).ToList();
                viewModel.DisplayNewUserLink = true;
            }
            return View(viewModel);
        }


        #region ChangePassword

        // Get: User/ChangePassword
        [ValidateInput(false)]
        [AllowAnonymous]
        public ActionResult ChangePassword(String userId, string returnUrl = "")
        {
            return View(new PasswordChangeRequest { UserId = userId, returnUrl = returnUrl });
        }

        // Post: User/ChangePassword
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> ChangePassword(PasswordChangeRequest passwordChangeRequest, String returnUrl = "")
        {
            if (passwordChangeRequest.NewPassword == passwordChangeRequest.ConfirmPassword)
            {
                if (passwordChangeRequest.OldPassword != passwordChangeRequest.NewPassword)
                {
                    IdentityResult result = await userManager.ChangePasswordAsync(passwordChangeRequest.UserId, passwordChangeRequest.OldPassword, passwordChangeRequest.NewPassword);

                    if (result.Succeeded)
                    {
                        var user = await userManager.FindByIdAsync(passwordChangeRequest.UserId);
                        user.PasswordChangeRequired = false;
                        result = await userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            await securityManager.LoginAsync(user);
                            if (String.IsNullOrWhiteSpace(returnUrl))
                            {
                                return RedirectToAction("Index", "Home");
                            }
                            else
                            {
                                return Redirect(returnUrl);
                            }
                        }
                    }
                    AddErrorsFromResult(result);

                }
                else
                {
                    ModelState.AddModelError("", Resources.User.Strings.ErrorPasswordsIdentical);
                }
            }
            else
            {
                ModelState.AddModelError("", Resources.User.Strings.ErrorPasswordsDoNotMatch);
            }
            passwordChangeRequest.OldPassword = String.Empty;
            passwordChangeRequest.NewPassword = String.Empty;
            passwordChangeRequest.ConfirmPassword = String.Empty;
            return View(passwordChangeRequest);


        }

        #endregion

        #region ResetPassword

        // GET: User/RequestPasswordReset
        [AllowAnonymous]
        public ActionResult RequestPasswordReset(string returnUrl = "")
        {
            PasswordResetRequestViewModel request = new PasswordResetRequestViewModel();
            return View(request);
        }

        // POST: User/RequestPasswordReset
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> RequestPasswordReset(PasswordResetRequestViewModel request, string returnUrl = "")
        {
            AuthManager.SignOut();

            if (!ModelState.IsValid)
            {
                return View(request);
            }
            // try to find user
            var user = await userManager.FindByNameAsync(request.EMail);

            if (user == null)
            {

                // if user was not found return form with error
                ModelState.AddModelError("", Resources.User.Strings.ErrorUserNotFound);
                return View(request);
            }

            // Generate token and send it to the user

            var token = userManager.GeneratePasswordResetToken(user.Id);

            // Send E-Mail

            var resetUrl = this.Url.Action(
                "ResetPassword",
                null,
                new
                {
                    userId = user.Id,
                    token = token,
                    returnUrl = returnUrl
                },
                Request.Url.Scheme
            );

            sendMail.SponsoringOrganization = user.SponsoringOrganization;
            sendMail.RequestPasswordReset(user, resetUrl);

            return View("RequestPasswordResetSent", request);
        }


        // GET: User/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(String userId, String token, String returnUrl = "")
        {
            AuthManager.SignOut();
            ViewBag.ReturnUrl = returnUrl;
            PasswordResetViewModel request = new PasswordResetViewModel
            {
                UserId = userId,
                Token = token
            };

            return View(request);
        }

        // POST: User/ResetPassword
        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ResetPassword(PasswordResetViewModel request)
        {
            AuthManager.SignOut();

            // compare passwords
            if (request.NewPassword != request.ConfirmPassword)
            {
                ModelState.AddModelError("", Resources.User.Strings.ErrorPasswordsDoNotMatch);
            }

            // if something is wrong in the model, return view

            if (!ModelState.IsValid)
            {
                request.NewPassword = String.Empty;
                request.ConfirmPassword = String.Empty;
                return View(request);
            }

            // Everything okay, reset password


            try
            {
                IdentityResult result = await userManager.ResetPasswordAsync(request.UserId, request.Token, request.NewPassword);
                if (result.Succeeded)
                {
                    return View("PasswordReset");
                }

                // password reset failed

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }

            }
            catch (InvalidOperationException e)
            {
                ModelState.AddModelError("", e.Message);
            }
            catch (Exception)
            {

                throw;
            }

            request.NewPassword = String.Empty;
            request.ConfirmPassword = string.Empty;
            return View(request);
        }

        #endregion


        #endregion

        #region Constructors

        //public UserController(
        //    IUnitOfWork unitOfWork,
        //    TournamentRegistrationsManager tournamentRegistrationsManager,
        //    ISendMail sendMail,
        //    OrganizationManager organizationManager,
        //    ISecurityManager securityManager,
        //    ICountryManager countryManager,
        //    ITournamentManager tournamentManager)
        //{
        //    this.unitOfWork = unitOfWork;
        //    this.tournamentRegistrationsManager = tournamentRegistrationsManager;
        //    this.sendMail = sendMail;
        //    this.organizationManager = organizationManager;
        //    this.userManager = this.UserManager;
        //    this.securityManager = securityManager;
        //    this.countryManager = countryManager;
        //    this.tournamentManager = tournamentManager;
        //}

        public UserController(
            IUnitOfWork unitOfWork,
            TournamentRegistrationsManager tournamentRegistrationsManager,
            ISendMail sendMail,
            OrganizationManager organizationManager,
            DebRegUserManager userManager,
            ISecurityManager securityManager,
            ICountryManager countryManager,
            ITournamentManager tournamentManager)
        {
            this.unitOfWork = unitOfWork;
            this.tournamentRegistrationsManager = tournamentRegistrationsManager;
            this.sendMail = sendMail;
            this.organizationManager = organizationManager;
            this.securityManager = securityManager;
            this.countryManager = countryManager;
            this.tournamentManager = tournamentManager;
            this.userManager = userManager;
        }


        #endregion

        #region Functions
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
        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (string error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        #endregion

    }
}