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
    }
}

