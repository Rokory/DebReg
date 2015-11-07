using DebReg.Models;
using System.Collections.Generic;

namespace DebReg.Web.Areas.TournamentManagement.Models
{
    public class OrganizationAttendeesViewModel
    {
        public TournamentOrganizationRegistration Registration { get; set; }
        public List<Team> Teams { get; set; }
        public List<Adjudicator> Adjudicators { get; set; }

        public List<User> Delegates { get; set; }

        public OrganizationAttendeesViewModel()
        {
            Teams = new List<Team>();
            Adjudicators = new List<Adjudicator>();
            Delegates = new List<User>();
        }
    }
}