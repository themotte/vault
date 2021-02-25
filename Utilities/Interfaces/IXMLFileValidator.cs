using System;
using System.Collections.Generic;
using System.Text;

namespace QCUtilities.Interfaces
{
    public interface IXMLFileValidator
    {
        public bool IsValidFileName(string fileName);

        public bool FileExists(string fileName);

        public bool IsFileValidXML(string fileName);

        public bool IsXMLSchemaCompliant(string fileName,string xsd);

    }
}
