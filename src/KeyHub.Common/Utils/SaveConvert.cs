using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KeyHub.Common.Utils
{
    //safe conversion class
    public static class SafeConvert
    {
        public static bool ToBoolean(object input, bool def)
        {
            if (Strings.IsEmpty(input)) return def;
            string strInput = Convert.ToString(input);
            if (strInput.ToUpper() == "TRUE" || strInput == "1") return true;
            if (strInput.ToUpper() == "FALSE" || strInput == "0") return false;
            return def;
        }

        public static string ToString(object input)
        {
            if (input == null)
                return String.Empty;

            return input.ToString();
        }

        public static bool ToBoolean(object input)
        {
            return ToBoolean(input, false);
        }

        public static int ToInt(object input, int def)
        {
            if (Strings.IsNumeric(input)) return Convert.ToInt32(input);
            return def;
        }

        public static int ToInt(object input)
        {
            return ToInt(input, 0);
        }

        public static decimal ToDecimal(object input, decimal def)
        {
            if (Strings.IsNumeric(input)) return Convert.ToDecimal(input);
            return def;
        }

        public static decimal ToDecimal(object input)
        {
            return ToDecimal(input, 0);
        }

        public static double ToDouble(object input, double def)
        {
            if (Strings.IsNumeric(input)) return Convert.ToDouble(input);
            return def;
        }

        public static double ToDouble(object input)
        {
            return ToDouble(input, 0);
        }

        public static string ToHexString(byte[] data)
        {
            if (data.Length == 0) return string.Empty;
            StringBuilder sb = new StringBuilder(data.Length * 2);
            foreach (byte b in data)
            {
                sb.AppendFormat("{0:x2}", b);
            }
            return sb.ToString();
        }

        public static System.IO.MemoryStream ToStream(string input)
        {
            return new System.IO.MemoryStream(ASCIIEncoding.ASCII.GetBytes(input.ToCharArray()));
        }
    }
}