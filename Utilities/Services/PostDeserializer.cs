using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using QCUtilities.Entities;
using QCUtilities.Interfaces;
using Newtonsoft.Json;
namespace QCUtilities
{
    public class PostDeserializer : IPostLoader
    {
        private readonly List<Post> posts;

        public List<Post> Posts { get => posts; }

        public PostDeserializer(IXMLFileValidator validator, string fileName, string xsd)
        {
            ValidateXML(validator, fileName, xsd);

            var ser = new XmlSerializer(typeof(PostCollection));
            var ps = new PostCollection();
            using (Stream reader = new FileStream(fileName, FileMode.Open))
            {
                ps = (PostCollection)ser.Deserialize(reader);
            }
            ValidatePostContent(ps.Posts);
            posts = ps.Posts;
        }

        private void ValidateXML(IXMLFileValidator fileValidator, string fileName, string xsd)
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

        private void ValidatePostContent(List<Post> posts)
        {

            if (posts.Count() > 0)
            {

                var guiltyURLS= (
                                            from post in posts
                                            group post.Title by post.Link into g
                                            where g.Count() > 1
                                            select g.Key
                                        )?.ToList();
                bool duplicateURLSFound = guiltyURLS?.Count() > 0;
                if (duplicateURLSFound)
                {
                    var guiltyPost = (
                                            from post in posts
                                            join URL in guiltyURLS on post.Link equals URL
                                            group post.Title by post.Link into g
                                            let link = g.Key
                                            let postTitles =g.AsEnumerable()
                                            select new { link,postTitles}
                                        ).ToList();

                    var postResults = JsonConvert.SerializeObject(guiltyPost).ToString();
                    throw new FileLoadException($"The xml contains the following duplicate URLS:\n{postResults}");
                }
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
