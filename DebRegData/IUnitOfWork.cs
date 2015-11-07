using System;
using System.Threading.Tasks;

namespace DebReg.Data {
    public interface IUnitOfWork : IDisposable {
        //IRepository<Organization> OrganizationRepository { get; }
        //IRepository<Address> AddressRepository { get; }
        //IRepository<User> UserRepository { get; }
        //IRepository<Tournament> TournamentRepository { get; }
        //IRepository<TournamentOrganizationRegistration> TournamentOrganizationRegistrationRepository { get; }
        //IRepository<TournamentUserRole> TournamentUserRoleRepository { get; }
        //IRepository<SlotAssignment> SlotAssignmentRepository { get; }
        //IRepository<Product> ProductRepository { get; }
        //IRepository<Currency> CurrencyRepository { get; }
        //IRepository<BookingRecord> BookingRecordRepository { get; }
        IRepository<T> GetRepository<T>() where T : class;
        void Detach(object entity);
        void Save();
        Task SaveAsync();
    }
}
