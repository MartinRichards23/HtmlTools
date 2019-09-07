using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlTools
{
    public class Highlighter
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

        /// <summary>
        /// Highlights the given nodes with a background coloured span
        /// </summary>
        public static void ApplyHighlights(HtmlDocument doc, string colour, IEnumerable<string> xpaths)
        {
            HtmlNode body = doc.GetBodyNode();

            foreach (string xpath in xpaths)
            {
                HtmlNodeCollection nodes = body.SelectNodes(xpath);

                if (nodes != null)
                {
                    for (int i = nodes.Count - 1; i >= 0; i--)
                    {
                        HtmlNode node = nodes[i];
                        HighlightText(node, colour);
                    }
                }
            }
        }

        public static void HighlightText(HtmlNode node, string colour)
        {
            HtmlNodeCollection nodes = node.SelectNodes(".//text()");

            if (nodes != null)
            {
                foreach (HtmlNode textNode in nodes)
                {
                    if (string.IsNullOrWhiteSpace(textNode.InnerText))
                        continue;

                    string html = textNode.InnerHtml;

                    textNode.InnerHtml = string.Format("<tspan style=\"background-color:{0};color:#000000\">{1}</tspan>", colour, html);
                }
            }
        }
    }
}
