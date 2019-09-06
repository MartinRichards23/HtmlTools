using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace HtmlTools.Converter
{
    /// <summary>
    /// Converts html to text or nicely formatted html
    /// </summary>
    public class HtmlConverter
    {
        #region Fields

        string lastWritten;
        int level;

        readonly char indentChar = ' ';

        static readonly Regex numOrPunctuationRegex = new Regex(@"[0-9,\.;::%\(\)\{\}\?!]+", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        static readonly Regex daysMonthsRegex = new Regex(@"\b(moday|tuesday|wednesday|thursday|friday|saturday|sunday|january|february|march|april|may|june|july|august|september|october|november|december)", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        #endregion

        public HtmlConverter()
        {

        }

        #region To lines

        /// <summary>
        /// Normalise text to allow better duplicate detection
        /// </summary>
        private string NormaliseText(string text)
        {
            if (text == null)
                return null;

            text = numOrPunctuationRegex.Replace(text, string.Empty);
            text = daysMonthsRegex.Replace(text, string.Empty);
            text = StringTools.CollapseAllWhiteSpace(text).Trim();
            text = text.ToUpperInvariant();

            return text;
        }

        /// <summary>
        /// Extract the text from a node
        /// </summary>
        public IList<HtmlLine> GetLines(string html, ConvertOptions options)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            HtmlNode node = doc.DocumentNode;

            HtmlLineCollection lineCollection = new HtmlLineCollection();

            GetLines(node, lineCollection);

            IList<HtmlLine> lines = lineCollection.Lines;

            // remove consequitive empty lines
            HtmlLine previous = null;
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                HtmlLine line = lines[i];

                line.NormalisedValue = NormaliseText(line.Text);

                if (previous != null)
                {
                    if (line.Text == null && previous.Text == null)
                        lines.RemoveAt(i + 1);
                }

                previous = line;
            }

            return lines;
        }

        private void GetLines(HtmlNode node, HtmlLineCollection lines)
        {
            string filterout = node.GetAttributeValue("filtered-out", null);

            if (!string.IsNullOrEmpty(filterout))
            {
                // ignore filtered out nodes
            }
            else if (node.NodeType == HtmlNodeType.Comment)
            {
                // ignore comments completely
            }
            else if (node.NodeType == HtmlNodeType.Text)
            {
                // get text
                string text = ((HtmlTextNode)node).Text;

                // is it in fact a special closing node output as text?
                if (HtmlNode.IsOverlappedClosingElement(text))
                    return;

                text = StringTools.HtmlDecode(text);

                // check the text is meaningful
                if (!string.IsNullOrWhiteSpace(text))
                {
                    lastWritten = text;

                    if (lines.CurrentLine == null)
                        lines.AddTextLine(node.Name, node.ParentNode.XPath);

                    lines.AddText(lastWritten);
                }
            }
            else
            {
                if (node.Name == "style" || node.Name == "script" || node.Name == "noscript")
                {
                    // ignore these
                }
                else if (node.Name == "img")
                {
                    string src = node.GetAttributeDecoded("src", null);
                    string alt = node.GetAttributeDecoded("alt", null);

                    if (src != null)
                    {
                        lastWritten = "\r\n";
                        lines.AddImageLine(node.Name, node.XPath, src, alt);
                    }
                }
                else if (node.Name == "br" || node.Name == "hr")
                {
                    lastWritten = "\r\n";
                    lines.AddTextLine(node.Name, node.XPath);
                }
                else if (node.Name == "div" || node.Name == "p" || node.Name == "ul" || node.Name == "ol" || node.Name == "li" || node.Name == "h1" || node.Name == "h2" || node.Name == "h3" || node.Name == "h4" || node.Name == "h5" || node.Name == "h6")
                {
                    lastWritten = "\r\n";
                    lines.AddTextLine(node.Name, node.XPath);

                    RecurseToLines(node, lines);

                    if (node.Name == "ul" || node.Name == "ol" || node.Name == "li")
                    {
                        lastWritten = "\r\n";
                        lines.AddTextLine(node.Name, node.XPath);
                    }
                }
                else
                {
                    if (lastWritten != null && !char.IsWhiteSpace(lastWritten.Last()))
                    {
                        lastWritten = " ";

                        if (lines.CurrentLine == null)
                            lines.AddTextLine(node.Name, node.XPath);

                        lines.AddText(lastWritten);
                    }

                    RecurseToLines(node, lines);
                }
            }
        }

        private void RecurseToLines(HtmlNode node, HtmlLineCollection lines)
        {
            if (node.HasChildNodes)
            {
                foreach (HtmlNode subnode in node.ChildNodes)
                {
                    GetLines(subnode, lines);
                }
            }
        }

        #endregion

        #region To tidy html

        /// <summary>
        /// Convert html to simplified html
        /// </summary>
        public string ToTidyHtml(string html)
        {
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            return ToTidyHtml(doc.DocumentNode);
        }

        /// <summary>
        /// Convert html to simplified html
        /// </summary>
        public string ToTidyHtml(HtmlNode node)
        {
            using (StringWriter sw = new StringWriter())
            {
                ToTidyHtml(node, sw);
                sw.Flush();

                string text = sw.ToString();

                return text.Trim();
            }
        }

        public void ToTidyHtml(HtmlNode node, TextWriter outText)
        {
            if (node.NodeType == HtmlNodeType.Document)
            {
                //lastWritten = node.InnerText.Trim() + "\r\n";
                //outText.Write(lastWritten);

                // Recurse children
                RecurseToTidyHtml(node, outText);
            }
            else if (node.NodeType == HtmlNodeType.Comment)
            {
            }
            else if (node.NodeType == HtmlNodeType.Text)
            {
                //string tabs = new string(indentChar, level);

                // get text
                string text = ((HtmlTextNode)node).Text;

                // is this a special closing node output as text?
                if (HtmlNode.IsOverlappedClosingElement(text))
                    return;
                if (string.IsNullOrWhiteSpace(text))
                    return;

                text = StringTools.CollapseAllWhiteSpace(text);

                lastWritten = text;
                //outText.Write(tabs);
                outText.Write(lastWritten);
                //outText.WriteLine();
            }
            else
            {
                string tabs = new string(indentChar, level);
                if (node.HasChildNodes)
                {
                    lastWritten = "<" + node.Name + node.Attributes.ToHtmlString() + ">";
                    outText.WriteLine();
                    outText.Write(tabs);
                    outText.Write(lastWritten);

                    level++;

                    // Recurse children
                    RecurseToTidyHtml(node, outText);

                    level--;

                    // write closing tag
                    lastWritten = "</" + node.Name + ">";
                    outText.WriteLine();
                    outText.Write(tabs);
                    outText.Write(lastWritten);
                }
                else
                {
                    lastWritten = "<" + node.Name + node.Attributes.ToHtmlString() + " />";
                    outText.WriteLine();
                    outText.Write(tabs);
                    outText.Write(lastWritten);
                }
            }
        }

        void RecurseToTidyHtml(HtmlNode node, TextWriter outText)
        {
            if (node.HasChildNodes)
            {
                foreach (HtmlNode subnode in node.ChildNodes)
                {
                    ToTidyHtml(subnode, outText);
                }
            }
        }

        #endregion

        #region Statics

        public static string HtmlToText(string html, ConvertOptions options)
        {
            HtmlConverter converter = new HtmlConverter();

            IList<HtmlLine> lines = converter.GetLines(html, options);

            return HtmlToText(lines, options);
        }

        /// <summary>
        /// Extract the text from a node
        /// </summary>
        public static string HtmlToText(IList<HtmlLine> lines, ConvertOptions options)
        {
            using (StringWriter sw = new StringWriter())
            {
                foreach (HtmlLine line in lines)
                {
                    string text = line.GetFormattedText(options);
                    sw.WriteLine(text);
                }

                sw.Flush();

                return sw.ToString().Trim();
            }
        }

        /// <summary>
        /// Adds the base tag so paths get redirected to correct path, not relative path
        /// </summary>
        public static string AddBaseTag(string html, string url)
        {
            if (!Regex.IsMatch(html, "<base", RegexOptions.IgnoreCase))
            {
                UriBuilder builder = new UriBuilder(url);
                string baseUrl = builder.Scheme + "://" + builder.Host;
                string baseTag = string.Format("<base href=\"{0}\" />", baseUrl);

                html = baseTag + html;
            }

            return html;
        }

        #endregion
    }

}
