using DebReg.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DebReg.Web.Models
{
    public class UserViewModel
    {
        public User User { get; set; }
        public List<UserPropertyValueViewModel> UserProperties { get; set; }
        public Tournament Tournament { get; set; }
        public Organization Organization { get; set; }

        public UserViewModel()
        {
            UserProperties = new List<UserPropertyValueViewModel>();
        }

        public UserViewModel(User user)
            : this()
        {
            User = user;
            if (user != null)
            {
                foreach (var value in user.PropertyValues)
                {
                    UserProperties.Add(new UserPropertyValueViewModel(value));
                }
            }
        }

        public UserViewModel(User user, Guid tournamentId, Organization organization)
            : this(user, tournamentId)
        {
            Organization = organization;
        }

        public UserViewModel(User user, Guid tournamentId)
            : this(user)
        {
            if (User != null)
            {
                var tournamentPropertyValues = user.TournamentPropertyValues.Where(v => v.TournamentId == tournamentId);
                foreach (var value in tournamentPropertyValues)
                {
                    UserProperties.Add(new UserPropertyValueViewModel(value.ToUserPropertyValue()));
                }
            }
        }
    }
}