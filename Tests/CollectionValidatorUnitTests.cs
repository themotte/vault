using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using QCUtilities;

namespace QCVault.Tests
{
    [TestFixture]
    public class CollectionValidatorUnitTests
    {
        [Test]
        public void PostCollection_UniqueURLS_ReturnsTrue()
        {
            var collectionValidator = new CollectionValidator();
            var posts = XMLDeserIntTestHelper.CreateTestCollection(true).Posts;
            var result = collectionValidator.CollectionContainsUniqueURLS(posts, out string errorMSG);
            Assert.IsTrue(result);
            Assert.IsEmpty(errorMSG);
        }

        [Test]
        public void PostCollection_DuplicateURLS_ReturnsFalse()
        {
            var collectionValidator = new CollectionValidator();
            var posts = XMLDeserIntTestHelper.CreateTestCollection(false).Posts;
            var result = collectionValidator.CollectionContainsUniqueURLS(posts, out string errorMSG);
            Assert.IsFalse(result);
            Assert.IsNotEmpty(errorMSG);
        }
    }
}
