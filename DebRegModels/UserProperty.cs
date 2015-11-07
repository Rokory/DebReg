using System;
using System.Collections.Generic;

namespace DebReg.Models
{
    public class UserProperty
    {
        public Guid Id { get; set; }
        public int Order { get; set; }
        public String Name { get; set; }
        public String Description { get; set; }

        public PropertyType Type { get; set; }
        public Decimal Min { get; set; }
        public Decimal Max { get; set; }
        public virtual List<UserPropertyOption> Options { get; set; }
        public Boolean Required { get; set; }
        public Boolean TournamentSpecific { get; set; }
    }

    public enum PropertyType
    {
        Boolean = 0,
        Int = 1,
        Decimal = 2,
        Date = 3,
        String = 4,
        Text = 5,
        PhoneNumber = 6,
        EMail = 7,
        SingleSelect = 8,
        // MultiSelect = 9,
        SinglePerson = 10,
        //MultiplePersons = 11,
        Country = 12
    }
}
