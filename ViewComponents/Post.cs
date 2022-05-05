using Microsoft.AspNetCore.Mvc;
using QCVault.Utilities.Entities;

namespace QCVault.Components
{
    public class PostViewComponent : ViewComponent
    {
        public PostViewComponent()
        {
        }

        public IViewComponentResult Invoke(Post post)
        {
            return View("Default", post);
        }
    }
}