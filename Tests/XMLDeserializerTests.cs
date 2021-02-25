using System;
using System.IO;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using QCUtilities;
using QCUtilities.Interfaces;

namespace QCVault.Tests
{
    [TestFixture]
    public class XMLDeserializerUnitTests
    {

        public XMLDeserializer DeserializerFactory(
            bool validFileName = true
           , bool fileExists = true
           , bool validXMLFile = true
            , bool isXMLSchemaCompliant = true
            )
        {
            var fakeFileValidator = Substitute.For<IXMLFileValidator>();

            fakeFileValidator.IsValidFileName(Arg.Any<string>()).Returns(validFileName);
            fakeFileValidator.FileExists(Arg.Any<string>()).Returns(fileExists);
            fakeFileValidator.IsFileValidXML(Arg.Any<string>()).Returns(validXMLFile);
            fakeFileValidator.IsFileValidXML(Arg.Any<string>()).Returns(validXMLFile);

            return new XMLDeserializer(fakeFileValidator);
        }


        [TestCase]
        public void DeserializeXML_InvalidFileName_Throws()
        {
            var Deserializer = DeserializerFactory(validFileName: false);

            Assert.Throws<ArgumentOutOfRangeException>(()=> Deserializer.DeserializeXML(""));
         //   Assert.throw
        }

        [TestCase]
        public void DeserializeXML_FileNotExists_Throws()
        {
            var Deserializer = DeserializerFactory(fileExists: false);
            Assert.Throws<FileNotFoundException>(() => Deserializer.DeserializeXML("whatever"));
        }

        [TestCase]
        public void DeserializeXML_NotAValidXMLFILE_Throws()
        {
            var Deserializer = DeserializerFactory(validXMLFile: false);
            Assert.Throws<InvalidDataException>(() => Deserializer.DeserializeXML("whatever"));
        }


    }


    [TestFixture]
    public class XMLDeserializerIntegrationTests
    {

        public XMLDeserializer DeserializerFactory(
            bool validFileName = true
           , bool fileExists = true
           , bool validXMLFile = true
            , bool isXMLSchemaCompliant = true
            )
        {
            var fakeFileValidator = Substitute.For<IXMLFileValidator>();

            fakeFileValidator.IsValidFileName(Arg.Any<string>()).Returns(validFileName);
            fakeFileValidator.FileExists(Arg.Any<string>()).Returns(fileExists);
            fakeFileValidator.IsFileValidXML(Arg.Any<string>()).Returns(validXMLFile);
            fakeFileValidator.IsFileValidXML(Arg.Any<string>()).Returns(validXMLFile);

            return new XMLDeserializer(fakeFileValidator);
        }

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
            var posts = XMLDeserIntTestHelper.CreateTestCollection().ToList();
            var deser = DeserializerFactory();
            var result = deser.DeserializeXML(XMLDeserIntTestHelper.ValidXMLPath);
            Assert.That(posts.SequenceEqual(result));
        }
    }

}
