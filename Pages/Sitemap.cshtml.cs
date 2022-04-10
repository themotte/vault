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
    [ResponseCache(CacheProfileName = "Static")]
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
            string baseUrl = "https://www.vault.themotte.org";

            // get a list of published posts
            var posts = postLoader.VisiblePosts();
            
            var siteMapBuilder = new SitemapBuilder();

            // add the home page to the sitemap
            siteMapBuilder.AddUrl(baseUrl + "/");

            // add the blog posts to the sitemap
            foreach (var post in posts)
            {
                siteMapBuilder.AddUrl(baseUrl + post.FullURL);
            }

            var pages = new List<string>() {
                "about"};
            foreach (var staticPage in pages)
            {
                    siteMapBuilder.AddUrl(baseUrl + "/" + staticPage);
            }


            // generate the sitemap xml
            string xml = siteMapBuilder.ToString();
            return Content(xml, "text/xml");
        }
    }
}
