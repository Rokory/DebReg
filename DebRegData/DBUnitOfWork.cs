using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DebReg.Data
{
    public class DBUnitOfWork : IUnitOfWork
    {
        protected DbContext context = null;
        //private IRepository<Organization> organizationRepository;
        //private IRepository<Address> addressRepository;
        //private IRepository<User> userRepository;
        //private IRepository<PhoneNumber> phoneNumberRepository;
        //private IRepository<OrganizationUser> organizationUserRepository;
        //private IRepository<Tournament> tournamentRepository;
        //private IRepository<TournamentOrganizationRegistration> tournamentOrganizationRegistrationRepository;
        //private IRepository<TournamentUserRole> tournamentUserRoleRepository;
        //private IRepository<SlotAssignment> tournamentOrganizationSlotAssignmentRepository;
        //private IRepository<Product> productRepository;
        //private IRepository<Currency> currencyRepository;
        //private IRepository<BookingRecord> bookingRecordRepository;
        private List<object> repositories = new List<object>();

        public DBUnitOfWork(DbContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("Context argument cannot be null in UnitOfWork.");
            }
            this.context = context;
        }

        #region IUnitOfWork Members



        public IRepository<T> GetRepository<T>() where T : class
        {
            foreach (var repository in repositories)
            {
                if (repository is IRepository<T>)
                {
                    return (IRepository<T>)repository;
                }
            }

            IRepository<T> newRepository = new DBRepository<T>(context);
            repositories.Add(newRepository);
            return newRepository;
        }

        //public IRepository<Organization> OrganizationRepository {
        //    get {

        //        if (this.GetRepository<Organization>() == null) {
        //            this.GetRepository<Organization>() = new DBRepository<Organization>(context);
        //        }
        //        return organizationRepository;
        //    }
        //}

        //public IRepository<Address> AddressRepository {
        //    get {
        //        if (this.addressRepository == null) {
        //            this.addressRepository = new DBRepository<Address>(context);
        //        }
        //        return addressRepository;
        //    }
        //}

        //public IRepository<User> UserRepository {
        //    get {
        //        if (this.GetRepository<User>() == null) {
        //            this.GetRepository<User>() = new DBRepository<User>(context);
        //        }
        //        return userRepository;
        //    }
        //}

        //public IRepository<PhoneNumber> PhoneNumberRepository {
        //    get {
        //        if (this.phoneNumberRepository == null) {
        //            this.phoneNumberRepository = new DBRepository<PhoneNumber>(context);
        //        }
        //        return phoneNumberRepository;
        //    }
        //}

        //public IRepository<OrganizationUser> OrganizationUserRepository {
        //    get {
        //        if (this.organizationUserRepository == null) {
        //            this.organizationUserRepository = new DBRepository<OrganizationUser>(context);
        //        }
        //        return organizationUserRepository;
        //    }
        //}

        //public IRepository<Tournament> TournamentRepository {
        //    get {
        //        if (this.GetRepository<Tournament>() == null) {
        //            this.GetRepository<Tournament>() = new DBRepository<Tournament>(context);
        //        }
        //        return tournamentRepository;
        //    }
        //}

        //public IRepository<TournamentOrganizationRegistration> TournamentOrganizationRegistrationRepository {
        //    get {
        //        if (this.GetRepository<TournamentOrganizationRegistration>() == null) {
        //            this.GetRepository<TournamentOrganizationRegistration>() = new DBRepository<TournamentOrganizationRegistration>(context);
        //        }
        //        return tournamentOrganizationRegistrationRepository;
        //    }
        //}

        //public IRepository<TournamentUserRole> TournamentUserRoleRepository {
        //    get {
        //        if (this.GetRepository<TournamentUserRole>() == null) {
        //            this.GetRepository<TournamentUserRole>() = new DBRepository<TournamentUserRole>(context);
        //        }
        //        return this.GetRepository<TournamentUserRole>();
        //    }
        //}

        //public IRepository<SlotAssignment> SlotAssignmentRepository {
        //    get {
        //        if (this.tournamentOrganizationSlotAssignmentRepository == null) {
        //            this.tournamentOrganizationSlotAssignmentRepository = new DBRepository<SlotAssignment>(context);
        //        }
        //        return this.tournamentOrganizationSlotAssignmentRepository;
        //    }
        //}

        //public IRepository<Product> ProductRepository {
        //    get {
        //        if (this.productRepository == null) {
        //            this.productRepository = new DBRepository<Product>(context);
        //        }
        //        return this.productRepository;
        //    }
        //}
        //public IRepository<Currency> CurrencyRepository {
        //    get {
        //        if (this.currencyRepository == null) {
        //            this.currencyRepository = new DBRepository<Currency>(context);
        //        }
        //        return this.currencyRepository;
        //    }
        //}

        //public IRepository<BookingRecord> BookingRecordRepository {
        //    get {
        //        if (this.bookingRecordRepository == null) {
        //            this.bookingRecordRepository = new DBRepository<BookingRecord>(context);
        //        }
        //        return this.bookingRecordRepository;
        //    }
        //}

        public void Detach(object entity)
        {
            if (entity != null)
            {
                context.Entry(entity).State = EntityState.Detached;
            }
        }

        public void Save()
        {
            try
            {
                context.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task SaveAsync()
        {
            try
            {
                await context.SaveChangesAsync();

            }
            catch (DbEntityValidationException e)
            {
                foreach (var error in e.EntityValidationErrors)
                {
                    Trace.WriteLine(String.Format("{0}", error.ValidationErrors));
                }
                throw e;
            }
        }

        #endregion

        #region IDisposable Members


        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }


        #endregion
    }
}