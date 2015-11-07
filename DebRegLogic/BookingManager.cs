using DebReg.Data;
using DebReg.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DebRegComponents
{
    public class BookingManager : BaseManager, IBookingManager
    {

        private BookingRecord CreateBookingRecord(
            DateTime date,
            Guid organizationId,
            Guid tournamentId,
            Decimal value,
            Boolean credit,
            String note,
            User user)
        {
            BookingRecord record = new BookingRecord
            {
                Id = Guid.NewGuid(),
                Date = date,
                OrganizationId = organizationId,
                TournamentId = tournamentId,
                Value = value,
                Credit = credit,
                Note = note
            };
            record.UpdateTrackingData(user);
            return record;
        }

        private BookingRecord CreateBookingRecord(
                    DateTime date,
                    Guid organizationId,
                    Guid tournamentId,
                    Product product,
                    Decimal quantity,
                    Decimal value,
                    Boolean credit,
                    User user)
        {
            BookingRecord record = CreateBookingRecord(
                date,
                organizationId,
                tournamentId,
                value,
                credit,
                null,
                user
            );
            record.Product = product;
            record.ProductId = product.Id;
            record.ProductName = product.Name;
            record.Price = product.Price;
            record.VatRate = product.VatRate;
            record.Quantity = quantity;
            return record;
        }


        #region IBookingManager Members


        public DebReg.Models.BookingRecord AddBooking(
            DateTime date,
            Guid organizationId,
            Guid tournamentId,
            Product product,
            Decimal quantity,
            Boolean credit,
            DateTime? paymentsDueDate,
            User user)
        {

            Decimal value = quantity * product.Price;

            BookingRecord record = CreateBookingRecord(
                date,
                organizationId,
                tournamentId,
                product,
                quantity,
                value,
                credit,
                user);

            record.PaymentsDueDate = paymentsDueDate;

            unitOfWork.GetRepository<BookingRecord>().Insert(record);
            unitOfWork.Save();
            return record;
        }

        public BookingRecord AddBooking(
            DateTime date,
            Guid organizationId,
            Guid tournamentId,
            Decimal value,
            Boolean credit,
            String note,
            User user)
        {

            BookingRecord record = CreateBookingRecord(
                date,
                organizationId,
                tournamentId,
                value,
                credit,
                note,
                user);

            unitOfWork.GetRepository<BookingRecord>().Insert(record);
            unitOfWork.Save();
            return record;

        }

        public DebReg.Models.BookingRecord AddBooking(
            DateTime date,
            Guid organizationId,
            Guid tournamentId,
            Decimal value,
            Boolean credit,
            User user)
        {

            return AddBooking(date, organizationId, tournamentId, value, credit, null, user);
        }

        public DebReg.Models.BookingRecord AddBooking(
            DateTime date,
            Guid organizationId,
            Guid tournamentId,
            Product product,
            Decimal quantity,
            Decimal value,
            Boolean credit,
            User user)
        {


            BookingRecord record = CreateBookingRecord(
                date,
                organizationId,
                tournamentId,
                product,
                quantity,
                value,
                credit,
                user);
            unitOfWork.GetRepository<BookingRecord>().Insert(record);
            unitOfWork.Save();
            return record;
        }

        public IList<DebReg.Models.BookingRecord> GetBookings(Guid organizationId, Guid tournamentId)
        {
            return unitOfWork.GetRepository<BookingRecord>().Get(
                b => b.OrganizationId == organizationId && b.TournamentId == tournamentId);
        }

        public IList<BookingRecord> GetBookings(Guid tournamentId)
        {
            return unitOfWork.GetRepository<BookingRecord>().Get(
                b => b.TournamentId == tournamentId);
        }

        public Decimal GetBalance(Guid organizationId, Guid tournamentId)
        {
            IList<BookingRecord> bookings = GetBookings(organizationId, tournamentId);
            var creditSum = bookings
                .Where(b => b.Credit)
                .Sum(b => b.Value);
            var debitSum = bookings
                .Where(b => !b.Credit)
                .Sum(b => b.Value);
            return creditSum - debitSum;
        }


        #endregion

        public BookingManager(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
