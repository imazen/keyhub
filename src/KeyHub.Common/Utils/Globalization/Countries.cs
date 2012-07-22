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
        /// Gets a list of available countries
        /// </summary>
        /// <returns>
        /// A dictionary with countries using the two-letter country indentifier as key,
        /// and the English name as value
        /// </returns>
        public Dictionary<string, string> GetAllCountries()
        {
            Dictionary<string, string> countryDictionary = new Dictionary<string, string>();

            // Get all specific cultures from the Framework
            foreach (CultureInfo cultureInfo in CultureInfo.GetCultures(System.Globalization.CultureTypes.SpecificCultures))
            {
                // Add the region information from this culture to the country list, if not already present
                RegionInfo regionInfo = new RegionInfo(cultureInfo.Name);
                if (!countryDictionary.ContainsKey(regionInfo.EnglishName))
                {
                    countryDictionary.Add(regionInfo.EnglishName, regionInfo.TwoLetterISORegionName.ToLower());
                }
            }

            return countryDictionary;
        }
    }
}