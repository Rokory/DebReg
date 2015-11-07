using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DebReg.Models
{
    public class Tournament : TrackableEntity
    {
        [HiddenInput]
        public Guid Id { get; set; }

        [HiddenInput]
        [ForeignKey("HostingOrganization")]
        public Guid HostingOrganizationID { get; set; }

        [Display(Name = "HostingOrganization", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public virtual Organization HostingOrganization { get; set; }

        [Display(Name = "Name", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        [MaxLength(70)]
        public string Name { get; set; }

        [Display(Name = "Location", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        [MaxLength(70)]
        public string Location { get; set; }

        [Display(Name = "Start", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        [DataType(DataType.Date)]
        public DateTime Start { get; set; }

        [Display(Name = "End", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        [DataType(DataType.Date)]
        public DateTime End { get; set; }

        [Display(Name = "RegistrationStart", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public DateTime? RegistrationStart { get; set; }

        [Display(Name = "RegistrationEnd", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public DateTime? RegistrationEnd { get; set; }

        [Display(Name = "Adjudicator policy (n - x, where n is number of teams and x is this value")]
        public int? AdjucatorSubtract { get; set; }

        [Display(Name = "TeamSize", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public int TeamSize { get; set; }

        [Display(Name = "TeamCap", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public int TeamCap { get; set; }

        [Display(Name = "AdjudicatorCap", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public int AdjudicatorCap { get; set; }

        public virtual List<Product> Products { get; set; }

        [ForeignKey("TeamProduct")]
        public Guid? TeamProductId { get; set; }

        public virtual Product TeamProduct { get; set; }

        [ForeignKey("AdjudicatorProduct")]
        public Guid? AdjudicatorProductId { get; set; }

        public virtual Product AdjudicatorProduct { get; set; }

        [Display(Name = "UniversityRequired", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public bool UniversityRequired { get; set; }

        public virtual List<TournamentOrganizationRegistration> Registrations { get; set; }

        [Display(Name = "ChiefAdjudicators", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public virtual List<TournamentUserRole> UserRoles { get; set; }

        [ForeignKey("Currency")]
        public Guid? CurrencyId { get; set; }

        public virtual Currency Currency { get; set; }

        [DataType(DataType.EmailAddress)]
        public String FinanceEMail { get; set; }

        [MaxLength(1500)]
        public String TermsConditions { get; set; }

        [MaxLength(1500)]
        [Display(Name = "TermsConditionsLink", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public String TermsConditionsLink { get; set; }

        [MaxLength(1500)]
        public String MoneyTransferLinkCaption { get; set; }

        [MaxLength(1500)]
        public String MoneyTransferLink { get; set; }

        [MaxLength(255)]
        [Display(Name = "PaymentReference", ResourceType = typeof(Resources.Models.Tournament.Strings))]
        public String PaymentReference { get; set; }

        [ForeignKey("BankAccount")]
        public Guid? BankAccountId { get; set; }

        public virtual BankAccount BankAccount { get; set; }

        public virtual List<Team> Teams { get; set; }

        public virtual List<Adjudicator> Adjudicators { get; set; }

        public Boolean FixedTeamNames { get; set; }

        public Tournament()
        {
            Registrations = new List<TournamentOrganizationRegistration>();
            UserRoles = new List<TournamentUserRole>();
            Products = new List<Product>();
            Teams = new List<Team>();
            Adjudicators = new List<Adjudicator>();
        }

    }


}