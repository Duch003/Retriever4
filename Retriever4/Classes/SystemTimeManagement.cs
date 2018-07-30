using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Retriever4.Classes
{
    public class SystemTimeManagement
    {
        [StructLayout(LayoutKind.Sequential)]
        private struct SYSTEMTIME
        {
            public short wYear;
            public short wMonth;
            public short wDayOfWeek;
            public short wDay;
            public short wHour;
            public short wMinute;
            public short wSecond;
            public short wMilliseconds;
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool SetSystemTime(ref SYSTEMTIME st);

        public static void SetSystemDateTime(DateTime time)
        {
            var st = new SYSTEMTIME
            {
                wYear = (short)time.Year,
                wMonth = (short)time.Month,
                wDay = (short)time.Day,
                wHour = (short)time.Hour,
                wMinute = (short)time.Minute,
                wSecond = (short)time.Second
            };
            SetSystemTime(ref st);
        }
        
    }
}
