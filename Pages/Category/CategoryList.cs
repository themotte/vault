using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace QCVault.Pages.Category
{
    public class CategoryListModel : PageModel
    {
        public HashSet<string> ValidCategories { get; set; }
        public void OnGet()
        {
            ValidCategories = new()
            {
                "coteries",
                "culture",
                "personal",
                "knowledge",
                "economics",
                "civilization",
                "moloch",
                "media",
                "flux"

            };
        }
    }
}
