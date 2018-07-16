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
        public static bool DoestModelListFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + "/Model.xml");
            }
        }

        public static void SerializeModelList(Configuration config)
        {
            var temp = GatherModels(config.DatabaseTableName, config.BiosTableName, config.Filepath,
                config.Filename, (int)config.DB_Model, (int)config.DB_PeaqModel, (int)config.DB_CaseModel, (int)config.Bios_CaseModel);

            var list = new ObservableCollection<Location>(temp.OrderBy(z => z.Model));

            var xs = new XmlSerializer(typeof(ObservableCollection<Location>));

            var sw = new StreamWriter(Environment.CurrentDirectory + @"\Model.xml");

            xs.Serialize(sw, list);

            sw.Close();
        }

        public static ObservableCollection<Location> DeserializeModelList()
        {
            var xs = new XmlSerializer(typeof(ObservableCollection<Location>));

            var sr = new StreamReader(Environment.CurrentDirectory + @"\Model.xml");

            var computerList = xs.Deserialize(sr) as ObservableCollection<Location>;

            sr.Close();
            return computerList;
        }

        private static IEnumerable<Location> GatherModels(string dbTableName, string biosTableName,
            string filepath, string filename, int modelColumn, int peaqModelColumn, int mdCaseModelColumn, int biosCaseModelColumn)
        {
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var stream = new FileStream(filepath + filename, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
            var excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            var result = excelReader.AsDataSet();

            var modelTable = result.Tables[dbTableName];
            for (var i = 1; i < modelTable.Rows.Count; i++)
            {
                var md = modelTable.Rows[i][modelColumn].ToString();
                if (string.IsNullOrEmpty(md))
                    continue;

                string peaqModel = modelTable.Rows[i][peaqModelColumn].ToString();

                var caseModel = modelTable.Rows[i][mdCaseModelColumn].ToString();

                var biosTable = result.Tables[biosTableName];
                int biosRow = 0;
                for (int j = 0; j < biosTable.Rows.Count; j++)
                {
                    if (biosTable.Rows[j][biosCaseModelColumn].ToString().Contains(caseModel))
                    {
                        biosRow = j;
                    }
                }

                yield return new Location(md, peaqModel, i, biosRow);
            }
        }
    }
}
