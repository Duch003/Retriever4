using System;
using System.IO;
using System.Xml.Serialization;

namespace Retriever4.FileManagement
{
    public static class ConfigFile
    {
        public static bool DoesConfigFileExists {
            get {
                return File.Exists(Environment.CurrentDirectory + @"\Config.xml");
            }
        }

        public static Configuration ReadConfiguration()
        {
            if (!DoesConfigFileExists)
                throw new FileNotFoundException("Nie znaleziono pliku konfiguracyjnego Config.xml.");
            Configuration config = new Configuration();

            var xs = new XmlSerializer(typeof(Configuration));

            var sr = new StreamReader(Environment.CurrentDirectory + @"\Config.xml");

            config = xs.Deserialize(sr) as Configuration;

            sr.Close();

            if (string.IsNullOrEmpty(config.Filepath))
                config.Filepath = Environment.CurrentDirectory;

            return config;
        }

        public static void WriteConfiguration()
        {
            Configuration config = new Configuration();
            var xs = new XmlSerializer(typeof(Configuration));

            var sw = new StreamWriter(Environment.CurrentDirectory + @"\Config.xml");

            xs.Serialize(sw, config);

            sw.Close();
        }
    }
}
