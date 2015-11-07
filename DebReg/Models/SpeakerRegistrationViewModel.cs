using DebReg.Models;
using System;

namespace DebReg.Web.Models
{
    public class SpeakerRegistrationViewModel
    {
        public Boolean PersonalDataComplete { get; set; }
        public Team Team { get; set; }
    }
}