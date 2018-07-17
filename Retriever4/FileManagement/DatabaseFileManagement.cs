using ExcelDataReader;
using System;
using System.IO;

namespace Retriever4.FileManagement
{
    public class DatabaseFileManagement
    {
        /// <summary>
        /// Checks if file exists. Depends on config file.
        /// </summary>
        /// <returns>True if file exists.</returns>
        public bool DoesDatabaseFileExists()
        {
            return File.Exists(_GLOBAL.Filepath + _GLOBAL.Filename);
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
                string message = $"Nie podano nazwy zakładki z pliku excel. Metoda: {nameof(ReadDetailsFromDatabase)}, klasa: DatabaseFileManagement.";
                throw new InvalidDataException(message);
            }
            if (row < 0)
            {
                string message = $"Ujemny numer wiersza. Wiersze numerowane są od 0. Metoda: {nameof(ReadDetailsFromDatabase)}, klasa: DatabaseFileManagement.";
                throw new InvalidDataException(message);
            }
            if (column < 0)
            {
                string message = $"Ujemny numer kolumny. Wiersze numerowane są od 0. Metoda: {nameof(ReadDetailsFromDatabase)}, klasa: DatabaseFileManagement.";
                throw new InvalidDataException(message);
            }

            object anwser = null;

            //An attempt to extract cell value
            try
            {
                using (FileStream stream = new FileStream(_GLOBAL.Filepath + _GLOBAL.Filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
                {
                    var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
                    var result = excelReader.AsDataSet();

                    var table = result.Tables[tableName];
                    anwser = table.Rows[row][column];
                }
            }
            catch (Exception e)
            {
                string message = $"Nie udało się utworzyć połączenia z plikiem excel Nie można odczytać bazy.\n\nTreść błędu:\n" +
                    $"{e.Message}\n\n" +
                    $"Wyjątek wewnętrzny:{e.InnerException}" +
                    $"\nWywołania:\n" +
                    $"{e.StackTrace}";
            }

            //Return anwser as object
            return anwser;
        }
    }
}
