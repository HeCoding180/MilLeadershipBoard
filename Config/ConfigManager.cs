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
        private const string USER_DATA_FILE_NAME = "UserData.json";

        /// <summary>
        /// Constant containing the file name of the settings file.
        /// </summary>
        private const string SETTINGS_FILE_NAME = "Settings.json";

        /// <summary>
        /// Gets a <see cref="JsonSerializerOptions"/> instance containing the serializer options used for generating the config file.
        /// </summary>
        private static readonly JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true
        };

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the <see cref="SettingsData"/> instance that contains user data.
        /// </summary>
        public static SettingsData Settings { set; get; } = new SettingsData();

        /// <summary>
        /// Sets or gets the <see cref="Config.UserData"/> instance that contains user data.
        /// </summary>
        public static UserData UserData { set; get; } = new UserData();

        /// <summary>
        /// Gets the <see cref="StorageFolder"/> instance of the local folder.
        /// </summary>
        public static string LocalFolderPath
        {
            get
            {
                return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "MilLeadershipBoard");
            }
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to load all config data.
        /// </summary>
        /// <returns>An awaitable task representing the operation.</returns>
        public static void LoadData()
        {
            LoadSettingsFile();
            LoadUserDataFile();
        }

        /// <summary>
        /// Method used to load all config data.
        /// </summary>
        /// <returns>An awaitable task representing the operation.</returns>
        public static void SaveData()
        {
            SaveSettingsFile();
            SaveUserDataFile();
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to load the contents of the settings file.
        /// </summary>
        /// <returns>An awaitable task representing the operation.</returns>
        private static void LoadSettingsFile()
        {
            try
            {
                if (!File.Exists(Path.Combine(LocalFolderPath, SETTINGS_FILE_NAME)))
                {
                    // File does not exist, return
                    return;
                }

                SettingsData? settings = JsonSerializer.Deserialize<SettingsData>(File.ReadAllText(Path.Combine(LocalFolderPath, SETTINGS_FILE_NAME)));

                if (settings is not null)
                {
                    Settings = settings;
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
        /// Method used to load the contents of the user data file.
        /// </summary>
        /// <returns>An awaitable task representing the operation.</returns>
        private static void LoadUserDataFile()
        {
            try
            {
                if (!File.Exists(Path.Combine(LocalFolderPath, USER_DATA_FILE_NAME)))
                {
                    // File does not exist, return
                    return;
                }

                UserData? userData = JsonSerializer.Deserialize<UserData>(File.ReadAllText(Path.Combine(LocalFolderPath, USER_DATA_FILE_NAME)));

                if (userData is not null)
                {
                    UserData = userData;
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
        /// Method used to save the data of the <see cref="Settings"/> property.
        /// </summary>
        private static void SaveSettingsFile()
        {
            string settingsFilePath = Path.Combine(LocalFolderPath, SETTINGS_FILE_NAME);

            if (!File.Exists(settingsFilePath))
            {
                File.Create(settingsFilePath);
            }

            File.WriteAllText(settingsFilePath, JsonSerializer.Serialize(Settings, serializerOptions));
        }

        /// <summary>
        /// Method used to save the data of the <see cref="UserData"/> property.
        /// </summary>
        private static void SaveUserDataFile()
        {
            string userDataFilePath = Path.Combine(LocalFolderPath, USER_DATA_FILE_NAME);

            if (!File.Exists(userDataFilePath))
            {
                File.Create(userDataFilePath);
            }

            File.WriteAllText(userDataFilePath, JsonSerializer.Serialize(UserData, serializerOptions));
        }
    }
}
