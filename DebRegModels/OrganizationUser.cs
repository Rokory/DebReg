using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models
{
    public class OrganizationUser : TrackableEntity
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey("User")]
        public String UserId { get; set; }

        public virtual User User { get; set; }

        [Key, Column(Order = 2)]
        [Display(Name = "Role", ResourceType = typeof(Resources.Models.OrganizationUser.Strings))]
        public OrganizationRole Role { get; set; }

    }

    public enum OrganizationRole
    {
        None = 0,
        Member = 1,
        Delegate = 2,
        OrganizationTournamentManager = 3
    }
}