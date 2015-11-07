using System;

namespace DebReg.Web.APIModels
{
    public class Address
    {
        public Guid id { get; set; }
        public String streetAddress1 { get; set; }
        public String streetAddress2 { get; set; }
        public String postalCode { get; set; }
        public String city { get; set; }
        public String region { get; set; }
        public String country { get; set; }

        public Address()
        {

        }

        public Address(DebReg.Models.Address address)
            : this()
        {
            if (address == null)
            {
                return;
            }
            this.id = address.Id;
            this.streetAddress1 = address.StreetAddress1;
            this.streetAddress2 = address.StreetAddress2;
            this.postalCode = address.PostalCode;
            this.city = address.City;
            this.region = address.Region;
            this.country = address.Country;
        }
    }
}