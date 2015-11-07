using DebReg.Data;
using DebReg.Models;
using System.Collections.Generic;

namespace DebRegComponents
{
    public class CountryManager : BaseManager, ICountryManager
    {

        #region ICountryManager Members

        public IEnumerable<Country> GetCountries()
        {
            return unitOfWork.GetRepository<Country>().Get();
        }

        #endregion

        public CountryManager(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
