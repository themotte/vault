using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

// Copied largely-verbatim from https://blog.andycook.com/code/2016/02/11/String-truncation-of-HTML/

namespace QCVault.Utilities
{
    public static class ExcerptGenerator
    {
        public static IEnumerable<HtmlNode> Descendants(this HtmlNode root)
        {
            return new[] { root }.Concat(root.ChildNodes.SelectMany(child => child.Descendants()));
        }

        public static IEnumerable<HtmlNode> TextDescendants(this HtmlNode root)
        {
            return root.Descendants().Where(n => n.NodeType == HtmlNodeType.Text && !String.IsNullOrWhiteSpace(n.InnerText));
        }

        public static string Generate(string html, int maxCharacters)
        {
            if (string.IsNullOrEmpty(html) || html.Length <= maxCharacters)
                return html;

            var doc = new HtmlDocument();
            doc.LoadHtml(html);
            if (doc.DocumentNode.InnerText.Length <= maxCharacters)
                return html;

            var textNodes = new LinkedList<HtmlNode>(doc.DocumentNode.TextDescendants());
            var preceedingText = 0;
            var lastNode = textNodes.First;
            while (preceedingText <= maxCharacters && lastNode != null)
            {
                var nodeTextLength = lastNode.Value.InnerText.Length;
                if (preceedingText + nodeTextLength > maxCharacters)
                {
                    var truncatedText = TruncateWords(lastNode.Value.InnerText, maxCharacters - preceedingText);

                    if (String.IsNullOrWhiteSpace(truncatedText) && lastNode.Previous != null)
                    {
                        // Put the ellipsis in the previous node and remove the empty node.
                        lastNode.Previous.Value.InnerHtml = lastNode.Previous.Value.InnerText.Trim() + "…";
                        lastNode.Value.InnerHtml = String.Empty;
                        lastNode = lastNode.Previous;
                    }
                    else
                    {
                        lastNode.Value.InnerHtml = truncatedText + "…";
                    }

                    break;
                }

                if (preceedingText + nodeTextLength == maxCharacters)
                    break;

                preceedingText += nodeTextLength;
                lastNode = lastNode.Next;
            }

            // Remove all the nodes after lastNode
            if (lastNode != null)
                RemoveFollowingNodes(lastNode.Value);

            return doc.DocumentNode.InnerHtml;
        }

        private static void RemoveFollowingNodes(HtmlNode lastNode)
        {
            while (lastNode.NextSibling != null)
                lastNode.NextSibling.Remove();
            if (lastNode.ParentNode != null)
                RemoveFollowingNodes(lastNode.ParentNode);
        }

        private static string TruncateWords(string value, int length)
        {
            if (String.IsNullOrWhiteSpace(value) || length <= 0)
                return String.Empty;
            if (length > value.Length)
                return value;

            var endIndex = length;
            while (Char.IsLetterOrDigit(value[endIndex - 1]) && Char.IsLetterOrDigit(value[endIndex]) && endIndex > 1)
                endIndex--;

            if (endIndex == 1)
                return String.Empty;
            return value.Substring(0, endIndex).Trim();
        }
    }
}
