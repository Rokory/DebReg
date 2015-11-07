using DebReg.Models;
using System.Collections.Generic;

namespace DebRegComponents
{
    public interface ICountryManager
    {
        IEnumerable<Country> GetCountries();
    }
}
