using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlTools
{
    public static class StringTools
    {
        /// <summary>
        /// Collapses consecutive whitespace into a single space, newlines become a single newline
        /// </summary>
        public static string CollapseWhiteSpace(string input)
        {
            if (input == null)
                return null;

            Regex whiteSpace = new Regex(@"[ \t]+");
            Regex newlines = new Regex(@"[\r\n]+");

            input = whiteSpace.Replace(input, " ");
            input = newlines.Replace(input, "\r\n");
            return input;
        }

        /// <summary>
        /// Collapses all consecutive whitespace into a single space
        /// </summary>
        public static string CollapseAllWhiteSpace(string input, string replacement = " ")
        {
            if (input == null)
                return null;

            Regex whiteSpaceAll = new Regex(@"[\s]+");

            input = whiteSpaceAll.Replace(input, replacement);
            return input;
        }

        public static string HtmlDecode(this string input)
        {
            if (input == null)
                return null;

            input = WebUtility.HtmlDecode(input);
            input = CollapseAllWhiteSpace(input);
            return input;
        }

        public static string HtmlEncode(this string input)
        {
            if (input == null)
                return null;

            return WebUtility.HtmlEncode(input);
        }
    }
}
