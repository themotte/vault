using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using NSubstitute;
using NUnit.Framework;
using QCUtilities;
using QCUtilities.Entities;
using QCUtilities.Interfaces;

namespace QCVault.Tests
{
    public static class DeserializerFactory
    {
        public static PostDeserializer Create(
                bool validFileName = true,
                bool fileExists = true,
                bool validXMLFile = true,
                bool isXMLSchemaCompliant = true,
                bool collectionContainsUniqueURLS = true
                )
        {
            var fakeFileValidator = Substitute.For<IXMLFileValidator>();
            var fakeCollectionValidator = Substitute.For<IPostCollectionValidator>();

            string fileName = XMLDeserIntTestHelper.ValidXMLPath;
            string xsd = XMLDeserIntTestHelper.ValidXSDPath;

            fakeFileValidator.IsValidFileName(Arg.Any<string>()).Returns(validFileName);
            fakeFileValidator.FileExists(Arg.Any<string>()).Returns(fileExists);
            fakeFileValidator.IsFileValidXML(Arg.Any<string>()).Returns(validXMLFile);
            fakeFileValidator.IsXMLSchemaCompliant(Arg.Any<string>(), Arg.Any<string>()).Returns(isXMLSchemaCompliant);


            

            fakeCollectionValidator
                .CollectionContainsUniqueURLS(Arg.Any<List<Post>>(), out Arg.Any<string>())
                .Returns(x =>
                {
                    x[1] = collectionContainsUniqueURLS? "":"Something is wrong!";
                    return collectionContainsUniqueURLS;
                });

            return new PostDeserializer(fakeFileValidator, fakeCollectionValidator, fileName,xsd);
        }
    }
    [TestFixture]
    public class XMLDeserializerUnitTests
    {
        [TestCase]
        public void DeserializeXML_InvalidFileName_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => DeserializerFactory.Create(validFileName: false));
        }

        [TestCase]
        public void DeserializeXML_FileNotExists_Throws()
        {
            Assert.Throws<FileNotFoundException>(() => DeserializerFactory.Create(fileExists: false));
        }

        [TestCase]
        public void DeserializeXML_NotAValidXMLFILE_Throws()
        {
            Assert.Throws<InvalidDataException>(() => DeserializerFactory.Create(validXMLFile: false));
        }

        [TestCase]
        public void DeserializeXML_NoSchemaCompliant_Throws()
        {
            Assert.Throws<XmlException>(() => DeserializerFactory.Create(isXMLSchemaCompliant: false));
        }
    }

    [TestFixture]
    public class XMLDeserializerIntegrationTests
    {
        [SetUp]
        public void Setup()
        {
            XMLDeserIntTestHelper.CreateTestDir();
        }
        [TearDown]
        public void TearDown()
        {
          
            XMLDeserIntTestHelper.DeleteTestDir();
        }

        [Test]
        public void Deserialize_CompareResultSetToOriginalSet_AreEqual()
        {
            XMLDeserIntTestHelper.CreateTestXML(true);

            var posts = XMLDeserIntTestHelper.CreateTestCollection().Posts;
            var deser = DeserializerFactory.Create();
            var result = deser.Posts;
            Assert.That(posts.SequenceEqual(result));
        }

        [TestCase]
        public void DeserializeXML_DuplicateURLS_Throws()
        {
            XMLDeserIntTestHelper.CreateTestXML(false);
            Assert.Throws<FileLoadException>(() => DeserializerFactory.Create(collectionContainsUniqueURLS: false));
        }
    }

}
