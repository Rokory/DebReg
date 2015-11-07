using DebReg.Models;
using System;

namespace DebReg.Web.Models
{
    public class AdjudicatorRegistrationViewModel
    {
        public Boolean PersonalDataComplete { get; set; }
        public Adjudicator Adjudicator { get; set; }
    }
}