
using Microsoft.Win32;
using System.Management;
using System.IO;
using System;
using System.Collections.Generic;
using Retriever4.Enums;
using Retriever4.Utilities;
using System.Linq;
using System.Net.NetworkInformation;
using Retriever4.Classes;
using Retriever4.Interfaces;

namespace Retriever4
{
    public class Retriever : IWmiReader
    {
        //Construcor for tests
        public Retriever() { }

        public Dictionary<string, dynamic>[] lastDictionaries { get; private set; }
        /// <summary>
        /// Retrieve data from wmi instaces and save them into dictionaries.
        /// </summary>
        /// <param name="query">WQL query.</param>
        /// <param name="properties">Properties You want to retrieve. If null - retrieve everything.</param>
        /// <param name="scope">Management scope.</param>
        /// <returns>Dictionary where key is the class property and value is an array of property values.</returns>
        private Dictionary<string, dynamic>[] GetDeviceData (string query, string[] properties, string scope = @"root/cimv2")
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
                            if (x != null && x.IsArray)
                                anwser[i].Add(x.Name, (dynamic) z[x.Name] == null ? "" : (dynamic) z[x.Name]);
                            else
                                anwser[i].Add(x.Name, x.Value ?? "");
                        }
                    //If particular properties are given, take only them
                    else
                        //Loop for properties array
                        for (var j = 0; j < properties.Length; j++)
                        {
                            //Check if is property an array
                            if (z[properties[j]] != null && z[properties[j]].GetType().IsArray)
                                anwser[i].Add(properties[j], (dynamic) z[properties[j]] == null ? "" : (dynamic)z[properties[j]]);
                            else
                                anwser[i].Add(properties[j], z[properties[j]] == null ? "" : z[properties[j]]);
                        }
                //Increment dictionary array size
                    i++;
                }
            }
            return anwser;
        }

        public Dictionary<string, dynamic>[] CheckDeviceManager()
            => GetDeviceData("SELECT Caption, ConfigManagerErrorCode FROM Win32_PNPEntity WHERE ConfigManagerErrorCode != 0", 
                new string[] { "Caption", "ConfigManagerErrorCode" }, @"root/cimv2");

        public Dictionary<string, dynamic>[] CheckWirelessConnection()
        {
            var anwser = new Dictionary<string, dynamic>[0];
            var devices = NetworkInterface.GetAllNetworkInterfaces()
                .Where(z => z.NetworkInterfaceType == NetworkInterfaceType.Wireless80211);
            var i = 0;
            foreach (var z in devices)
            {
                anwser = anwser.Expand();
                anwser[i] = new Dictionary<string, dynamic>
                {
                    {"Description", z.Description},
                    {"OperationalStatus", z.OperationalStatus},
                    {"BytesSent", z.GetIPv4Statistics().BytesSent},
                    {"BytesReceived", z.GetIPv4Statistics().BytesReceived}
                };
                i++;
            }

            return anwser;

        }

        public Dictionary<string, string> CheckEthernetInterfaceMAC()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            var ethInterface = interfaces.Where(z => z.NetworkInterfaceType == NetworkInterfaceType.Ethernet).ToArray(); 
            var ans = new Dictionary<string, string>();
            foreach (var z in ethInterface)
            {
                var name = z.Description;
                var mac = z.GetPhysicalAddress().ToString();
                ans.Add(name, mac);
            }
            return ans;
        }

        public Dictionary<string, dynamic>[] CheckWindowsActivationStatus()
            => GetDeviceData("SELECT LicenseStatus FROM SoftwareLicensingProduct WHERE ApplicationID = '55c92734-d682-4d71-983e-d6ec3f16059f' AND PartialProductKey != null",
                new string[] { "LicenseStatus" }, "root/cimv2");

        public SecureBootStatus CheckSecureBootStatus()
        {
            var temp = Registry.GetValue(@"HKEY_LOCAL_MACHINE\SYSTEM\ControlSet001\Control\SecureBoot\State",
               "UEFISecureBootEnabled", 2);
            SecureBootStatus status;
            if (temp == null)
                status = SecureBootStatus.NieWspierany;
            else
                status = (SecureBootStatus)((int)temp);
            return status;
        }

        public Dictionary<string, dynamic>[] GetWindowsProductKey()
            => GetDeviceData("SELECT * FROM SoftwareLicensingService Where OA3xOriginalProductKey != null", new string[] { "OA3xOriginalProductKey" }, @"root/cimv2");

        public string[] GetSwm()
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

        public Dictionary<string, dynamic>[] GetModelString()
            => GetDeviceData("SELECT Model FROM Win32_ComputerSystem", new string[] { "Model" }, @"root/cimv2");

        public Dictionary<string, dynamic>[] MainboardBiosData() 
            => GetDeviceData("SELECT SMBIOSBIOSVersion, ReleaseDate FROM Win32_BIOS", new string[] { "SMBIOSBIOSVersion", "ReleaseDate" });

        public Dictionary<string, dynamic>[] MainboardModel()
        {
            var firstData = GetDeviceData("SELECT Product FROM Win32_BaseBoard", new string[] { "Product" });
            if(firstData.Count() != 1)
            {
                var value = firstData == null ? "null" : firstData.Count().ToString();
                var message = "Bląd podczas pobierania Product z Win32_BaseBoard. Zapytanie zwróciło" +
                    $"nieoczekiwaną liczbę rekordów: {value}. Metoda: {nameof(MainboardModel)}, klasa: Retriever.cs.";
                throw new Exception(message);

            }
            var secondData = GetDeviceData("SELECT OEMStringArray FROM Win32_ComputerSystem", new string[] { "OEMStringArray" });
            if (secondData.Count() != 1)
            {
                var value = secondData == null ? "null" : secondData.Count().ToString();
                var message = "Bląd podczas pobierania OEMStringArray z Win32_ComputerSystem. Zapytanie zwróciło" +
                    $"nieoczekiwaną liczbę rekordów: {value}. Metoda: {nameof(MainboardModel)}, klasa: Retriever.cs.";
                throw new Exception(message);

            }
            Dictionary<string, dynamic>[] ans = new Dictionary<string, dynamic>[1];
            ans[0] = new Dictionary<string, dynamic>();
            ans[0].Add("Product", firstData[0]["Product"]);
            ans[0].Add("OEMStringArray", secondData[0]["OEMStringArray"]);
            return ans;
        }   

        public Dictionary<string, dynamic>[] ProcessorID()
            => GetDeviceData("SELECT Name FROM Win32_Processor", new string[] { "Name" });

        public Dictionary<string, dynamic>[] RamData()
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

        public Dictionary<string, dynamic>[] StorageData()
            => GetDeviceData("SELECT Size FROM Win32_DiskDrive", new string[] { "Size" });

        public Dictionary<string, dynamic>[] BatteriesData()
        {
            var Data = GetDeviceData("SELECT Tag, DesignedCapacity FROM BatteryStaticData", new string[] { "Tag", "DesignedCapacity" }, @"root\wmi");
            var anwser = new Dictionary<string, dynamic>[0];
            if (Data == null || !Data.Any())
            {
                //Dictionary without battery instances
                return anwser;
            }
            var j = 0;
            for (var i = 0; i < Data.Length; i++)
            {
                
                var fullChargeCapacity = GetDeviceData($"SELECT FullChargedCapacity FROM BatteryFullChargedCapacity WHERE Tag = {Data[i]["Tag"]}", new string[] { "FullChargedCapacity" }, @"root\wmi");
                if(fullChargeCapacity == null || fullChargeCapacity.Count() != 1)
                {
                    var value = fullChargeCapacity == null ? "null" : fullChargeCapacity.Count().ToString();
                    var message = "Bląd podczas pobierania FullChargeCapacity z BatteryFullChargedCapacity. Zapytanie zwróciło" +
                        $"nieoczekiwaną liczbę rekordów: {value}. Tag: {Data[i]["Tag"]}. Metoda: {nameof(BatteriesData)}, klasa: Retriever.cs.";
                    throw new Exception(message);
                }
                var currentChargeLevel = GetDeviceData($"SELECT EstimatedChargeRemaining, BatteryStatus FROM Win32_Battery", new string[] { "EstimatedChargeRemaining" , "BatteryStatus" });
                if (fullChargeCapacity == null || fullChargeCapacity.Count() != 1)
                {
                    var value = currentChargeLevel == null ? "null" : currentChargeLevel.Count().ToString();
                    var message = "Bląd podczas pobierania EstimatedChargeRemaining z Win32_Battery. Zapytanie zwróciło" +
                        $"nieoczekiwaną liczbę rekordów: {value}. Metoda: {nameof(BatteriesData)}, klasa: Retriever.cs.";
                    throw new Exception(message);
                }
                anwser = anwser.Expand();
                try
                {
                    anwser[i] = new Dictionary<string, dynamic>
                    {
                        {"Tag", Data[i]["Tag"]},
                        {"DesignedCapacity", Data[i]["DesignedCapacity"] / 1000},
                        {"FullChargedCapacity", fullChargeCapacity[0]["FullChargedCapacity"] / 1000},
                        {"EstimatedChargeRemaining", currentChargeLevel[0]["EstimatedChargeRemaining"]},
                        //Poniższe może wypluć pusty string/null
                        {"Status", BatteryStatucDescription.BatteryStatus((BatteryStatus)((int)currentChargeLevel[0]["BatteryStatus"]))}
                    };
                    anwser[i].Add("Wearlevel", CalculateVearLevel(anwser[i]["FullChargedCapacity"], anwser[i]["DesignedCapacity"]));
                }
                catch(Exception e)
                {
                    string message = e.Message + $" DesignedCapacity: {Data[i]["DesignedCapacity"]}, FullChargeCapacity: {fullChargeCapacity[0]["FullChargedCapacity"]}, CurrentChargeLevel: {currentChargeLevel[0]["EstimatedChargeRemaining"]}, BatteryStatus: {currentChargeLevel[0]["BatteryStatus"]}";
                    throw new Exception(message);
                }
                j++;
            }
            return anwser;

        }

        private double CalculateVearLevel(double fullChargeCapacity, double designedCapacity)
        {
            var rawlevel = 1 - (fullChargeCapacity / designedCapacity);
            var wearlevel = Math.Round(rawlevel, 2);
            return wearlevel;
        }

        public Dictionary<string, dynamic>[] OS()
            => GetDeviceData("SELECT Caption FROM Win32_OperatingSystem", new string[] { "Caption" });

        //public void ActivateWindowsOnline()
        //{
        //    var productSearch = new ManagementObjectSearcher("SELECT * FROM Win32_WindowsProductActivation");

        //    foreach (var o in productSearch.Get())
        //    {
        //        var product = (ManagementObject) o;
        //        var inParams = product.GetMethodParameters("ActivateOnline");
        //        var outParams = product.InvokeMethod("ActivateOnline", inParams, null);
        //    }
        //}
    }
}
