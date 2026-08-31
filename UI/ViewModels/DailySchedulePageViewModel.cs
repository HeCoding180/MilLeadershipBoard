using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MilLeadershipBoard.UI.ViewModels
{
    internal class DailySchedulePageViewModel : IDisposable, INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the <see cref="RelayCommand"/> instance used for the <see cref="AddCommand"/> property.
        /// </summary>
        private RelayCommand _addCommand;

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
        public DailySchedulePageViewModel()
        {
            // Initialize fields
            _addCommand = new RelayCommand(InvokeAddDailySchedule);

            // Initialize properties
            DailySchedules = new ObservableCollection<DailySchedule>();

            // Subscribe to events
            ConfigManager.Config.PropertyChanged += Config_PropertyChanged;
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

        //   ---   Protected Methods   ---

        /// <summary>
        /// Method used to invoke the user prompting to add a new daily schedule.
        /// </summary>
        protected async void InvokeAddDailySchedule()
        {
            throw new NotImplementedException();
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
            ConfigManager.Config.PropertyChanged -= Config_PropertyChanged;
        }
    }
}
