using System;
using System.IO;
using System.Linq;
using System.Xml;
using NSubstitute;
using NUnit.Framework;
using QCUtilities;
using QCUtilities.Interfaces;

namespace QCVault.Tests
{
    public static class DeserializerFactory
    {
        public static XMLDeserializer Create(
                bool validFileName = true,
                bool fileExists = true,
                bool validXMLFile = true,
                bool isXMLSchemaCompliant = true
                )
        {
            var fakeFileValidator = Substitute.For<IXMLFileValidator>();

            fakeFileValidator.IsValidFileName(Arg.Any<string>()).Returns(validFileName);
            fakeFileValidator.FileExists(Arg.Any<string>()).Returns(fileExists);
            fakeFileValidator.IsFileValidXML(Arg.Any<string>()).Returns(validXMLFile);
            fakeFileValidator.IsXMLSchemaCompliant(Arg.Any<string>(), Arg.Any<string>()).Returns(isXMLSchemaCompliant);

            return new XMLDeserializer(fakeFileValidator);

        }
    }
    [TestFixture]
    public class XMLDeserializerUnitTests
    {




        [TestCase]
        public void DeserializeXML_InvalidFileName_Throws()
        {
            var Deserializer = DeserializerFactory.Create(validFileName: false);

            Assert.Throws<ArgumentOutOfRangeException>(() => Deserializer.DeserializeXML("", ""));
        }

        [TestCase]
        public void DeserializeXML_FileNotExists_Throws()
        {
            var Deserializer = DeserializerFactory.Create(fileExists: false);
            Assert.Throws<FileNotFoundException>(() => Deserializer.DeserializeXML("whatever", ""));
        }

        [TestCase]
        public void DeserializeXML_NotAValidXMLFILE_Throws()
        {
            var Deserializer = DeserializerFactory.Create(validXMLFile: false);
            Assert.Throws<InvalidDataException>(() => Deserializer.DeserializeXML("whatever", ""));
        }

        [TestCase]
        public void DeserializeXML_NoSchemaCompliant_Throws()
        {
            var Deserializer = DeserializerFactory.Create(isXMLSchemaCompliant: false);
            Assert.Throws<XmlException>(() => Deserializer.DeserializeXML("whatever", ""));
        }


    }


    [TestFixture]
    public class XMLDeserializerIntegrationTests
    {
        [SetUp]
        public void Setup()
        {
            XMLDeserIntTestHelper.CreateTestDir();
            var posts = XMLDeserIntTestHelper.CreateTestCollection();
            XMLDeserIntTestHelper.CreateXMLFromCollection(posts);
        }
        [TearDown]
        public void TearDown()
        {
            XMLDeserIntTestHelper.DeleteTestDir();
        }

        [Test]
        public void Deserialize_CompareResultSetToOriginalSet_AreEqual()
        {
            var posts = XMLDeserIntTestHelper.CreateTestCollection().Posts;
            var deser = DeserializerFactory.Create();
            var result = deser.DeserializeXML(XMLDeserIntTestHelper.ValidXMLPath, XMLDeserIntTestHelper.ValidXSDPath);
            Assert.That(posts.SequenceEqual(result));
        }
    }

}
