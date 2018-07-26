using Retriever4.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retriever4.Interfaces
{
    public interface IWmiReader
    {
        Dictionary<string, dynamic>[] CheckDeviceManager();
        bool CheckWirelessConnection();
        Dictionary<string, string> CheckEthernetInterfaceMAC();
        Dictionary<string, dynamic>[] CheckWindowsActivationStatus();
        SecureBootStatus CheckSecureBootStatus();
        Dictionary<string, dynamic>[] GetWindowsProductKey();
        string[] GetSwm();
        Dictionary<string, dynamic>[] GetModelString();
        Dictionary<string, dynamic>[] MainboardBiosData();
        Dictionary<string, dynamic>[] MainboardModel();
        Dictionary<string, dynamic>[] ProcessorID();
        Dictionary<string, dynamic>[] RamData();
        Dictionary<string, dynamic>[] StorageData();
        Dictionary<string, dynamic>[] BatteriesData();
        Dictionary<string, dynamic>[] OS();
    }
}
