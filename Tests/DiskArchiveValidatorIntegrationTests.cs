using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using NUnit.Framework;
using QCUtilities;

namespace QCVault.Tests
{
    [TestFixture]
    public class DiskArchiveValidatorIntegrationTests
    {

        [OneTimeSetUp]
        public void Setup()
        {
            XMLDeserIntTestHelper.CreateTestDir();
            CreateTestFiles();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            XMLDeserIntTestHelper.DeleteTestDir();
        }

        private void CreateTestFiles()
        {
            CreateInvalidXMLDirectory();
            CreateEmptyDirectory();
            CreateValidXMLDirectory();
            LoadXSDFile();
        }

        private static void CreateValidXMLDirectory()
        {
            XMLDeserIntTestHelper.CreateTestXML(true);
        }

        private void CreateInvalidXMLDirectory()
        {
            var path = Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.INVALIDXML);
            Directory.CreateDirectory(path);

            var invalidFilePath = Path.Combine(path, "invalid.xml");
            using (var writer = XmlWriter.Create(invalidFilePath))
            {
                writer.WriteStartElement("book");
                writer.WriteElementString("title", "Graphics Programming using GDI+");
                writer.WriteElementString("author", "Mahesh Chand");
                writer.WriteElementString("publisher", "Addison-Wesley");
                writer.WriteElementString("price", "64.95");
                writer.WriteEndElement();
                writer.Flush();
            }
        }

        private void CreateEmptyDirectory()
        {
            var invalidFilePath = Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.EMPTYDIR);
            Directory.CreateDirectory(invalidFilePath);
        }

        public static void LoadXSDFile()
        {
            var pathFrom = Path.Combine(XMLDeserIntTestHelper.SchemaFolder, XMLDeserIntTestHelper.SCHEMA);
            var pathTo = Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.SCHEMA);
            File.Copy(pathFrom, pathTo, true);
        }

        [Test]
        public void DirectoryExists_ReturnsFalse()
        {
            var val = new DiskArchiveValidator();
            string randomDirName = "randomStuffAtTheTestDir" + new Random().Next(0, 100000);
            var result = val.DirectoryExists(Path.Combine(XMLDeserIntTestHelper.IntDestDir, randomDirName));
            Assert.IsFalse(result);
        }

        [Test]
        public void DirectoryExists_ReturnsTrue()
        {
            var val = new DiskArchiveValidator();
            var result = val.DirectoryExists(Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.VALIDXML));
            Assert.IsTrue(result);
        }

        [Test]
        public void AFileExists_ReturnsFalse()
        {
            var val = new DiskArchiveValidator();
            var result = val.AFileExists(Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.EMPTYDIR));
            Assert.IsFalse(result);
        }

        [Test]
        public void AFileExists_ReturnsTrue()
        {
            var val = new DiskArchiveValidator();
            var result = val.AFileExists(Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.VALIDXML));
            Assert.IsTrue(result);
        }
    }
}
