using Microsoft.AspNetCore.Mvc;

namespace QCVault.Components
{
    public class PostViewComponent : ViewComponent
    {
        public PostViewComponent()
        {
        }

        public IViewComponentResult Invoke(QCUtilities.Entities.Post post)
        {
            return View("Default", post);
        }
    }
}