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
        public const string INVALIDXML = "invalid.xml";
        public const string EMPTYXML = "empty.xml";
        public const string VALIDXML = "valid.xml";
        public const string EMPTYTXT = "test.adad";
        public const string SCHEMA = "posts.xsd";

        public static string ValidXMLPath => Path.Combine(IntDestDir, VALIDXML);
      
        public static string ValidXSDPath => Path.Combine(IntDestDir, SCHEMA);

        public static PostCollection CreateTestCollection(bool isDataValid = true)
        {
            bool addDuplicateURL = !isDataValid;


            var posts = new PostCollection();
            var post1 = new Post()
            {
                Author = "Cheezemansam",
                Date = DateTime.Parse("2020-06-29T05:17:08+00:00"),
                Link = @"https://www.reddit.com/r/TheMotte/comments/hhtwxi/culture_war_roundup_for_the_week_of_june_29_2020/fwe75qg/?context=3&amp;sort=best",
                Title = "The Role of Precedent in the Supreme Court",
                Body = $@"nice test"
            };
            var post2 = new Post()
            {
                Author = "questionMark",
                Date = DateTime.Parse("2020-06-29T23:32:56+00:00"),
                Link = "https://www.reddit.com/r/TheMotte/comments/hhtwxi/culture_war_roundup_for_the_week_of_june_29_2020/fwf5vss/?context=3&amp;sort=best",
                Title = "The Role of Energy and Entropy in Cultural Conflict",
                Body = $@"nice test 2 have a line too"
            };

            posts.Posts.Add(post1);
            posts.Posts.Add(post2);

            if (addDuplicateURL)
            {
                var post3 = new Post()
                {
                    Author = "Cheezemansam",
                    Date = DateTime.Parse("2020-06-29T05:17:08+00:00"),
                    Link = @"https://www.reddit.com/r/TheMotte/comments/hhtwxi/culture_war_roundup_for_the_week_of_june_29_2020/fwe75qg/?context=3&amp;sort=best",
                    Title = "The Role of Precedent in the Supreme Court",
                    Body = $@"nice test"
                };
                var post4 = new Post()
                {
                    Author = "questionMark",
                    Date = DateTime.Parse("2020-06-29T23:32:56+00:00"),
                    Link = "https://www.reddit.com/r/TheMotte/comments/hhtwxi/culture_war_roundup_for_the_week_of_june_29_2020/fwf5vss/?context=3&amp;sort=best",
                    Title = "The Role of Energy and Entropy in Cultural Conflict",
                    Body = $@"nice test 2 have a line too"
                };

                posts.Posts.Add(post3);
                posts.Posts.Add(post4);
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

        private static void CreateXMLFromCollection(PostCollection posts)
        {
            var path = ValidXMLPath;
            using (var writer = XmlWriter.Create(path))
            {
                var ser = new XmlSerializer(typeof(PostCollection));
                ser.Serialize(writer, posts);
            }
        }
    }
}
