using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace KeyHub.Common.Utils
{
    /// <summary>
    /// A collection of common string utility methods
    /// </summary>
    public static class Strings
    {
        public static bool IsEmpty(object input)
        {
            return input == null || Convert.ToString(input).Length == 0;
        }

        public static bool IsEmpty(string input)
        {
            return IsEmpty((Object)input);
        }

        public static bool IsNumeric(object input)
        {
            return RegexUtils.IsExactMatch(CutWhitespace(Convert.ToString(input)), RegexUtils.REGEX_NUMERIC);
        }

        public static bool IsEmail(string input)
        {
            return RegexUtils.IsExactMatch(input, RegexUtils.REGEX_EMAIL);
        }

        //just to take nulls into consideration
        public static string Trim(string input)
        {
            if (IsEmpty(input)) return input;
            return input.Trim();
        }

        public static string CutWhitespace(string input)
        {
            if (IsEmpty(input)) return input;
            return Trim(Regex.Replace(input, @"\s+", " "));
        }

        public static string CutBreaks(string input)
        {
            if (IsEmpty(input)) return input;
            return Trim(Regex.Replace(input, @"\n+", " "));
        }

        public static string CutEnd(string input, int length)
        {
            if (IsEmpty(input)) return input;
            if (input.Length <= length) return input;
            return input.Substring(0, length);
        }

        public static string CutEnd(string input, int length, string endString)
        {
            if (IsEmpty(input)) return input;
            if (input.Length <= length) return input;
            return input.Substring(0, length) + endString;
        }

        public static string CutStart(string input, int length)
        {
            if (IsEmpty(input)) return input;
            if (input.Length <= length) return input;
            return input.Substring(length);
        }

        public static string Start(string input, int length)
        {
            if (IsEmpty(input)) return input;
            if (input.Length <= length) return input;
            return input.Substring(0, length);
        }

        public static string End(string input, int length)
        {
            if (IsEmpty(input)) return input;
            if (input.Length <= length) return input;
            return input.Substring(input.Length - length);
        }

        public static string ToTitleCase(string Input)
        {
            return System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Input);
        }
    }
}