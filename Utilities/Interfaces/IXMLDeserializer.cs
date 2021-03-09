using System;
using System.Collections.Generic;
using System.Text;
using QCUtilities.Entities;

namespace QCUtilities.Interfaces
{
    public interface IXMLDeserializer
    {
        public List<Post> DeserializeXML(string fileName,string xsd);

    }
}
