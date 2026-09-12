using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.Windows.Storage.Pickers;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.UI.Pages;
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
    public class WeeklySchedulePageViewModel : IDisposable, INotifyPropertyChanged
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

        //   ---   Private Properties (static)   ---

        /// <summary>
        /// Gets the <see cref="DateOnly"/> of the dated resource of the current weekly schedule.
        /// </summary>
        private static DateOnly CurrentWeeklyScheduleDate => Util.Util.GetMondayOfWeek(DateOnly.FromDateTime(DateTime.Now));

        //   ---   Private Properties   ---

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> that is to be used for image loading tasks.
        /// </summary>
        private CancellationToken imageLoadingCancellationToken => _imageLoadingCancellationTokenSource.Token;

        /// <summary>
        /// Gets the View this ViewModel instance is assigned to.
        /// </summary>
        private WeeklySchedulePage View { get; }

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
        /// Creates a new instance of the <see cref="WeeklySchedulePageViewModel"/> class.
        /// </summary>
        /// <param name="view">The View instance this <see cref="WeeklySchedulePageViewModel"/> ViewModel instance is assgined to.</param>
        public WeeklySchedulePageViewModel(WeeklySchedulePage view)
        {
            View = view;

            ResourceManager.DatedResourceChanged += ResourceManager_DatedResourceChanged;

            WeeklyScheduleAImage.ImageOpened += WeeklyScheduleAImage_ImageOpened;
            WeeklyScheduleAImage.ImageFailed += WeeklyScheduleAImage_ImageFailed;
            WeeklyScheduleBImage.ImageOpened += WeeklyScheduleBImage_ImageOpened;
            WeeklyScheduleBImage.ImageFailed += WeeklyScheduleBImage_ImageFailed;
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to request the user to enter the weekly schedule image and get its path.
        /// </summary>
        /// <returns>The file path of a weekly schedule image file.</returns>
        private async Task PickWeeklyScheduleImageResource(string resourceName)
        {
            AddWeeklySchedulePage content = new AddWeeklySchedulePage(resourceName);

            ContentDialog dialog = new ContentDialog()
            {
                Title = ResourceManager.GetString("WeeklySchedulePage/AddDialog/Title"),
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = ResourceManager.GetString("WeeklySchedulePage/AddDialog/PrimaryButtonText"),
                PrimaryButtonCommand = content.AddScheduleCommand,
                IsPrimaryButtonEnabled = content.CanAdd,
                SecondaryButtonText = ResourceManager.GetString("WeeklySchedulePage/AddDialog/SecondaryButtonText"),
                Content = content,
                XamlRoot = View.XamlRoot
            };

            // Ensure the enabled state of the primary button gets updated automatically
            content.CanAddChanged += (s, canAdd) => dialog.IsPrimaryButtonEnabled = canAdd;

            await dialog.ShowAsync();
        }

        private void OnWeeklyScheduleLoadingTaskCompleted(Task loadingTask)
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

            loadingTask.ContinueWith(OnWeeklyScheduleLoadingTaskCompleted);
        }

        /// <summary>
        /// Method used to try and load the image for the <see cref="WeeklyScheduleBImage"/>
        /// </summary>
        private void TryLoadWeeklyScheduleBImage()
        {
            Task loadingTask = ResourceManager.TryLoadDatedImageResource(WEEKLY_SCHEDULE_B_DATED_RESOURCE_NAME,
                                                                         CurrentWeeklyScheduleDate,
                                                                         WeeklyScheduleBImage);

            loadingTask.ContinueWith(OnWeeklyScheduleLoadingTaskCompleted);
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
            ResourceManager.DatedResourceChanged -= ResourceManager_DatedResourceChanged;

            WeeklyScheduleAImage.ImageOpened -= WeeklyScheduleAImage_ImageOpened;
            WeeklyScheduleAImage.ImageFailed -= WeeklyScheduleAImage_ImageFailed;
            WeeklyScheduleBImage.ImageOpened -= WeeklyScheduleBImage_ImageOpened;
            WeeklyScheduleBImage.ImageFailed -= WeeklyScheduleBImage_ImageFailed;

            _imageLoadingCancellationTokenSource.Cancel();
        }

        /// <summary>
        /// Method used to invoke the change of the weekly schedule A.
        /// </summary>
        public async Task InvokeWeeklyScheduleAChange()
        {
            await PickWeeklyScheduleImageResource(WEEKLY_SCHEDULE_A_DATED_RESOURCE_NAME);
        }

        /// <summary>
        /// Method used to invoke the change of the weekly schedule B.
        /// </summary>
        public async Task InvokeWeeklyScheduleBChange()
        {
            await PickWeeklyScheduleImageResource(WEEKLY_SCHEDULE_B_DATED_RESOURCE_NAME);
        }

        public void OnParentPageLoaded()
        {
            TryLoadWeeklyScheduleAImage();
            TryLoadWeeklyScheduleBImage();
        }
    }
}
