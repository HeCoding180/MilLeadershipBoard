using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MilLeadershipBoard.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MilLeadershipBoard.UI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddWeeklySchedulePage : AddSchedulePageBase
    {
        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="AddWeeklySchedulePage"/> class.
        /// </summary>
        /// <param name="resourceName">The name of the resource used for the schedule.</param>
        public AddWeeklySchedulePage(string resourceName) : base(resourceName)
        {
            // Set the DateFormatter of the ViewModel to always use the monday of the week
            ViewModel.DateFormatter = Util.Util.GetMondayOfWeek;

            InitializeComponent();
        }
    }
}
