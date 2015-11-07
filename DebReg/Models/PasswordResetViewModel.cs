
namespace DebReg.Web.Models {
    using System;
    using System.ComponentModel.DataAnnotations;

    public class PasswordResetViewModel {

        public String UserId { get; set; }
        public String Token { get; set; }

        [Display(Name = "NewPassword", ResourceType = typeof(Resources.Models.PasswordChangeRequest.Strings))]
        [Required]
        public string NewPassword { get; set; }

        [Display(Name = "ConfirmPassword", ResourceType = typeof(Resources.Models.PasswordChangeRequest.Strings))]
        [Required]
        public string ConfirmPassword { get; set; }

    }
}