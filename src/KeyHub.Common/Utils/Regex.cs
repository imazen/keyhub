using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace KeyHub.Common.Utils
{
    public static class RegexUtils
    {
        public const string REGEX_EMAIL = @"([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,8}|[0-9]{1,8})(\]?)";

        //Matches a positive or negative decimal value with any precision and scale (whole number or decimal).
        //Allows for left-padded zeroes, commas as group separator, negative sign (-) or parenthesis to indicate negative number.
        public const string REGEX_NUMERIC = @"^\-?\(?([0-9]{0,3}(\,?[0-9]{3})*(\.?[0-9]*))\)?$";

        public const string REGEX_URL = @"(?<Protocol>\w+):\/\/(?<Domain>[\w@][\w.:@]+)\/?[\w\.?=%&=\-@/$,]*";

        public const string REGEX_IPADDRESS = @"(?<First>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Second>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Third>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Fourth>2[0-4]\d|25[0-5]|[01]?\d\d?)";

        //wraps the default regex isMatch to include regex options to pre compile and ignore case. also checks for nulls and empty strings
        public static bool IsExactMatch(string input, string pattern)
        {
            if (Strings.IsEmpty(input)) return false;
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (!m.Success) return false;
            return m.Groups[0].Value == input;
        }

        //wraps the default regex Contains to include regex options to pre compile and ignore case. also checks for nulls and empty strings
        public static bool Contains(string input, string pattern)
        {
            if (Strings.IsEmpty(input)) return false;
            System.Text.RegularExpressions.Match m = System.Text.RegularExpressions.Regex.Match(input, pattern);
            return m.Success;
        }

        //wraps the default regex Replace to include regex options to pre compile and ignore case. also checks for nulls and empty strings
        public static string Replace(string input, string pattern, string replace)
        {
            if (Strings.IsEmpty(input)) return input;
            return System.Text.RegularExpressions.Regex.Replace(input, pattern, replace, RegexOptions.Compiled | RegexOptions.IgnoreCase);
        }

        //pattern must be something like :
        // @"Subject\s*\:\s*(?<SubjectReturn>.*)\r\n"
        //from a string "Subject: Testing", with groupname "SubjectReturn" will return "Testing"
        // the pattern must contain the groupname text ?<AnyGroupName>. in it to return anything
        public static string GetMatch(string input, string pattern, string groupname)
        {
            Match match = System.Text.RegularExpressions.Regex.Match(input, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                Group grp = match.Groups[groupname];
                if (grp != null)
                    return grp.Value;
            }
            return string.Empty;
        }
    }
}