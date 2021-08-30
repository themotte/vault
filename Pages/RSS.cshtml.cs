using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using QCUtilities.Interfaces;

namespace QCVault.Pages
{
    [ResponseCache(CacheProfileName = "Static")]
    public class RSSModel : PageModel
    {
        private readonly ILogger<PageModel> logger;
        private readonly IPostLoader postLoader;
        public RSSModel(ILogger<PageModel> logger, IPostLoader postLoader)
        {
            this.logger = logger;
            this.postLoader = postLoader;
        }
        public IActionResult OnGet()
        {
            var items = new List<SyndicationItem>();
            foreach (var item in postLoader.Posts)
            {
                var postUrl = string.Concat("https://www.vault.themotte.org" + item.FullURL);
                var title = item.Title;
                var description = item.BodyExcerpt;
                items.Add(new SyndicationItem(title, description, new Uri(postUrl), item.URLSlug, item.Date));
            }

            var feed = new SyndicationFeed("Motte Quality Contributions", "The best from The Motte", new Uri("https://www.vault.themotte.org/rss"), "RSSUrl", DateTime.Now)
            {
                Copyright = new TextSyndicationContent($"{DateTime.Now.Year} Motte Quality Vault"),
                Items = items,
            };
            var settings = new XmlWriterSettings
            {
                Encoding = Encoding.UTF8,
                NewLineHandling = NewLineHandling.Entitize,
                NewLineOnAttributes = true,
                Indent = true
            };
            using (var stream = new MemoryStream())
            {
                using (var xmlWriter = XmlWriter.Create(stream, settings))
                {
                    var rssFormatter = new Rss20FeedFormatter(feed, false);
                    rssFormatter.WriteTo(xmlWriter);
                    xmlWriter.Flush();
                }
                return File(stream.ToArray(), "application/rss+xml; charset=utf-8");
            }
        }
    }
}
