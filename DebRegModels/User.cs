using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;


namespace DebReg.Models
{
    public class User : IdentityUser
    {

        [Display(Name = "FirstName", ResourceType = typeof(Resources.Models.User.Strings))]
        [Required]
        [MaxLength(70)]
        public string FirstName { get; set; }

        [Display(Name = "LastName", ResourceType = typeof(Resources.Models.User.Strings))]
        [Required]
        [MaxLength(70)]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "EMail", ResourceType = typeof(Resources.Models.User.Strings))]
        [EmailAddress]
        [RegularExpression(@"^[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?$", ErrorMessageResourceName = "ErrorEMailRegEx", ErrorMessageResourceType = typeof(Resources.Models.Strings))]
        [MaxLength(255)]
        public override string Email
        {
            get
            {
                return base.Email;
            }
            set
            {
                base.Email = value;
            }
        }


        [MaxLength(255)]
        [EmailAddress]
        [Display(Name = "NewEMail", ResourceType = typeof(Resources.Models.User.Strings))]
        public String NewEMail { get; set; }


        [Display(Name = "Phone", ResourceType = typeof(Resources.Models.User.Strings))]
        [MaxLength(16), MinLength(7)]
        [RegularExpression(@"^[\+][0-9]{2,3}([ -]?\d+)+$", ErrorMessageResourceName = "ErrorPhoneNumberRegEx", ErrorMessageResourceType = typeof(Resources.Models.User.Strings))]
        public override string PhoneNumber
        {
            get
            {
                return base.PhoneNumber;
            }
            set
            {
                base.PhoneNumber = value;
            }
        }

        [HiddenInput]
        [ForeignKey("SponsoringOrganization")]
        public Guid SponsoringOrganizationId { get; set; }
        public virtual Organization SponsoringOrganization { get; set; }

        public virtual List<OrganizationUser> OrganizationAssociations { get; set; }


        [HiddenInput]
        [ForeignKey("CurrentOrganization")]
        public Guid? CurrentOrganizationId { get; set; }

        [Display(Name = "CurrentOrganization", ResourceType = typeof(Resources.Models.User.Strings))]
        public virtual Organization CurrentOrganization { get; set; }

        [HiddenInput]
        [ForeignKey("CurrentTournament")]
        public Guid? CurrentTournamentId { get; set; }

        [Display(Name = "CurrentTournament", ResourceType = typeof(Resources.Models.User.Strings))]
        public virtual Tournament CurrentTournament { get; set; }

        [HiddenInput]
        [Display(Name = "Password change required")]
        public bool PasswordChangeRequired { get; set; }

        [Display(Name = "TournamentRoles", ResourceType = typeof(Resources.Models.User.Strings))]
        public virtual List<TournamentUserRole> TournamentRoles { get; set; }

        [HiddenInput]
        [ForeignKey("LastSMTPErrorHostConfiguration")]
        public Guid? LastSMTPErrorHostConfigurationId { get; set; }
        public virtual SMTPHostConfiguration LastSMTPErrorHostConfiguration { get; set; }

        public virtual List<Team> Teams { get; set; }

        public virtual List<Adjudicator> Adjudicator { get; set; }

        public virtual List<UserPropertyValue> PropertyValues { get; set; }
        public virtual List<UserTournamentPropertyValue> TournamentPropertyValues { get; set; }

        public User()
        {
            OrganizationAssociations = new List<OrganizationUser>();
            TournamentRoles = new List<TournamentUserRole>();
            PropertyValues = new List<UserPropertyValue>();
            TournamentPropertyValues = new List<UserTournamentPropertyValue>();
            Teams = new List<Team>();
        }
    }
}

