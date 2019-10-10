using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using SystemPlus.Text;

namespace HtmlTools
{
    /// <summary>
    /// Various extentions to the HtmlAgilityPack
    /// </summary>
    public static class HtmlAgilityExtensions
    {
        public static void Load(this HtmlDocument doc, byte[] data, Encoding encoding)
        {
            using (MemoryStream ms = new MemoryStream(data))
            {
                doc.Load(ms, encoding);
            }
        }

        /// <summary>
        /// Writes the html of the document to string
        /// </summary>
        public static string Save(this HtmlDocument doc)
        {
            using (StringWriter sw = new StringWriter())
            {
                doc.Save(sw);
                return sw.ToString();
            }
        }

        public static string ToHtmlString(this HtmlAttribute attribute)
        {
            return attribute.Name + "=\"" + attribute.Value + "\"";
        }

        public static string ToHtmlString(this HtmlAttributeCollection attributes)
        {
            string result = string.Empty;

            if (attributes == null || attributes.Count < 1)
                return result;

            foreach (HtmlAttribute att in attributes)
            {
                result += " " + att.ToHtmlString();
            }

            return result;
        }

        /// <summary>
        /// Gets the html decoded attribute value
        /// </summary>
        public static string GetAttributeDecoded(this HtmlNode node, string name, string def = null)
        {
            if (node == null)
                return def;

            string val = node.GetAttributeValue(name, def);

            return SystemPlus.Net.HtmlTools.HtmlDecode(val);
        }

        /// <summary>
        /// Gets the html decoded attribute value
        /// </summary>
        public static string ValueDecoded(this HtmlAttribute att)
        {
            if (att == null)
                return null;

            string val = att.Value;

            return SystemPlus.Net.HtmlTools.HtmlDecode(val);
        }

        /// <summary>
        /// Remove all nodes of given type, e.g. "script"
        /// </summary>
        public static void RemoveNodesOfType(this HtmlNode node, string type)
        {
            HtmlNodeCollection nodes = node.SelectNodes("//" + type);
            if (nodes != null)
            {
                foreach (HtmlNode n in nodes)
                {
                    n.Remove();
                }
            }
        }

        /// <summary>
        /// Gets the decoded innertext, returns null if node is null or text is null
        /// </summary>
        public static string InnerTextDecoded(this HtmlNode node)
        {
            if (node == null)
                return null;

            return SystemPlus.Net.HtmlTools.HtmlDecode(node.InnerText);
        }

        /// <summary>
        /// Gets the base url of this document if it has one
        /// </summary>
        public static string GetRootUrl(this HtmlDocument doc)
        {
            string url = doc.DocumentNode.SelectSingleNode("//base")?.GetAttributeDecoded("href");
            return url;
        }

        /// <summary>
        /// Gets the body node
        /// </summary>
        public static HtmlNode GetBodyNode(this HtmlDocument doc)
        {
            return doc.DocumentNode.SelectSingleNode("//body");
        }

        public static string GetTitle(this HtmlDocument doc)
        {
            HtmlNode titleNode = doc.DocumentNode.SelectSingleNode("//title");
            return titleNode?.InnerTextDecoded();
        }

        public static string GetMetaDescription(this HtmlDocument doc)
        {
            HtmlNode descriptionNode = doc.DocumentNode.SelectSingleNode("//meta[@name='description']");
            return descriptionNode?.GetAttributeDecoded("content");
        }

        public static IEnumerable<string> GetMetaKeywords(this HtmlDocument doc)
        {
            IDictionary<string, string> metaTags = doc.GetMetaTags();

            if (metaTags.TryGetValue("keywords", out string value))
            {
                return value.Split(',');
            }

            return null;
        }

        public static IDictionary<string, string> GetMetaTags(this HtmlDocument doc)
        {
            IDictionary<string, string> meta = new Dictionary<string, string>();

            HtmlNodeCollection metaTags = doc.DocumentNode.SelectNodes("//meta");

            if (metaTags == null)
                return meta;

            foreach (HtmlNode tag in metaTags)
            {
                string name = tag.GetAttributeDecoded("name", null);
                string content = tag.GetAttributeDecoded("content", null);

                if (name == null || content == null)
                    continue;

                name = name.ToLowerInvariant();
                content = SystemPlus.Net.HtmlTools.HtmlDecode(content);

                if (!meta.ContainsKey(name))
                    meta.Add(name, content);
            }

            return meta;
        }

        public static IList<string> GetLinks(this HtmlNode node)
        {
            IList<string> links = new List<string>();

            HtmlNodeCollection urlNodes = node.SelectNodes(".//a");
            if (urlNodes != null)
            {
                foreach (HtmlNode urlNode in urlNodes)
                {
                    string href = urlNode.GetAttributeDecoded("href");

                    if (string.IsNullOrEmpty(href))
                        continue;

                    links.Add(href);
                }
            }

            return links;
        }

        public static IList<string> GetImages(this HtmlNode node)
        {
            IList<string> images = new List<string>();

            HtmlNodeCollection imageNodes = node.SelectNodes(".//img");
            if (imageNodes != null)
            {
                foreach (HtmlNode imageNode in imageNodes)
                {
                    string src = imageNode.GetAttributeDecoded("src");

                    if (string.IsNullOrEmpty(src))
                        continue;

                    images.Add(src);
                }
            }

            return images;
        }

        public static IList<string> GetCssFiles(this HtmlDocument doc)
        {
            IList<string> links = new List<string>();

            HtmlNodeCollection cssNodes = doc.DocumentNode.SelectNodes("//link[@rel='stylesheet']");
            if (cssNodes != null)
            {
                foreach (HtmlNode cssNode in cssNodes)
                {
                    string href = cssNode.GetAttributeDecoded("href");

                    if (!string.IsNullOrEmpty(href))
                        links.Add(href);
                }
            }

            return links;
        }

        public static IList<string> GetJsFiles(this HtmlDocument doc)
        {
            IList<string> links = new List<string>();

            HtmlNodeCollection jsNodes = doc.DocumentNode.SelectNodes("//script");
            if (jsNodes != null)
            {
                foreach (HtmlNode jsNode in jsNodes)
                {
                    string src = jsNode.GetAttributeDecoded("src");

                    if (!string.IsNullOrEmpty(src))
                        links.Add(src);
                }
            }

            return links;
        }

        public static bool IsFirstChild(this HtmlNode node)
        {
            return node.ParentNode.FirstChild == node;
        }

        public static bool IsLastChild(this HtmlNode node)
        {
            return node.ParentNode.LastChild == node;
        }

        /// <summary>
        /// Adds the base url to the html, this means relative links will still work
        /// </summary>
        public static string AddBaseTag(string html, Uri uri)
        {
            if (!Regex.IsMatch(html, "<base", RegexOptions.IgnoreCase))
            {
                string baseUrl = uri.Scheme + "://" + uri.Host;
                string baseTag = string.Format("<base href=\"{0}\" />", baseUrl);

                html = baseTag + html;
            }

            return html;
        }
    }
}
