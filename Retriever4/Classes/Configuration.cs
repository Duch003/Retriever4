using System;

namespace Retriever4
{
    public class Configuration : IConfiguration
    {
        public string Filepath;
        public string Filename;
        public string DatabaseTableName;
        public string BiosTableName;
        public int? WearLevel;

        public int? DB_Model;
        public int? DB_PeaqModel;
        public int? DB_Storage;
        public int? DB_Ram;
        public int? DB_Cpu;
        public int? DB_OS;
        public int? DB_SWM;
        public int? DB_CaseModel;
        public int? DB_MainboardVendor;
        public int? DB_ShippingMode;

        public int? Bios_CaseModel;
        public int? Bios_MainboardModel;
        public int? Bios_Bios;
        public int? Bios_EC;
        public int? Bios_BuildDate;

        public Configuration() { }

        public bool CheckFieldsForNulls()
        {
            throw new NotImplementedException();
        }
    }
}
