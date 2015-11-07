using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;

namespace DebReg.Models
{
    public class Organization : TrackableEntity, IValidatableObject
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [MaxLength(70), MinLength(3)]
        [Required]
        [Display(Name = "Name", ResourceType = typeof(Resources.Models.Organization.Strings))]
        public string Name { get; set; }

        // [MaxLength(10), MinLength(3)]
        // [RegularExpression("^[a-zA-Z][a-zA-Z0-9 -_]{2,9}$", ErrorMessageResourceName = "ErrorAbbreviationRegEx", ErrorMessageResourceType = typeof(Resources.Models.Organization.Strings))]
        [Display(Name = "Abbreviation", ResourceType = typeof(Resources.Models.Organization.Strings))]
        public string Abbreviation { get; set; }

        [Display(Name = "IsUniversity", ResourceType = typeof(Resources.Models.Organization.Strings))]
        public bool University { get; set; }

        // A university can have a linked organization, e. g. a debating society
        // with a different name and address.
        [HiddenInput]
        [ForeignKey("LinkedOrganization")]
        public Guid? LinkedOrganizationId { get; set; }

        [Display(Name = "LinkedOrganization", ResourceType = typeof(Resources.Models.Organization.Strings))]
        public virtual Organization LinkedOrganization { get; set; }

        [Display(Name = "LinkedOrganizations", ResourceType = typeof(Resources.Models.Organization.Strings))]
        public virtual List<Organization> LinkedOrganizations { get; set; }

        [HiddenInput]
        [ForeignKey("Address")]
        public Guid? AddressId { get; set; }

        public virtual Address Address { get; set; }

        [Display(Name = "VATId", ResourceType = typeof(Resources.Models.Organization.Strings))]
        [MaxLength(14)]
        public string VatId { get; set; }

        public Boolean Universitary
        {
            get
            {
                if (University
                    || (
                        LinkedOrganization != null
                        && LinkedOrganization.Universitary
                    ))
                {

                    return true;
                }
                var linkedUniversities = LinkedOrganizations.FirstOrDefault(
                    o => o.University);
                return linkedUniversities != null;
            }
        }

        public virtual List<OrganizationUser> UserAssociations { get; set; }

        public OrganizationStatus Status { get; set; }


        [Display(Name = "TournamentRegistrations", ResourceType = typeof(Resources.Models.Organization.Strings))]
        public virtual List<TournamentOrganizationRegistration> TournamentRegistrations { get; set; }

        public virtual SMTPHostConfiguration SMTPHostConfiguration { get; set; }

        public Organization()
        {
            UserAssociations = new List<OrganizationUser>();
            TournamentRegistrations = new List<TournamentOrganizationRegistration>();
            LinkedOrganizations = new List<Organization>();
            Name = String.Empty;
            Abbreviation = String.Empty;
        }

        #region IValidatableObject Members

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResults = new List<ValidationResult>();
            List<String> memberNames;

            // If this is a root orgnization, it needs an abbreviation

            if (((LinkedOrganizationId == Guid.Empty || LinkedOrganizationId == null)
                && !Deleted))
            {
                memberNames = new List<string>();
                if (String.IsNullOrEmpty(Abbreviation))
                {

                    memberNames.Add("Abbreviation");
                    validationResults.Add(new ValidationResult(
                        Resources.Models.Organization.Strings.ErrorAbbreviationRequired,
                        memberNames));
                }
                else if (!Regex.IsMatch(Abbreviation, @"^[a-zA-Z][a-zA-Z0-9 -_]{2,9}$"))
                {
                    memberNames.Add("Abbreviation");
                    validationResults.Add(new ValidationResult(
                        Resources.Models.Organization.Strings.ErrorAbbreviationRegEx,
                        memberNames));

                }
            }

            return validationResults;
        }

        #endregion
    }

    public enum OrganizationStatus
    {
        Unknown,
        Approved,
        Dropped
    }
}