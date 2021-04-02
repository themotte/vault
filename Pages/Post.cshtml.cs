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

        public Post Post {get;set;}

        public PostModel(ILogger<PageModel> logger, IPostLoader postLoader)
        {
            _logger = logger;
            _postLoader = postLoader;
        }

        public async Task<PageResult> OnGetAsync(string title)
        {
            var result = await Task.FromResult(_postLoader.DeserializeXML());
            Post = result.Where(x => x.URLSlug == title).FirstOrDefault();
            return Page();
        }
    }
}
