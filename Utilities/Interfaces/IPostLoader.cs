using System;
using System.Collections.Generic;
using System.Text;
using QCUtilities.Entities;

namespace QCUtilities.Interfaces
{
    public interface IPostLoader
    {
        public List<Post> DeserializeXML();

    }
}
