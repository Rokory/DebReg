using DebReg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DebReg.Web.Areas.TournamentManagement.Models
{
    public class MissingPersonalDataViewModel
    {
        public User User { get; set; }
        public Organization Organization { get; set; }
    }
}