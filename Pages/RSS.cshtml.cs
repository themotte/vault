using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Text;
using System.Threading.Tasks;
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
            var feed = new SyndicationFeed("Motte news", "New Quality Contributions", new Uri("https://www.vault.themotte.org/RSS"), "RSSUrl", DateTime.Now)
            {
                Copyright = new TextSyndicationContent($"{DateTime.Now.Year} Motte Quality Vault")
            };

            var items = new List<SyndicationItem>();
            var postings = postLoader.Posts;
            foreach (var item in postings)
            {
                //post/beware_of_these_7_playtime_mistakes
                var postUrl = string.Concat("https://www.vault.themotte.org/posts/", item.URLSlug);
                var title = item.Title;
                var description = item.Body.contents.Substring(0, 300);
                items.Add(new SyndicationItem(title, description, new Uri(postUrl), item.URLSlug, item.Date));
            }

            feed.Items = items;
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
