using System.Collections.Generic;

namespace DebReg.Models.Comparers {
    public class TournamentUserRoleTournamentComparer : IEqualityComparer<TournamentUserRole> {

        #region IEqualityComparer<TournamentUserRole> Members

        public bool Equals(TournamentUserRole x, TournamentUserRole y) {
            return x.TournamentId == y.TournamentId;
        }

        public int GetHashCode(TournamentUserRole obj) {
            return obj.TournamentId.GetHashCode();
        }

        #endregion
    }
}
