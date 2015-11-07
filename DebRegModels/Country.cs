using System;
using System.ComponentModel.DataAnnotations;

namespace DebReg.Models
{
    public class Country
    {
        public Guid Id { get; set; }
        public String ShortName { get; set; }

        [MaxLength(2)]
        public String Alpha2 { get; set; }

        [MaxLength(3)]
        public String Alpha3 { get; set; }

        public Int16 NumericCode { get; set; }
    }
}
