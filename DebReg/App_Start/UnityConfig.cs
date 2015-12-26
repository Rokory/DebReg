using DebReg.Data;
using DebReg.Models;
using DebReg.Security;
using DebReg.Web.Infrastructure;
using DebRegCommunication;
using DebRegComponents;
using DebRegOrchestration;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Microsoft.Practices.Unity;
using System;
using System.Data.Entity;
using System.Web;

namespace DebReg.Web
{
    /// <summary>
    /// Specifies the Unity configuration for the main container.
    /// </summary>
    public class UnityConfig
    {
        #region Unity Container
        private static Lazy<IUnityContainer> container = new Lazy<IUnityContainer>(() =>
        {
            var container = new UnityContainer();
            RegisterTypes(container);
            return container;
        });

        /// <summary>
        /// Gets the configured Unity container.
        /// </summary>
        public static IUnityContainer GetConfiguredContainer()
        {
            return container.Value;
        }
        #endregion

        /// <summary>Registers the type mappings with the Unity container.</summary>
        /// <param name="container">The unity container to configure.</param>
        /// <remarks>There is no need to register concrete types such as controllers or API controllers (unless you want to 
        /// change the defaults), as Unity allows resolving a concrete type even if it was not previously registered.</remarks>
        public static void RegisterTypes(IUnityContainer container)
        {
            // NOTE: To load from web.config uncomment the line below. Make sure to add a Microsoft.Practices.Unity.Configuration to the using statements.
            // container.LoadConfiguration();

            // TODO: Register your types here
            // container.RegisterType<IProductRepository, ProductRepository>();

            // Data Layer
            container.RegisterType<DebRegContext>(new PerResolveLifetimeManager());
            container.RegisterType<DbContext, DebRegContext>();
            //container.RegisterType<IUnitOfWork, DBUnitOfWork>(new InjectionConstructor(DebRegContext.Get()));
            container.RegisterType<IUnitOfWork, DBUnitOfWork>();

            // Security
            //container.RegisterType<IUserStore<User>, UserStore<User>>(new InjectionConstructor(DebRegContext.Get()));
            container.RegisterType<ApplicationUserStore<User>>();
            container.RegisterType<IUserStore<User>, ApplicationUserStore<User>>();
            //container.RegisterType<DebRegUserManager>(new InjectionFactory(c => HttpContext.Current.GetOwinContext().GetUserManager<DebRegUserManager>()));
            container.RegisterType<DebRegUserManager>();
            container.RegisterType<IAuthenticationManager>(new InjectionFactory(c => HttpContext.Current.GetOwinContext().Authentication));
            container.RegisterType<ISecurityManager, SecurityManager>();


            // Communication layer
            container.RegisterType<IEMailService, SMTPEMail>();
            container.RegisterType<ISendMail, SendMail>();

            // Infrastructure
            container.RegisterType<IPasswordHelper, PasswordHelper>();

            // Components
            container.RegisterType<ISlotAssignmentManager, SlotAssignmentManager>();
            container.RegisterType<ITournamentManager, TournamentManager>();
            container.RegisterType<ITournamentRegistrationsManager, TournamentRegistrationsManager>();
            container.RegisterType<ISlotManager, SlotManager>();
            container.RegisterType<IBookingManager, BookingManager>();
            container.RegisterType<IOrganizationManager, OrganizationManager>();
            container.RegisterType<IPaymentManager, PaymentManager>();
            container.RegisterType<ICountryManager, CountryManager>();
        }
    }
}
