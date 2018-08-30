using ExcelDataReader;
using Retriever4.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public bool DoesDatabaseFileExists => File.Exists(Configuration.Filepath + Configuration.Filename);
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

        /// <summary>
        /// Save / overwrite Model.xml file with current model list.
        /// </summary>
        public List<Location> ReadModelList()
        {
            var ans = new List<Location>();
            try
            {
                //Invoke method thet gather essential data
                var temp = GatherModels();
                //Convert to collection
                ans = new List<Location>(temp.OrderBy(z => z.Model));
            }
            catch (Exception e)
            {
                var message = $"\nNie można utworzyć pliku Model.xml.\nTreść błędu: {e.Message}\nWewnętrzny wyjątek: {e.InnerException?.Message}.\nMetoda: SerializeModelList, klasa: ModelFileManagement.cs.";
                throw new Exception(message);
            }

            return ans;
        }

        /// <summary>
        /// Open and read excel database file. Gather essential informations from specific cells. Depends on Config.
        /// </summary>
        /// <returns>Collection of devices locations.</returns>
        private IEnumerable<Location> GatherModels()
        {
            //Uncomment in case of encoding troubles.
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            //Open connection
            var stream = new FileStream(Configuration.Filepath + Configuration.Filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            var result = excelReader.AsDataSet();

            //Read Model Database
            var modelTable = result.Tables[Configuration.DatabaseTableName];
            for (var i = 1; i < modelTable.Rows.Count; i++)
            {
                //Save model
                var md = modelTable.Rows[i][Configuration.DB_Model].ToString();
                if (string.IsNullOrEmpty(md))
                    continue;
                //Save peaq Model
                var peaqModel = modelTable.Rows[i][Configuration.DB_PeaqModel].ToString();
                //Save case model to join tables on it
                var caseModel = modelTable.Rows[i][Configuration.DB_CaseModel].ToString();
                //Read Bios Database
                var biosTable = result.Tables[Configuration.BiosTableName];
                var biosRow = 0;
                for (var j = 0; j < biosTable.Rows.Count; j++)
                {
                    //Searching for matching for case model
                    var bios = biosTable.Rows[j][Configuration.Bios_CaseModel].ToString().ToUpper();
                    if (bios.Contains(caseModel.ToUpper()))
                    {
                        //If found, save line number
                        biosRow = j;
                    }
                }
                yield return new Location(md, peaqModel, i, biosRow);
            }
        }

    }


}
