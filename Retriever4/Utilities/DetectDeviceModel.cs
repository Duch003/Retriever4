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
                Match match = Regex.Match(raw, _patterns[i]);
                if (match.Success)
                {
                    return match.Value;
                }
            }
            return null;
        }

        public static string FindModel(string raw)
        {
            string model = DetectModel(raw);
            if(model == null)
            {
                return "Nie wykryto.";
            }
            var ans = Program._modelList.Where(z => z.Model.Contains(model) || z.PeaqModel.Contains(model));
            if (ans == null || ans.Count() == 0)
            {
                return $"Brak modelu {model} w bazie.";
            }
            else
            {
                return ans.First().Model;
            }
        }
    }
}
