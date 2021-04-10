using System;
using System.Collections.Generic;
using System.Text;
using QCUtilities.Entities;

namespace QCUtilities.Interfaces
{
    public interface IXMLFileValidator
    {
        public bool IsValidFileName(string fileName);

        public bool FileExists(string fileName);

        public bool IsFileValidXML(string fileName);

        public bool IsXMLSchemaCompliant(string fileName,string xsd);

    }

    public interface IPostCollectionValidator
    {
        public bool CollectionContainsUniqueURLS(List<Post> postCollection, out string errorMSG);
    }
}
