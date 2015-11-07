using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models {
    public class BankAccount : TrackableEntity {
        public Guid Id { get; set; }

        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        [Display(Name = "BankName", ResourceType = typeof(Resources.Models.Bank.Strings))]
        public String BankName { get; set; }

        [ForeignKey("BankAddress")]
        public Guid? BankAddressId { get; set; }

        [Display(Name = "Address", ResourceType = typeof(Resources.Models.Bank.Strings))]
        public virtual Address BankAddress { get; set; }

        [MaxLength(34)]
        [Display(Name = "Iban", ResourceType = typeof(Resources.Models.BankAccount.Strings))]
        public String Iban { get; set; }

        [Display(Name = "Bic", ResourceType = typeof(Resources.Models.Bank.Strings))]
        public String Bic { get; set; }

    }
}
