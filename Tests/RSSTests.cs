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
    public class RSSTests : LiveServerTests
    {
        [Test]
        public async Task VerifyRSSLinksExist()
        {
            var client = factory.CreateClient();
            var response = await client.GetAsync("/rss");
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            Assert.AreEqual("application/rss+xml; charset=utf-8",
                response.Content.Headers.ContentType.ToString());
            var rss = await response.Content.ReadAsStringAsync();

            var des = TestUtil.CreateDeserializer();

            var posts = des.Posts;
            foreach (var post in posts)
            {
                Assert.IsTrue(rss.Contains(post.FullURL));
            }
        }
    }
}

