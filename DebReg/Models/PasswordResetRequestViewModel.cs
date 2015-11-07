using System;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Web.Models {
    public class PasswordResetRequestViewModel {
        [Display(Name = "EMail", ResourceType = typeof(Resources.Models.PasswordResetRequest.Strings))]
        [Required]
        [DataType(DataType.EmailAddress)]
        public String EMail { get; set; }
    }
}