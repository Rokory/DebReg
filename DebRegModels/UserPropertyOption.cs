using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace DebReg.Models
{
    public class UserPropertyOption
    {
        public Guid Id { get; set; }
        public String Name { get; set; }

        public int Order { get; set; }

        [ForeignKey("UserProperty")]
        public Guid UserPropertyId { get; set; }
        public UserProperty UserProperty { get; set; }
    }
}
