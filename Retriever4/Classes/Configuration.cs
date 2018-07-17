using System;

namespace Retriever4
{
    public class Configuration
    {
        public string filepath;
        public string filename;
        public string databaseTableName;
        public string biosTableName;
        public int? wearLevel;

        public int? db_Model;
        public int? db_PeaqModel;
        public int? db_Storage;
        public int? db_Ram;
        public int? db_Cpu;
        public int? db_OS;
        public int? db_SWM;
        public int? db_CaseModel;
        public int? db_MainboardVendor;
        public int? db_ShippingMode;

        public int? bios_CaseModel;
        public int? bios_MainboardModel;
        public int? bios_Bios;
        public int? bios_EC;
        public int? bios_BuildDate;

        public Configuration() { }
    }
}
