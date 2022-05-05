using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using QCVault.Utilities.Entities;

namespace QCVault.Tests
{
    [TestFixture]
    public class PostTests : LiveServerTests
    {
        [Test]
        public async Task Blockquote()
        {
            var client = factory.CreateClient();

            // This is extremely fragile and relies on example data, but right now that's the easiest way to accomplish this, and if it breaks, well, we'll know about it, yes?
            var response = await client.GetAsync("/post/beware_of_these_7_playtime_mistakes");

            var body = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(body.Contains("<blockquote>nested context test</blockquote>"));
        }

        private Post ReadTestPost(string filename)
        {
            var schemas = new System.Xml.Schema.XmlSchemaSet();
            using (var input = new StreamReader("Posts/posts.xsd"))
            {
                schemas.Add(System.Xml.Schema.XmlSchema.Read(input, (sender, args) => throw args.Exception));
            }

            var deserializer = TestUtil.CreateDeserializer();

            // just gonna reach in here and use a private function, don't mind me . . .
            var readPost = deserializer.GetType().GetMethod("ReadPost", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);

            // I don't like the filename, this should be better. Couldn't find a good solution though. FIXME.
            return readPost.Invoke(deserializer, new object[] { "../../../" + filename, schemas }) as Post;
        }

        [Test]
        public void AdjacentLink()
        {
            var adjacent = ReadTestPost("Tests/AdjacentLink/AdjacentLink.xml");
            Assert.IsFalse(adjacent.Body.contents.Contains("</a><a"));
            Assert.IsTrue(adjacent.Body.contents.Contains("</a> <a"));
        }
    }
}

