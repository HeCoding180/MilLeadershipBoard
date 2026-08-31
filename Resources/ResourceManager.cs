using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Enum used to specify the action that has been done to a dated resource.
    /// </summary>
    internal enum DatedResourceChangedAction
    {
        /// <summary>
        /// The dated resource was newly added.
        /// </summary>
        Add,
        /// <summary>
        /// An existing dated resource was removed.
        /// </summary>
        Remove,
        /// <summary>
        /// An existing dated resource was modified.
        /// </summary>
        Modify
    }

    /// <summary>
    /// <see cref="EventArgs"/> class used for the <see cref="ResourceManager.DatedResourceChanged"/> event.
    /// </summary>
    internal class DatedResourceChangedEventArgs : EventArgs
    {
        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="DatedResourceChangedAction"/> that was done to the dated resource.
        /// </summary>
        public DatedResourceChangedAction Action { get; }

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
        /// <param name="action">The <see cref="DatedResourceChangedAction"/> that was done to the dated resource.</param>
        public DatedResourceChangedEventArgs(string resourceName, DateOnly date, DatedResourceChangedAction action)
        {
            ResourceName = resourceName;
            Date = date;
            Action = action;
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
        /// Method used to generate the file path for a dated resource.
        /// </summary>
        /// <param name="resourceName">The name of the resource.</param>
        /// <param name="date">The date of theat resource instance.</param>
        /// <param name="fileExtension">The file extension.</param>
        /// <returns>A full path for the resource.</returns>
        private static string GenerateDatedResourcePath(string resourceName, DateOnly date, string fileExtension)
        {
            return Path.Combine(DatedResourcePath, GenerateDatedResourceFileName(resourceName, date, fileExtension));
        }

        /// <summary>
        /// Method used to get the date of a dated resource file's file name.
        /// </summary>
        /// <param name="fileName">File name of which the date should be extracted.</param>
        /// <returns>A <see cref="DateOnly"/> struct containing the date of the file.</returns>
        private static DateOnly GetDatedResourceFileDate(string fileName)
        {
            string dateStr = Path.GetFileNameWithoutExtension(fileName).Split('_').Last();

            return DateOnly.ParseExact(dateStr, DATE_FORMAT_STRING, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Raises the <see cref="DatedResourceChanged"/> event
        /// </summary>
        /// <param name="resourceName">Name of the dated resource that changed.</param>
        /// <param name="date">Date of the dated resource that changed.</param>
        /// <param name="action">The <see cref="DatedResourceChangedAction"/> that was done to the dated resource.</param>
        private static void OnDatedResourceChanged(string resourceName, DateOnly date, DatedResourceChangedAction action)
        {
            DatedResourceChanged?.Invoke(new DatedResourceChangedEventArgs(resourceName, date, action));
        }

        /// <summary>
        /// Method used to get all dated resource file paths for the specified resource.
        /// </summary>
        /// <param name="resourceName">The name of the dated resource.</param>
        /// <param name="paths"><see langword="out"/> array of strings containing available dated resources.</param>
        public static bool TryGetDatedResourceFiles(string resourceName, out string[] paths)
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
                .Where(path => Path.GetFileName(path).StartsWith(resourceName, StringComparison.OrdinalIgnoreCase))];

            return paths.Length != 0;
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

            string fileName = resourceName + "_" + date.ToString(DATE_FORMAT_STRING);

            paths = [.. Directory.EnumerateFiles(DatedResourcePath, "*", SearchOption.TopDirectoryOnly)
                .Where(path => Path.GetFileName(path).StartsWith(fileName, StringComparison.OrdinalIgnoreCase))];

            return paths.Length != 0;
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
            string resourcePath = GenerateDatedResourcePath(resourceName, date, fileExtension);

            EnsureDatedResourceDirectory();

            bool resourceExists = File.Exists(resourcePath);

            File.Copy(filePath, resourcePath, overwrite);

            OnDatedResourceChanged(resourceName, date, resourceExists ? DatedResourceChangedAction.Modify : DatedResourceChangedAction.Add);
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

        /// <summary>
        /// Method used to get all available dates for a specific dated resource based on the resource name.
        /// </summary>
        /// <param name="resourceName">Name of the resource.</param>
        /// <returns>A <see cref="DateOnly"/> array containing all available dated resource dates.</returns>
        public static DateOnly[] GetAvailableResourceDates(string resourceName)
        {
            bool anyResourceFiles = TryGetDatedResourceFiles(resourceName, out string[] paths);

            if (!anyResourceFiles)
            {
                // No dated resources.
                return [];
            }

            return [.. paths.Select(GetDatedResourceFileDate).Distinct()];
        }

        /// <inheritdoc cref="ResourceLoader.GetString"/>
        public static string GetString(string resourceId) => ResourceLoader.GetString(resourceId);

        /// <summary>
        /// Method used to check if a path is valid for a dated image resource.
        /// </summary>
        /// <param name="path">The path that is to be checked.</param>
        /// <returns><see langword="true"/> if the path is a valid image resource path.</returns>
        public static bool IsValidImageResourcePath(string path) => VALID_IMAGE_RESOURCE_FILE_EXTENSIONS.Contains(Path.GetExtension(path));

        /// <summary>
        /// Method used to try loading a dated image resource based on the resource's name and its date.
        /// </summary>
        /// <param name="resourceName">Name of the dated resource that should be loaded.</param>
        /// <param name="date">Date of the dated resource that should be loaded.</param>
        /// <returns><see langword="true"/> if the image could be loaded, otherwise <see langword="false"/>.</returns>
        public static async Task TryLoadDatedImageResource(string resourceName, DateOnly date, BitmapImage image)
        {
            bool resourceExists = TryGetDatedResourceFiles(resourceName, date, out string[] paths);

            if (!resourceExists)
            {
                return;
            }

            string? imageResourcePath = paths.FirstOrDefault(IsValidImageResourcePath);

            if (imageResourcePath is null)
            {
                return;
            }

            // Set the image source
            using (IRandomAccessStream stream = await FileRandomAccessStream.OpenAsync(imageResourcePath, FileAccessMode.Read))
            {
                await image?.SetSourceAsync(stream);
            }
        }
    }
}
