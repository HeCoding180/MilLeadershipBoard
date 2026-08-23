using CommunityToolkit.Mvvm.Input;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Models.TroopData;
using MilLeadershipBoard.TroopData.Location;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;

namespace MilLeadershipBoard.UI.ViewModels
{
    internal class SoldiersPageViewModel : INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="CreateCommand"/> property.
        /// </summary>
        private RelayCommand _createCommand;

        /// <summary>
        /// Field containing the last registered state of the <see cref="canCreateSoldier"/> property.
        /// </summary>
        private bool _lastCanCreateSoldierState = false;

        /// <summary>
        /// Field containing the value of the <see cref=""/> property.
        /// </summary>
        private string _newSoldierFirstName = string.Empty;

        /// <summary>
        /// Field containing the value of the <see cref=""/> property.
        /// </summary>
        private string _newSoldierLastName = string.Empty;

        /// <summary>
        /// Field containing the value of the <see cref=""/> property.
        /// </summary>
        private string _newSoldierRank = string.Empty;

        //   ---   Private Properties   ---

        /// <summary>
        /// Gets if a new <see cref="SoldierData"/> instance can be created based on the specified input data.
        /// </summary>
        private bool canCreateSoldier
        {
            get
            {
                return (!string.IsNullOrWhiteSpace(NewSoldierFirstName))
                    && (!string.IsNullOrWhiteSpace(NewSoldierLastName))
                    && (!string.IsNullOrWhiteSpace(NewSoldierRank));
            }
        }

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="ICommand"/> that can be used to create a new soldier based on the specified input data.
        /// </summary>
        public ICommand CreateCommand => _createCommand;

        /// <summary>
        /// Sets or gets the current first name for a new, to be created soldier.
        /// </summary>
        public string NewSoldierFirstName
        {
            set
            {
                if (value == _newSoldierFirstName)
                {
                    return;
                }

                _newSoldierFirstName = value;

                OnPropertyChanged();
                OnCanCreateSoldierChanged();
            }
            get => _newSoldierFirstName;
        }

        /// <summary>
        /// Sets or gets the current last name for a new, to be created soldier.
        /// </summary>
        public string NewSoldierLastName
        {
            set
            {
                if (value == _newSoldierLastName)
                {
                    return;
                }

                _newSoldierLastName = value;

                OnPropertyChanged();
                OnCanCreateSoldierChanged();
            }
            get => _newSoldierLastName;
        }

        /// <summary>
        /// Sets or gets the current rank for a new, to be created soldier.
        /// </summary>
        public string NewSoldierRank
        {
            set
            {
                if (value == _newSoldierRank)
                {
                    return;
                }

                _newSoldierRank = value;

                OnPropertyChanged();
                OnCanCreateSoldierChanged();
            }
            get => _newSoldierRank;
        }

        /// <summary>
        /// Gets an <see cref="ObservableCollection{T}"/> containing all <see cref="SoldierData"/> instances.
        /// </summary>
        public ObservableCollection<SoldierData> Soldiers => ConfigManager.Config.Soldiers;

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raised when a new <see cref="SoldierData"/> instance is created.
        /// </summary>
        public event TypedEventHandler<SoldiersPageViewModel, EventArgs>? SoldierCreated;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="SoldiersPageViewModel"/> class.
        /// </summary>
        public SoldiersPageViewModel()
        {
            _createCommand = new RelayCommand(addSoldier, () => canCreateSoldier);
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to add a new soldier based on the specified input data.
        /// </summary>
        private void addSoldier()
        {
            SoldierData soldier = new SoldierData(NewSoldierFirstName, NewSoldierLastName)
            {
                Rank = NewSoldierRank,
                LocationId = ConfigManager.Config.DefaultLocationId
            };

            ConfigManager.Config.Soldiers.Add(soldier);

            NewSoldierFirstName = string.Empty;
            NewSoldierLastName = string.Empty;
            NewSoldierRank = string.Empty;

            OnSoldierCreated();
        }

        //   ---   Protected Methods   ---

        /// <summary>
        /// Method used to notify that the state of the <see cref="canCreateSoldier"/> property has changed.
        /// </summary>
        protected void OnCanCreateSoldierChanged()
        {
            bool currentCanCreateSoldierState = canCreateSoldier;

            if (_lastCanCreateSoldierState == currentCanCreateSoldierState)
            {
                return;
            }

            _lastCanCreateSoldierState = currentCanCreateSoldierState;

            _createCommand.NotifyCanExecuteChanged();
        }

        /// <summary>
        /// Method used to call the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that has changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Method used to raise the <see cref="SoldierCreated"/> event.
        /// </summary>
        protected void OnSoldierCreated()
        {
            SoldierCreated?.Invoke(this, EventArgs.Empty);
        }
    }
}
