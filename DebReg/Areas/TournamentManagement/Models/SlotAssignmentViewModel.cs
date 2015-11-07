using DebReg.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Web.Areas.TournamentManagement.Models {
    public class SlotAssignmentViewModel : IValidatableObject {
        public TournamentOrganizationRegistration Registration { get; set; }
        public SlotAssignment Assignment { get; set; }


        #region IValidatableObject Members

        public System.Collections.Generic.IEnumerable<ValidationResult> Validate(ValidationContext validationContext) {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            List<String> memberNames;
            if (Assignment != null
                && Registration != null
                && Assignment.AdjucatorsGranted < Registration.AdjudicatorsPaid) {

                memberNames = new List<String>();
                memberNames.Add("Assignment.AdjucatorsGranted");
                validationResults.Add(new ValidationResult(
                    Resources.TournamentManagement.Models.SlotAssignmentViewModel.Strings.AdjudicatorsAssignedBelowAdjudicatorsPaidErrorMessage,
                    memberNames));
            }

            if (Assignment != null
                && Registration != null
                && Assignment.TeamsGranted < Registration.TeamsPaid) {

                memberNames = new List<string>();
                memberNames.Add("Assignment.TeamsGranted");
                validationResults.Add(new ValidationResult(
                    Resources.TournamentManagement.Models.SlotAssignmentViewModel.Strings.TeamsAssignedBelowTeamsPaidErrorMessage,
                    memberNames));
            }

            return validationResults;
        }

        #endregion
    }
}