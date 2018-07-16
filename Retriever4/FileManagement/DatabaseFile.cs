using ExcelDataReader;
using Retriever4.Interfaces;
using System;
using System.IO;

namespace Retriever4.FileManagement
{
    public class DatabaseFile : IDatabaseFile
    {
        /// <summary>
        /// Checks if database file exists. Depends on config file.
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        public bool DoesDatabaseFileExists(string filepath, string filename)
        {
            return File.Exists(filepath + filename);
        }

        public object ReadDetailsFromDatabase(string filepath, string filename, string tableName, int row, int column)
        {
            if (string.IsNullOrEmpty(filepath) || string.IsNullOrEmpty(filename))
            {
                throw new InvalidDataException("Nie można połączyć się z plikiem excel. Któryś z argumentów kontruktora jest pusty:\n" +
                    $"filepath = {filepath}\n" +
                    $"filename = {filename}");
            }

            object anwser = null;

            try
            {
                using (FileStream stream = new FileStream(filepath + filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
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
                    $"Wywołania:\n" +
                    $"{e.StackTrace}";
            }

            return anwser;
        }
    }
}
