using Retriever4.Validation;
using System;
using System.Drawing;
using System.Linq;

namespace Retriever4
{
    public class Configuration
    {
        //Raw data which neet to be checked
        //File info
        public string filepath;
        public string filename;
        public string databaseTableName;
        public string biosTableName;
        public int? wearLevel;
        //Model table info
        public int? db_Model;
        public int? db_PeaqModel;
        public int? db_Storage;
        public int? db_Ram;
        public int? db_Cpu;
        public int? db_OS;
        public int? db_SWM;
        public int? db_CaseModel;
        public int? db_MainboardVendor;
        public int? db_MainboardModel;
        public int? db_ShippingMode;
        public int? db_Tip;
        //Bios table info
        public int? bios_CaseModel;
        public int? bios_MainboardModel;
        public int? bios_Bios;
        public int? bios_BuildDate;
        
        //Console colors info
        public string passColor;
        public string failColor;
        public string warningColor;
        public string headerForegroundColor;
        public string headerBackgroundColor;
        public string separatorColor;
        public string minorInformationColor;
        public string majorInformationColor;
        public string defaultBackgroundColor;
        public string defaultForegroundColor;
        //Checked file info
        public static string Filepath;
        public static string Filename;
        public static string DatabaseTableName;
        public static string BiosTableName;
        public static int WearLevel;
        //Checked model table info
        public static int DB_Model;
        public static int DB_PeaqModel;
        public static int DB_Storage;
        public static int DB_Ram;
        public static int DB_Cpu;
        public static int DB_OS;
        public static int DB_SWM;
        public static int DB_CaseModel;
        public static int DB_MainboardVendor;
        public static int DB_MainboardModel;
        public static int DB_ShippingMode;
        public static int DB_Tip;
        //Checked bios table info
        public static int Bios_CaseModel;
        public static int Bios_MainboardModel;
        public static int Bios_Bios;
        public static int Bios_BuildDate;
        //Checked colors info
        public static Color PassColor;
        public static Color FailColor;
        public static Color WarningColor;
        public static Color HeaderBackgroundColor;
        public static Color HeaderForegroundColor;
        public static Color SeparatorColor;
        public static Color MinorInformationColor;
        public static Color MajorInformationColor;
        public static Color DefaultBackgroundColor;
        public static Color DefaultForegroundColor;

        public Configuration() { }

        /// <summary>
        /// Checks if every field have assigned value;
        /// </summary>
        /// <returns>False if there are null values.</returns>
        public bool MakeDataStatic()
        {
            if (!ObjectsValidation.CheckFieldsForNulls(this, null) || !ObjectsValidation.CheckFieldsForNegativeNumbers(this, 
                GetType().GetFields().Where(z => !z.IsNumericType()).Select(z => z.Name).ToArray()))
                return false;
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
            DB_MainboardModel = (int)db_MainboardModel;
            DB_ShippingMode = (int)db_ShippingMode;
            DB_Tip = (int)db_Tip;
            Bios_CaseModel = (int)bios_CaseModel;
            Bios_MainboardModel = (int)bios_MainboardModel;
            Bios_Bios = (int)bios_Bios;
            Bios_BuildDate = (int)bios_BuildDate;
            PassColor = Color.FromName(passColor);
            FailColor = Color.FromName(failColor);
            WarningColor = Color.FromName(warningColor);
            HeaderBackgroundColor = Color.FromName(headerBackgroundColor);
            HeaderForegroundColor = Color.FromName(headerForegroundColor);
            SeparatorColor = Color.FromName(separatorColor);
            MinorInformationColor = Color.FromName(minorInformationColor);
            MajorInformationColor = Color.FromName(majorInformationColor);
            DefaultBackgroundColor = Color.FromName(defaultBackgroundColor);
            DefaultForegroundColor = Color.FromName(defaultForegroundColor);
            
            return true;
        }
    }
}
