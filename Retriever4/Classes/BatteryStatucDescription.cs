﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Retriever4.Enums;

namespace Retriever4.Classes
{
    public static class BatteryStatucDescription
    {
        public static string BatteryStatus(BatteryStatus status)
        {
            switch ((int) status)
            {
                case 1:
                    return "Bateria jest naładowana, rozładowywanie";
                case 2:
                    return "Podpięto ładowarkę, bateria odcięta";
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
}
