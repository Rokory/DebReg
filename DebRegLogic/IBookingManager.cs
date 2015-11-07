using DebReg.Models;
using System;
using System.Collections.Generic;

namespace DebRegComponents {
    public interface IBookingManager {
        BookingRecord AddBooking(DateTime date, Guid organizationId, Guid tournamentId, Product product, Decimal quantity, Boolean credit, DateTime? paymentsDueDate, User user);
        BookingRecord AddBooking(DateTime date, Guid organizationId, Guid tournamentId, Decimal value, Boolean credit, User user);
        BookingRecord AddBooking(DateTime date, Guid organizationId, Guid tournamentId, Decimal value, Boolean credit, String Note, User user);
        BookingRecord AddBooking(DateTime date, Guid organizationId, Guid tournamentId, Product product, Decimal quantity, Decimal value, Boolean credit, User user);
        IList<BookingRecord> GetBookings(Guid organizationId, Guid tournamentId);
        IList<BookingRecord> GetBookings(Guid tournamentId);
        Decimal GetBalance(Guid organizationId, Guid tournamentId);
    }
}
