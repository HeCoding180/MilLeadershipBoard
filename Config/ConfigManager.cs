using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MilLeadershipBoard.Config
{
    internal static class ConfigManager
    {
        //   ---   Private Constants   ---

        /// <summary>
        /// Constant containing the file name of the user data file.
        /// </summary>
        private const string CONFIG_DATA_FILE_NAME = "Config.json";

        /// <summary>
        /// Gets a <see cref="JsonSerializerOptions"/> instance containing the serializer options used for generating the config file.
        /// </summary>
        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the <see cref="ConfigData"/> instance that contains user data.
        /// </summary>
        public static ConfigData Config { set; get; } = new ConfigData();

        /// <summary>
        /// Gets the <see cref="StorageFolder"/> instance of the local folder.
        /// </summary>
        public static string LocalFolderPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MilLeadershipBoard");
            }
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to load all config data.
        /// </summary>
        /// <returns>An awaitable task representing the operation.</returns>
        public static void LoadData()
        {
            LoadConfigFile();
        }

        /// <summary>
        /// Method used to load all config data.
        /// </summary>
        /// <returns>An awaitable task representing the operation.</returns>
        public static void SaveData()
        {
            SaveConfigFile();
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to ensure that the local folder exists.
        /// </summary>
        private static void EnsureLocalFolder()
        {
            if (!Directory.Exists(LocalFolderPath))
            {
                Directory.CreateDirectory(LocalFolderPath);
            }
        }

        /// <summary>
        /// Method used to load the contents of the config file.
        /// </summary>
        /// <returns>An awaitable task representing the operation.</returns>
        private static void LoadConfigFile()
        {
            try
            {
                if (!File.Exists(Path.Combine(LocalFolderPath, CONFIG_DATA_FILE_NAME)))
                {
                    // File does not exist, return
                    return;
                }

                ConfigData? config = JsonSerializer.Deserialize<ConfigData>(File.ReadAllText(Path.Combine(LocalFolderPath, CONFIG_DATA_FILE_NAME)));

                if (config is not null)
                {
                    Config = config;
                }
            }
            catch (FileNotFoundException)
            {
                // File does not exist
                return;
            }
            catch (JsonException)
            {
                // JSON deserialization failed
                return;
            }
        }

        /// <summary>
        /// Method used to save the data of the <see cref="Config"/> property.
        /// </summary>
        private static void SaveConfigFile()
        {
            string configFilePath = Path.Combine(LocalFolderPath, CONFIG_DATA_FILE_NAME);

            EnsureLocalFolder();

            File.WriteAllText(configFilePath, JsonSerializer.Serialize(Config, serializerOptions));
        }
    }
}
