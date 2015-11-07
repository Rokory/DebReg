using DebReg.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace DebReg.Web.Models
{
    public class TournamentOrganizationRegistrationViewModel
    {
        public TournamentOrganizationRegistration Registration { get; set; }
        public List<APIModels.Team> Teams { get; set; }
        public List<APIModels.Adjudicator> Adjudicators { get; set; }

        public List<User> IncompleteUsers { get; set; }
        public int TeamsCompleted { get; set; }
        public int AdjudicatorsCompleted { get; set; }

        [Display(Name = "Balance", ResourceType = typeof(Resources.Models.TournamentOrganizationRegistrationViewModel.Strings))]
        public Decimal Balance
        {
            get
            {
                return Bookings.Sum(b => b.Value * (b.Credit ? 1 : -1));
            }
        }

        public IEnumerable<BookingRecord> Bookings { get; set; }

        public TournamentOrganizationRegistrationViewModel()
        {
            Teams = new List<APIModels.Team>();
            Adjudicators = new List<DebReg.Web.APIModels.Adjudicator>();
            Bookings = new List<BookingRecord>();
        }
    }
}