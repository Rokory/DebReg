using System.Collections.Generic;

namespace DebReg.Models.Comparers
{
    public class TournamentComparer : IEqualityComparer<Tournament>
    {
        #region IEqualityComparer<Tournament> Members

        public bool Equals(Tournament x, Tournament y)
        {
            return x.Id == y.Id;
        }

        public int GetHashCode(Tournament obj)
        {
            return obj.Id.GetHashCode();
        }

        #endregion
    }
}
