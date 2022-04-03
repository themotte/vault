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
                bool directoryExists = true,
                bool aFileExists = true,
                bool filesValid = true,
                bool collectionContainsUniqueURLS = true
                )
        {
            var fakeFileValidator = Substitute.For<IDiskArchiveValidator>();
            var fakeCollectionValidator = Substitute.For<IPostCollectionValidator>();

            string directoryName = XMLDeserIntTestHelper.ValidXMLPath;
            string xsd = XMLDeserIntTestHelper.ValidXSDPath;

            fakeFileValidator.DirectoryExists(Arg.Any<string>()).Returns(directoryExists);
            fakeFileValidator.AFileExists(Arg.Any<string>()).Returns(aFileExists);
            fakeFileValidator.FilesValid(Arg.Any<string>(), Arg.Any<string>()).Returns(filesValid);


            

            fakeCollectionValidator
                .CollectionContainsUniqueURLS(Arg.Any<List<Post>>(), out Arg.Any<string>())
                .Returns(x =>
                {
                    x[1] = collectionContainsUniqueURLS? "":"Something is wrong!";
                    return collectionContainsUniqueURLS;
                });

            return new PostDeserializer(fakeFileValidator, fakeCollectionValidator, directoryName, xsd);
        }
    }
    [TestFixture]
    public class XMLDeserializerUnitTests
    {
        [TestCase]
        public void DeserializeXML_DirectoryNotExists_Throws()
        {
            Assert.Throws<DirectoryNotFoundException>(() => DeserializerFactory.Create(directoryExists: false));
        }

        [TestCase]
        public void DeserializeXML_FileNotExists_Throws()
        {
            Assert.Throws<FileNotFoundException>(() => DeserializerFactory.Create(aFileExists: false));
        }

        [TestCase]
        public void DeserializeXML_FilesNotValid_Throws()
        {
            Assert.Throws<XmlException>(() => DeserializerFactory.Create(filesValid: false));
        }
    }

    [TestFixture]
    public class XMLDeserializerIntegrationTests
    {
        [OneTimeSetUp]
        public void Setup()
        {
            XMLDeserIntTestHelper.CreateTestDir();
        }

        [OneTimeTearDown]
        public void TearDown()
        {
          
            XMLDeserIntTestHelper.DeleteTestDir();
        }

        [Test]
        public void Deserialize_CompareResultSetToOriginalSet_AreEqual()
        {
            XMLDeserIntTestHelper.CreateTestXML(true);

            var posts = XMLDeserIntTestHelper.CreateTestCollection();
            var deser = DeserializerFactory.Create();
            var result = deser.VisiblePosts();
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
