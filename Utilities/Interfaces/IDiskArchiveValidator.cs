using System;
using System.Collections.Generic;
using System.Text;
using QCUtilities.Entities;

namespace QCUtilities.Interfaces
{
    public interface IDiskArchiveValidator
    {
        public bool DirectoryExists(string path);

        public bool AFileExists(string path);
    }

    public interface IPostCollectionValidator
    {
        public bool CollectionContainsUniqueURLS(List<Post> postCollection, out string errorMSG);
    }
}
