using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.Windows.Storage.Pickers;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MilLeadershipBoard.UI.ViewModels
{
    /// <summary>
    /// ViewModel class for the <see cref="Pages.AddDailySchedulePage"/> class.
    /// </summary>
    public class AddDailySchedulePageViewModel : INotifyPropertyChanged, IDisposable
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the <see cref="RelayCommand"/> instance of the <see cref="BrowseCommand"/> property.
        /// </summary>
        RelayCommand _browseCommand;

        /// <summary>
        /// Field containing the value of the <see cref="ScheduleDate"/> property.
        /// </summary>
        DateOnly _scheduleDate = DateOnly.FromDateTime(DateTime.Today);

        /// <summary>
        /// Field containing the value of the <see cref="ScheduleImagePath"/> property.
        /// </summary>
        string _scheduleImagePath = string.Empty;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the command used to browse for the schedule image file.
        /// </summary>
        public ICommand BrowseCommand => _browseCommand;

        /// <summary>
        /// Sets or gets the <see cref="DateOnly"/> of the schedule image.
        /// </summary>
        public DateOnly ScheduleDate
        {
            set
            {
                if (value == _scheduleDate)
                {
                    return;
                }

                _scheduleDate = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ScheduleDateTimeOffset));
                OnPropertyChanged(nameof(ValidInput));
            }
            get => _scheduleDate;
        }

        /// <summary>
        /// Sets or gets the date of the schedule image using the 
        /// </summary>
        public DateTimeOffset ScheduleDateTimeOffset
        {
            set => ScheduleDate = DateOnly.FromDateTime(value.DateTime);
            get => new DateTimeOffset(ScheduleDate.ToDateTime(TimeOnly.MinValue),TimeSpan.Zero);
        }

        /// <summary>
        /// Sets or gets the path of the schedule image.
        /// </summary>
        public string ScheduleImagePath
        {
            set
            {
                if (value == _scheduleImagePath)
                {
                    return;
                }

                _scheduleImagePath = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ValidInput));
            }
            get => _scheduleImagePath;
        }

        /// <summary>
        /// Gets if the input is valid.
        /// </summary>
        public bool ValidInput
        {
            get
            {
                return !string.IsNullOrWhiteSpace(ScheduleImagePath) &&
                       Path.Exists(ScheduleImagePath) &&
                       ResourceManager.IsValidImageResourcePath(ScheduleImagePath) &&
                       (ScheduleDate >= DateOnly.FromDateTime(DateTime.Today));
            }
        }

        /// <summary>
        /// Sets or gets the <see cref="Microsoft.UI.Xaml.XamlRoot"/> instance of the view.
        /// </summary>
        public XamlRoot? XamlRoot { set; get; }

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="AddDailySchedulePageViewModel"/> class.
        /// </summary>
        /// <param name="xamlRoot">The <see cref="Microsoft.UI.Xaml.XamlRoot"/> instance of the view.</param>
        public AddDailySchedulePageViewModel()
        {
            _browseCommand = new RelayCommand(InvokeScheduleImageBrowse);
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Invokes the file picker used to select 
        /// </summary>
        private async void InvokeScheduleImageBrowse()
        {
            if (XamlRoot is null)
            {
                return;
            }

            FileOpenPicker picker = new FileOpenPicker(XamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                Title = ResourceManager.GetString("AddDailySchedulePage/SelectScheduleFileOpenPicker/Title"),
                CommitButtonText = ResourceManager.GetString("AddDailySchedulePage/SelectScheduleFileOpenPicker/CommitButtonText")
            };

            if (Path.Exists(ScheduleImagePath))
            {
                picker.SuggestedFolder = Path.GetDirectoryName(ScheduleImagePath);
                picker.SuggestedStartFolder = Path.GetDirectoryName(ScheduleImagePath);
            }

            PickFileResult? result = await picker.PickSingleFileAsync();

            if (result is null)
            {
                return;
            }

            ScheduleImagePath = result.Path;
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
        /// Method used to dispose of this <see cref="BreaksPageViewModel"/> instance.
        /// </summary>
        public void Dispose()
        {
            
        }
    }
}
