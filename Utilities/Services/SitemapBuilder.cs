using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using QCVault.Utilities.Entities;

namespace QCVault.Utilities.Services
{
    public class SitemapBuilder
    {
        private readonly XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";

        private readonly List<SitemapUrl> urls;

        public SitemapBuilder()
        {
            urls = new List<SitemapUrl>();
        }

        public void AddUrl(string url, DateTimeOffset? modified = null, ChangeFrequency? changeFrequency = null, double? priority = null)
        {
            urls.Add(new SitemapUrl()
            {
                Url = url,
                Modified = modified,
                ChangeFrequency = changeFrequency,
                Priority = priority,
            });
        }

        public override string ToString()
        {
            var sitemap = new XDocument(
                new XDeclaration("1.0", "utf-8", "yes"),
                new XElement(ns + "urlset",
                    from item in urls
                    select CreateItemElement(item)
                    ));

            return sitemap.ToString();
        }

        private XElement CreateItemElement(SitemapUrl url)
        {
            var itemElement = new XElement(ns + "url", new XElement(ns + "loc", url.Url.ToLower()));

            if (url.Modified.HasValue)
            {
                itemElement.Add(new XElement(ns + "lastmod", url.Modified.Value.ToString("yyyy-MM-ddTHH:mm:ss.f") + "+00:00"));
            }

            if (url.ChangeFrequency.HasValue)
            {
                itemElement.Add(new XElement(ns + "changefreq", url.ChangeFrequency.Value.ToString().ToLower()));
            }

            if (url.Priority.HasValue)
            {
                itemElement.Add(new XElement(ns + "priority", url.Priority.Value.ToString("N1")));
            }

            return itemElement;
        }
    }
}
