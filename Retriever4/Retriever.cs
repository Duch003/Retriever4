using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Management;
using System.IO;
using System;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ExcelDataReader;
using Retriever4.Enums;
using Retriever4.Utilities;

namespace Retriever4
{
    public static class Retriever
    {
        //private static Match match;

        public static bool DoesHashFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + "/SHA1.txt");
            }
        }

        public static bool DoestModelListFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + "/Model.xml");
            }
        }

        public static bool DoesConfigFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + @"\Config.xml");
            }
        }

        public static bool DoesSchemaFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + @"\Schema.xml");
            }
        }

        #region Reader methods
        public static bool DoesDatabaseFileExists(string filepath, string filename)
        {
            return File.Exists(filepath + filename);
        }

        public static object ReadDetailsFromDatabase(string filepath, string filename, string tableName, int row, int column)
        {
            if (string.IsNullOrEmpty(filepath) || string.IsNullOrEmpty(filename))
            {
                throw new InvalidDataException("Nie można połączyć się z plikiem excel. Któryś z argumentów kontruktora jest pusty:\n" +
                    $"filepath = {filepath}\n" +
                    $"filename = {filename}");
            }

            object anwser = null;

            try
            {
                using (FileStream stream = new FileStream(filepath + filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    var result = excelReader.AsDataSet();

                    var table = result.Tables[tableName];
                    anwser = table.Rows[row][column];
                }
            }
            catch (Exception e)
            {
                string message = $"Nie udało się utworzyć połączenia z plikiem excel Nie można odczytać bazy.\n\nTreść błędu:\n" +
                    $"{e.Message}\n\n" +
                    $"Wywołania:\n" +
                    $"{e.StackTrace}";
            }

            return anwser;
        }
        #endregion

        #region Gatherer methods
        public static object ReadDetailsFromComputer(string query, string property, string scope = @"root/cimv2")
        {
            object anwser = null;
            ManagementScope _scope = new ManagementScope(scope);
            ObjectQuery _query = new ObjectQuery(query);
            using (ManagementObjectSearcher search = new ManagementObjectSearcher(_scope, _query))
            {
                foreach (var z in search.Get())
                {
                    anwser = z[property];
                }
            }
            return anwser;
        }

        public static IEnumerable<object> ReadArrayFromComputer(string query, string property, string scope = @"root/cimv2")
        {
            using (System.Management.ManagementObjectSearcher search = new System.Management.ManagementObjectSearcher(scope, query))
            {
                foreach (var z in search.Get())
                {
                    if (z[property] != null)
                        yield return z[property];
                    else
                        yield return null;
                }
            }
        }

        public static void _SPECIAL_WearLevelData(string model)
        {
            var scope = @"ROOT\wmi";
            var query = "SELECT * FROM BatteryStaticData";
            using (System.Management.ManagementObjectSearcher search = new System.Management.ManagementObjectSearcher(scope, query))
            {
                foreach (var z in search.Get())
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream(
                            Environment.CurrentDirectory + @"\" + $"{model}.BatteryStaticData.{DateTime.Now.ToString().Replace(@"/", ".").Replace(":", ".")}.txt",
                            FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite)))
                    {
                        foreach (var x in z.Properties)
                        {
                            if (x.Value == null)
                            {
                                sw.WriteLine("{0}: {1}", x.Name.PadRight(50), x.Value);
                                continue;
                            }
                            Type tempType = x?.Value?.GetType();
                            if (tempType.IsArray)
                            {
                                var tempArray = (object[])x.Value;
                                sw.WriteLine($"{x.Name}");
                                foreach (var y in tempArray)
                                    sw.WriteLine("          {y}");
                            }
                            else
                            {
                                sw.WriteLine("{0}: {1}", x.Name.PadRight(50), x.Value);
                            }
                        }
                    }
                }
            }

            query = "SELECT * FROM BatteryFullChargedCapacity";
            using (System.Management.ManagementObjectSearcher search = new System.Management.ManagementObjectSearcher(scope, query))
            {
                foreach (var z in search.Get())
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream(
                            Environment.CurrentDirectory + @"\" + $"{model}.BatteryFullChargedCapacity.{DateTime.Now.ToString().Replace(@"/", ".").Replace(":", ".")}.txt",
                            FileMode.CreateNew, FileAccess.Write, FileShare.ReadWrite)))
                    {
                        foreach (var x in z.Properties)
                        {
                            if (x.Value == null)
                            {
                                sw.WriteLine("{0}: {1}", x.Name.PadRight(50), x.Value);
                                continue;
                            }
                            Type tempType = x.Value.GetType();
                            if (tempType.IsArray)
                            {
                                var tempArray = (object[])x.Value;
                                sw.WriteLine($"{x.Name}");
                                foreach (var y in tempArray)
                                    sw.WriteLine($"          {y}");
                            }
                            else
                            {
                                sw.WriteLine("{0}: {1}", x.Name.PadRight(50), x.Value);
                            }
                        }
                    }
                }
            }
        }

        public static Dictionary<string, DeviceManagerErrorCode> CheckDeviceManager()
        {
            string query = "SELECT Caption, ConfigManagerErrorCode FROM Win32_PNPEntity WHERE ConfigManagerErrorCode != 0";
            string scope = @"root/cimv2";
            Dictionary<string, DeviceManagerErrorCode> ans = new Dictionary<string, DeviceManagerErrorCode>();
            using (System.Management.ManagementObjectSearcher search = new System.Management.ManagementObjectSearcher(scope, query))
            {
                foreach (var z in search.Get())
                {
                    var key = z["Caption"].ToString();
                    var value = (DeviceManagerErrorCode)((uint)z["ConfigManagerErrorCode"]);
                    ans.Add(key, value);
                }
            }
            return ans;
        }

        public static bool CheckWirelessConnection()
        {
            return NetworkInterface.GetIsNetworkAvailable();
        }

        public static void Test()
        {
            Console.WriteLine("Active TCP Connections");

            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            TcpConnectionInformation[] connections = properties.GetActiveTcpConnections();
            foreach (TcpConnectionInformation c in connections)
            {
                Console.WriteLine("{0} <==> {1}",
                    c.LocalEndPoint.ToString(),
                    c.RemoteEndPoint.ToString());
            }
        }

        public static Dictionary<string, string> CheckEthernetInterfaceMAC()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var ethInterface = interfaces.Where(z => z.NetworkInterfaceType == NetworkInterfaceType.Ethernet).ToArray();
            Dictionary<string, string> ans = new Dictionary<string, string>();
            for (int i = 0; i < ethInterface.Length; i++)
            {
                var name = ethInterface[i].Name;
                var mac = ethInterface[i].GetPhysicalAddress().ToString();
                ans.Add(name, mac);
            }
            return ans;
        }

        public static WindowsActivationStatus CheckWindowsActivationStatus()
        {
            string query = "SELECT LicenseStatus FROM SoftwareLicensingProduct WHERE ApplicationID = '55c92734-d682-4d71-983e-d6ec3f16059f' AND PartialProductKey != null";
            string property = "LicenseStatus";
            string scope = "root/cimv2";
            WindowsActivationStatus status = WindowsActivationStatus.NotFound;
            using (System.Management.ManagementObjectSearcher search = new System.Management.ManagementObjectSearcher(scope, query))
            {
                foreach (var z in search.Get())
                {
                    status = (WindowsActivationStatus)((uint)z[property]);
                }
            }
            return status;
        }

        public static SecureBootStatus CheckSecureBootStatus()
        {
            var temp = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\SecureBoot\State",
               "UEFISecureBootEnabled", 2);
            SecureBootStatus status;
            if (temp == null)
                status = SecureBootStatus.NotSupported;
            else
                status = (SecureBootStatus)((int)temp);
            return status;
        }

        public static string GetWindowsProductKey()
        {
            string query = "SELECT * FROM SoftwareLicensingService Where OA3xOriginalProductKey != null";
            string property = "OA3xOriginalProductKey";
            string scope = "root/cimv2";
            string ans = null;
            using (System.Management.ManagementObjectSearcher search = new System.Management.ManagementObjectSearcher(scope, query))
            {
                foreach (var z in search.Get())
                {
                    try
                    {
                        ans = z[property].ToString();
                    }
                    catch (Exception e)
                    {
                        string message = $"Wartosc z[{property}]: {z[property]}";
                        throw new Exception(message);

                    }
                }
            }
            return ans;
        }

        //Pobieranie SWM z plików swconf.dat
        public static string[] GetSwm()
        {
            var i = 0;
            DriveInfo[] AllDrives = DriveInfo.GetDrives();
            string[] ans = new string[0];
            string[] swm = new string[0];
            string[] letter = new string[0];
            foreach (var d in AllDrives)
            {
                //Sprawdza gotowość dysku do odczytu
                if (d.IsReady)
                {
                    //Utwórz instancję pliku swconf.dat na danym dysku
                    var fInfo = new FileInfo(d.Name + "swconf.dat");
                    //Jeżeli pliku nie ma na dysku, przejdź do następnego
                    if (!fInfo.Exists)
                    {
                        continue;
                    }
                    //W innym wypadku odczytaj 3 linię z pliku i dodaj do smiennej SWM
                    else
                    {
                        ans.Expand();
                        swm.Expand();
                        letter.Expand();
                        //Daną wyjściową jest np: D:\12345678
                        swm[i] = $"{File.ReadLines(d.Name + "swconf.dat").Skip(2).Take(1).First()}";
                        letter[i] = $"{d.Name}";
                        ans[i] = letter[i] + ";" + swm[i];
                        i++;
                    }
                }
            }
            if (swm.Length == 0)
            {
                return null;
            }
            return ans;
        }

        public static string AnalyzeForModel()
        {
            object[] model = new object[2];
            string query = "SELECT * FROM Win32_ComputerSystem";

            model[0] = ReadDetailsFromComputer(query, "Model", @"root/cimv2");



            query = "SELECT * FROM Win32_BIOS";
            model[1] = ReadDetailsFromComputer(query, "Manufacturer");

            query = "SELECT * FROM Win32_ComputerSystem";
            string[] oemstring = (string[])ReadDetailsFromComputer(query, "OEMStringArray");
            string[] patterns = new string[]
            {
                @"\d{5}",
                @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z]\d",
                @"[A-Za-z]\d{4}\W[A-Za-z]\d\d[A-Za-z]\d",
                @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z]\d[A-Za-z]",
                @"[A-Za-z]\d{4}\W[A-Za-z][A-Za-z]\d[A-Za-z]"
            };
            for (int i = 0; i < model.Length; i++)
            {
                if (model[i] == null)
                    continue;
                for (int j = 0; j < patterns.Length; j++)
                {
                    Match match = Regex.Match(model[i].ToString(), patterns[j].ToString());
                    if (match.Success)
                    {
                        return match.Value;
                    }
                }
            }
            return null;
        }
        #endregion

        #region Hash management
        public static string ComputeSHA1(string filepath, string filename)
        {
            FileStream stream = new FileStream(filepath + filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(stream);
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string ReadHash()
        {
            string anwser = "";

            if (!File.Exists(Environment.CurrentDirectory + "/SHA1.txt"))
            {
                File.Create(Environment.CurrentDirectory + "/SHA1.txt");
                return anwser;
            }

            using (StreamReader sr = new StreamReader(new FileStream(Environment.CurrentDirectory + "/SHA1.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
            {
                anwser = sr.ReadLine();
            }

            return anwser;
        }

        public static void WriteHash(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                throw new InvalidDataException($"Nie można zapisać hasha do pliku. Argument metody jest pusty: {hash}.");
            using (StreamWriter sw = new StreamWriter(new FileStream(Environment.CurrentDirectory + "/SHA1.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
            {
                sw.WriteLine(hash);
            }
        }
        #endregion

        #region Model list management
        public static void SerializeModelList(Configuration config)
        {
            var temp = GatherModels(config.DatabaseTableName, config.BiosTableName, config.Filepath, 
                config.Filename, (int)config.DB_Model, (int)config.DB_PeaqModel, (int)config.DB_CaseModel, (int)config.Bios_CaseModel);

            var list = new ObservableCollection<Location>(temp.OrderBy(z => z.Model));

            var xs = new XmlSerializer(typeof(ObservableCollection<Location>));

            var sw = new StreamWriter(Environment.CurrentDirectory + @"\Model.xml");

            xs.Serialize(sw, list);

            sw.Close();
        }

        public static ObservableCollection<Location> DeserializeModelList()
        {
            var xs = new XmlSerializer(typeof(ObservableCollection<Location>));

            var sr = new StreamReader(Environment.CurrentDirectory + @"\Model.xml");

            var computerList = xs.Deserialize(sr) as ObservableCollection<Location>;

            sr.Close();
            return computerList;
        }

        private static IEnumerable<Location> GatherModels(string dbTableName, string biosTableName,
            string filepath, string filename, int modelColumn, int peaqModelColumn, int mdCaseModelColumn, int biosCaseModelColumn)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var stream = new FileStream(filepath + filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            var result = excelReader.AsDataSet();

            var modelTable = result.Tables[dbTableName];
            for (var i = 1; i < modelTable.Rows.Count; i++)
            {
                var md = modelTable.Rows[i][modelColumn].ToString();
                if (string.IsNullOrEmpty(md))
                    continue;

                string peaqModel = modelTable.Rows[i][peaqModelColumn].ToString();

                var caseModel = modelTable.Rows[i][mdCaseModelColumn].ToString();

                var biosTable = result.Tables[biosTableName];
                int biosRow = 0;
                for (int j = 0; j < biosTable.Rows.Count; j++)
                {
                    if (biosTable.Rows[j][biosCaseModelColumn].ToString().Contains(caseModel))
                    {
                        biosRow = j;
                    }
                }

                yield return new Location(md, peaqModel, i, biosRow);
            }
        }

        #endregion

        #region Configuration management
        public static Configuration ReadConfiguration()
        {
            if (!DoesConfigFileExists)
                throw new FileNotFoundException("Nie znaleziono pliku konfiguracyjnego Config.xml.");
            Configuration config = new Configuration();

            var xs = new XmlSerializer(typeof(Configuration));

            var sr = new StreamReader(Environment.CurrentDirectory + @"\Config.xml");

            config = xs.Deserialize(sr) as Configuration;

            sr.Close();

            if (string.IsNullOrEmpty(config.Filepath))
                config.Filepath = Environment.CurrentDirectory;

            return config;
        }

        public static void WriteConfiguration()
        {
            Configuration config = new Configuration();
            var xs = new XmlSerializer(typeof(Configuration));

            var sw = new StreamWriter(Environment.CurrentDirectory + @"\Config.xml");

            xs.Serialize(sw, config);

            sw.Close();
        }
        #endregion

        //#region Schema management
        //public static void WriteSchema()
        //{
        //    var obj = new ObservableCollection<Container>
        //    {
        //        new Container()
        //    };

        //    var xs = new XmlSerializer(typeof(ObservableCollection<Container>));

        //    var sw = new StreamWriter(Environment.CurrentDirectory + @"\Schema.xml");

        //    xs.Serialize(sw, obj);

        //    sw.Close();
        //}

        //public static ObservableCollection<Container> ReadSchema()
        //{
        //    if (!DoesConfigFileExists)
        //        throw new FileNotFoundException("Nie znaleziono pliku konfiguracyjnego Schema.xml.");

        //    var xs = new XmlSerializer(typeof(ObservableCollection<Container>));

        //    var sr = new StreamReader(Environment.CurrentDirectory + @"\Schema.xml");

        //    var container = xs.Deserialize(sr) as ObservableCollection<Container>;

        //    sr.Close();

        //    return container;
        //}
        //#endregion

    }
}
