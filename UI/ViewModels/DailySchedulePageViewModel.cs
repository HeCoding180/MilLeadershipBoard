using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Models;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.UI.Pages;
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
    public class DailySchedulePageViewModel : IDisposable, INotifyPropertyChanged
    {
        //   ---   Public Constants   ---

        /// <summary>
        /// Constant string containing the resource name for daily schedule images.
        /// </summary>
        public const string DAILY_SCHEDULE_IMAGE_RESOURCE_NAME = "DailyScheduleImage";

        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the <see cref="RelayCommand"/> instance used for the <see cref="AddCommand"/> property.
        /// </summary>
        private RelayCommand _addCommand;

        /// <summary>
        /// Field containing the <see cref="CancellationTokenSource"/> used to cancel running tasks still running at the end of the ViewModel's lifetime.
        /// </summary>
        private readonly CancellationTokenSource _vmLifetimeCts = new CancellationTokenSource();

        /// <summary>
        /// Field containing the <see cref="CancellationTokenSource"/> used to cancel list updates.
        /// </summary>
        private volatile CancellationTokenSource? _listUpdateCancellationTokenSource;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="AddCommand"/>
        /// </summary>
        public ICommand AddCommand => _addCommand;

        /// <summary>
        /// Gets an <see cref="ObservableCollection{T}"/> containing all <see cref="DailySchedule"/> instances that should be displayed.
        /// </summary>
        public ObservableCollection<DailySchedule> DailySchedules { get; }

        /// <summary>
        /// Gets the <see cref="Microsoft.UI.Dispatching.DispatcherQueue"/> instance of the view.
        /// </summary>
        public DispatcherQueue DispatcherQueue { get; }

        /// <summary>
        /// Gets the <see cref="CancellationToken"/> used to cancel task at the end of the ViewModel's lifetime.
        /// </summary>
        public CancellationToken VMLifetimeCancellationToken => _vmLifetimeCts.Token;

        /// <summary>
        /// Sets or gets the <see cref="Microsoft.UI.Xaml.XamlRoot"/> instance used for any dialogs.
        /// </summary>
        public XamlRoot? XamlRoot { set; get; }

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="BreaksPageViewModel"/> class.
        /// </summary>
        /// <param name="dispatcherQueue"><see cref="Microsoft.UI.Dispatching.DispatcherQueue"/> instance of the view.</param>
        public DailySchedulePageViewModel(DispatcherQueue dispatcherQueue)
        {
            DispatcherQueue = dispatcherQueue;

            // Initialize fields
            _addCommand = new RelayCommand(InvokeAddDailySchedule);

            // Initialize properties
            DailySchedules = new ObservableCollection<DailySchedule>();

            // Subscribe to events
            ResourceManager.DatedResourceChanged += ResourceManager_DatedResourceChanged;

            // Load all existing daily schedule images
            DateOnly[] availableResourceDates = ResourceManager.GetAvailableResourceDates(DAILY_SCHEDULE_IMAGE_RESOURCE_NAME);
            foreach (DateOnly date in availableResourceDates)
            {
                if (date < DateOnly.FromDateTime(DateTime.Today))
                {
                    ResourceManager.DeleteDatedResource(DAILY_SCHEDULE_IMAGE_RESOURCE_NAME, date);

                    continue;
                }

                LoadDailyScheduleImage(date);
            }

            if(DailySchedules.Count > 0)
            {
                RefreshSchedulesList();
            }
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to load a new daily schedule image dated resource.
        /// </summary>
        /// <param name="date">The <see cref="DateOnly"/> of the dated resource that is to be loaded.</param>
        private void LoadDailyScheduleImage(DateOnly date)
        {
            lock (DailySchedules)
            {
                for (int i = 0; i < DailySchedules.Count; i++)
                {
                    DailySchedule iSchedule = DailySchedules[i];

                    if (iSchedule.Date < date)
                    {
                        continue;
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        BitmapImage image;
                        if (iSchedule.Date == date)
                        {
                            image = iSchedule.Source;
                        }
                        else
                        {
                            image = new BitmapImage();
                        }

                        Task loadingTask = ResourceManager.TryLoadDatedImageResource(DAILY_SCHEDULE_IMAGE_RESOURCE_NAME, date, image);
                        loadingTask.ContinueWith(OnImageLoadingTaskComplete, VMLifetimeCancellationToken);

                        if (i == 0)
                        {
                            // Renew the list updating task, as the first item changed
                            RefreshSchedulesList();
                        }

                        // No new DailySchedule instance needs to be added, return
                        if (iSchedule.Date == date)
                        {
                            return;
                        }

                        // Add the new schedule to the ObservableCollection
                        lock (DailySchedules)
                        {
                            DailySchedules.Insert(i, new DailySchedule(date, image));
                        }
                    });

                    return;
                }

                // The new schedule is the last element add it at the end
                DispatcherQueue.TryEnqueue(() =>
                {
                    BitmapImage image = new BitmapImage();

                    Task loadingTask = ResourceManager.TryLoadDatedImageResource(DAILY_SCHEDULE_IMAGE_RESOURCE_NAME, date, image);
                    loadingTask.ContinueWith(OnImageLoadingTaskComplete, VMLifetimeCancellationToken);

                    DailySchedules.Add(new DailySchedule(date, image));
                });
            }
        }

        /// <summary>
        /// Method used to refresh the <see cref="DailySchedules"/> list and remove and delete outdated items.
        /// </summary>
        private void RefreshSchedulesList() => RefreshSchedulesList(Task.CompletedTask);

        /// <summary>
        /// Method used to refresh the <see cref="DailySchedules"/> list and remove and delete outdated items.
        /// Matches the method outline of the <see cref="Task.ContinueWith(Action{Task})"/> method.
        /// </summary>
        /// <param name="completedTask">A completed task. Used to match the pattern of the <see cref="Task.ContinueWith(Action{Task})"/> method.</param>
        private void RefreshSchedulesList(Task completedTask)
        {
            // Ensure no task is running and clean up the cancellation token source
            _listUpdateCancellationTokenSource?.Cancel();
            _listUpdateCancellationTokenSource?.Dispose();
            _listUpdateCancellationTokenSource = null;

            // Abort if no daily schedules are registered
            if (DailySchedules.Count == 0)
            {
                return;
            }

            // Ignore if the task was cancelled
            if (completedTask.IsCanceled)
            {
                return;
            }

            // Ignore if the task resulted in an exception
            if (completedTask.IsFaulted)
            {
                // TODO: (optional) Add logging of this exception
                return;
            }

            // Get the date of the most recent / next daily schedule
            DateOnly mostRecentScheduleDate = DailySchedules[0].Date;

            // Calculate the time to the next list update
            DateTime listUpdateTime = new DateTime(mostRecentScheduleDate, TimeOnly.MinValue) + TimeSpan.FromDays(1);
            TimeSpan timeToListUpdate = listUpdateTime - DateTime.Now;

            // If less than 1 minute is left, do the refreshing
            if (timeToListUpdate.Minutes < 1)
            {
                // Immediately refresh the list and re-call the method
                // Note: This recursive call is needed to cover the case that the daily schedule this delay task was created for got deleted in the meanwhile.
                // If the task got deleted in the meanwhile this code would not be called and the task would just be delayed instead.
                // It also covers the case that there are multiple daily schedules that are outdated.

                // Ensure that the schedule is removed before deleting the file as deleting the file will cause the schedule to be removed with a delay
                DispatcherQueue.TryEnqueue(() =>
                {
                    lock (DailySchedules)
                    {
                        DailySchedules.RemoveAt(0);
                    }

                    RefreshSchedulesList();
                });

                // Delete the dated resource
                ResourceManager.DeleteDatedResource(DAILY_SCHEDULE_IMAGE_RESOURCE_NAME, mostRecentScheduleDate);

                return;
            }

            // Abort if the ViewModel lifetime cancellation token is cancelled
            if (VMLifetimeCancellationToken.IsCancellationRequested)
            {
                return;
            }

            // Create a linked cancellation token source that is linked to the ViewModel lifetime cancellation token
            _listUpdateCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(VMLifetimeCancellationToken);

            Task.Delay(timeToListUpdate.Milliseconds, _listUpdateCancellationTokenSource.Token).ContinueWith(RefreshSchedulesList);
        }

        /// <summary>
        /// Method used to remove a daily schedule image based on its <paramref name="date"/>.
        /// </summary>
        /// <param name="date">The date at which the daily schedule is to be removed.</param>
        private void RemoveDailyScheduleImage(DateOnly date)
        {
            lock (DailySchedules)
            {
                foreach (DailySchedule schedule in DailySchedules)
                {
                    if (schedule.Date != date)
                    {
                        continue;
                    }

                    DispatcherQueue.TryEnqueue(() =>
                    {
                        lock (DailySchedules)
                        {
                            DailySchedules.Remove(schedule);
                        }
                    });
                    break;
                }
            }
        }

        /// <summary>
        /// Calback method for the <see cref="ResourceManager.DatedResourceChanged"/> event.
        /// </summary>
        private void ResourceManager_DatedResourceChanged(DatedResourceChangedEventArgs args)
        {
            // Check if the resource name matches
            if (args.ResourceName != DAILY_SCHEDULE_IMAGE_RESOURCE_NAME)
            {
                return;
            }

            // Check if resource is outdated
            if (args.Date < DateOnly.FromDateTime(DateTime.Today))
            {
                // Delete outdated resource
                ResourceManager.DeleteDatedResource(DAILY_SCHEDULE_IMAGE_RESOURCE_NAME, args.Date);
                return;
            }

            switch (args.Action)
            {
                case DatedResourceChangedAction.Add:
                    LoadDailyScheduleImage(args.Date);
                    break;
                case DatedResourceChangedAction.Remove:
                    RemoveDailyScheduleImage(args.Date);
                    break;
                case DatedResourceChangedAction.Modify:
                    LoadDailyScheduleImage(args.Date);
                    break;
            }
        }

        /// <summary>
        /// Method used to handle the continuation action of the image loading task.
        /// </summary>
        /// <param name="completedTask">The task that completed.</param>
        private void OnImageLoadingTaskComplete(Task completedTask)
        {
            if (completedTask.IsFaulted)
            {
                // TODO: Handle task exception
            }
        }

        //   ---   Protected Methods   ---

        /// <summary>
        /// Method used to invoke the user prompting to add a new daily schedule.
        /// </summary>
        protected async void InvokeAddDailySchedule()
        {
            AddDailySchedulePage content = new AddDailySchedulePage();

            // RelayCommand for adding the daily schedule
            RelayCommand addCommand = new RelayCommand(()
                => ResourceManager.CreateDatedResourceFile(content.ViewModel.ScheduleImagePath,
                                                           DAILY_SCHEDULE_IMAGE_RESOURCE_NAME,
                                                           content.ViewModel.ScheduleDate));

            ContentDialog dialog = new ContentDialog()
            {
                PrimaryButtonText = ResourceManager.GetString("DailySchedulePage/AddDialog/PrimaryButtonText"),
                PrimaryButtonCommand = addCommand,
                IsPrimaryButtonEnabled = false,
                SecondaryButtonText = ResourceManager.GetString("DailySchedulePage/AddDialog/SecondaryButtonText"),
                DefaultButton = ContentDialogButton.Primary,
                Content = content,
                XamlRoot = XamlRoot
            };

            content.ViewModel.PropertyChanged += (s, e) => dialog.IsPrimaryButtonEnabled = content.ViewModel.ValidInput;

            await dialog.ShowAsync();
        }

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
            ResourceManager.DatedResourceChanged -= ResourceManager_DatedResourceChanged;

            _vmLifetimeCts.Cancel();
        }
    }
}
