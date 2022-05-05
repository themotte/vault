using System;
using System.Collections.Generic;
using System.Text;
using QCVault.Utilities.Entities;

namespace QCVault.Utilities.Interfaces
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
