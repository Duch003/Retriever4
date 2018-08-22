using ExcelDataReader;
using Retriever4.Interfaces;
using System;
using System.IO;

namespace Retriever4.FileManagement
{
    public class DatabaseFileManagement : IDatabaseManager
    {
        /// <summary>
        /// Checks if file exists. Depends on config file.
        /// </summary>
        /// <returns>True if file exists.</returns>
        public bool DoesDatabaseFileExists => File.Exists(Configuration.Filepath + Configuration.Filename);

        public string LastValue { get; private set; }
        public string LastColumnName { get; private set; }

        public DatabaseFileManagement() { }
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
                using (var stream = new FileStream(Configuration.Filepath + Configuration.Filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    var result = excelReader.AsDataSet();

                    var table = result.Tables[tableName];
                    anwser = table.Rows[row][column];
                    LastValue = anwser.ToString();
                    LastColumnName = table.Rows[0][column].ToString();
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

        //public string FindDatabaseFile(string name)
        //{
        //    foreach(var drive in DriveInfo.GetDrives())
        //    {

        //    }
        //}
    }
}
