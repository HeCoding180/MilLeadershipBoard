using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
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
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MilLeadershipBoard.UI.ViewModels
{
    internal class WeeklySchedulePageViewModel : IDisposable, INotifyPropertyChanged
    {
        //   ---   Private Constants   ---

        /// <summary>
        /// Constant containing the name of the dated resource of the weekly schedule A.
        /// </summary>
        private const string WEEKLY_SCHEDULE_A_DATED_RESOURCE_NAME = "WeeklyScheduleA";

        /// <summary>
        /// Constant containing the name of the dated resource of the weekly schedule B.
        /// </summary>
        private const string WEEKLY_SCHEDULE_B_DATED_RESOURCE_NAME = "WeeklyScheduleB";

        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing a <see cref="CancellationTokenSource"/> that can be used to cancel any ongoing image loading tasks.
        /// </summary>
        private CancellationTokenSource _imageLoadingCancellationTokenSource = new CancellationTokenSource();

        /// <summary>
        /// Field containing the value of the <see cref="WeeklyScheduleAImageLoaded"/> property.
        /// </summary>
        private bool _weeklyScheduleAImageLoaded = false;

        /// <summary>
        /// Field containing the value of the <see cref="WeeklyScheduleBImageLoaded"/> property.
        /// </summary>
        private bool _weeklyScheduleBImageLoaded = false;

        //   ---   Private Properties   ---

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> that is to be used for image loading tasks.
        /// </summary>
        private CancellationToken imageLoadingCancellationToken => _imageLoadingCancellationTokenSource.Token;

        //   ---   Private Properties (static)   ---

        /// <summary>
        /// Gets the <see cref="DateOnly"/> of the dated resource of the current weekly schedule.
        /// </summary>
        private static DateOnly CurrentWeeklyScheduleDate => Util.Util.GetMondayOfWeek(DateOnly.FromDateTime(DateTime.Now));

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="BitmapImage"/> for the weekly schedule A.
        /// </summary>
        public BitmapImage WeeklyScheduleAImage { get; } = new BitmapImage();

        /// <summary>
        /// Gets if the <see cref="WeeklyScheduleAImage"/> is loaded.
        /// </summary>
        public bool WeeklyScheduleAImageLoaded
        {
            private set
            {
                if (value == _weeklyScheduleAImageLoaded)
                {
                    return;
                }

                _weeklyScheduleAImageLoaded = value;

                OnPropertyChanged();
            }
            get => _weeklyScheduleAImageLoaded;
        }

        /// <summary>
        /// Gets the <see cref="BitmapImage"/> for the weekly schedule B.
        /// </summary>
        public BitmapImage WeeklyScheduleBImage { get; } = new BitmapImage();

        /// <summary>
        /// Gets if the <see cref="WeeklyScheduleAImage"/> is loaded.
        /// </summary>
        public bool WeeklyScheduleBImageLoaded
        {
            private set
            {
                if (value == _weeklyScheduleBImageLoaded)
                {
                    return;
                }

                _weeklyScheduleBImageLoaded = value;

                OnPropertyChanged();
            }
            get => _weeklyScheduleBImageLoaded;
        }

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="BreaksPageViewModel"/> class.
        /// </summary>
        public WeeklySchedulePageViewModel()
        {
            ConfigManager.Config.PropertyChanged += Config_PropertyChanged;
            ResourceManager.DatedResourceChanged += ResourceManager_DatedResourceChanged;

            WeeklyScheduleAImage.ImageOpened += WeeklyScheduleAImage_ImageOpened;
            WeeklyScheduleAImage.ImageFailed += WeeklyScheduleAImage_ImageFailed;
            WeeklyScheduleBImage.ImageOpened += WeeklyScheduleBImage_ImageOpened;
            WeeklyScheduleBImage.ImageFailed += WeeklyScheduleBImage_ImageFailed;
        }

        //   ---   Private Methods (static)   ---

        /// <summary>
        /// Method used to request the user to enter the weekly schedule image and get its path.
        /// </summary>
        /// <param name="xamlRoot">The <see cref="XamlRoot"/> used for dialogs.</param>
        /// <returns>The file path of a weekly schedule image file.</returns>
        private static async Task<string> GetNewWeeklyScheduleImageResourcePath(XamlRoot xamlRoot)
        {
            FileOpenPicker picker = new FileOpenPicker(xamlRoot.ContentIslandEnvironment.AppWindowId)
            {
                CommitButtonText = ResourceManager.GetString("WeeklySchedulePage/SelectScheduleStoragePicker/CommitButtonText")
            };

            PickFileResult? result = await picker.PickSingleFileAsync();

            if (result is null)
            {
                return string.Empty;
            }

            return result.Path;
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Callback method for the <see cref="ConfigData.PropertyChanged"/> event of the <see cref="ConfigManager"/>'s <see cref="ConfigData"/> instance.
        /// </summary>
        private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConfigData):
                    break;
            }
        }

        private void OnWeeklyScheduleALoadingTaskCompleted(Task loadingTask)
        {
            if (!loadingTask.IsCompletedSuccessfully)
            {
                // TODO: Implement exception handling
                return;
            }
        }

        private void OnWeeklyScheduleBLoadingTaskCompleted(Task loadingTask)
        {
            if (!loadingTask.IsCompletedSuccessfully)
            {
                // TODO: Implement exception handling
                return;
            }
        }

        /// <summary>
        /// Callback method for the <see cref="ResourceManager.DatedResourceChanged"/> event.
        /// </summary>
        private void ResourceManager_DatedResourceChanged(DatedResourceChangedEventArgs args)
        {
            switch (args.ResourceName)
            {
                case WEEKLY_SCHEDULE_A_DATED_RESOURCE_NAME:
                    TryLoadWeeklyScheduleAImage();
                    break;
                case WEEKLY_SCHEDULE_B_DATED_RESOURCE_NAME:
                    TryLoadWeeklyScheduleBImage();
                    break;
                default:
                    return;
            }
        }

        /// <summary>
        /// Method used to try and load the image for the <see cref="WeeklyScheduleAImage"/>
        /// </summary>
        private void TryLoadWeeklyScheduleAImage()
        {
            Task loadingTask = ResourceManager.TryLoadDatedImageResource(WEEKLY_SCHEDULE_A_DATED_RESOURCE_NAME,
                                                                         CurrentWeeklyScheduleDate,
                                                                         WeeklyScheduleAImage);

            loadingTask.ContinueWith(OnWeeklyScheduleALoadingTaskCompleted);
        }

        /// <summary>
        /// Method used to try and load the image for the <see cref="WeeklyScheduleBImage"/>
        /// </summary>
        private void TryLoadWeeklyScheduleBImage()
        {
            Task loadingTask = ResourceManager.TryLoadDatedImageResource(WEEKLY_SCHEDULE_B_DATED_RESOURCE_NAME,
                                                                         CurrentWeeklyScheduleDate,
                                                                         WeeklyScheduleBImage);

            loadingTask.ContinueWith(OnWeeklyScheduleBLoadingTaskCompleted);
        }

        /// <summary>
        /// Callback method for the <see cref="BitmapImage.ImageFailed"/> event of the <see cref="WeeklyScheduleAImage"/> instance.
        /// </summary>
        private void WeeklyScheduleAImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            WeeklyScheduleAImageLoaded = false;
        }

        /// <summary>
        /// Callback method for the <see cref="BitmapImage.ImageOpened"/> event of the <see cref="WeeklyScheduleAImage"/> instance.
        /// </summary>
        private void WeeklyScheduleAImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            WeeklyScheduleAImageLoaded = true;
        }

        /// <summary>
        /// Callback method for the <see cref="BitmapImage.ImageFailed"/> event of the <see cref="WeeklyScheduleBImage"/> instance.
        /// </summary>
        private void WeeklyScheduleBImage_ImageFailed(object sender, ExceptionRoutedEventArgs e)
        {
            WeeklyScheduleBImageLoaded = false;
        }

        /// <summary>
        /// Callback method for the <see cref="BitmapImage.ImageOpened"/> event of the <see cref="WeeklyScheduleBImage"/> instance.
        /// </summary>
        private void WeeklyScheduleBImage_ImageOpened(object sender, RoutedEventArgs e)
        {
            WeeklyScheduleBImageLoaded = true;
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
        void IDisposable.Dispose()
        {
            ConfigManager.Config.PropertyChanged -= Config_PropertyChanged;
            ResourceManager.DatedResourceChanged -= ResourceManager_DatedResourceChanged;

            WeeklyScheduleAImage.ImageOpened -= WeeklyScheduleAImage_ImageOpened;
            WeeklyScheduleAImage.ImageFailed -= WeeklyScheduleAImage_ImageFailed;
            WeeklyScheduleBImage.ImageOpened -= WeeklyScheduleBImage_ImageOpened;
            WeeklyScheduleBImage.ImageFailed -= WeeklyScheduleBImage_ImageFailed;
        }

        /// <summary>
        /// Method used to invoke the change of the weekly schedule A.
        /// </summary>
        public async Task InvokeWeeklyScheduleAChange(XamlRoot xamlRoot)
        {
            string imagePath = await GetNewWeeklyScheduleImageResourcePath(xamlRoot);

            if (imagePath == string.Empty)
            {
                // File picking failed
                return;
            }

            ResourceManager.CreateDatedResourceFile(imagePath, WEEKLY_SCHEDULE_A_DATED_RESOURCE_NAME, CurrentWeeklyScheduleDate);
        }

        /// <summary>
        /// Method used to invoke the change of the weekly schedule B.
        /// </summary>
        public async Task InvokeWeeklyScheduleBChange(XamlRoot xamlRoot)
        {
            string imagePath = await GetNewWeeklyScheduleImageResourcePath(xamlRoot);

            if (imagePath == string.Empty)
            {
                // File picking failed
                return;
            }

            ResourceManager.CreateDatedResourceFile(imagePath, WEEKLY_SCHEDULE_B_DATED_RESOURCE_NAME, CurrentWeeklyScheduleDate);
        }

        public void OnParentPageLoaded()
        {
            TryLoadWeeklyScheduleAImage();
            TryLoadWeeklyScheduleBImage();
        }
    }
}
