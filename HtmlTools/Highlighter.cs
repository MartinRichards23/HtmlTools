using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlTools
{
    class Highlighter
    {
        public static string ApplyHighlights(string html, string colour, IEnumerable<Regex> regexes)
        {
            ISet<string> allMatches = new HashSet<string>();

            string openTag = string.Format("<span style=\"background:{0};\">", colour);
            string closeTag = "</span>";

            foreach (Regex regex in regexes)
            {
                MatchCollection matches = regex.Matches(html);

                foreach (Match match in matches)
                {
                    // make sure doesn't contain html
                    if (match.Value.Contains("<"))
                        continue;

                    allMatches.Add(match.Value);
                }
            }

            IList<string> orderedMatches = allMatches.OrderByDescending(a => a.Length).ToList();

            foreach (string match in orderedMatches)
            {
                html = html.Replace(match, openTag + match + closeTag);
            }

            return html;
        }
    }
}
