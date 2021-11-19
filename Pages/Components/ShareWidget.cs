using Microsoft.AspNetCore.Mvc;

namespace QCVault.Components
{
    public class ShareWidgetViewComponent : ViewComponent
    {
        public ShareWidgetViewComponent()
        {
        }

        public IViewComponentResult Invoke(QCUtilities.Entities.Post post)
        {
            return View("Default", post);
        }
    }
}