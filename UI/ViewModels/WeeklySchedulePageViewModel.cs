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
        /// Sets or gets the <see cref="DispatcherQueue"/> of the page this ViewModel is part of.
        /// </summary>
        public DispatcherQueue? PageDispatcherQueue { set; get; }

        /// <summary>
        /// Gets the <see cref="BitmapImage"/> for the weekly schedule A.
        /// </summary>
        public BitmapImage? WeeklyScheduleAImage { private set; get; }

        /// <summary>
        /// Gets the <see cref="BitmapImage"/> for the weekly schedule B.
        /// </summary>
        public BitmapImage? WeeklyScheduleBImage { private set; get; }

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

            return (await picker.PickSingleFileAsync()).Path;
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

        private void OnWeeklyScheduleALoadingTaskCompleted(Task<BitmapImage?> loadingTask)
        {
            if (!loadingTask.IsCompletedSuccessfully)
            {
                // TODO: Implement exception handling
                return;
            }

            BitmapImage? image = loadingTask.Result;

            if (image is null)
            {
                return;
            }

            WeeklyScheduleAImage = image;

            // Dispatch the PropertyChanged event invocation to the page's dispatcher queue
            PageDispatcherQueue?.TryEnqueue(() => OnPropertyChanged(nameof(WeeklyScheduleAImage)));
        }

        private void OnWeeklyScheduleBLoadingTaskCompleted(Task<BitmapImage?> loadingTask)
        {
            if (!loadingTask.IsCompletedSuccessfully)
            {
                // TODO: Implement exception handling
                return;
            }

            BitmapImage? image = loadingTask.Result;

            if (image is null)
            {
                return;
            }

            WeeklyScheduleBImage = image;

            // Dispatch the PropertyChanged event invocation to the page's dispatcher queue
            PageDispatcherQueue?.TryEnqueue(() => OnPropertyChanged(nameof(WeeklyScheduleBImage)));
        }

        /// <summary>
        /// Callback method for the <see cref="ResourceManager.DatedResourceChanged"/> event.
        /// </summary>
        private void ResourceManager_DatedResourceChanged(DatedResourceChangedEventArgs args)
        {
            if (PageDispatcherQueue is null)
            {
                // No page dispatcher queue set
                // Return as the result cannot be processed.
                return;
            }

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

        private void TryLoadWeeklyScheduleAImage()
        {
            Task<BitmapImage?> loadingTask = Task.Run(() => ResourceManager.TryLoadDatedImageResource(WEEKLY_SCHEDULE_A_DATED_RESOURCE_NAME, CurrentWeeklyScheduleDate),
                                                      imageLoadingCancellationToken);

            loadingTask.ContinueWith(OnWeeklyScheduleALoadingTaskCompleted);
        }

        private void TryLoadWeeklyScheduleBImage()
        {
            Task<BitmapImage?> loadingTask = Task.Run(() => ResourceManager.TryLoadDatedImageResource(WEEKLY_SCHEDULE_B_DATED_RESOURCE_NAME, CurrentWeeklyScheduleDate),
                                                      imageLoadingCancellationToken);

            loadingTask.ContinueWith(OnWeeklyScheduleALoadingTaskCompleted);
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
            PageDispatcherQueue = null;

            ConfigManager.Config.PropertyChanged -= Config_PropertyChanged;
            ResourceManager.DatedResourceChanged -= ResourceManager_DatedResourceChanged;
        }

        /// <summary>
        /// Method used to invoke the change of the weekly schedule A.
        /// </summary>
        public async Task InvokeWeeklyScheduleAChange(XamlRoot xamlRoot)
        {
            string imagePath = await GetNewWeeklyScheduleImageResourcePath(xamlRoot);

            ResourceManager.CreateDatedResourceFile(imagePath, WEEKLY_SCHEDULE_A_DATED_RESOURCE_NAME, CurrentWeeklyScheduleDate);
        }

        /// <summary>
        /// Method used to invoke the change of the weekly schedule B.
        /// </summary>
        public async Task InvokeWeeklyScheduleBChange(XamlRoot xamlRoot)
        {
            string imagePath = await GetNewWeeklyScheduleImageResourcePath(xamlRoot);

            ResourceManager.CreateDatedResourceFile(imagePath, WEEKLY_SCHEDULE_B_DATED_RESOURCE_NAME, CurrentWeeklyScheduleDate);
        }

        public void OnParentPageLoaded()
        {
            TryLoadWeeklyScheduleAImage();
            TryLoadWeeklyScheduleBImage();
        }
    }
}
