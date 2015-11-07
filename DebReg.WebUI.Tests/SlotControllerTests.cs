using DebReg.Data;
using DebReg.Mocks;
using DebReg.Security;
using DebRegCommunication;
using DebRegComponents;
using DebRegOrchestration;
using DebReg.Web.Areas.TournamentManagement.Controllers;
using Microsoft.Owin.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DebReg.WebUI.Tests
{
    [TestClass]
    public class SlotControllerTests
    {

        [TestInitialize]
        public void Init()
        {
            var dataMocks = new DebRegDataMocks();
            IUnitOfWork unitOfWork = dataMocks.UnitOfWork;
            ISlotAssignmentManager slotAssignmentManager = new SlotAssignmentManager(unitOfWork);
            IBookingManager bookingManager = new BookingManager(unitOfWork);

            DebRegUserManager userManager = dataMocks.UserManager;
            ITournamentManager tournamentManager = new TournamentManager(unitOfWork, userManager);

            var communiationMocks = new DebRegCommunicationMocks();
            IEMailService emailService = communiationMocks.EMailService;
            ITournamentRegistrationsManager tournamentRegistrationsManager = new TournamentRegistrationsManager(unitOfWork, emailService, userManager);
            ISlotManager slotManager = new SlotManager(slotAssignmentManager, tournamentRegistrationsManager, tournamentManager, bookingManager, emailService, userManager);

            SecurityMocks securityMocks = new SecurityMocks();
            IAuthenticationManager authManager = securityMocks.AuthManager;
            ISecurityManager securityManager = new SecurityManager(userManager, authManager);
            IOrganizationManager organizationManager = new OrganizationManager(unitOfWork, userManager);
            SlotController slotController = new SlotController(
                unitOfWork, 
                tournamentRegistrationsManager, 
                slotAssignmentManager, 
                slotManager, 
                organizationManager, 
                tournamentManager,
                userManager);
        }

        [TestMethod]
        public void TestMethod1()
        {
            // Arrange

            // Act


            // Assert
        }
    }
}
