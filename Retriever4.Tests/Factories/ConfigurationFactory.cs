using System;

namespace Retriever4.Tests.Factories
{
    public class ConfigurationFactory
    {
        public static Configuration ConfigurationFullyFilled()
        {
            var config = new Configuration
            {
                //filepath = Environment.CurrentDirectory,
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
                bios_MainboardModel = 5

            };
            return config;
        }

        public static Configuration ConfigurationWithNulls()
        {
            var config = new Configuration
            {
                //filepath = Environment.CurrentDirectory,
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
                bios_MainboardModel = 5
            };
            return config;
        }

        public static Configuration ConfigurationWithNegativeNumbers()
        {
            var config = new Configuration
            {
                //filepath = Environment.CurrentDirectory,
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
