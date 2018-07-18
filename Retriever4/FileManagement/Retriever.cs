﻿using System.Linq;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Management;
using System.IO;
using System;
using System.Collections.Generic;
using Retriever4.Enums;
using Retriever4.Utilities;

namespace Retriever4
{
    public static class Retriever
    {
        /// <summary>
        /// Retrieve data from wmi instaces and save them into dictionaries.
        /// </summary>
        /// <param name="query">WQL query.</param>
        /// <param name="properties">Properties You want to retrieve. If null - retrieve everything.</param>
        /// <param name="scope">Management scope.</param>
        /// <returns>Dictionary where key is the class property and value is an array of property values.</returns>
        public static Dictionary<string, dynamic>[] GetDeviceData (string query, string[] properties, string scope = @"root/cimv2")
        {
            Dictionary<string, dynamic>[] anwser = new Dictionary<string, dynamic>[0];
            //Creating an object searcher
            using(ManagementObjectSearcher searcher = new ManagementObjectSearcher(scope, query))
            {
                //Iterator for every object gathered by ManagementObjectSearcher
                int i = 0;
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
                                anwser[i].Add(x.Name, (dynamic)z[x.Name]);
                            else
                                anwser[i].Add(x.Name, x.Value );
                        }
                    //If particular properties are given, take only them
                    else
                        //Loop for properties array
                        for (int j = 0; j < properties.Length; j++)
                        {
                            //Check if is property an array
                            if (z.Properties[properties[j]].IsArray)
                                anwser[i].Add(properties[j], (dynamic)z[properties[j]]);
                            else
                                anwser[i].Add(properties[i], z[properties[i]]);
                        }
                    //Increment dictionary array size
                    i++;
                }
            }
            return anwser;
        }

        public static void _SPECIAL_WearLevelData(string title)
        {
            var scope = @"ROOT\wmi";
            var query = "SELECT * FROM BatteryStaticData";
            using (ManagementObjectSearcher search = new ManagementObjectSearcher(scope, query))
            {
                foreach (var z in search.Get())
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream(
                            Environment.CurrentDirectory + @"\" + $"{title}.BatteryStaticData.{DateTime.Now.ToString().Replace(@"/", ".").Replace(":", ".")}.txt",
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
            using (ManagementObjectSearcher search = new ManagementObjectSearcher(scope, query))
            {
                foreach (var z in search.Get())
                {
                    using (StreamWriter sw = new StreamWriter(new FileStream(
                            Environment.CurrentDirectory + @"\" + $"{title}.BatteryFullChargedCapacity.{DateTime.Now.ToString().Replace(@"/", ".").Replace(":", ".")}.txt",
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

        /// <summary>
        /// Checks if any device throws error.
        /// </summary>
        /// <returns>Dictionary of devices names with theier error messages. Empty if everything is fine.</returns>
        public static Dictionary<string, DeviceManagerErrorCode> CheckDeviceManager()
        {
            string query = "SELECT Caption, ConfigManagerErrorCode FROM Win32_PNPEntity WHERE ConfigManagerErrorCode != 0";
            string scope = @"root/cimv2";
            Dictionary<string, DeviceManagerErrorCode> ans = new Dictionary<string, DeviceManagerErrorCode>();
            using (ManagementObjectSearcher search = new ManagementObjectSearcher(scope, query))
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

        //public static WindowsActivationStatus CheckWindowsActivationStatus()
        //{
        //    string query = "SELECT LicenseStatus FROM SoftwareLicensingProduct WHERE ApplicationID = '55c92734-d682-4d71-983e-d6ec3f16059f' AND PartialProductKey != null";
        //    string property = "LicenseStatus";
        //    string scope = "root/cimv2";

        //    object ans = ReadDetailsFromComputer(query, property, scope);
        //    WindowsActivationStatus status = WindowsActivationStatus.NotFound;
        //    using (ManagementObjectSearcher search = new ManagementObjectSearcher(scope, query))
        //    {
        //        foreach (var z in search.Get())
        //        {
        //            status = (WindowsActivationStatus)((uint)z[property]);
        //        }
        //    }
        //    return status;
        //}

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

        //public static string AnalyzeForModel()
        //{
        //    object[] model = new object[2];
        //    string query = "SELECT * FROM Win32_ComputerSystem";

        //    model[0] = ReadDetailsFromComputer(query, "Model", @"root/cimv2");



        //    query = "SELECT * FROM Win32_BIOS";
        //    model[1] = ReadDetailsFromComputer(query, "Manufacturer");

        //    query = "SELECT * FROM Win32_ComputerSystem";
        //    string[] oemstring = (string[])ReadDetailsFromComputer(query, "OEMStringArray");
        //    string[] patterns = new string[]
        //    {
        //        @"\d{5}",
        //        @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z]\d",
        //        @"[A-Za-z]\d{4}\W[A-Za-z]\d\d[A-Za-z]\d",
        //        @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z]\d[A-Za-z]",
        //        @"[A-Za-z]\d{4}\W[A-Za-z][A-Za-z]\d[A-Za-z]"
        //    };
        //    for (int i = 0; i < model.Length; i++)
        //    {
        //        if (model[i] == null)
        //            continue;
        //        for (int j = 0; j < patterns.Length; j++)
        //        {
        //            Match match = Regex.Match(model[i].ToString(), patterns[j].ToString());
        //            if (match.Success)
        //            {
        //                return match.Value;
        //            }
        //        }
        //    }
        //    return null;
        //}

    }
}
