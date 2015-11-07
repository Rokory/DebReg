using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models
{
    public class TournamentOrganizationRegistration : TrackableEntity, IValidatableObject
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Tournament")]
        public Guid TournamentId { get; set; }

        public virtual Tournament Tournament { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }

        [MaxLength(6)]
        [Index]
        public String BookingCode { get; set; }

        public virtual Organization Organization { get; set; }

        [ForeignKey("BilledOrganization")]
        [Required(ErrorMessageResourceName = "ErrorBilledOrganizationMissing", ErrorMessageResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        public Guid? BilledOrganizationId { get; set; }

        [Display(Name = "BilledOrganization", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        public virtual Organization BilledOrganization { get; set; }

        [Display(Name = "TeamsWanted", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        [Range(0, int.MaxValue)]
        public int TeamsWanted { get; set; }

        [Display(Name = "AdjucatorsWanted", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        [Range(0, int.MaxValue)]
        public int AdjudicatorsWanted { get; set; }

        [Display(Name = "TeamsGranted", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        [Range(0, int.MaxValue)]
        public int TeamsGranted { get; set; }


        [Display(Name = "AdjucatorsGranted", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        [Range(0, int.MaxValue)]
        public int AdjudicatorsGranted { get; set; }

        [Display(Name = "TeamsPaid", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        [Range(0, int.MaxValue)]
        public int TeamsPaid { get; set; }

        [Display(Name = "AdjucatorsPaid", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        [Range(0, int.MaxValue)]
        public int AdjudicatorsPaid { get; set; }

        [Column(TypeName = "text")]
        [DataType(DataType.MultilineText)]
        [MaxLength(1500)]
        [Display(Name = "Notes", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        public string Notes { get; set; }

        [Display(Name = "OrganizationStatus", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]

        public OrganizationStatus OrganizationStatus { get; set; }

        [Display(Name = "OrganizationStatusDraft", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        public Boolean OrganizationStatusDraft { get; set; }

        [MaxLength(1500)]
        [DataType(DataType.MultilineText)]
        [Column(TypeName = "text")]
        [Display(Name = "OrganizationStatusNote", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        public String OrganizationStatusNote { get; set; }

        [Display(Name = "Rank", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        public Decimal Rank { get; set; }

        public int RandomRank { get; set; }

        [Display(Name = "LockAutoAssign", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistration.Strings))]
        public Boolean LockAutoAssign { get; set; }

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            if (OrganizationStatus == OrganizationStatus.Dropped
                && (
                    String.IsNullOrWhiteSpace(OrganizationStatusNote)
                    || OrganizationStatusNote.Length < 4
                ))
            {

                var memberNames = new List<String>();
                memberNames.Add("OrganizationStatusNote");
                validationResults.Add(new ValidationResult(
                    Resources.Models.TournamentOrganizationRegistration.Strings.ErrorOrganizationStatusNoteMissing,
                    memberNames));
            }
            return validationResults;
        }

        #endregion
    }


}