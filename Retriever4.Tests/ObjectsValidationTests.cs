using NUnit.Framework;
using Retriever4.Validation;
using System;
using Retriever4.Tests.Factories;

namespace Retriever4.Tests
{
    [TestFixture]
    public class ObjectsValidationTests
    {
        [Test]
        public void CheckFieldsForNulls_CheckingCorrectlyFilledConfiguration()
        {
            var config = ConfigurationFactory.ConfigurationFullyFilled();

            var shouldBeTrue = ObjectsValidation.CheckFieldsForNulls(config, null);

            Assert.IsTrue(shouldBeTrue);
        }


        [Test]
        public void CheckFieldsForNulls_CheckingObjectWithNullFields()
        {
            var config = ConfigurationFactory.ConfigurationWithNulls();

            var shouldBeFalse = ObjectsValidation.CheckFieldsForNulls(config, null);

            Assert.IsFalse(shouldBeFalse);
        }

        [Test]
        public void CheckFieldsForNulls_NullObject_ThrowsException()
        {
            Assert.Throws<ArgumentNullException>(() => ObjectsValidation.CheckFieldsForNulls(null, null));
        }

        [Test]
        public void CheckFieldsForNulls__CheckingObjectWithUnacceptableNullFieldsWithIgnoredFileds_NullFiledsAreIgnored()
        {
            var config = ConfigurationFactory.ConfigurationWithNulls();
            var ignoredFileds = new[]
            {
                "databaseTableName",
                "db_PeaqModel",
                "db_Cpu",
                "db_ShippingMode"
            };

            var shouldBeTrue = ObjectsValidation.CheckFieldsForNulls(config, ignoredFileds);

            Assert.IsTrue(shouldBeTrue);
        }

        [Test]
        public void CheckFieldsForNulls__IgnoredFieldsArrayContainsNullWithCorrectlyFilledObject_ThrowsException()
        {
            var config = ConfigurationFactory.ConfigurationFullyFilled();
            var ignoredFileds = new[]
            {
                "databaseTableName",
                "db_PeaqModel",
                null,
                "db_ShippingMode"
            };

            Assert.Throws<ArgumentNullException>(() => ObjectsValidation.CheckFieldsForNulls(config, ignoredFileds));
        }

        [Test]
        public void CheckFieldsForNulls__IgnoredFieldsArrayContainsNullWithUncorrectlyFilledObject_ThrowsException()
        {
            var config = ConfigurationFactory.ConfigurationWithNulls();
            var ignoredFileds = new[]
            {
                "databaseTableName",
                "db_PeaqModel",
                null,
                "db_ShippingMode"
            };

            Assert.Throws<ArgumentNullException>(() => ObjectsValidation.CheckFieldsForNulls(config, ignoredFileds));
        }

        [Test]
        public void CheckFieldsForNulls_CheckingObjectFilledByNulls()
        {
            var config = ConfigurationFactory.ConfigurationFullyNull();

            var shouldBeFalse = ObjectsValidation.CheckFieldsForNulls(config, null);

            Assert.IsFalse(shouldBeFalse);
        }
    }

    
}
