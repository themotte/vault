using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using NUnit.Framework;
using QCVault;

namespace QCVault.Tests
{
    [TestFixture]
    public class LiveServerTests
    {
        public WebApplicationFactory<QCVault.Startup> factory;

        [OneTimeSetUp]
        public void Setup()
        {
            factory =
                new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder => builder.UseSolutionRelativeContentRoot(""));
        }

        [OneTimeTearDown]
        public void Teardown()
        {
            factory?.Dispose();
        }
    }
}

