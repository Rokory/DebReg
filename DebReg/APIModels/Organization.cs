using System;
using System.Collections.Generic;
using System.Linq;

namespace DebReg.Web.APIModels
{
    public class Organization
    {
        public Organization(DebReg.Models.Organization organization)
            : base()
        {
            this.id = organization.Id;
            this.name = organization.Name;
            this.abbreviation = organization.Abbreviation;
            this.university = organization.University;
            this.address = new Address(organization.Address);
            this.linkedOrganizations = organization.LinkedOrganizations
                .Select(o => new Organization(o))
                .ToList();
        }

        public Organization()
        {
            linkedOrganizations = new List<Organization>();
        }

        public Guid id { get; set; }

        public string name { get; set; }

        public string abbreviation { get; set; }

        public bool university { get; set; }
        public Address address { get; set; }

        public List<Organization> linkedOrganizations { get; set; }
    }
}