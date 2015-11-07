using System;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Web.Areas.TournamentManagement.Models {
    public class PaymentViewModel {
        [Display(Name = "Balance", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        public Decimal Balance { get; set; }

        [Display(Name = "Date", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }

        [Display(Name = "Value", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        public Decimal Value { get; set; }

        [Display(Name = "Note", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        [MaxLength(1500)]
        public String Note { get; set; }

        public Guid OrganizationId { get; set; }

        [Display(Name = "OrganizationName", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        public String OrganizationName { get; set; }


        [Display(Name = "TeamsGranted", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        public int TeamsGranted { get; set; }

        [Display(Name = "TeamsPaidOld", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        public int TeamsPaidOld { get; set; }

        [Display(Name = "TeamsPaid", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        public int TeamsPaid { get; set; }

        [Display(Name = "AdjudicatorsGranted", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        public int AdjudicatorsGranted { get; set; }

        [Display(Name = "AdjudicatorsPaidOld", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        public int AdjudicatorsPaidOld { get; set; }

        [Display(Name = "AdjudicatorsPaid", ResourceType = typeof(Resources.TournamentManagement.Models.PaymentViewModel.Strings))]
        public int AdjudicatorsPaid { get; set; }

        public PaymentViewModel() {

        }

    }
}