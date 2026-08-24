using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MilLeadershipBoard.UI.ViewModels
{
    /// <summary>
    /// ViewModel class used to display <see cref="LessionData"/> instances.
    /// </summary>
    internal class LessionDataViewModel : INotifyPropertyChanged, IDisposable
    {
        //   ---   Private Constants   ---

        /// <summary>
        /// Constant containing the duration that the lession completion visual is shown.
        /// </summary>
        private const double LESSION_COMPLETION_DELAY_MS = 1500.0;

        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="CompleteLessionCommand"/> property.
        /// </summary>
        private RelayCommand _completeLessionCommand;

        /// <summary>
        /// Field containing the value of the <see cref="MainContentVisibility"/> property.
        /// </summary>
        private Visibility _mainContentVisibility = Visibility.Visible;

        /// <summary>
        /// Field containing the value of the <see cref="Model"/> property.
        /// </summary>
        private LessionData? _model = null;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance used to complete a lession.
        /// </summary>
        public ICommand CompleteLessionCommand => _completeLessionCommand;

        /// <summary>
        /// Gets the visibility of the completion content.
        /// </summary>
        public Visibility CompletionContentVisibility => _mainContentVisibility == Visibility.Collapsed ? Visibility.Visible : Visibility.Collapsed;

        /// <summary>
        /// Sets or gets the <see cref="Microsoft.UI.Dispatching.DispatcherQueue"/> instance used to 
        /// </summary>
        public DispatcherQueue? DispatcherQueue { set; get; }

        /// <summary>
        /// Gets the Lession's name.
        /// </summary>
        public string LessionName => Model?.LessionName ?? string.Empty;

        /// <summary>
        /// Gets the name of the lession chief.
        /// </summary>
        public string LessionChiefName => Model?.LessionChiefFullName ?? string.Empty;

        /// <summary>
        /// Gets the visibility of the main content.
        /// </summary>
        public Visibility MainContentVisibility
        {
            private set
            {
                if (value == _mainContentVisibility)
                {
                    return;
                }

                _mainContentVisibility = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(CompletionContentVisibility));
            }
            get => _mainContentVisibility;
        }

        /// <summary>
        /// Sets or gets the <see cref="LessionData"/> instance.
        /// </summary>
        public LessionData? Model
        {
            set
            {
                if (value == _model)
                {
                    return;
                }

                // Unsubscribe from the PropertyChanged event of the old model
                if (_model is not null)
                {
                    _model.PropertyChanged -= Model_PropertyChanged;
                }

                _model = value;

                // Subscribe to the PropertyChanged event of the new model
                if (_model is not null)
                {
                    _model.PropertyChanged += Model_PropertyChanged;
                }

                OnPropertyChanged();
                OnPropertyChanged(nameof(LessionName));
                OnPropertyChanged(nameof(LessionChiefName));

                MainContentVisibility = Visibility.Visible;
            }
            get => _model;
        }

        /// <summary>
        /// Callback method for the <see cref="INotifyPropertyChanged.PropertyChanged"/> event of the <see cref="Model"/>.
        /// </summary>
        private void Model_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(LessionData.LessionName):
                    OnPropertyChanged(nameof(LessionName));
                    break;
                case nameof(LessionData.LessionChiefFullName):
                    OnPropertyChanged(nameof(LessionChiefName));
                    break;
            }
        }

        //   ---   Public Events   ---

        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="LessionDataViewModel"/>.
        /// </summary>
        public LessionDataViewModel()
        {
            _completeLessionCommand = new RelayCommand(InvokeLessionCompletion);
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Callback method for the <see cref="DispatcherQueueTimer.Tick"/> event of the <see cref="DispatcherQueueTimer"/> used to delay the deletion of this instance's lession.
        /// </summary>
        private void DeletionDelayTimer_Tick(DispatcherQueueTimer sender, object args)
        {
            if (Model is null) return;

            ConfigManager.Config.Lessions.Remove(Model);
        }

        //   ---   Protected Methods   ---

        /// <summary>
        /// Method used to raise the <see cref="PropertyChanged"/> event with automatic property name assignment.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to dispose of this <see cref="LessionDataViewModel"/> instance.
        /// </summary>
        public void Dispose()
        {
            if (Model is not null)
            {
                Model.PropertyChanged -= Model_PropertyChanged;
            }
        }

        /// <summary>
        /// Method used to invoke the completion of the <see cref="LessionData"/> instance associated with this instance.
        /// </summary>
        public void InvokeLessionCompletion()
        {
            MainContentVisibility = Visibility.Collapsed;

            if (DispatcherQueue is null)
            {
                if (Model is null) return;

                ConfigManager.Config.Lessions.Remove(Model);

                return;
            }

            DispatcherQueueTimer timer = DispatcherQueue.CreateTimer();
            timer.IsRepeating = false;
            timer.Interval = TimeSpan.FromMilliseconds(LESSION_COMPLETION_DELAY_MS);
            timer.Tick += DeletionDelayTimer_Tick;

            timer.Start();
        }
    }
}
