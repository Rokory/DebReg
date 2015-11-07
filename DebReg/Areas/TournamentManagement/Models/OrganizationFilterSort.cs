using DebReg.Models;
using System;
using System.Collections.Generic;

namespace DebReg.Web.Areas.TournamentManagement.Models {
    public class OrganizationFilterSort {
        public SortField Orderby { get; set; }

        public Boolean Descending { get; set; }
        public Boolean? Universitary { get; set; }
        public IEnumerable<OrganizationStatus> Status { get; set; }
        public IEnumerable<String> Country { get; set; }
        public IEnumerable<String> Region { get; set; }
        public IEnumerable<String> City { get; set; }

        public OrganizationFilterSort() {
            Status = new List<OrganizationStatus>();
            Country = new List<String>();
            Region = new List<String>();
            City = new List<String>();
        }
    }

    public enum SortField {
        Name,
        Abbreviation,
        Universitary,
        Status,
        Country,
        Region,
        City,
        Draft
    }
}