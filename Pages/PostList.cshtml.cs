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
    public class PostListModel : PageModel
    {
        private readonly ILogger<TestModel> _logger;
        private readonly IPostLoader _postLoader;

        public List<Post> Posts { get; set; }

        public PostListModel(ILogger<TestModel> logger, IPostLoader postLoader)
        {
            _logger = logger;
            _postLoader = postLoader;
        }

        public async Task<IActionResult> OnGetAsync(int? pageNumber)
        {
            var result = await Task.FromResult(_postLoader.DeserializeXML("", ""));
            Posts = result;
            return Page();
        }
    }
}
