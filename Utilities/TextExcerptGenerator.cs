using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;

// Loosely based on https://blog.andycook.com/code/2016/02/11/String-truncation-of-HTML/

namespace QCVault.Utilities
{
    // Generate an excerpt suitable for text output. Strips blockquotes, removes HTML tags, and truncates to a given length.
    public static class TextExcerptGenerator
    {
        private static IEnumerable<HtmlNode> Descendants(this HtmlNode root)
        {
            return new[] { root }.Concat(root.ChildNodes.SelectMany(child => child.Descendants()));
        }

        public static string Generate(string html, int maxCharacters)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            // Strip blockquotes entirely
            foreach (var descendent in doc.DocumentNode.Descendants().ToArray())
            {
                if (descendent.Name == "blockquote")
                {
                    descendent.Remove();
                }
            }

            // Adapted from https://stackoverflow.com/questions/12787449/html-agility-pack-removing-unwanted-tags-without-removing-content
            string text = string.Join(" ", doc.DocumentNode.SelectNodes("./*|./text()").Select(node => node.InnerText)).Replace("  ", " ");
            if (text.Length > maxCharacters)
            {
                text = text.Substring(0, maxCharacters - 1) + "…";
            }

            return text;
        }
    }
}
