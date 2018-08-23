﻿using ExcelDataReader;
using Retriever4.Interfaces;
using System;
using System.IO;
using System.Linq;

namespace Retriever4.FileManagement
{
    public class DatabaseFileManagement : IDatabaseManager
    {
        /// <summary>
        /// Checks if file exists. Depends on config file.
        /// </summary>
        /// <returns>True if file exists.</returns>
        public bool DoesDatabaseFileExists => File.Exists(Filepath);
        public bool DoesTestFileExists => File.Exists(FilepathToTests);
        public string Filepath;
        public string FilepathToTests;

        public DatabaseFileManagement(string name)
        {
            Configuration.Filepath = Filepath = FindFile(name);
        }
        public DatabaseFileManagement()
        {
            FilepathToTests = FindFile(Configuration.TestFileName);
        }
        /// <summary>
        /// Reads specific cell in excel file.
        /// </summary>
        /// <param name="tableName">Sheet name.</param>
        /// <param name="row">Row number of desired cell (counts from 0).</param>
        /// <param name="column">Column number of desired cell (counts from 0).</param>
        /// <returns>Cell value. If </returns>
        public object ReadDetailsFromDatabase(string tableName, int row, int column)
        {
            //Validation
            if (string.IsNullOrEmpty(tableName))
            {
                var message = $"Nie podano nazwy zakładki z pliku excel. Metoda: {nameof(ReadDetailsFromDatabase)}, klasa: DatabaseFileManagement.";
                throw new InvalidDataException(message);
            }
            if (row < 0)
            {
                var message = $"Ujemny numer wiersza. Wiersze numerowane są od 0. Metoda: {nameof(ReadDetailsFromDatabase)}, klasa: DatabaseFileManagement.";
                throw new InvalidDataException(message);
            }
            if (column < 0)
            {
                var message = $"Ujemny numer kolumny. Wiersze numerowane są od 0. Metoda: {nameof(ReadDetailsFromDatabase)}, klasa: DatabaseFileManagement.";
                throw new InvalidDataException(message);
            }

            object anwser = null;

            //An attempt to extract cell value
            try
            {
                using (var stream = new FileStream(Configuration.Filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    var result = excelReader.AsDataSet();

                    var table = result.Tables[tableName];
                    anwser = table.Rows[row][column];
                }
            }
            catch (Exception e)
            {
                var message = $"Nie udało się utworzyć połączenia z plikiem excel Nie można odczytać bazy.\n\nTreść błędu:\n" +
                    $"{e.Message}\n\n" +
                    $"Wyjątek wewnętrzny:{e.InnerException}" +
                    $"\nWywołania:\n" +
                    $"{e.StackTrace}";
            }

            //Return anwser as object
            return anwser;
        }

        public string FindFile(string name)
        {
            name = name.Replace(@"\", "").Replace(@"/", "");

            foreach (var drive in DriveInfo.GetDrives())
            {
                try
                {
                    var fi = new DirectoryInfo(drive.RootDirectory.FullName);
                    var ans = fi.GetFiles(name).FirstOrDefault()?.FullName;

                    if (!string.IsNullOrEmpty(ans))
                        return ans;
                    var di = fi.GetDirectories();

                    foreach (var directory in di)
                    {
                        ans = directory.GetFiles(name).FirstOrDefault()?.FullName;
                        if (!string.IsNullOrEmpty(ans))
                            return ans;
                    }
                }
                catch (Exception) { }
            }
            
            return "";
        }
    }
}
