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
    public class PostModel : PageModel
    {

        private readonly ILogger<PageModel> _logger;
        private readonly IPostLoader _postLoader;

        public Post post {get;set;}

        public PostModel(ILogger<PageModel> logger, IPostLoader postLoader)
        {
            _logger = logger;
            _postLoader = postLoader;
        }

        public PageResult OnGet(string title)
        {
            post = _postLoader.Posts.Where(x => x.URLSlug == title).FirstOrDefault();
            return Page();
        }
    }
}
