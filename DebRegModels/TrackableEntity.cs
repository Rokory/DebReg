using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace DebReg.Models
{
    public abstract class TrackableEntity : DeleteableEntity
    {
        [HiddenInput(DisplayValue = false)]
        [ForeignKey("CreatedBy")]
        public String CreatedById { get; set; }

        [HiddenInput(DisplayValue = false)]
        [Display(Name = "Created by")]
        public virtual User CreatedBy { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime? Created { get; set; }

        [HiddenInput(DisplayValue = false)]
        [ForeignKey("ModifiedBy")]
        public String ModifiedById { get; set; }

        [HiddenInput(DisplayValue = false)]
        [Display(Name = "Modified by")]
        public virtual User ModifiedBy { get; set; }

        [HiddenInput(DisplayValue = false)]
        public DateTime? Modified { get; set; }

        public void UpdateTrackingData(User user)
        {
            if (Created == null)
            {
                Created = DateTime.UtcNow;
            }
            if (CreatedBy == null)
            {
                CreatedBy = user;
            }
            if (String.IsNullOrWhiteSpace(CreatedById))
            {
                CreatedById = CreatedBy.Id;
            }
            Modified = DateTime.UtcNow;
            ModifiedBy = user;
            if (user != null)
            {
                ModifiedById = user.Id;
            }
        }
    }
}