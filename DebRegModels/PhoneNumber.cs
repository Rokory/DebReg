
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
namespace DebReg.Models {
    public class PhoneNumber : TrackableEntity {
        [HiddenInput]
        public Guid Id { get; set; }

        [Display(Name = "Country Code")]
        public string CountryCode { get; set; }
        public string Number { get; set; }
    }
}