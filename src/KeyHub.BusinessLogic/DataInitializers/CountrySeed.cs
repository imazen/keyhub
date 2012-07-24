using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KeyHub.Runtime;

namespace KeyHub.BusinessLogic.DataInitializers
{
    /// <summary>
    /// Prefills the country table with all countries from the .NET Framework
    /// </summary>
    internal class CountrySeed
    {
        internal static void Seed(DataContext context)
        {
            // Get all countries from the framework and insert them into the table
            foreach (var country in Common.Utils.Globalization.Countries.GetAllCountries())
            {
                context.Countries.Add(new Model.Country()
                {
                    CountryCode = country.CountryCode,
                    CountryName = country.CountryName,
                    NativeCountryName = country.NativeCountryName
                });
            }
        }
    }
}