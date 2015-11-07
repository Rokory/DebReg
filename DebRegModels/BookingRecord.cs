using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models {
    public class BookingRecord : TrackableEntity {
        public Guid Id { get; set; }

        [Display(Name = "Date", ResourceType = typeof(Resources.Models.BookingRecord.Strings))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        [ForeignKey("Tournament")]
        public Guid TournamentId { get; set; }

        public virtual Tournament Tournament { get; set; }

        [ForeignKey("Product")]
        public Guid? ProductId { get; set; }

        public virtual Product Product { get; set; }

        [Display(Name = "ProductName", ResourceType = typeof(Resources.Models.BookingRecord.Strings))]
        public String ProductName { get; set; }

        [Display(Name = "Price", ResourceType = typeof(Resources.Models.BookingRecord.Strings))]
        public Decimal Price { get; set; }

        [Display(Name = "VatRate", ResourceType = typeof(Resources.Models.BookingRecord.Strings))]
        public Decimal VatRate { get; set; }

        [Display(Name = "Quantity", ResourceType = typeof(Resources.Models.BookingRecord.Strings))]
        public Decimal Quantity { get; set; }

        [Display(Name = "Value", ResourceType = typeof(Resources.Models.BookingRecord.Strings))]
        public Decimal Value { get; set; }

        [Display(Name = "Credit", ResourceType = typeof(Resources.Models.BookingRecord.Strings))]
        public Boolean Credit { get; set; }

        [Display(Name = "PaymentsDueDate", ResourceType = typeof(Resources.Models.BookingRecord.Strings))]
        public DateTime? PaymentsDueDate { get; set; }


        [Display(Name = "Note", ResourceType = typeof(Resources.Models.BookingRecord.Strings))]
        [MaxLength(1500)]
        public String Note { get; set; }

        public void SetProduct(Product product) {
            this.Product = product;
            this.ProductId = product.Id;
            this.ProductName = product.Name;
            this.Price = product.Price;
            this.VatRate = product.VatRate;
        }

    }
}
