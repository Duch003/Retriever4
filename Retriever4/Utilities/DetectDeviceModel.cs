using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Retriever4.Validation
{
    public static class DetectDeviceModel
    {
        private static string[] _patterns =
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
            for(int i = 0, j = 0; i < _patterns.Length; i++)
            {
                var match = Regex.Match(raw, _patterns[i]);
                if (match.Success)
                {
                    return match.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// Checks string for model pattern.
        /// </summary>
        /// <param name="raw">Raw text to being looked for.</param>
        /// <param name="result">In case if model has been found, there will be its model. Null in any other case.</param>
        /// <returns>-1 if model not found in database, 0 if model not found or 1 if model found.</returns>
        public static int FindModel(string raw, out string result)
        {
            var returnState = 0;
            result = null;
            var model = DetectModel(raw);
            if(model == null)
            {
                returnState = 0;
            }
            var ans = Program.ModelList.Where(z => z.Model.Contains(model) || z.PeaqModel.Contains(model));
            var enumerable = ans as Location[] ?? ans.ToArray();
            if (!enumerable.Any())
                returnState = -1;
            else
            {
                returnState = 1;
                result = enumerable.First().Model;
            }

            return returnState;
        }
    }
}
