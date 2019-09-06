using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlTools
{
    public static class HtmlTools
    {
        /// <summary>
        /// Wraps the content text with the tag
        /// </summary>
        public static string WrapContent(string content, string tag)
        {
            return $"<{tag}>{content}</{tag}>";
        }

        public static string HtmlDecode(this string input)
        {
            if (input == null)
                return null;

            input = WebUtility.HtmlDecode(input);
            input = StringTools.CollapseAllWhiteSpace(input);
            return input;
        }

        public static string HtmlEncode(this string input)
        {
            if (input == null)
                return null;

            return WebUtility.HtmlEncode(input);
        }

        public static string RemoveHtmlTags(this string input)
        {
            if (input == null)
                return null;

            Regex htmlTagRegex = new Regex(@"<[^>]*>", RegexOptions.IgnoreCase);

            return htmlTagRegex.Replace(input, string.Empty);
        }

        /// <summary>
        /// Converts br tags to newlines
        /// </summary>
        public static string ReplaceBrTags(string input)
        {
            if (input == null)
                return null;

            Regex brTagRegex = new Regex(@"</?w?br ?/?>", RegexOptions.IgnoreCase);

            input = brTagRegex.Replace(input, "\r\n");
            return input.Trim();
        }
    }
}
