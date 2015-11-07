using DebReg.Models;
using DebReg.Web.Models;
using System.Collections.Generic;

namespace DebReg.Web.Areas.TournamentManagement.Models
{
    public class AttendeeListViewModel
    {
        public List<UserProperty> UserProperties { get; set; }
        public List<UserViewModel> Users { get; set; }

        public AttendeeListViewModel()
        {
            Users = new List<UserViewModel>();
            UserProperties = new List<UserProperty>();
        }
    }
}