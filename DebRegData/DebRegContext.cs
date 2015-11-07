using DebReg.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Data.Entity.Validation;
using System.Linq;

namespace DebReg.Data
{
    public class DebRegContext : IdentityDbContext<User>
    {

        public virtual IDbSet<Organization> Organizations { get; set; }
        public virtual IDbSet<Address> Addresses { get; set; }
        public virtual IDbSet<OrganizationUser> OrganizationUsers { get; set; }
        public virtual IDbSet<Tournament> Tournaments { get; set; }
        public virtual IDbSet<TournamentOrganizationRegistration> TournamentOrganizationRegistrations { get; set; }
        public virtual IDbSet<TournamentUserRole> TournamentUserRoles { get; set; }
        public virtual IDbSet<SlotAssignment> SlotAssignments { get; set; }
        public virtual IDbSet<Product> Products { get; set; }
        public virtual IDbSet<Currency> Currencies { get; set; }
        public virtual IDbSet<BookingRecord> BookingRecords { get; set; }
        public virtual IDbSet<BankAccount> BankAccounts { get; set; }
        public virtual IDbSet<UserProperty> UserProperties { get; set; }
        public virtual IDbSet<UserPropertyValue> UserPropertyValues { get; set; }
        public virtual IDbSet<UserTournamentProperty> UserTournamentProperties { get; set; }
        public virtual IDbSet<UserTournamentPropertyValue> UserTournamentPropertyValues { get; set; }
        public virtual IDbSet<Country> Countries { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();
            modelBuilder.Entity<TournamentOrganizationRegistration>()
                .Property(p => p.Notes)
                .HasColumnType("text");
            modelBuilder.Entity<User>()
                .HasMany<OrganizationUser>(u => u.OrganizationAssociations)
                .WithRequired()
                .HasForeignKey(a => a.UserId);
            modelBuilder.Entity<User>()
                .HasMany<Adjudicator>(u => u.Adjudicator)
                .WithRequired()
                .HasForeignKey(a => a.UserId);
            modelBuilder.Entity<User>()
                .HasMany<Team>(u => u.Teams)
                .WithMany(t => t.Speaker)
                .Map(model =>
                {
                    model.ToTable("TeamSpeaker");
                    model.MapLeftKey("UserId");
                    model.MapRightKey("TeamId");
                });
            modelBuilder.Entity<Organization>()
                .HasMany<OrganizationUser>(u => u.UserAssociations);
            modelBuilder.Entity<OrganizationUser>()
                .HasRequired<User>(a => a.User);
            modelBuilder.Entity<OrganizationUser>()
                .HasRequired<Organization>(a => a.Organization);
            modelBuilder.Entity<Tournament>()
                .HasMany<Product>(u => u.Products);
            modelBuilder.Entity<Tournament>()
                .HasOptional<Product>(t => t.TeamProduct);
            modelBuilder.Entity<Tournament>()
                .HasOptional<Product>(t => t.AdjudicatorProduct);

            base.OnModelCreating(modelBuilder);
        }

        public DebRegContext()
            : base("DebRegContext")
        {
        }

        static DebRegContext()
        {
            Database.SetInitializer<DebRegContext>(new IdentityDbInit());
        }

        public static DebRegContext Create()
        {
            //context = new DebRegContext();
            //return context;
            return new DebRegContext();
        }

        //public static IdentityDbContext<User> Get()
        //{
        //    if (context == null)
        //    {
        //        context = Create();
        //    }
        //    return context as IdentityDbContext<User>;
        //}

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();

            }
            catch (DbEntityValidationException ex)
            {

                // Retrieve the error messages as a list of strings.
                var errorMessages = ex.EntityValidationErrors
                        .SelectMany(x => x.ValidationErrors)
                        .Select(x => x.ErrorMessage);

                // Join the list to a single string.
                var fullErrorMessage = string.Join("; ", errorMessages);

                // Combine the original exception message with the new one.
                var exceptionMessage = string.Concat(ex.Message, " The validation errors are: ", fullErrorMessage);

                // Throw a new DbEntityValidationException with the improved exception message.
                throw new DbEntityValidationException(exceptionMessage, ex.EntityValidationErrors);
            }
        }
    }
}