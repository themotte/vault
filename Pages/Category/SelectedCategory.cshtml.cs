using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using QCUtilities.Entities;
using QCUtilities.Interfaces;
using QCUtilities;

namespace QCVault.Pages.Category
{
    public class SelectedCategoryModel : PageModel
    {

        private readonly ILogger<PageModel> logger;
        private readonly IPostLoader postLoader;

        public string CategoryName;
        public string CategoryText;

        public List<Post> Posts { get; set; }

        public SelectedCategoryModel(ILogger<PageModel> logger, IPostLoader postLoader)
        {
            this.logger = logger;
            this.postLoader = postLoader;
        }

        public IActionResult OnGet(int? pageNumber, string categoryName)
        {
            Posts = postLoader.Posts;

            if (!string.IsNullOrEmpty(categoryName))
            {
                Posts = Posts.Where(p => p.Category.Contains(categoryName, StringComparer.OrdinalIgnoreCase)).ToList();
                if (!Posts.Any())
                {
                    return NotFound();
                }
            }

            CategoryName = categoryName.Capitalize();

            if (!Utilities.Constants.Categories.TryGetValue(categoryName, out CategoryText))
            {
                CategoryText = "";
            }

            return Page();
        }
    }
}
