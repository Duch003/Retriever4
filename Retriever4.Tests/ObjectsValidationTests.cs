using NUnit.Framework;
using Retriever4.Validation;
using System;

namespace Retriever4.Tests
{
    [TestFixture]
    public class ObjectsValidationTests
    {
        [Test]
        public void CheckFieldsForNulls_CheckingCorrectlyFilledConfiguration()
        {
            var config = ObjectFactory.ConfigurationFullyFilled();

            var shouldBeTrue = ObjectsValidation.CheckFieldsForNulls(config, null);

            Assert.IsTrue(shouldBeTrue);
        }


        [Test]
        public void CheckFieldsForNulls_CheckingObjectWithNullFields()
        {
            var config = ObjectFactory.ConfigurationWithNulls();

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
            var config = ObjectFactory.ConfigurationWithNulls();
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
            var config = ObjectFactory.ConfigurationFullyFilled();
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
            var config = ObjectFactory.ConfigurationWithNulls();
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
            var config = ObjectFactory.ConfigurationFullyNull();

            var shouldBeFalse = ObjectsValidation.CheckFieldsForNulls(config, null);

            Assert.IsFalse(shouldBeFalse);
        }
    }

    public class ObjectFactory
    {
        public static Configuration ConfigurationFullyFilled()
        {
            var config = new Configuration
            {
                filepath = Environment.CurrentDirectory,
                filename = "TestBase.xlsx",
                databaseTableName = "DB",
                biosTableName = "BIOS",
                db_Model = 0,
                db_PeaqModel = 1,
                db_CaseModel = 17,
                db_Cpu = 7,
                db_MainboardVendor = 6,
                db_OS = 4,
                db_Ram = 5,
                db_ShippingMode = 16,
                db_Storage = 9,
                db_SWM = 8,
                wearLevel = 12,
                bios_Bios = 1,
                bios_BuildDate = 2,
                bios_CaseModel = 3,
                bios_EC = 4,
                bios_MainboardModel = 5
                
            };
            return config;
        }

        public static Configuration ConfigurationWithNulls()
        {
            var config = new Configuration
            {
                filepath = Environment.CurrentDirectory,
                filename = "TestBase.xlsx",
                databaseTableName = null,
                biosTableName = "BIOS",
                db_Model = 0,
                db_PeaqModel = null,
                db_CaseModel = 17,
                db_Cpu = null,
                db_MainboardVendor = 6,
                db_OS = 4,
                db_Ram = 5,
                db_ShippingMode = null,
                db_Storage = 9,
                db_SWM = 8,
                wearLevel = 12,
                bios_Bios = 1,
                bios_BuildDate = 2,
                bios_CaseModel = 3,
                bios_EC = 4,
                bios_MainboardModel = 5
            };
            return config;
        }

        public static Configuration ConfigurationWithNegativeNumbers()
        {
            var config = new Configuration
            {
                filepath = Environment.CurrentDirectory,
                filename = "TestBase.xlsx",
                databaseTableName = "Test",
                biosTableName = "BIOS",
                db_Model = 0,
                db_PeaqModel = -5,
                db_CaseModel = 17,
                db_Cpu = -7,
                db_MainboardVendor = 6,
                db_OS = 4,
                db_Ram = 5,
                db_ShippingMode = -3,
                db_Storage = 9,
                db_SWM = 8,
                wearLevel = 12,
                bios_Bios = 1,
                bios_BuildDate = 2,
                bios_CaseModel = 3,
                bios_EC = 4,
                bios_MainboardModel = 5
            };
            return config;
        }

        public static Configuration ConfigurationFullyNull()
        {
            return new Configuration();
        }
    }
}
