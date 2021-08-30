
using System;
using System.IO;
using QCUtilities;

namespace QCVault.Tests
{
    public static class TestUtil
    {
        public static PostDeserializer CreateDeserializer()
        {
            string xmlPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posts", "posts");
            string xsd = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Posts", "posts.xsd");
            return new PostDeserializer(new DiskArchiveValidator(), new CollectionValidator(), xmlPath, xsd);
        }
    }
}