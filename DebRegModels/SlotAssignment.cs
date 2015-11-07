using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models
{
    public class SlotAssignment : TrackableEntity
    {
        [Key, Column(Order = 1)]
        [ForeignKey("Tournament")]
        public Guid TournamentId { get; set; }

        public virtual Tournament Tournament { get; set; }

        [Key, Column(Order = 2)]
        [ForeignKey("Organization")]
        public Guid OrganizationId { get; set; }

        public virtual Organization Organization { get; set; }

        [Key, Column(Order = 3)]
        [ForeignKey("Version")]
        public Guid VersionId { get; set; }

        public virtual Version Version { get; set; }

        [Range(0, int.MaxValue)]
        public int TeamsGranted { get; set; }

        [Range(0, int.MaxValue)]
        public int AdjucatorsGranted { get; set; }

        public void CopyTo(SlotAssignment target)
        {
            target.AdjucatorsGranted = this.AdjucatorsGranted;
            target.Created = this.Created;
            target.CreatedBy = this.CreatedBy;
            target.CreatedById = this.CreatedById;
            target.Modified = this.Modified;
            target.ModifiedBy = this.ModifiedBy;
            target.ModifiedById = this.ModifiedById;
            target.Organization = this.Organization;
            target.OrganizationId = this.OrganizationId;
            target.TeamsGranted = this.TeamsGranted;
            target.Tournament = this.Tournament;
            target.TournamentId = this.TournamentId;
            target.Version = this.Version;
            target.VersionId = this.VersionId;
        }

        public SlotAssignment Clone()
        {
            SlotAssignment result = new SlotAssignment();
            CopyTo(result);
            return result;
        }
    }
}
