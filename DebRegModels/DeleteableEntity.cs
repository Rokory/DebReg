using System;
using System.Web.Mvc;

namespace DebReg.Models {
    public abstract class DeleteableEntity {
        [HiddenInput(DisplayValue = false)]
        public Boolean Deleted { get; set; }
    }
}
