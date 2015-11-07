using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Web.Areas.TournamentManagement.Models {
    public class PostPaymentsViewModel {
        [Display(Name = "SearchTerm", ResourceType = typeof(Resources.TournamentManagement.Models.PostPaymentsViewModel.Strings))]
        public String SearchTerm { get; set; }

        [Display(Name = "Date", ResourceType = typeof(Resources.TournamentManagement.Models.PostPaymentsViewModel.Strings))]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Value", ResourceType = typeof(Resources.TournamentManagement.Models.PostPaymentsViewModel.Strings))]
        public Decimal Value { get; set; }

        [Display(Name = "Note", ResourceType = typeof(Resources.TournamentManagement.Models.PostPaymentsViewModel.Strings))]
        [MaxLength(1500)]
        public String Note { get; set; }

        public List<PaymentViewModel> Payments { get; set; }

        public PostPaymentsViewModel() {
            Payments = new List<PaymentViewModel>();
        }
    }
}