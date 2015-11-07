using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Web.Areas.TournamentManagement.Models {
    public class PublishSlotAssignmentsViewModel {
        [Required]
        [Display(Name = "PaymentsDueDate", ResourceType = typeof(Resources.TournamentManagement.Models.PublishSlotAssignmentsViewModel.Strings))]
        public DateTime? PaymentsDueDate { get; set; }
        public IEnumerable<SlotAssignmentViewModel> SlotAssignments { get; set; }

        public Boolean Publish { get; set; }
    }
}