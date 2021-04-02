using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using QCUtilities.Entities;
using QCUtilities.Interfaces;

namespace QCUtilities
{
    public class PostDeserializer : IPostLoader
    {
        private readonly IXMLFileValidator fileValidator;
        private readonly string fileName = "";
        private readonly string xsd = "";
        private List<Post> cachedPosts = new List<Post>();

        public PostDeserializer(IXMLFileValidator fVal)
        {
            fileValidator = fVal;
        }

        public PostDeserializer(IXMLFileValidator fVal, string dFile, string dXSD)
        {
            fileValidator = fVal;
            fileName = dFile;
            xsd = dXSD;
            DeserializeXML();
        }

        public List<Post> DeserializeXML()
        {

            if (cachedPosts.Count <= 0)
            {
                ValidateXML(fileName, xsd);

                var ser = new XmlSerializer(typeof(PostCollection));
                var ps = new PostCollection();
                using (Stream reader = new FileStream(fileName, FileMode.Open))
                {
                    ps = (PostCollection)ser.Deserialize(reader);
                }


                cachedPosts = ps.Posts;
            }

            return cachedPosts;

        }
        private void ValidateXML(string fileName, string xsd)
        {
            if (!fileValidator.IsValidFileName(fileName))
            {
                throw new ArgumentOutOfRangeException($"Invalid fileName. Make sure it is an .xml file. Filename {fileName}");
            }
            if (!fileValidator.FileExists(fileName))
            {
                throw new FileNotFoundException($"File not found. Make sure the path is correct");
            }
            if (!fileValidator.IsFileValidXML(fileName))
            {
                throw new InvalidDataException($"File is not a valid xml!");
            }
            if (!fileValidator.IsXMLSchemaCompliant(fileName, xsd))
            {
                throw new XmlException($"File is not schema compliant!");
            }
        }
    }

    public class FileValidator : IXMLFileValidator
    {
        public bool FileExists(string fileName)
        {
            return File.Exists(fileName);
        }

        public bool IsFileValidXML(string fileName)
        {
            try
            {
                var xd1 = new XDocument();
                xd1 = XDocument.Load(fileName);
            }
            catch (XmlException)
            {
                return false;
            }
            return true;
        }
        public bool IsValidFileName(string fileName)
        {
            try
            {
                var fileInfo = new FileInfo(fileName);
                return fileInfo.Name.EndsWith(".xml");
            }
            catch
            {
                return false;
            }
        }
        public bool IsXMLSchemaCompliant(string fileName, string xsd)
        {

            var schema = new XmlSchemaSet();
            schema.Add("", xsd);
            var result = false;
            string msg = "";
            var msgType = (XmlSeverityType)1;
            using (var rd = XmlReader.Create(fileName))
            {
                var doc = XDocument.Load(rd);

                doc.Validate(
                    schema, (o, e) =>
                    {

                        msg = e.Message;
                        msgType = e.Severity;
                    }
                );
            }
            result = string.IsNullOrEmpty(msg) || msgType == XmlSeverityType.Warning;
            return result;
        }
    }
}
