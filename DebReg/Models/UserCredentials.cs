
using System;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Web.Models {
    public class UserCredentials {
        [MaxLength(254), MinLength(4)]
        [Display(Name = "EMail", ResourceType = typeof(Resources.Models.UserCredentials.Strings))]
        [Required]
        public string EMail { get; set; }

        [Display(Name = "Password", ResourceType = typeof(Resources.Models.UserCredentials.Strings))]
        [Required]
        public string Password { get; set; }
        public Guid SponsoringOrganizationId { get; set; }

        public string returnUrl { get; set; }
    }
}