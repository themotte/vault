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
    public class FileValidatorUnitTests
    {
        [TestCase(@"C:\Users\User\invalidfileName")]
        public void IsValidFileName_InvalidName_ReturnsFalse(string fileName)
        {
            var fileVal = new FileValidator();
            var result = fileVal.IsValidFileName(fileName);
            Assert.IsFalse(result);
        }

        [TestCase(@"C:\Users\User\validFileName.xml")]
        public void IsValidFileName_ValidName_ReturnsTrue(string fileName)
        {
            var fileVal = new FileValidator();
            var result = fileVal.IsValidFileName(fileName);
            Assert.IsTrue(result);
        }
    }

    [TestFixture]
    public class FileValidatorIntegrationTests
    {

        [SetUp]
        public void Setup()
        {
            XMLDeserIntTestHelper.CreateTestDir();
            CreateTestFiles();
        }

        [TearDown]
        public void TearDown()
        {
            XMLDeserIntTestHelper.DeleteTestDir();
        }

        private void CreateTestFiles()
        {
            CreateInvalidXMLFile();
            CreateEmptyXMLFile();
            CreateValidXMLFile();
            CreateEmptyTXTFile();
            LoadXSDFile();
        }

        private static void CreateValidXMLFile()
        {
            XMLDeserIntTestHelper.CreateTestXML(true);
        }

        private void CreateInvalidXMLFile()
        {
            var invalidFilePath = Path.Combine(XMLDeserIntTestHelper.IntDestDir, "invalid.xml");
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

        private void CreateEmptyXMLFile()
        {
            var invalidFilePath = Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.EMPTYXML);
            using (var writer = XmlWriter.Create(invalidFilePath))
            {
                ;
            }
        }

        private void CreateEmptyTXTFile()
        {
            File.Create(Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.EMPTYTXT)).Close();
        }

        private void LoadXSDFile()
        {
            var pathFrom = Path.Combine(XMLDeserIntTestHelper.SchemaFolder, XMLDeserIntTestHelper.SCHEMA);
            var pathTo = Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.SCHEMA);
            File.Copy(pathFrom, pathTo, true);
        }

        [Test]
        public void FileValidator_FileNotExists_ReturnsFalse()
        {
            var val = new FileValidator();
            string randomFileName = "randomStuffAtTheTestDir" + new Random().Next(0, 100000);
            var result = val.FileExists(Path.Combine(XMLDeserIntTestHelper.IntDestDir, randomFileName));
            Assert.IsFalse(result);
        }

        [Test]
        public void FileValidator_FileExists_ReturnsTrue()
        {
            var val = new FileValidator();
            var result = val.FileExists(Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.VALIDXML));
            Assert.IsTrue(result);
        }

        [Test]
        public void IsFileValidXML_NotValid_ReturnsFalse()
        {
            var val = new FileValidator();
            var result1 = val.IsFileValidXML(Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.EMPTYXML));
            var result2 = val.IsFileValidXML(Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.EMPTYTXT));
            var result = result1 && result2;
            Assert.IsFalse(result);
        }

        [Test]
        public void IsFileValidXML_Valid_ReturnsTrue()
        {
            var val = new FileValidator();
            var result = val.IsFileValidXML(Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.VALIDXML));
            Assert.IsTrue(result);
        }


        [Test]
        public void IsXMLSchemaCompliant_NotCompliant_ReturnsFalse()
        {
            var val = new FileValidator();
            var xmlPath = Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.INVALIDXML);
            var schemaPath = Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.SCHEMA);
            var result = val.IsXMLSchemaCompliant(xmlPath, schemaPath);
            Assert.IsFalse(result);
        }

        [Test]
        public void IsXMLSchemaCompliant_Compliant_ReturnsTrue()
        {
            var val = new FileValidator();
            var xmlPath = Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.VALIDXML);
            var schemaPath = Path.Combine(XMLDeserIntTestHelper.IntDestDir, XMLDeserIntTestHelper.SCHEMA);
            var result = val.IsXMLSchemaCompliant(xmlPath, schemaPath);
            Assert.IsTrue(result);
        }


    }
}
