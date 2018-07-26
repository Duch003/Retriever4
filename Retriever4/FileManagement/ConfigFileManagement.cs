using Retriever4.Interfaces;
using System;
using System.IO;
using System.Xml.Serialization;

namespace Retriever4.FileManagement
{
    public class ConfigFileManagement : IConfigFileManager
    {
        /// <summary>
        /// Returns true if file exists.
        /// </summary>
        public bool DoesConfigFileExists => File.Exists(Environment.CurrentDirectory + @"\Config.xml");

        /// <summary>
        /// Read config file. 
        /// </summary>
        /// <returns>Config instance to analysis.</returns>
        public Configuration ReadConfiguration()
        {
            //If file doesnt exists throw exception. File must exists and be readable.
            if (!DoesConfigFileExists)
                throw new FileNotFoundException("Nie znaleziono pliku konfiguracyjnego Config.xml.");
            if (new FileInfo(Environment.CurrentDirectory + @"\Config.xml").Length == 0)
                return new Configuration();
            //Create serializer
            var xs = new XmlSerializer(typeof(Configuration));
            //Open stream
            var sr = new StreamReader(Environment.CurrentDirectory + @"\Config.xml");
            //Deserialize file
            var config = xs.Deserialize(sr) as Configuration;
            //Close stream
            sr.Close();
            //If path is empty, fill it with default path
            if (string.IsNullOrEmpty(config.filepath))
                config.filepath = Environment.CurrentDirectory;

            return config;
        }

        /// <summary>
        /// Writes example configuration file.
        /// </summary>
        public bool WriteConfiguration()
        {
            try
            {
                var config = new Configuration();
                var xs = new XmlSerializer(typeof(Configuration));
                var sw = new StreamWriter(Environment.CurrentDirectory + @"\Config.xml");
                xs.Serialize(sw, config);
                sw.Close();
            }
            catch(Exception e)
            {
                var message = $"Nie można utworzyć pliku Config.xml.\nTreść błędu: {e.Message}\nWewnętrzny wyjątek: " +
                              $"{e.InnerException?.Message}.\nMetoda: WriteConfiguration, klasa: ConfigFileManagement.cs.";
                throw new Exception(message);
            }
            return true;
        }
    }
}
