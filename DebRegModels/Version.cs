using System;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Models {
    public class Version {
        public Guid Id { get; set; }

        [Display(Name = "Number", ResourceType = typeof(Resources.Models.Version.Strings))]
        public int Number { get; set; }

        [Display(Name = "Status", ResourceType = typeof(Resources.Models.Version.Strings))]
        public VersionStatus Status { get; set; }
    }

    public enum VersionStatus {
        Draft,
        Public,
        Outdated
    }
}
