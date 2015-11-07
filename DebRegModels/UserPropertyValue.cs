using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models
{
    public class UserPropertyValue
    {
        //private char[] separators = new char[] { ';' };

        //List<String> memberNames = new List<string> { "Value" };

        [ForeignKey("User")]
        [Key, Column(Order = 0)]
        public String UserId { get; set; }

        public virtual User User { get; set; }

        [ForeignKey("UserProperty")]
        [Key, Column(Order = 1)]
        public Guid UserPropertyId { get; set; }

        public virtual UserProperty UserProperty { get; set; }

        public String Value { get; set; }
    }
}
