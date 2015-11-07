using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models {
    public class Product : TrackableEntity {
        public Guid Id { get; set; }

        [ForeignKey("Tournament")]
        public Guid TournamentId { get; set; }

        public Tournament Tournament { get; set; }

        [MaxLength(70)]
        public String Name { get; set; }

        [MaxLength(1500)]
        public String Description { get; set; }

        public Decimal Price { get; set; }

        public Decimal VatRate { get; set; }


    }
}
