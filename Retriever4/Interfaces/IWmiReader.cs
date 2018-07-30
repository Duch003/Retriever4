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
        Dictionary<string, dynamic>[] CheckWirelessConnection();
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
        //void ActivateWindowsOnline();
        //https://msdn.microsoft.com/en-us/library/aa394520(v=vs.85).aspx
        //https://docs.microsoft.com/pl-pl/previous-versions/windows/desktop/licwmiprov/activateonline-method-in-class-win32-windowsproductactivation
        //https://stackoverflow.com/questions/19033703/wmi-methods-using-managementobject-invokemethod
        //https://docs.microsoft.com/en-us/sccm/develop/core/clients/programming/how-to-call-a-wmi-class-method-by-using-system.management
    }
}
