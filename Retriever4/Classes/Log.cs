using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

namespace Retriever4
{
    public static class Log
    {
        /// <summary>
        /// Create txt file and writes information into it.
        /// </summary>
        /// <param name="info">Information that will be writed to document.</param>
        /// <returns>Log file name.</returns>
        public static bool WriteLog(string header, string userInfo, Exception e)
        {
            if (e == null)
                return false;
            if (string.IsNullOrEmpty(header))
                header = "";
            if (string.IsNullOrEmpty(header))
                header = "";
            //Create title
            var title = $"Log {DateTime.Now}.txt";
            //Replace forbidden chars
            title = title.Replace(@"/", @".").Replace(@":", @"");
            //Create path
            var path = Path.Combine(Environment.CurrentDirectory, title);
            //Write log
            using (var sw = new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Write)))
            {
                sw.WriteLine(header);
                sw.WriteLine(userInfo);
                sw.WriteLine($"Wyjątek: {e?.Message}");
                sw.WriteLine($"Stos wywołań: {e?.StackTrace}");
                sw.WriteLine($"Wewnętrzny wyjątek: {e?.InnerException?.Message}");
            }
            //Return name
            return false;
        }
    }
}
