using CommunityToolkit.Mvvm.Input;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Resources;
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
    internal class BreaksPageViewModel : IDisposable, INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="Add5MinCommand"/>.
        /// </summary>
        private RelayCommand _add5MinCommand;

        /// <summary>
        /// Field containing the value of the <see cref="Add10MinCommand"/>.
        /// </summary>
        private RelayCommand _add10MinCommand;

        /// <summary>
        /// Field containing the value of the <see cref="Add15MinCommand"/>.
        /// </summary>
        private RelayCommand _add15MinCommand;

        /// <summary>
        /// Field containing the value of the <see cref="ResetBreaksCommand"/>.
        /// </summary>
        private RelayCommand _resetBreaksCommand;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="ICommand"/> used to add 5 minutes to the breaks counter.
        /// </summary>
        public ICommand Add5MinCommand => _add5MinCommand;

        /// <summary>
        /// Gets the <see cref="ICommand"/> used to add 10 minutes to the breaks counter.
        /// </summary>
        public ICommand Add10MinCommand => _add10MinCommand;

        /// <summary>
        /// Gets the <see cref="ICommand"/> used to add 15 minutes to the breaks counter.
        /// </summary>
        public ICommand Add15MinCommand => _add15MinCommand;

        /// <summary>
        /// Gets the text that is to be displayed 
        /// </summary>
        public string BreaksText => $"{ConfigManager.Config.BreaksCounter} {ResourceManager.GetString("BreaksPage/BreaksCounter/MinutesText")}";

        /// <summary>
        /// Gets the <see cref="ICommand"/> used to reset the breaks counter.
        /// </summary>
        public ICommand ResetBreaksCommand => _resetBreaksCommand;

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="BreaksPageViewModel"/> class.
        /// </summary>
        public BreaksPageViewModel()
        {
            ConfigManager.Config.PropertyChanged += Config_PropertyChanged;

            _add5MinCommand = new RelayCommand(() => IncreaseBreaksCounter(5));
            _add10MinCommand = new RelayCommand(() => IncreaseBreaksCounter(10));
            _add15MinCommand = new RelayCommand(() => IncreaseBreaksCounter(15));
            _resetBreaksCommand = new RelayCommand(() => ConfigManager.Config.BreaksCounter = 0);
        }

        //   ---   Private Methods (static)   ---

        /// <summary>
        /// Method used to increase the time of the breaks counter.
        /// </summary>
        /// <param name="time">The time amount by which the breaks counter should be increased.</param>
        private static void IncreaseBreaksCounter(int time)
        {
            ConfigManager.Config.BreaksCounter += time;
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Callback method for the <see cref="ConfigData.PropertyChanged"/> event of the <see cref="ConfigManager"/>'s <see cref="ConfigData"/> instance.
        /// </summary>
        private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConfigData.BreaksCounter):
                    OnPropertyChanged(nameof(BreaksText));
                    break;
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
        /// Method used to dispose of this <see cref="BreaksPageViewModel"/> instance.
        /// </summary>
        void IDisposable.Dispose()
        {
            ConfigManager.Config.PropertyChanged -= Config_PropertyChanged;
        }
    }
}
