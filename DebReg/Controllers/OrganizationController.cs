using DebReg.Data;
using DebReg.Models;
using DebReg.Security;
using DebRegComponents;
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
    public class OrganizationController : BaseController
    {
        private IUnitOfWork unitOfWork;
        private ISecurityManager securityManager;
        private DebRegUserManager userManager;
        private IOrganizationManager organizationManager;




        // GET: Organization/Register
        public ActionResult Register(Guid? organizationId = null, String returnUrl = "")
        {

            // Get organization from repository
            Organization organization = null;

            if (organizationId != null)
            {
                organization = organizationManager.GetOrganization((Guid)organizationId);

                // if user is not a delegate of organization, return to home

                if (organization != null
                    && !userManager.HasOrganizationRole(HttpContext.User.Identity.GetUserId(), (Guid)organizationId, OrganizationRole.Delegate))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            if (organization == null)
            {
                organization = new Organization { Address = new Address() };
            }

            ViewBag.returnUrl = returnUrl;
            return View(organization);
        }

        // POST: Organization/Register
        [HttpPost]
        [ValidateInput(false)]
        public async Task<ActionResult> Register(Organization organization, string action = "", int remove = -1, String returnUrl = "")
        {
            var userId = HttpContext.User.Identity.GetUserId();
            User user = userManager.FindById(userId);

            // Add university button was clicked

            if (action == "Add")
            {
                organization.University = false;

                // We might need to generate a temporary ID for the organization
                // because it is checked in the linked organization
                if (organization.Id == Guid.Empty || organization.Id == null)
                {
                    organization.Id = Guid.NewGuid();
                }

                organization.LinkedOrganizations.Add(new Organization { University = true, Address = new Address(), LinkedOrganization = organization, LinkedOrganizationId = organization.Id });
                return View(organization);
            }

            // A remove button was clicked
            if (action == "Remove"
                && remove > -1
                && remove < organization.LinkedOrganizations.Count)
            {

                // remove organization from linked organizations
                organization.LinkedOrganizations.RemoveAt(remove);
                ModelState.Clear();
                return View(organization);
            }



            CreateOrUpdateOrganizationResult result;

            if (ModelState.IsValid)
            {
                // Try to get organization from store
                var savedOrganization = organizationManager.GetOrganization(organization.Id);

                if (savedOrganization != null)
                {

                    // Check if user is authorized to edit the organization
                    if (!userManager.HasOrganizationRole(userId, savedOrganization.Id, OrganizationRole.Delegate))
                    {
                        return RedirectToAction("Index", "Home");
                    }

                    result = await organizationManager.UpdateOrganizationAsync(organization, user);
                }
                else
                {
                    result = await organizationManager.CreateOrganizationAsync(organization, user);
                }

                switch (result)
                {
                    case CreateOrUpdateOrganizationResult.Success:
                        // Update currentOrganizationId for user and repeat login to update roles

                        user.CurrentOrganizationId = organization.Id;
                        await securityManager.LoginAsync(user);


                        // Redirect

                        if (String.IsNullOrWhiteSpace(returnUrl))
                        {
                            return RedirectToAction("Display");
                        }
                        else
                        {
                            return Redirect(returnUrl);
                        }
                    case CreateOrUpdateOrganizationResult.DuplicateName:
                        ModelState.AddModelError("", Resources.Organization.Strings.ErrorDuplicateOrganization);
                        break;
                    case CreateOrUpdateOrganizationResult.DuplicateAbbreviation:
                        ModelState.AddModelError("", Resources.Organization.Strings.ErrorDuplicateOrganizationAbbreviation);
                        break;
                    case CreateOrUpdateOrganizationResult.DuplicateNameOnLinkedOrganization:
                        ModelState.AddModelError("", Resources.Organization.Strings.ErrorDuplicateLinkedOrganization);
                        break;
                    default:
                        break;
                }
            }
            return View(organization);
        }

        //private static void UpdateOrganizationProperties(Organization organization, DebReg.Models.User user, Organization savedOrganization) {
        //    savedOrganization.Name = organization.Name;
        //    savedOrganization.Abbreviation = organization.Abbreviation;
        //    savedOrganization.University = organization.University;
        //    if (organization.LinkedOrganizations != null && organization.LinkedOrganizations.Count > 0) {
        //        savedOrganization.University = false;
        //    }
        //    savedOrganization.VatId = organization.VatId;
        //    if (organization.Address != null) {
        //        if (savedOrganization.Address == null) {
        //            savedOrganization.Address = new Address();
        //        }
        //        savedOrganization.Address.City = organization.Address.City;
        //        savedOrganization.Address.Country = organization.Address.Country;
        //        savedOrganization.Address.PostalCode = organization.Address.PostalCode;
        //        savedOrganization.Address.Region = organization.Address.Region;
        //        savedOrganization.Address.StreetAddress1 = organization.Address.StreetAddress1;
        //        savedOrganization.Address.StreetAddress2 = organization.Address.StreetAddress2;
        //        organization.Address.UpdateTrackingData(user);
        //    }
        //    organization.UpdateTrackingData(user);
        //}


        // GET: Organization/DuplicateOrganization
        public ActionResult DuplicateOrganization()
        {
            return View();
        }

        // GET: Organization/Display
        public ActionResult Display()
        {
            var ident = HttpContext.User.Identity as ClaimsIdentity;
            var currentOrganizationId = userManager.FindById(ident.GetUserId()).CurrentOrganizationId;
            //var currentOrganizationId = claimsManager.GetCurrentOrganizationId(HttpContext.User.Identity as ClaimsIdentity);
            if (currentOrganizationId == Guid.Empty || currentOrganizationId == null)
            {
                return RedirectToAction("Select");
            }
            Organization organization = organizationManager.GetOrganization((Guid)currentOrganizationId);
            if (organization == null)
            {
                return RedirectToAction("Select");
            }
            return View(organization);
        }

        // GET: Organization/Select
        [HttpGet]
        public async Task<ActionResult> Select(string returnUrl = "")
        {
            ViewBag.returnUrl = returnUrl;

            User user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (user != null)
            {
                if (user.OrganizationAssociations.Count > 0)
                {
                    var organizationAssociations = from oa in user.OrganizationAssociations
                                                   where oa.Organization.LinkedOrganization == null
                                                   select oa;
                    ViewBag.returnUrl = returnUrl;
                    return View(organizationAssociations);
                }
                else
                {
                    return RedirectToAction("Register", new { returnUrl = returnUrl });
                }
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
        }

        // POST: Organization/Select
        [HttpPost]
        public async Task<ActionResult> Select(Guid id, string returnUrl = "")
        {

            // Get user

            User user = await userManager.FindByNameAsync(HttpContext.User.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("Login", "User");
            }

            // Set currentOrganizationId

            user.CurrentOrganizationId = id;
            await userManager.UpdateAsync(user);

            // Repeat signin of user to update roles

            await securityManager.LoginAsync(user);

            // Redirect

            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Display");
            }
        }

        //public OrganizationController(IOrganizationManager organizationManager, IUnitOfWork unitOfWork, ISecurityManager securityManager)
        //{
        //    this.organizationManager = organizationManager;
        //    this.unitOfWork = unitOfWork;
        //    this.securityManager = securityManager;
        //    this.userManager = UserManager;
        //}

        public OrganizationController(IOrganizationManager organizationManager, IUnitOfWork unitOfWork, ISecurityManager securityManager, DebRegUserManager userManager)
        {
            this.organizationManager = organizationManager;
            this.unitOfWork = unitOfWork;
            this.securityManager = securityManager;
            this.userManager = userManager;
        }
    }
}