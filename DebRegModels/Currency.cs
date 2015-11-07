using System;

namespace DebReg.Models {
    public class Currency : TrackableEntity {
        public Guid Id { get; set; }
        public String Name { get; set; }
        public String Symbol { get; set; }
    }
}
