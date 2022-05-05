using System;
using System.Collections.Generic;
using System.Text;
using QCVault.Utilities.Entities;

namespace QCVault.Utilities.Interfaces
{
    public interface IPostLoader
    {
        public IEnumerable<Post> VisiblePosts();
    }
}
