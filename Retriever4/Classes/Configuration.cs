using Retriever4.Validation;
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

        public static string Filepath;
        public static string Filename;
        public static string DatabaseTableName;
        public static string BiosTableName;
        public static int WearLevel;

        public static int DB_Model;
        public static int DB_PeaqModel;
        public static int DB_Storage;
        public static int DB_Ram;
        public static int DB_Cpu;
        public static int DB_OS;
        public static int DB_SWM;
        public static int DB_CaseModel;
        public static int DB_MainboardVendor;
        public static int DB_ShippingMode;

        public static int Bios_CaseModel;
        public static int Bios_MainboardModel;
        public static int Bios_Bios;
        public static int Bios_EC;
        public static int Bios_BuildDate;

        public Configuration() { }

        /// <summary>
        /// Checks if every field have assigned value;
        /// </summary>
        /// <returns>False if there are null values.</returns>
        public bool MakeDataStatic()
        {
            if (!ObjectsValidation.CheckFieldsForNulls(this, null) || !ObjectsValidation.CheckFieldsForNegativeNumbers(this, null))
            {
                return false;
            }
            Filepath = filepath;
            Filename = filename;
            DatabaseTableName = databaseTableName;
            BiosTableName = biosTableName;
            WearLevel = (int)wearLevel;
            DB_Model = (int)db_Model;
            DB_PeaqModel = (int)db_PeaqModel;
            DB_Storage = (int)db_Storage;
            DB_Ram = (int)db_Ram;
            DB_Cpu = (int)db_Cpu;
            DB_OS = (int)db_OS;
            DB_SWM = (int)db_SWM;
            DB_CaseModel = (int)db_CaseModel;
            DB_MainboardVendor = (int)db_MainboardVendor;
            DB_ShippingMode = (int)db_ShippingMode;
            Bios_CaseModel = (int)bios_CaseModel;
            Bios_MainboardModel = (int)bios_MainboardModel;
            Bios_Bios = (int)bios_Bios;
            Bios_EC = (int)bios_EC;
            Bios_BuildDate = (int)bios_BuildDate;
            return true;
        }
    }
}
