using System;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Web.Areas.TournamentManagement.Models
{
    public class RegistrationFinancialViewModel
    {
        public Guid TournamentId { get; set; }
        public Guid OrganizationId { get; set; }
        public String OrganizationName { get; set; }
        public String BookingCode { get; set; }
        public int TeamsGranted { get; set; }
        public int TeamsPaid { get; set; }
        public int AdjudicatorsGranted { get; set; }
        public int AdjudicatorsPaid { get; set; }

        [Display(Name = "Balance", ResourceType = typeof(Resources.TournamentManagement.Models.RegistrationFinancialViewModel.Strings))]
        public Decimal Balance { get; set; }
    }
}