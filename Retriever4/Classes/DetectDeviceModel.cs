using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Retriever4.Classes
{
    public static class DetectDeviceModel
    {
        private static readonly string[] Patterns =
        {
            @"\d{5}",
            @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z][A-Za-z]\s[A-Za-z]",
            @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z][A-Za-z][A-Za-z]",
            @"[A-Za-z]\d{4}\W[A-Za-z][A-Za-z][A-Za-z][A-Za-z]",
            @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z]\d[A-Za-z]",
            @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z][A-Za-z]",
            @"[A-Za-z]\d{4}\W[A-Za-z][A-Za-z]\d[A-Za-z]",
            @"[A-Za-z]\d{4}\W[A-Za-z]\d\d[A-Za-z]\d",
            @"[A-Za-z]\d{4}\W[A-Za-z]\d\d[A-Za-z]",
            @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z]\d",
            //The shortest pattern must be last
            @"[A-Za-z]\d{4}\W[A-Za-z]\d[A-Za-z]"

        };

        public static string DetectModel(string raw)
        {
            return (from z in Patterns select Regex.Match(raw, z) into match where match.Success select match.Value).FirstOrDefault();
        }

        /// <summary>
        /// Checks string for model pattern.
        /// </summary>
        /// <param name="raw">Raw text to being looked for.</param>
        /// <param name="devices">Model list to be searched.</param>
        /// <param name="result">In case if model has been found, there will be its model. Null in any other case.</param>
        /// <returns>-1 if model not found in list, 0 if model not found or 1 if model found.</returns>
        public static int FindModel(string raw, List<Location> devices, out Location result)
        {
            //Check inputs
            if (string.IsNullOrEmpty(raw))
            {
                var message = $"Argument wejściowy jest pusty lub null. Metoda: {nameof(FindModel)}, klasa: DetectDeviceModel.cs.";
                throw new ArgumentException(message, nameof(raw));
            }

            if (devices == null || devices.Count == 0)
            {
                var message = $"Lista wejściowa jest pusta lub null. Metoda: {nameof(FindModel)}, klasa: DetectDeviceModel.cs.";
                throw new ArgumentException(message, nameof(devices));
            }

            int returnState;
            result = null;
            var model = DetectModel(raw);
            if(model == null)
            {
                returnState = 0;
                return returnState;
            }
            var ans = devices.Where(z => z.Model.Contains(model) || z.PeaqModel.Contains(model));
            var enumerable = ans as Location[] ?? ans.ToArray();
            if (!enumerable.Any())
                returnState = -1;
            else
            {
                returnState = 1;
                result = enumerable.First();
            }

            return returnState;
        }
    }
}
