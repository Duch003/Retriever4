using System;
using System.IO;

namespace Retriever4
{
    public static class Log
    {
        public static string WriteLog(string info)
        {
            string title = $"Log {DateTime.Now}.txt";
            title = title.Replace(@"/", @".").Replace(@":", @"");
            string path = Path.Combine(Environment.CurrentDirectory, title);
            using (StreamWriter sw = new StreamWriter(new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.Write)))
            {
                sw.WriteLine(info);
            }
            return title;
        }

        public static void PrintErrorMessage(string entryMessage, Exception e, string file)
        {
            string message = $"{entryMessage}\nTreść błędu:{e.Message}\nŹródło:{e.Source}\nStos wywołań:{e.StackTrace}\nPlik {file}.";
            Console.WriteLine(message);
            Console.WriteLine($"\nAplikacja nie może zostać odpalona. W folderze z programem został zapisany log o nazwie {WriteLog(message)}.\nPrześlij go proszę na ten email: Tomasz.Mankin@hemmersbach.com.");
            Console.WriteLine("\nWciśnij ENTER aby kontynuować.");
        }
    }
}
