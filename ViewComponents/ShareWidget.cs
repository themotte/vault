using Microsoft.AspNetCore.Mvc;
using QCVault.Utilities.Entities;

namespace QCVault.Components
{
    public class ShareWidgetViewComponent : ViewComponent
    {
        public ShareWidgetViewComponent()
        {
        }

        public IViewComponentResult Invoke(Post post)
        {
            return View("Default", post);
        }
    }
}