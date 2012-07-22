using System;
using System.Collections.Generic;

namespace KeyHub.Common.Utils.Linq
{
    /// <summary>
    /// Compares two string instances in a Lambda expression using case insensitive comparrison
    /// </summary>
    public class CaseInsensitiveStringComparer : IEqualityComparer<string>
    {
        /// <summary>
        /// Determines whether the specified objects are equal.
        /// </summary>
        /// <param name="x">The first string to compare.</param>
        /// <param name="y">The second string to compare</param>
        /// <returns>true if the specified objects are equal; otherwise, false</returns>
        public bool Equals(string x, string y)
        {
            return string.Compare(x, y, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        /// <summary>
        /// Returns a hash code for the specified object.
        /// </summary>
        /// <param name="obj">The string for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        public int GetHashCode(string obj)
        {
            return obj.GetHashCode();
        }
    }
}