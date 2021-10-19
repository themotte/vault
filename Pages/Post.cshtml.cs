using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using QCUtilities.Entities;
using System.IO;
using Microsoft.Extensions.Logging;
using QCUtilities.Interfaces;

namespace QCVault.Pages
{
    [ResponseCache(CacheProfileName = "Static")]
    public class PostModel : PageModel
    {

        private readonly ILogger<PageModel> logger;
        private readonly IPostLoader postLoader;

        public Post Post {get;set;}

        public PostModel(ILogger<PageModel> logger, IPostLoader postLoader)
        {
            this.logger = logger;
            this.postLoader = postLoader;
        }

        public IActionResult OnGet(string title)
        {
            Post = postLoader.Posts.Where(x => x.URLSlug == title).FirstOrDefault();

            if (Post == null)
            {
                Post = postLoader.Posts.Where(x => x.RedirectURLSlug.Contains(title)).FirstOrDefault();

                if (Post != null)
                {
                    return RedirectPermanent(Post.FullURL);
                }
            }

            if (Post == null)
            {
                return NotFound();
            }

            return Page();
        }
    }
}
