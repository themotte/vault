using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace QCVault.Components
{
    public class PostListViewComponent : ViewComponent
    {
        public PostListViewComponent()
        {
        }

        public IViewComponentResult Invoke(IEnumerable<QCVault.Utilities.Entities.Post> postList)
        {
            return View("Default", postList);
        }
    }
}