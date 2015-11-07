using System;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Web.Models {
    public class PasswordChangeRequest {
        public String UserId { get; set; }

        [Display(Name = "OldPassword", ResourceType = typeof(Resources.Models.PasswordChangeRequest.Strings))]
        [Required]
        public string OldPassword { get; set; }

        [Display(Name = "NewPassword", ResourceType = typeof(Resources.Models.PasswordChangeRequest.Strings))]
        [Required]
        public string NewPassword { get; set; }

        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.Models.PasswordChangeRequest.Strings))]
        [Required]
        public string ConfirmPassword { get; set; }

        public string returnUrl { get; set; }
    }
}