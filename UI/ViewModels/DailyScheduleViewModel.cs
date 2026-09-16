using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media.Imaging;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Messaging;
using MilLeadershipBoard.Models;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.UI.Pages;
using MilLeadershipBoard.UI.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace MilLeadershipBoard.UI.ViewModels
{
    public class DailyScheduleViewModel : IDisposable, INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the <see cref="RelayCommand"/> instance used by the <see cref="DeleteCommand"/> property.
        /// </summary>
        private RelayCommand _deleteCommand;

        /// <summary>
        /// Field containing the <see cref="RelayCommand"/> instance used by the <see cref="EditDateCommand"/> property.
        /// </summary>
        private RelayCommand _editDateCommand;

        /// <summary>
        /// Field containing the value of the <see cref="OptionsButtonVisibility"/> property.
        /// </summary>
        private Visibility _optionsButtonVisibility = Visibility.Collapsed;

        //   ---   Private Properties   ---

        /// <summary>
        /// Gets the model behind this ViewModel.
        /// </summary>
        private DailySchedule Model => View.Model ?? throw new InvalidProgramException("No model set in the DailyScheduleView.");

        /// <summary>
        /// Gets the view instance this ViewModel is assigned to.
        /// </summary>
        private DailyScheduleView View { get; }

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="ICommand"/> used to delete this daily schedule.
        /// </summary>
        public ICommand DeleteCommand => _deleteCommand;

        /// <summary>
        /// Gets the <see cref="ICommand"/> used to edit the date of this daily schedule.
        /// </summary>
        public ICommand EditDateCommand => _editDateCommand;

        /// <summary>
        /// Gets the header text that is to be displayed.
        /// </summary>
        public string HeaderText => View.Model?.Date.ToString(ConfigManager.Config.DailyScheduleDateFormat) ?? string.Empty;

        /// <summary>
        /// Sets or gets the <see cref="Visibility"/> of the options button in the view.
        /// </summary>
        public Visibility OptionsButtonVisibility
        {
            set
            {
                if (value == _optionsButtonVisibility)
                {
                    return;
                }

                _optionsButtonVisibility = value;

                OnPropertyChanged();
            }
            get => _optionsButtonVisibility;
        }

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="DailyScheduleViewModel"/> class.
        /// </summary>
        /// <param name="view">The view this ViewModel instance is assigned to.</param>
        public DailyScheduleViewModel(DailyScheduleView view)
        {
            View = view;

            // Create the RelayCommands for the button actions
            _deleteCommand = new RelayCommand(InvokeScheduleDeletion);
            _editDateCommand = new RelayCommand(InvokeDateEditing);
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to invoke the deletion of this daily schedule.
        /// </summary>
        private async Task DeleteAsync()
        {
            RelayCommand primaryCommand = new RelayCommand(() => ResourceManager.DeleteDatedResource(ResourceManager.DAILY_SCHEDULE_IMAGE_RESOURCE_NAME, Model.Date));

            ContentDialog dialog = new ContentDialog()
            {
                Title = ResourceManager.GetString("DailyScheduleViewModel/DeleteDialog/Title"),
                Content = ResourceManager.GetString("DailyScheduleViewModel/DeleteDialog/Content"),
                PrimaryButtonText = ResourceManager.YesString,
                PrimaryButtonCommand = primaryCommand,
                SecondaryButtonText = ResourceManager.NoString,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = View.XamlRoot
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Method used to invoke the deletion of this daily schedule.
        /// </summary>
        private async Task EditDateAsync()
        {
            DatePicker content = new DatePicker()
            {
                Header = ResourceManager.GetString("AddDailySchedulePage/ScheduleDatePicker/Header"),
                Date = Util.Util.DateOnlyToDateTimeOffset(Model.Date),
            };

            RelayCommand primaryCommand = new RelayCommand(()
                => ResourceManager.ChangeResourceDate(ResourceManager.DAILY_SCHEDULE_IMAGE_RESOURCE_NAME,
                                                      Model.Date,
                                                      Util.Util.DateTimeOffsetToDateOnly(content.Date)));

            ContentDialog dialog = new ContentDialog()
            {
                Title = ResourceManager.GetString("DailyScheduleViewModel/EditDateDialog/Title"),
                Content = content,
                PrimaryButtonText = ResourceManager.AcceptString,
                PrimaryButtonCommand = primaryCommand,
                SecondaryButtonText = ResourceManager.CancelString,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = View.XamlRoot
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Method used to invoke the editing of the schedule date.
        /// </summary>
        private void InvokeDateEditing()
        {
            Task dateEditingTask = EditDateAsync();

            dateEditingTask.ContinueWith(OnDateEdtiningCompleted);
        }

        /// <summary>
        /// Method used to invoke the deleton of this daily schedule.
        /// </summary>
        private void InvokeScheduleDeletion()
        {
            Task deletionTask = DeleteAsync();

            deletionTask.ContinueWith(OnScheduleDeletionCompleted);
        }

        /// <summary>
        /// Method executed upon completion of the date editing task.
        /// </summary>
        /// <param name="completionTask">The <see cref="Task"/> that completed.</param>
        private void OnDateEdtiningCompleted(Task completionTask)
        {
            if (completionTask.IsFaulted)
            {
                MessageDispatcher.ShowExceptionMessage(ResourceManager.GetString("DailyScheduleView/DateEditingFailedMessage"), completionTask.Exception);
            }
        }

        /// <summary>
        /// Method executed upon completion of the schedule deletion task.
        /// </summary>
        /// <param name="completionTask">The <see cref="Task"/> that completed.</param>
        private void OnScheduleDeletionCompleted(Task completionTask)
        {
            if (completionTask.IsFaulted)
            {
                MessageDispatcher.ShowExceptionMessage(ResourceManager.GetString("DailyScheduleView/DeletionFailedMessage"), completionTask.Exception);
            }
        }

        //   ---   Protected Methods   ---

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name">Name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to dispose of this <see cref="DailyScheduleViewModel"/> instance.
        /// </summary>
        public void Dispose()
        {

        }

        /// <summary>
        /// Callback method for when the model changes.
        /// </summary>
        public void OnModelChanged(DependencyObject sender, DependencyProperty modelProperty)
        {
            OnPropertyChanged(nameof(HeaderText));
        }

        /// <summary>
        /// Callback method for the <see cref="UIElement.PointerEntered"/> event of the view.
        /// </summary>
        public void OnPointerEntered(object sender, PointerRoutedEventArgs e)
        {
            OptionsButtonVisibility = Visibility.Visible;
        }

        /// <summary>
        /// Callback method for the <see cref="UIElement.PointerExited"/> event of the view.
        /// </summary>
        public void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            OptionsButtonVisibility = Visibility.Collapsed;
        }
    }
}
