using DebReg.Models;
using System;
using System.Collections.Generic;

namespace DebReg.Web.Models {
    public class FindUserViewModel {
        public String SearchTerm { get; set; }
        public List<User> Results { get; set; }

        public Boolean DisplayNewUserLink { get; set; }

        public String SelectedUserId { get; set; }

        public FindUserViewModel() {
            Results = new List<User>();
        }

    }
}