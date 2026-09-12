using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using MilLeadershipBoard.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;

namespace MilLeadershipBoard.UI.Pages
{
    /// <summary>
    /// Base class for pages that are used for adding schedule images.
    /// </summary>
    public abstract class AddSchedulePageBase : Page
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the <see cref="RelayCommand"/> instance used for the <see cref="AddScheduleCommand"/> property.
        /// </summary>
        private RelayCommand _addScheduleCommand;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets an <see cref="ICommand"/> instance that can be used to add the currently specified image or pdf page as the schedule for this adding page's dated resource.
        /// </summary>
        public ICommand AddScheduleCommand => _addScheduleCommand;

        /// <inheritdoc cref="AddSchedulePageViewModel.CanAdd"/>
        public bool CanAdd => ViewModel.CanAdd;

        /// <summary>
        /// Gets the name of the dated resource this adding page is for.
        /// </summary>
        public string ResourceName { get; }

        /// <summary>
        /// Gets the ViewModel instance of this View.
        /// </summary>
        public AddSchedulePageViewModel ViewModel { get; }

        //   ---   Public Events   ---

        /// <inheritdoc cref="AddSchedulePageViewModel.CanAddChanged"/>
        public event TypedEventHandler<AddSchedulePageViewModel, bool>? CanAddChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="AddSchedulePageBase"/> class.
        /// </summary>
        /// <param name="resourceName">The name of the resource used for the schedule.</param>
        public AddSchedulePageBase(string resourceName)
        {
            // Assign the property values
            ResourceName = resourceName;

            // Create the ViewModel
            ViewModel = new AddSchedulePageViewModel(this);
            ViewModel.CanAddChanged += OnCanAddChanged;

            // Create the RelayCommand instances
            _addScheduleCommand = new RelayCommand(() => AddSchedule(ResourceName));
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Raises the <see cref="CanAddChanged"/> event.
        /// </summary>
        private void OnCanAddChanged(AddSchedulePageViewModel sender, bool canExecute)
        {
            CanAddChanged?.Invoke(sender, canExecute);
        }

        //   ---   Public Methods   ---

        // TODO: Implement exception handling using a task continuation action.
        /// <inheritdoc cref="AddSchedulePageViewModel.AddSchedule(string)"/>
        public void AddSchedule(string resourceName) => _ = ViewModel.AddSchedule(resourceName);
    }
}
