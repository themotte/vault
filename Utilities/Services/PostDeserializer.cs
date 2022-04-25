using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;
using Newtonsoft.Json;
using QCUtilities.Entities;
using QCUtilities.Interfaces;

namespace QCUtilities
{
    public class PostDeserializer : IPostLoader
    {
        private List<Post> Posts { get; }
        public IEnumerable<Post> VisiblePosts()
        {
            return Posts.Where(post => post.Date <= DateTimeOffset.Now);
        }

        private Post ReadPost(string filename, XmlSchemaSet schemas)
        {
            Console.WriteLine($"Reading {filename} . . .");

            var settings = new XmlReaderSettings
            {
                Schemas = schemas,
                ValidationType = ValidationType.Schema,
                ValidationFlags =
                           XmlSchemaValidationFlags.ProcessIdentityConstraints |
                           XmlSchemaValidationFlags.ReportValidationWarnings
            };

            settings.ValidationEventHandler +=
                delegate (object sender, ValidationEventArgs args)
                {
                    throw args.Exception;
                };

            using (var input = new StreamReader(filename))
            {
                using (XmlReader reader = XmlReader.Create(input, settings))
                {
                    XmlSerializer ser = new XmlSerializer(typeof(Post));
                    return (Post)ser.Deserialize(reader);
                }
            }
        }

        public PostDeserializer(IDiskArchiveValidator validator, IPostCollectionValidator collectionValidator, string path, string xsd)
        {
            ValidateDirectory(validator, path, xsd);

            XmlSchemaSet schemas = new XmlSchemaSet();
            using (var input = new StreamReader(xsd))
            {
                schemas.Add(XmlSchema.Read(input, (sender, args) => throw args.Exception));
            }

            Posts = new List<Post>();

            // Load our main posts first
            foreach (var extension in new string[] { "main" })
            {
                var fullPath = Path.Combine(path, extension);
                if (!Directory.Exists(fullPath))
                {
                    continue;
                }

                Posts.AddRange(Directory.GetFiles(fullPath).Select(fileName => ReadPost(fileName, schemas)));
            }

            // Now load all our timed posts
            string timedFilename = Path.Combine(path, "timed.xml");
            if (File.Exists(timedFilename))
            {
                var doc = XDocument.Load(timedFilename);

                foreach (var elem in doc.Root.Elements())
                {
                    var time = elem.Attribute("time").Value;
                    var fname = elem.Value;

                    var post = ReadPost(Path.Combine(path, fname), schemas);
                    post.Date = DateTimeOffset.Parse(time);   // just kinda sneak this in
                    Posts.Add(post);
                }
            }

            Console.WriteLine($"Loaded {Posts.Count} posts");

            // We want posts to be shown in chronological order, newest-to-oldest, so we just do this here because it's silly to redo it every time we reload.
            Posts = Posts.OrderByDescending(post => post.Date).ToList();

            // used for sorting
            var categoryCount = new Dictionary<string, int>();

            foreach (var post in Posts)
            {
                foreach (var cat in post.Category)
                {
                    if (!QCVault.Utilities.Constants.Categories.ContainsKey(cat))
                    {
                        throw new InvalidDataException($"Invalid category {cat} in {post.Title}");
                    }

                    if (!categoryCount.ContainsKey(cat))
                    {
                        categoryCount[cat] = 1;
                    }
                    else
                    {
                        categoryCount[cat] = categoryCount[cat] + 1;
                    }
                }
            }

            // order by frequency increasing, so the rarest category is at the front
            var categories = categoryCount.OrderBy(kvp => kvp.Value).Select(kvp => kvp.Key).ToList();

            // go back through the posts and reorder the categories
            foreach (var post in Posts)
            {
                post.Category = categories.Where(cat => post.Category.Contains(cat)).ToList();
            }

            ValidatePostCollection(collectionValidator, Posts);

            foreach (var post in Posts)
            {
                // get any parsing errors out of the way sooner rather than later, at the cost of a tiny slice of startup time
                post.GenerateCachedData();
            }
        }

        private void ValidateDirectory(IDiskArchiveValidator archiveValidator, string path, string xsd)
        {
            if (!archiveValidator.DirectoryExists(path))
            {
                throw new DirectoryNotFoundException($"Directory not found. Make sure the path is correct.");
            }

            if (!archiveValidator.AFileExists(path))
            {
                throw new FileNotFoundException($"No files found in directory. Make sure the path is correct.");
            }
        }

        private void ValidatePostCollection(IPostCollectionValidator collectionValidator, List<Post> postcollection)
        {
            if (!collectionValidator.CollectionContainsUniqueURLS(postcollection, out string errorMsg))
            {
                throw new FileLoadException(errorMsg);
            }
        }
    }

    public class DiskArchiveValidator : IDiskArchiveValidator
    {
        public bool DirectoryExists(string path)
        {
            return Directory.Exists(path);
        }

        public bool AFileExists(string path)
        {
            return Directory.GetFiles(path, "*.xml", SearchOption.AllDirectories).Any();
        }
    }

    public class CollectionValidator : IPostCollectionValidator
    {
        public bool CollectionContainsUniqueURLS(List<Post> postCollection, out string errorMSG)
        {
            errorMSG = "";
            
            var guiltyTitles = postCollection
                    .SelectMany(p => p.RedirectURLSlug.Concat(new [] { p.URLSlug }))
                    .GroupBy(t => t)
                    .Where(g => g.Count() > 1)
                    .Select(g => (title: g.Key, count: g.Count()))
                    .ToList();

            bool duplicateURLSFound = guiltyTitles.Count() > 0;
            if (duplicateURLSFound)
            {
                var postResults = JsonConvert.SerializeObject(guiltyTitles).ToString();
                errorMSG = $"The xml contains the following duplicate URLS:\n{postResults}";
            }
            
            return string.IsNullOrEmpty(errorMSG);
        }
    }
}
