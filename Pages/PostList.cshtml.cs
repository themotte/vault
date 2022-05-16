using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using QCVault.Utilities.Entities;
using QCVault.Utilities.Interfaces;


namespace QCVault.Pages
{
    [ResponseCache(CacheProfileName = "Static")]
    public class PostListModel : PageModel
    {
        private readonly ILogger<PageModel> logger;
        private readonly IPostLoader postLoader;

        public PaginatedList<Post> Posts { get; set; }

        public PostListModel(ILogger<PageModel> logger, IPostLoader postLoader)
        {
            this.logger = logger;
            this.postLoader = postLoader;
        }

        public IActionResult OnGet(int? pageNumber)
        {
            Posts = PaginatedList<Post>.Create(postLoader.VisiblePosts().AsQueryable(), pageNumber ?? 1, 5);
            return Page();
        }
    }
}
