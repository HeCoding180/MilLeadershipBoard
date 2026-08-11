using Microsoft.Windows.ApplicationModel.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Resources
{
    /// <summary>
    /// Utility class used for managing resources.
    /// </summary>
    internal static class ResourceManager
    {
        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="ResourceLoader"/> instance that can be used to load resources such as UI strings.
        /// </summary>
        public static ResourceLoader ResourceLoader { get; } = new ResourceLoader();

        //   ---   Public Methods   ---

        /// <inheritdoc cref="Microsoft.Windows.ApplicationModel.Resources.ResourceLoader.GetString"/>
        public static string GetString(string resourceId) => ResourceLoader.GetString(resourceId);
    }
}
