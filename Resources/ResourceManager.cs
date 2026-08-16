using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;

namespace MilLeadershipBoard.Resources
{
    /// <summary>
    /// <see cref="EventArgs"/> class used for the <see cref="ResourceManager.DatedResourceChanged"/> event.
    /// </summary>
    internal class DatedResourceChangedEventArgs : EventArgs
    {
        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the date of the dated resource that changed.
        /// </summary>
        public DateOnly Date { get; }

        /// <summary>
        /// Gets the name of the dated resource that changed.
        /// </summary>
        public string ResourceName { get; }

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="DatedResourceChangedEventArgs"/>.
        /// </summary>
        /// <param name="resourceName">Name of the dated resource that changed.</param>
        /// <param name="date">The date of the dated resource that changed.</param>
        public DatedResourceChangedEventArgs(string resourceName, DateOnly date)
        {
            ResourceName = resourceName;
            Date = date;
        }
    }

    /// <summary>
    /// Callback for the <see cref="ResourceManager.DatedResourceChanged"/> event.
    /// </summary>
    /// <param name="args">Event arguments.</param>
    internal delegate void DatedResourceChangedEventHandler(DatedResourceChangedEventArgs args);

    /// <summary>
    /// Utility class used for managing resources.
    /// </summary>
    internal static class ResourceManager
    {
        //   ---   Private Constants   ---

        /// <summary>
        /// Constant containing the date format string for dated resource filenames.
        /// </summary>
        private const string DATE_FORMAT_STRING = "yyyyMMdd";

        /// <summary>
        /// Constant <see cref="string[]"/> containing all invalid characters for resource names.
        /// </summary>
        private static readonly string[] INVALID_RESOURCE_NAME_CHARS = [".", "/"];

        //   ---   Public Constants   ---

        /// <summary>
        /// Constant <see cref="string[]"/> containing all valid file extensions for dated image resource files.
        /// </summary>
        public static readonly string[] VALID_IMAGE_RESOURCE_FILE_EXTENSIONS = [".jpeg", ".png", ".bmp", ".gif", ".tiff", ".jxr", ".hdp", ".wdp", ".ico", ".svg"];

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets a <see langword="string"/> containing the path to the application's local appdata path.
        /// </summary>
        public static string LocalAppDataPath => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "MilLeadershipBoard");

        /// <summary>
        /// Gets a <see langword="string"/> containing the path to the folder containing the dated resources.
        /// </summary>
        public static string DatedResourcePath => Path.Combine(LocalAppDataPath, "DatedResources");

        /// <summary>
        /// Gets the <see cref="ResourceLoader"/> instance that can be used to load resources such as UI strings.
        /// </summary>
        public static ResourceLoader ResourceLoader { get; } = new ResourceLoader();

        //   ---   Public Events   ---

        /// <summary>
        /// Raised when a dated resource changes.
        /// </summary>
        public static event DatedResourceChangedEventHandler? DatedResourceChanged;

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to generate a filename for a dated resource.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="date">The date of theat resource instance.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>A full filename for the resource.</returns>
        private static string GenerateDatedResourceFileName(string resourceName, DateOnly date, string fileExtension)
        {
            // Check for invalid characters in the resource 
            if (INVALID_RESOURCE_NAME_CHARS.Any(resourceName.Contains))
            {
                throw new ArgumentException($"Invalid resourceName \"{resourceName}\". resourceName cannot contain any of the following characters: '{string.Join("', '", INVALID_RESOURCE_NAME_CHARS)}'");
            }

            return resourceName + "_" + date.ToString(DATE_FORMAT_STRING) + (fileExtension.StartsWith(".") ? fileExtension : "." + fileExtension);
        }

        /// <summary>
        /// Method used to check if a path is valid for a dated image resource.
        /// </summary>
        /// <param name="path">The path that is to be checked.</param>
        /// <returns><see langword="true"/> if the path is a valid image resource path.</returns>
        private static bool IsValidImageResourcePath(string path) => VALID_IMAGE_RESOURCE_FILE_EXTENSIONS.Contains(Path.GetExtension(path));

        /// <summary>
        /// Raises the <see cref="DatedResourceChanged"/> event
        /// </summary>
        /// <param name="resourceName">Name of the dated resource that changed.</param>
        /// <param name="date">Date of the dated resource that changed.</param>
        private static void OnDatedResourceChanged(string resourceName, DateOnly date)
        {
            DatedResourceChanged?.Invoke(new DatedResourceChangedEventArgs(resourceName, date));
        }

        /// <summary>
        /// Method used to try and get the file path(s) of a dated resource.
        /// </summary>
        /// <param name="resourceName">Name of the dated resource of which available file paths are to be retrieved.</param>
        /// <param name="date">Date of the dated resource of which available file paths are to be retrieved.</param>
        /// <param name="paths"><see langword="out"/> array of strings containing available dated resources.</param>
        /// <returns><see langword="true"/> if any dated resource files could be extracted.</returns>
        private static bool TryGetDatedResourceFiles(string resourceName, DateOnly date, out string[] paths)
        {
            if (!Directory.Exists(DatedResourcePath))
            {
                paths = [];
                return false;
            }

            // Check for invalid characters in the resource 
            if (INVALID_RESOURCE_NAME_CHARS.Any(resourceName.Contains))
            {
                throw new ArgumentException($"Invalid resourceName \"{resourceName}\". resourceName cannot contain any of the following characters: '{string.Join("', '", INVALID_RESOURCE_NAME_CHARS)}'");
            }

            paths = [.. Directory.EnumerateFiles(DatedResourcePath, "*", SearchOption.TopDirectoryOnly)
                .Where(path => Path.GetFileName(path).StartsWith(resourceName + "_" + date.ToString(DATE_FORMAT_STRING), StringComparison.OrdinalIgnoreCase))];

            return paths.Any();
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to create a dated resource out of a file.
        /// </summary>
        /// <param name="filePath">Path of the file that is to be cached.</param>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="date">Date of the dated resource.</param>
        /// <param name="overwrite">Defines if the dated resource should be overwritten if it already exists.</param>
        public static void CreateDatedResourceFile(string filePath, string resourceName, DateOnly date, bool overwrite = true)
        {
            string fileExtension = Path.GetExtension(filePath);
            string resourcePath = GenerateDatedResourceFileName(resourceName, date, fileExtension);

            File.Copy(filePath, resourcePath, overwrite);

            OnDatedResourceChanged(resourceName, date);
        }

        /// <summary>
        /// Metod used to ensure that the directory for dated resource file exists.
        /// </summary>
        public static void EnsureDatedResourceDirectory()
        {
            Directory.CreateDirectory(DatedResourcePath);
        }

        /// <summary>
        /// Method used to check if a dated resource file exists.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <param name="date"></param>
        /// <returns><see langword="true"/> if the specified resource file exists, otherwise <see langword="false"/>.</returns>
        public static bool DatedResourceExists(string resourceName, DateOnly date)
        {
            return TryGetDatedResourceFiles(resourceName, date, out _);
        }

        /// <inheritdoc cref="ResourceLoader.GetString"/>
        public static string GetString(string resourceId) => ResourceLoader.GetString(resourceId);

        /// <summary>
        /// Method used to get the date of a dated resource file's file name.
        /// </summary>
        /// <param name="fileName">File name of which the date should be extracted.</param>
        /// <returns>A <see cref="DateOnly"/> struct containing the date of the file.</returns>
        public static DateOnly GetDatedResourceFileDate(string fileName)
        {
            string dateStr = fileName.Split('_').Last();

            return DateOnly.ParseExact(dateStr, DATE_FORMAT_STRING, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Method used to get all dated resource file paths for the specified resource.
        /// </summary>
        /// <param name="resourceName">The name of the dated resource.</param>
        public static List<string> GetDatedResourceFiles(string resourceName)
        {
            if (!Directory.Exists(DatedResourcePath))
            {
                return new List<string>();
            }

            return Directory.EnumerateFiles(DatedResourcePath, "*", SearchOption.TopDirectoryOnly)
                .Where(path => Path.GetFileName(path).StartsWith(resourceName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        /// <summary>
        /// Method used to try loading a dated image resource based on the resource's name and its date.
        /// </summary>
        /// <param name="resourceName">Name of the dated resource that should be loaded.</param>
        /// <param name="date">Date of the dated resource that should be loaded.</param>
        /// <returns><see langword="true"/> if the image could be loaded, otherwise <see langword="false"/>.</returns>
        public static async Task<BitmapImage?> TryLoadDatedImageResource(string resourceName, DateOnly date)
        {
            bool resourceExists = TryGetDatedResourceFiles(resourceName, date, out string[] paths);

            if (!resourceExists)
            {
                return null;
            }

            string? imageResourcePath = paths.FirstOrDefault(IsValidImageResourcePath);

            if (imageResourcePath is null)
            {
                return null;
            }

            BitmapImage image = new BitmapImage();

            // Set the image source
            using (IRandomAccessStream stream = await FileRandomAccessStream.OpenAsync(imageResourcePath, FileAccessMode.Read))
            {
                await image.SetSourceAsync(stream);
            }

            return image;
        }
    }
}
