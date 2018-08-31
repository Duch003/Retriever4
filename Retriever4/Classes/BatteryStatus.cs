namespace Retriever4.Classes
{
    public static class BatteryStatucDescription
    {
        /// <summary>
        /// Return battery status depends on status number.
        /// </summary>
        /// <param name="status">Status number.</param>
        /// <returns>Status message.</returns>
        public static string BatteryStatus(BatteryStatus status)
        {
            switch ((int) status)
            {
                case 1:
                    return "Bateria jest naładowana, rozładowywanie";
                case 2:
                    return "Podpięto ładowarkę, ładowanie";
                case 3:
                    return "Bateria jest naładowana, rozładowywanie";
                case 4:
                    return "Niski poziom naładowania, rozładowywanie";
                case 5:
                    return "Krytyczny stan naładowania, rozładowywanie";
                case 6:
                    return "Ładowanie";
                case 7:
                    return "Wysoki poziom naładowania, ładowanie";
                case 8:
                    return "Niski poziom naładowania, ładowanie";
                case 9:
                    return "WKrytyczny poziom naładowania, ładowanie";
                case 10:
                    return "Niezdefiniowany stan";
                case 11:
                    return "Częściowo naładowana";
                default:
                    return $"Nieznany stan: {(int) status}";
            }
        }
    }

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
