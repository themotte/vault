using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using QCUtilities;

namespace QCVault.Tests
{
    [TestFixture]
    public class PageValidationTests : LiveServerTests
    {
        public PostDeserializer CreateDeserializer()
        {
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posts", "posts");
            string xsd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posts", "posts.xsd");
            return new PostDeserializer(new DiskArchiveValidator(), new CollectionValidator(), xmlPath, xsd);
        }
        [TestCase("/")]
        [TestCase("/About")]
        [TestCase("/Error")]
        [TestCase("/PostList")]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("text/html; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
        }

        [Test]
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType_ForPosts()
        {
            var client = factory.CreateClient();
            
            var des = CreateDeserializer();

            var posts = des.Posts;
            foreach (var post in posts)
            {
                string url = $@"/post/{post.URLSlug}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode(); // Status Code 200-299
                Assert.AreEqual("text/html; charset=utf-8",
                    response.Content.Headers.ContentType.ToString());
            }
        }

        [TestCase("/GibberishForever")]
        [TestCase("/post/GibberishForever")]
        public async Task Get_EndpointsReturnFailure_ForInvalidPost(string url)
        {
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert

            
            Assert.AreEqual(null,
                response.Content.Headers.ContentType);
            Assert.AreEqual(response.IsSuccessStatusCode, false);
            Assert.AreEqual(response.StatusCode, System.Net.HttpStatusCode.NotFound);
        }
    }
}

