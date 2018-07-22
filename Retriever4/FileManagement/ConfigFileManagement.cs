using System;
using System.IO;
using System.Xml.Serialization;

namespace Retriever4.FileManagement
{
    public static class ConfigFileManagement
    {
        /// <summary>
        /// Returns true if file exists.
        /// </summary>
        public static bool DoesConfigFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + @"\Config.xml");
            }
        }

        /// <summary>
        /// Read config file. 
        /// </summary>
        /// <returns>Config instance to analysis.</returns>
        public static Configuration ReadConfiguration()
        {
            //If file doesnt exists throw exception. File must exists and be readable.
            if (!DoesConfigFileExists)
                throw new FileNotFoundException("Nie znaleziono pliku konfiguracyjnego Config.xml.");
            //Create instance of config
            var config = new Configuration();
            //Create serializer
            var xs = new XmlSerializer(typeof(Configuration));
            //Open stream
            var sr = new StreamReader(Environment.CurrentDirectory + @"\Config.xml");
            //Deserialize file
            config = xs.Deserialize(sr) as Configuration;
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
        public static void WriteConfiguration()
        {
            var config = new Configuration();
            var xs = new XmlSerializer(typeof(Configuration));
            var sw = new StreamWriter(Environment.CurrentDirectory + @"\Config.xml");
            xs.Serialize(sw, config);
            sw.Close();
        }
    }
}
