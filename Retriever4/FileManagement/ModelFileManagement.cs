using ExcelDataReader;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace Retriever4.FileManagement
{
    public static class ModelFile
    {
        /// <summary>
        /// Return true if exists.
        /// </summary>
        public static bool DoestModelListFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + "/Model.xml");
            }
        }

        /// <summary>
        /// Save / overwrite Model.xml file with current model list.
        /// </summary>
        public static void SerializeModelList()
        {
            //Invoke method thet gather essential data
            var temp = GatherModels();
            //Convert to collection
            var list = new ObservableCollection<Location>(temp.OrderBy(z => z.Model));
            //Create serializer
            var xs = new XmlSerializer(typeof(ObservableCollection<Location>));
            if (!DoestModelListFileExists)
                File.Create(Environment.CurrentDirectory + @"\Model.xml");
            //Open stream
            var sw = new StreamWriter(Environment.CurrentDirectory + @"\Model.xml");
            //Serialze data
            xs.Serialize(sw, list);
            //Close
            sw.Close();
        }

        /// <summary>
        /// Reads serialized datafrom Model.xml
        /// </summary>
        /// <returns>Collection of locations.</returns>
        public static ObservableCollection<Location> DeserializeModelList()
        {
            //Create serializer
            var xs = new XmlSerializer(typeof(ObservableCollection<Location>));
            //Open stream
            var sr = new StreamReader(Environment.CurrentDirectory + @"\Model.xml");
            //Deserialize data
            var computerList = xs.Deserialize(sr) as ObservableCollection<Location>;
            //Close
            sr.Close();
            return computerList;
        }

        /// <summary>
        /// Open and read excel database file. Gather essential informations from specific cells. Depends on Config.
        /// </summary>
        /// <returns>Collection of devices locations.</returns>
        private static IEnumerable<Location> GatherModels()
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
                    if (biosTable.Rows[j][Configuration.Bios_CaseModel].ToString().Contains(caseModel))
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
