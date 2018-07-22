using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Management;
using System.IO;
using System;
using System.Collections.Generic;
using Retriever4.Enums;
using Retriever4.Utilities;
using System.Linq;

namespace Retriever4
{
    public class Retriever
    {
        //Construcor for tests
        public Retriever() { }

        /// <summary>
        /// Retrieve data from wmi instaces and save them into dictionaries.
        /// </summary>
        /// <param name="query">WQL query.</param>
        /// <param name="properties">Properties You want to retrieve. If null - retrieve everything.</param>
        /// <param name="scope">Management scope.</param>
        /// <returns>Dictionary where key is the class property and value is an array of property values.</returns>
        private static Dictionary<string, dynamic>[] GetDeviceData (string query, string[] properties, string scope = @"root/cimv2")
        {
            var anwser = new Dictionary<string, dynamic>[0];
            //Creating an object searcher
            using(var searcher = new ManagementObjectSearcher(scope, query))
            {
                //Iterator for every object gathered by ManagementObjectSearcher
                var i = 0;
                //Get class instances
                foreach (var z in searcher.Get())
                {
                    //For each instance create different dictionary
                    anwser = anwser.Expand();
                    anwser[i] = new Dictionary<string, dynamic>();
                    //If there arent particular properties given, take everything
                    if (properties == null || properties?.Length == 0)
                        foreach (var x in z.Properties)
                        {
                            //Check if is property an array
                            if (x.IsArray)
                                anwser[i].Add(x.Name, (dynamic)z[x.Name] == null ? "" : (dynamic)z[x.Name]);
                            else
                                anwser[i].Add(x.Name, x.Value == null ? "" : x.Value);
                        }
                    //If particular properties are given, take only them
                    else
                        //Loop for properties array
                        for (var j = 0; j < properties.Length; j++)
                        {
                            //Check if is property an array
                            if (z.Properties[properties[j]].IsArray)
                                anwser[i].Add(properties[j], (dynamic)z[properties[j]] == null ? "" : (dynamic)z[properties[j]]);
                            else
                                anwser[i].Add(properties[j], z[properties[j]] == null ? "" : z[properties[j]]);
                        }
                    //Increment dictionary array size
                    i++;
                }
            }
            return anwser;
        }

        public static Dictionary<string, dynamic>[] CheckDeviceManager()
            => GetDeviceData("SELECT Caption, ConfigManagerErrorCode FROM Win32_PNPEntity WHERE ConfigManagerErrorCode != 0", 
                new string[] { "Caption", "ConfigManagerErrorCode" }, @"root/cimv2");

        public static bool CheckWirelessConnection() => NetworkInterface.GetIsNetworkAvailable();

        public static Dictionary<string, string> CheckEthernetInterfaceMAC()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var ethInterface = interfaces.Where(z => z.NetworkInterfaceType == NetworkInterfaceType.Ethernet).ToArray();
            var ans = new Dictionary<string, string>();
            for (var i = 0; i < ethInterface.Length; i++)
            {
                var name = ethInterface[i].Name;
                var mac = ethInterface[i].GetPhysicalAddress().ToString();
                ans.Add(name, mac);
            }
            return ans;
        }

        public static Dictionary<string, dynamic>[] CheckWindowsActivationStatus()
            => GetDeviceData("SELECT LicenseStatus FROM SoftwareLicensingProduct WHERE ApplicationID = '55c92734-d682-4d71-983e-d6ec3f16059f' AND PartialProductKey != null",
                new string[] { "LicenseStatus" }, "root/cimv2");

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

        public static Dictionary<string, dynamic>[] GetWindowsProductKey()
            => GetDeviceData("SELECT * FROM SoftwareLicensingService Where OA3xOriginalProductKey != null", new string[] { "OA3xOriginalProductKey" }, @"root/cimv2");

        public static string[] GetSwm()
        {
            var i = 0;
            var AllDrives = DriveInfo.GetDrives();
            var ans = new string[0];
            var swm = new string[0];
            var letter = new string[0];
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
                        ans = ans.Expand();
                        swm = swm.Expand();
                        letter = letter.Expand();
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

        public static Dictionary<string, dynamic>[] GetModelString()
            => GetDeviceData("SELECT Model FROM Win32_ComputerSystem", new string[] { "Model" }, @"root/cimv2");

        public static Dictionary<string, dynamic>[] MainboardBiosData() 
            => GetDeviceData("SELECT SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS", new string[] { "SMBIOSBIOSVersion", "ReleaseDate" });

        public static Dictionary<string, dynamic>[] MainboardModel()
            => GetDeviceData("SELECT Product FROM Win32_BaseBoard", new string[] { "Product" });

        public static Dictionary<string, dynamic>[] ProcessorID()
            => GetDeviceData("SELECT Name FROM Win32_Processor", new string[] { "Name" });

        public static Dictionary<string, dynamic>[] RamData()
        {
            var result = GetDeviceData("SELECT Capacity FROM Win32_PhysicalMemory", new string[] { "Capacity" });
            var anwser = new Dictionary<string, dynamic>[1];
            anwser[0] = new Dictionary<string, dynamic>();
            long total = 0;
            for (var i = 0; i < result.Length; i++)
            {
                total += (long)result[i]["Capacity"];
                anwser[0].Add($"MemoryChip{i}", result[i]["Capacity"]);
            }
            anwser[0].Add("Total", total);
            return anwser;
        }

        public static Dictionary<string, dynamic>[] StorageData()
            => GetDeviceData("SELECT Size FROM Win32_DiskDrive", new string[] { "Size" });

        public static Dictionary<string, dynamic>[] BatteriesData()
        {
            var staticData = GetDeviceData("SELECT Tag, DesignedCapacity FROM BatteryStaticData", new string[] { "Tag", "DesignedCapacity" }, @"root\wmi");
            var anwser = new Dictionary<string, dynamic>[0];
            if (staticData == null || staticData.Count() == 0)
            {
                //Dictionary without battery instances
                return anwser;
            }
            var j = 0;
            for (var i = 0; i < staticData.Length; i++)
            {
                
                var fullChargeCapacity = GetDeviceData($"SELECT FullChargedCapacity FROM BatteryFullChargedCapacity WHERE Tag = {staticData[i]["Tag"]}", new string[] { "FullChargedCapacity" }, @"root\wmi");
                if(fullChargeCapacity == null || fullChargeCapacity.Count() != 1)
                {
                    var value = fullChargeCapacity == null ? "null" : fullChargeCapacity.Count().ToString();
                    var message = "Bląd podczas pobierania FullChargeCapacity z BatteryFullChargedCapacity. Zapytanie zwróciło" +
                        $"nieoczekiwaną liczbę rekordów: {value}. Tag: {staticData[i]["Tag"]}. Metoda: {nameof(BatteriesData)}, klasa: Retriever.cs.";
                    throw new Exception(message);
                }
                var currentChargeLevel = GetDeviceData($"SELECT EstimatedChargeRemaining FROM Win32_Battery", new string[] { "EstimatedChargeRemaining" });
                if (fullChargeCapacity == null || fullChargeCapacity.Count() != 1)
                {
                    var value = currentChargeLevel == null ? "null" : currentChargeLevel.Count().ToString();
                    var message = "Bląd podczas pobierania EstimatedChargeRemaining z Win32_Battery. Zapytanie zwróciło" +
                        $"nieoczekiwaną liczbę rekordów: {value}. Metoda: {nameof(BatteriesData)}, klasa: Retriever.cs.";
                    throw new Exception(message);
                }
                anwser = anwser.Expand();
                anwser[i] = new Dictionary<string, dynamic>();
                anwser[i].Add("Tag", staticData[i]["Tag"]);
                anwser[i].Add("DesignedCapacity", staticData[i]["DesignedCapacity"] / 1000);
                anwser[i].Add("FullChargedCapacity", fullChargeCapacity[0]["FullChargedCapacity"] / 1000);
                anwser[i].Add("EstimatedChargeRemaining", currentChargeLevel[0]["EstimatedChargeRemaining"]);
                anwser[i].Add("Wearlevel", CalculateVearLevel(anwser[i]["FullChargedCapacity"], anwser[i]["DesignedCapacity"]));
                j++;
            }
            return anwser;

        }

        private static double CalculateVearLevel(double fullChargeCapacity, double designedCapacity)
        {
            var rawlevel = 1 - (fullChargeCapacity / designedCapacity);
            var wearlevel = Math.Round(rawlevel, 2);
            return wearlevel;
        }

        public static Dictionary<string, dynamic>[] OS()
            => GetDeviceData("SELECT Caption FROM Win32_OperatingSystem", new string[] { "Caption" });
    }
}
