using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using QCUtilities.Entities;
using QCUtilities.Interfaces;


namespace QCVault.Pages
{
    [ResponseCache(CacheProfileName = "Static")]
    public class PostListModel : PageModel
    {
        private readonly ILogger<PageModel> logger;
        private readonly IPostLoader postLoader;

        public List<Post> Posts { get; set; }

        public PostListModel(ILogger<PageModel> logger, IPostLoader postLoader)
        {
            this.logger = logger;
            this.postLoader = postLoader;
        }

        public IActionResult OnGet(int? pageNumber)
        {
            Posts = postLoader.Posts;
            return Page();
        }
    }
}
