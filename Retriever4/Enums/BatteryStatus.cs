using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Retriever4.Enums
{
    public enum BatteryStatus
    {
        Discharging = 1,
        Unknown = 2,
        FullyCharged = 3,
        Low = 4,
        Critical = 5,
        Charging = 6,
        ChargingAndHigh = 7,
        ChargingAndLow = 8,
        ChargingAndCritical = 9,
        Undefined = 10,
        PartiallyCharged = 11
    }
}
