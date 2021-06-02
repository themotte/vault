using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using QCUtilities.Interfaces;
using QCVault.Utilities.Entities;
using QCVault.Utilities.Services;

namespace QCVault.Pages
{
    public class SitemapModel : PageModel
    {

        private readonly ILogger<PageModel> logger;
        private readonly IPostLoader postLoader;


        public SitemapModel(ILogger<PageModel> logger, IPostLoader postLoader)
        {
            this.logger = logger;
            this.postLoader = postLoader;
        }

        public IActionResult OnGet()
        {
            string baseUrl = "https://www.vault.themotte.org/";

            // get a list of published posts
            var posts = postLoader.Posts;

            // get last modified date of the home page
            var siteMapBuilder = new SitemapBuilder();

            // add the home page to the sitemap
            siteMapBuilder.AddUrl(baseUrl, modified: DateTime.UtcNow, changeFrequency: ChangeFrequency.Weekly, priority: 1.0);

            // add the blog posts to the sitemap
            foreach (var post in posts)
            {
                siteMapBuilder.AddUrl(baseUrl + @"post/" + post.URLSlug, modified: post.Date, changeFrequency: null, priority: 0.9);
            }

            var path = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Pages\");
            var dir = new DirectoryInfo(path);

            var Pages = new List<string>() {
                "About","Error","PostList" };
            foreach (var staticPage in Pages)
            {
                    siteMapBuilder.AddUrl(baseUrl + staticPage, modified: null, changeFrequency: null, priority: 0.9);
            }


            // generate the sitemap xml
            string xml = siteMapBuilder.ToString();
            return Content(xml, "text/xml");
        }
    }
}
