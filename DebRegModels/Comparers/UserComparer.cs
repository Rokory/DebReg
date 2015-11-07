using System.Collections.Generic;

namespace DebReg.Models.Comparers {
    public class UserComparer : IEqualityComparer<User> {
        #region IEqualityComparer<User> Members

        public bool Equals(User x, User y) {
            return x.Id == y.Id;
        }

        public int GetHashCode(User obj) {
            return obj.Id.GetHashCode();
        }

        #endregion
    }
}
