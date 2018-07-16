using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Retriever4.FileManagement
{
    public static class SHA1File
    {
        public static bool DoesHashFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + "/SHA1.txt");
            }
        }

        public static string ComputeSHA1(string filepath, string filename)
        {
            FileStream stream = new FileStream(filepath + filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(stream);
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    sb.Append(b.ToString("X2"));
                }
                return sb.ToString();
            }
        }

        public static string ReadHash()
        {
            string anwser = "";

            if (!File.Exists(Environment.CurrentDirectory + "/SHA1.txt"))
            {
                File.Create(Environment.CurrentDirectory + "/SHA1.txt");
                return anwser;
            }

            using (StreamReader sr = new StreamReader(new FileStream(Environment.CurrentDirectory + "/SHA1.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
            {
                anwser = sr.ReadLine();
            }

            return anwser;
        }

        public static void WriteHash(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                throw new InvalidDataException($"Nie można zapisać hasha do pliku. Argument metody jest pusty: {hash}.");
            using (StreamWriter sw = new StreamWriter(new FileStream(Environment.CurrentDirectory + "/SHA1.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
            {
                sw.WriteLine(hash);
            }
        }
    }
}
