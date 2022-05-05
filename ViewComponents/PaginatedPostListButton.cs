using Microsoft.AspNetCore.Mvc;
using QCVault.Utilities.Entities;

namespace QCVault.Components
{
    public class PaginatedPostListButtonsViewComponent : ViewComponent
    {
        public PaginatedPostListButtonsViewComponent()
        {
        }

        public IViewComponentResult Invoke(PaginatedList<Post> paginatedPosts)
        {
            return View("Default", paginatedPosts);
        }
    }
}