using System;
using System.IO;

namespace Retriever4
{
    public static class Log
    {
        /// <summary>
        /// Create txt file and writes information into it.
        /// </summary>
        /// <param name="info">Information that will be writed to document.</param>
        /// <returns>Log file name.</returns>
        public static string WriteLog(string info)
        {
            //Create title
            string title = $"Log {DateTime.Now}.txt";
            //Replace forbidden chars
            title = title.Replace(@"/", @".").Replace(@":", @"");
            //Create path
            string path = Path.Combine(Environment.CurrentDirectory, title);
            //Write log
            using (StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Write)))
            {
                sw.WriteLine(info);
            }
            //Return name
            return title;
        }

        /// <summary>
        /// Print error message in the console and create log file.
        /// </summary>
        /// <param name="entryMessage">Message about exception.</param>
        /// <param name="e">Exception itself.</param>
        /// <param name="file">File against whom exception occur.</param>
        public static void PrintErrorMessage(string entryMessage, Exception e, string file)
        {
            //Validation
            if(e == null)
            {
                string mess = $"Parametr typu System.Exception jest null. Metoda: {nameof(PrintErrorMessage)}, klasa: Log.cs.";
                throw new ArgumentNullException("e", mess);
            }

            //Checking inputs strings
            if (string.IsNullOrEmpty(entryMessage))
                entryMessage = ">>>>>Brak opisu wyjątku.<<<<<";
            if (string.IsNullOrEmpty(file))
                entryMessage = ">>>>>Nie podano nazwy pliku.<<<<<";
            //Printing
            string message = $"{entryMessage}\nTreść błędu:{e.Message}\nŹródło:{e.Source}\nStos wywołań:{e.StackTrace}\nPlik {file}.";
            Console.WriteLine(message);
            Console.WriteLine($"\nAplikacja nie może zostać odpalona. W folderze z programem został zapisany log o nazwie {WriteLog(message)}.\nPrześlij go proszę na ten email: Tomasz.Mankin@hemmersbach.com.");
            Console.WriteLine("\nWciśnij ENTER aby kontynuować.");
        }
    }
}
