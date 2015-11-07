using DebReg.Data;
using DebReg.Mocks;
using DebRegComponents;
using DebReg.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace DebReg.Components.Tests {
    [TestClass]
    public class BookingManagerTests {
        IUnitOfWork unitOfWork;
        BookingManager bookingManager;

        [TestInitialize]
        public void Init() {
            unitOfWork = new DebRegDataMocks().UnitOfWork;
            bookingManager = new BookingManager(unitOfWork);
        }
        [TestMethod]
        public void AddBooking_ShouldCreateBooking() {
            // Arrange

            Random rand = new Random();
            DateTime date = DateTime.Now;
            Guid organizationId = Guid.NewGuid();
            Guid tournamentId = Guid.NewGuid();
            Decimal value = rand.Next(10, 1000);
            Boolean credit = false;
            User user = new User { Id = Guid.NewGuid().ToString() };

            // Act

            var record = bookingManager.AddBooking(
                date,
                organizationId,
                tournamentId,
                value,
                credit,
                user
            );

            var id = record.Id;

            // Assert

            record = null;

            record = bookingManager.GetBookings(organizationId, tournamentId)
                .FirstOrDefault(r => r.Id == id);
            Assert.AreEqual(date, record.Date);
            Assert.AreEqual(organizationId, record.OrganizationId);
            Assert.AreEqual(tournamentId, record.TournamentId);
            Assert.AreEqual(value, record.Value);
            Assert.AreEqual(credit, record.Credit);
            Assert.AreEqual(user.Id, record.CreatedById);
            Assert.AreEqual(user.Id, record.ModifiedById);
            Assert.IsNotNull(record.Created);
            Assert.IsNotNull(record.Modified);
        }

        [TestMethod]
        public void AddBooking_WithNote_ShouldCreateBooking() {
            // Arrange

            Random rand = new Random();
            DateTime date = DateTime.Now;
            Guid organizationId = Guid.NewGuid();
            Guid tournamentId = Guid.NewGuid();
            Decimal value = rand.Next(10, 1000);
            Boolean credit = false;
            String note = "Testnote";
            User user = new User { Id = Guid.NewGuid().ToString() };

            // Act

            var record = bookingManager.AddBooking(
                date,
                organizationId,
                tournamentId,
                value,
                credit,
                note,
                user
            );

            var id = record.Id;

            // Assert

            record = null;

            record = bookingManager.GetBookings(organizationId, tournamentId)
                .FirstOrDefault(r => r.Id == id);
            Assert.AreEqual(note, record.Note);
        }

        [TestMethod]
        public void AddBooking_WithProduct_ShouldCreateBooking() {
            // Arrange

            Random rand = new Random();
            DateTime date = DateTime.Now;
            Guid organizationId = Guid.NewGuid();
            Guid tournamentId = Guid.NewGuid();
            String productName = "Test Product";
            Decimal price = rand.Next(10, 1000);
            Decimal vatRate = 0.2M;
            Product product = new Product {
                Id = Guid.NewGuid(),
                Price = price,
                Name = productName,
                VatRate = vatRate
            };
            Decimal quantity = rand.Next(1, 3);
            Decimal value = rand.Next(10, 1000);
            Boolean credit = false;
            User user = new User { Id = Guid.NewGuid().ToString() };

            // Act

            var record = bookingManager.AddBooking(
                date,
                organizationId,
                tournamentId,
                product,
                quantity,
                value,
                credit,
                user
            );

            var id = record.Id;

            // Assert

            record = null;

            record = bookingManager.GetBookings(organizationId, tournamentId)
                .FirstOrDefault(r => r.Id == id);
            Assert.AreEqual(product.Id, record.ProductId);
            Assert.AreEqual(product.Name, record.ProductName);
            Assert.AreEqual(product.Price, record.Price);
            Assert.AreEqual(product.VatRate, record.VatRate);
            Assert.AreEqual(quantity, record.Quantity);
        }

        [TestMethod]
        public void AddBooking_WithProductAndDueDate_ShouldCreateBooking() {
            // Arrange

            Random rand = new Random();
            DateTime date = DateTime.Now;
            Guid organizationId = Guid.NewGuid();
            Guid tournamentId = Guid.NewGuid();
            Product product = new Product {
                Id = Guid.NewGuid(),
                Price = rand.Next(10, 1000),
                Name = "Test Product",
                VatRate = 0.2M
            };
            Decimal quantity = rand.Next(1, 3);
            DateTime dueDate = date.AddDays(14);
            Boolean credit = false;
            User user = new User { Id = Guid.NewGuid().ToString() };

            // Act

            var record = bookingManager.AddBooking(
                date,
                organizationId,
                tournamentId,
                product,
                quantity,
                credit,
                dueDate,
                user
            );

            var id = record.Id;

            // Assert

            record = null;

            record = bookingManager.GetBookings(organizationId, tournamentId)
                .FirstOrDefault(r => r.Id == id);
            Assert.AreEqual(product.Name, record.ProductName);
            Assert.AreEqual(product.Price, record.Price);
            Assert.AreEqual(product.VatRate, record.VatRate);
            Assert.AreEqual(product.Price * quantity, record.Value);
            Assert.AreEqual(dueDate, record.PaymentsDueDate);
        }

        [TestMethod]
        public void GetBalance_ShouldReturnBalance() {
            // Arrange

            Random rand = new Random();
            Guid tournamentId = Guid.NewGuid();
            Guid organizationId = Guid.NewGuid();
            User user = new User { Id = Guid.NewGuid().ToString() };
            int count = 5;
            var oldBalance = bookingManager.GetBalance(tournamentId, organizationId);
            Decimal calculatedBalance = oldBalance;
            for (int i = 0; i < count; i++) {
                Decimal value = rand.Next();
                Boolean credit = rand.Next(0, 1) == 0 ? false : true;
                bookingManager.AddBooking(DateTime.Now, organizationId, tournamentId, value, credit, user);
                calculatedBalance += value * (credit ? 1 : -1);
            }

            // Act

            var newBalance = bookingManager.GetBalance(organizationId, tournamentId);

            // Assert
            Assert.AreEqual(newBalance, calculatedBalance);
        }
    }
}
