using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DebReg.Models
{
    public class Address : TrackableEntity
    {
        [HiddenInput(DisplayValue = false)]
        public Guid Id { get; set; }

        [Display(Name = "StreetAddress1", ResourceType = typeof(Resources.Models.Address.Strings))]
        [MaxLength(70)]
        public string StreetAddress1 { get; set; }

        [Display(Name = "StreetAddress2", ResourceType = typeof(Resources.Models.Address.Strings))]
        [MaxLength(70)]
        public string StreetAddress2 { get; set; }

        [Display(Name = "PostalCode", ResourceType = typeof(Resources.Models.Address.Strings))]
        [MaxLength(9)]
        public string PostalCode { get; set; }

        [Display(Name = "City", ResourceType = typeof(Resources.Models.Address.Strings))]
        [MaxLength(70)]
        public string City { get; set; }

        [Display(Name = "Region", ResourceType = typeof(Resources.Models.Address.Strings))]
        [MaxLength(70)]
        public string Region { get; set; }

        [Display(Name = "Country", ResourceType = typeof(Resources.Models.Address.Strings))]
        [MaxLength(70)]
        public string Country { get; set; }

        [ForeignKey("Country2")]
        public Guid? CountryId { get; set; }
        public Country Country2 { get; set; }
    }
}