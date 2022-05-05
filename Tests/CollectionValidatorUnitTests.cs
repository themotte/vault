using NUnit.Framework;
using QCVault.Utilities.Services;

namespace QCVault.Tests
{
    [TestFixture]
    public class CollectionValidatorUnitTests
    {
        [Test]
        public void PostCollection_UniqueURLS_ReturnsTrue()
        {
            var collectionValidator = new CollectionValidator();
            var posts = XMLDeserIntTestHelper.CreateTestCollection(true);
            var result = collectionValidator.CollectionContainsUniqueURLS(posts, out string errorMSG);
            Assert.IsTrue(result);
            Assert.IsEmpty(errorMSG);
        }

        [Test]
        public void PostCollection_DuplicateURLS_ReturnsFalse()
        {
            var collectionValidator = new CollectionValidator();
            var posts = XMLDeserIntTestHelper.CreateTestCollection(false);
            var result = collectionValidator.CollectionContainsUniqueURLS(posts, out string errorMSG);
            Assert.IsFalse(result);
            Assert.IsNotEmpty(errorMSG);
        }
    }
}
