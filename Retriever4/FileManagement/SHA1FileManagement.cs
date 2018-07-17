using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Retriever4.FileManagement
{
    public static class SHA1FileManagement
    {
        /// <summary>
        /// Return true if file exists
        /// </summary>
        public static bool DoesHashFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + "/SHA1.txt");
            }
        }

        /// <summary>
        /// Compute hash code. Depends on Config.
        /// </summary>
        /// <returns>Hashcode of database file.</returns>
        public static string ComputeSHA1()
        {
            FileStream stream = new FileStream(_GLOBAL.Filepath + _GLOBAL.Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
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

        /// <summary>
        /// Reads hash string from SHA1.txt.
        /// </summary>
        /// <returns>Returns hash string.</returns>
        public static string ReadHash()
        {
            //Default anwser
            string anwser = "";
            //Return if file doesnt exists
            if (!File.Exists(Environment.CurrentDirectory + "/SHA1.txt"))
            {
                File.Create(Environment.CurrentDirectory + "/SHA1.txt");
                return anwser;
            }
            //Else read string from file.
            using (StreamReader sr = new StreamReader(new FileStream(Environment.CurrentDirectory + "/SHA1.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
            {
                anwser = sr.ReadLine();
            }
            return anwser;
        }

        /// <summary>
        /// Saves hash to SHA1.txt file.
        /// </summary>
        /// <param name="hash">Hash to write.</param>
        public static void WriteHash(string hash)
        {
            if (string.IsNullOrEmpty(hash))
                throw new InvalidDataException($"Nie można zapisać hasha do pliku. Argument metody jest pusty: {hash}. Metoda: {nameof(WriteHash)}, klasaL SHA1File.cs.");
            using (StreamWriter sw = new StreamWriter(new FileStream(Environment.CurrentDirectory + "/SHA1.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite)))
            {
                sw.WriteLine(hash);
            }
        }
    }
}
