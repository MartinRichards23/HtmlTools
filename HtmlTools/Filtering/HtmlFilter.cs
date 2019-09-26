using HtmlAgilityPack;
using System.Collections.Generic;
using System.Linq;

namespace HtmlTools.Filtering
{
    /// <summary>
    /// Html Filtering logic
    /// </summary>
    public class HtmlFilter
    {
        public HtmlFilter()
        {

        }

        #region public methods

        /// <summary>
        /// Filters out elements including scripts, comments, adverts, and common types of navigation
        /// </summary>
        public void CleanHtml(HtmlNode node)
        {
            RemoveScriptNodes(node);
            RemoveCommentNodes(node);

            FilterAdverts(node);
            FilterRoles(node);

            node.RemoveNodesOfType("header");
            node.RemoveNodesOfType("aside");
            node.RemoveNodesOfType("footer");
            node.RemoveNodesOfType("nav");
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Remove adverts
        /// </summary>
        private void FilterAdverts(HtmlNode node)
        {
            RemoveNodes(node, "//div[contains(@id, 'adsense')]");
            RemoveNodes(node, "//iframe[contains(@id, 'google_ads')]");
        }

        /// <summary>
        /// Remove divs where the role attribute is typically not useful
        /// </summary>
        private void FilterRoles(HtmlNode node)
        {
            RemoveNodes(node, "//div[@role='complementary']");
            RemoveNodes(node, "//div[@role='banner']");
            RemoveNodes(node, "//div[@role='navigation']");
        }

        /// <summary>
        /// Removes all script nodes
        /// </summary>
        private void RemoveScriptNodes(HtmlNode node)
        {
            node.RemoveNodesOfType("script");
            node.RemoveNodesOfType("noscript");
        }

        /// <summary>
        /// Removes all comment nodes
        /// </summary>
        private void RemoveCommentNodes(HtmlNode node)
        {
            for (int i = node.ChildNodes.Count - 1; i >= 0; i--)
            {
                var n = node.ChildNodes[i];
                RemoveCommentNodes(n);
            }

            if (node.NodeType == HtmlNodeType.Comment)
                node.Remove();
        }

        private void RemoveNodes(HtmlNode node, string xpath)
        {
            HtmlNodeCollection nodes = node.SelectNodes(xpath);

            if (nodes != null)
            {
                foreach (HtmlNode n in nodes)
                {
                    n.Remove();
                }
            }
        }

        #endregion
    }
}
