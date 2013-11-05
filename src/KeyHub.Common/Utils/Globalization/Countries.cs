using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyHub.Common.Utils.Globalization
{
    /// <summary>
    /// Provides access to common country related tasks
    /// </summary>
    public static class Countries
    {
        /// <summary>
        /// Information about a country
        /// </summary>
        public class CountryInfo
        {
            /// <summary>
            /// Unique country code to identify the country
            /// </summary>
            public string CountryCode { get; set; }

            /// <summary>
            /// English speaking country name
            /// </summary>
            public string CountryName { get; set; }

            /// <summary>
            /// Native country name
            /// </summary>
            public string NativeCountryName { get; set; }
        }

        /// <summary>
        /// Gets a list of available countries
        /// </summary>
        /// <returns>
        /// A dictionary with countries using the two-letter country indentifier as key,
        /// and the English name as value
        /// </returns>
        public static IEnumerable<CountryInfo> GetAllCountries()
        {
            Dictionary<string, CountryInfo> countryDictionary = new Dictionary<string, CountryInfo>();

            // Get all specific cultures from the Framework
            foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
            {
                // Add the region information from this culture to the country list, if not already present
                RegionInfo regionInfo = new RegionInfo(cultureInfo.Name);
                string countryCode = regionInfo.TwoLetterISORegionName.ToLower();
                if (!countryDictionary.ContainsKey(countryCode))
                {
                    countryDictionary.Add(countryCode, new CountryInfo()
                    {
                        CountryCode = countryCode,
                        CountryName = regionInfo.EnglishName,
                        NativeCountryName = regionInfo.NativeName
                    });
                }
            }
            return countryDictionary.Values;
        }
    }
}