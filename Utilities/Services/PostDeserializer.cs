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
        public List<Post> Posts { get; }

        public PostDeserializer(IDiskArchiveValidator validator, IPostCollectionValidator collectionValidator, string path, string xsd)
        {
            ValidateDirectory(validator, path, xsd);

            Posts = new List<Post>();

            // Hacky approach to priority blocks
            foreach (var extension in new string[] { "p1", "p2", "p3" })
            {
                var fullPath = Path.Combine(path, extension);
                if (!Directory.Exists(fullPath))
                {
                    continue;
                }

                var postChunk = new List<Post>();
                foreach (var fileName in Directory.GetFiles(fullPath))
                {
                    var ser = new XmlSerializer(typeof(Post));
                    Post post = null;
                    using (Stream reader = new FileStream(fileName, FileMode.Open))
                    {
                        post = (Post)ser.Deserialize(reader);
                    }
                    postChunk.Add(post);
                }

                // We want posts to be shown in chronological order, newest-to-oldest, so we just do this here because it's silly to redo it every time we reload.
                Posts.AddRange(postChunk.OrderByDescending(post => post.Date));
            }

            // hardcoded list of authorized categories to help me avoid dumb typos
            var validCategories = new HashSet<string> { "coteries", "culture", "personal", "knowledge", "economics", "civilization", "moloch", "media", "flux" };

            // used for sorting
            var categoryCount = new Dictionary<string, int>();

            foreach (var post in Posts)
            {
                foreach (var cat in post.Category)
                {
                    if (!validCategories.Contains(cat))
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
                post.RegenerateCachedData();
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

            if (!archiveValidator.FilesValid(path, xsd))
            {
                throw new XmlException($"Parse or schema errors found in files!");
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

    // Note: This is currently very inefficient because it ends up reading the same files multiple times.
    // This should technically be fixed, but it's also irrelevant in terms of performance.
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

        public bool FilesValid(string path, string xsd)
        {
            var schema = new XmlSchemaSet();
            schema.Add("", xsd);

            bool valid = true;

            foreach (var file in Directory.GetFiles(path))
            {
                using (var rd = XmlReader.Create(file))
                {
                    var doc = XDocument.Load(rd, LoadOptions.PreserveWhitespace);

                    doc.Validate(
                        schema, (o, e) =>
                        {
                            valid = false;

                            Console.WriteLine($"{e.Severity} in file {file}: {e.Message}");
                        }
                    );
                }
            }

            return valid;
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
