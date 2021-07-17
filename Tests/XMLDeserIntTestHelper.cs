using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using QCUtilities.Entities;

namespace QCVault.Tests
{
    public static class XMLDeserIntTestHelper
    {
        public static readonly string IntDestDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "IntegrationTestFolder");
        public static readonly string SchemaFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posts");
        public const string INVALIDXML = "invalid";
        public const string EMPTYDIR = "empty";
        public const string VALIDXML = "valid";
        public const string EMPTYTXT = "test.adad";
        public const string SCHEMA = "posts.xsd";

        public static string ValidXMLPath => Path.Combine(IntDestDir, VALIDXML);
      
        public static string ValidXSDPath => Path.Combine(IntDestDir, SCHEMA);

        public static List<Post> CreateTestCollection(bool isDataValid = true)
        {
            bool addDuplicateURL = !isDataValid;

            // Right now the deserializer always returns in order sorted by date decreasing, so we ensure our test data has the same sorting.

            var posts = new List<Post>();
            var post1 = new Post()
            {
                Author = "questionMark",
                Date = DateTime.Parse("2020-06-29T23:32:56+00:00"),
                Link = "https://www.reddit.com/r/TheMotte/comments/hhtwxi/culture_war_roundup_for_the_week_of_june_29_2020/fwf5vss/?context=3&amp;sort=best",
                Title = "The Role of Energy and Entropy in Cultural Conflict",
                Body = new Post.VerbatimBlob("nice test 2 have a line too"),
            };
            var post2 = new Post()
            {
                Author = "Cheezemansam",
                Date = DateTime.Parse("2020-06-29T05:17:08+00:00"),
                Link = "https://www.reddit.com/r/TheMotte/comments/hhtwxi/culture_war_roundup_for_the_week_of_june_29_2020/fwe75qg/?context=3&amp;sort=best",
                Title = "The Role of Precedent in the Supreme Court",
                Body = new Post.VerbatimBlob("nice test"),
            };

            posts.Add(post1);
            posts.Add(post2);

            if (addDuplicateURL)
            {
                var post3 = new Post()
                {
                    Author = "questionMark",
                    Date = DateTime.Parse("2020-06-28T23:32:56+00:00"),
                    Link = "https://www.reddit.com/r/TheMotte/comments/hhtwxi/culture_war_roundup_for_the_week_of_june_29_2020/fwf5vss/?context=3&amp;sort=best",
                    Title = "The Role of Energy and Entropy in Cultural Conflict",
                    Body = new Post.VerbatimBlob("nice test 2 have a line too"),
                };
                var post4 = new Post()
                {
                    Author = "Cheezemansam",
                    Date = DateTime.Parse("2020-06-28T05:17:08+00:00"),
                    Link = "https://www.reddit.com/r/TheMotte/comments/hhtwxi/culture_war_roundup_for_the_week_of_june_29_2020/fwe75qg/?context=3&amp;sort=best",
                    Title = "The Role of Precedent in the Supreme Court",
                    Body = new Post.VerbatimBlob("nice test"),
                };

                posts.Add(post3);
                posts.Add(post4);
            }
            return posts;
        }

        public static void CreateTestXML(bool isDataValid)
        {
            var posts=CreateTestCollection(isDataValid);
            XMLDeserIntTestHelper.CreateXMLFromCollection(posts);
        }

        public static void CreateTestDir()
        {

            if (!Directory.Exists(XMLDeserIntTestHelper.IntDestDir))
            {
                Directory.CreateDirectory(XMLDeserIntTestHelper.IntDestDir);
            }
        }

        public static void DeleteTestDir()
        {
            if (Directory.Exists(XMLDeserIntTestHelper.IntDestDir))
            {
                Directory.Delete(XMLDeserIntTestHelper.IntDestDir, true);
            }
        }

        private static void CreateXMLFromCollection(List<Post> posts)
        {
            Directory.CreateDirectory(ValidXMLPath);

            foreach (var post in posts)
            {
                using (var writer = XmlWriter.Create($"{ValidXMLPath}/{post.Author}-{post.Date.ToString("yyyyMMddHHmmss")}.xml"))
                {
                    var ser = new XmlSerializer(typeof(Post));
                    ser.Serialize(writer, post);
                }
            }
        }
    }
}
