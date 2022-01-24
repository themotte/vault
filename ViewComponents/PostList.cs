using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace QCVault.Components
{
    public class PostListViewComponent : ViewComponent
    {
        public PostListViewComponent()
        {
        }

        public IViewComponentResult Invoke(IEnumerable<QCUtilities.Entities.Post> postList)
        {
            return View("Default", postList);
        }
    }
}