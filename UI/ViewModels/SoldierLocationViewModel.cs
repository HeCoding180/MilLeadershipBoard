using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.TroopData.Location;
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
    class SoldierLocationViewModel : INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="DeleteLocationCommand"/> property.
        /// </summary>
        private RelayCommand _deleteLocationCommand;

        /// <summary>
        /// Field containing the value of the <see cref="Location"/> property.
        /// </summary>
        private SoldierLocation? _location = null;

        /// <summary>
        /// Field containing the value of the <see cref="MakeDefaultCommand"/> property.
        /// </summary>
        private RelayCommand _makeDefaultCommand;

        /// <summary>
        /// Field containing the value of the <see cref="RenameLocationCommand"/> property.
        /// </summary>
        private RelayCommand _renameLocationCommand;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance that is used for deleting a Location.
        /// </summary>
        public ICommand DeleteLocationCommand => _deleteLocationCommand;

        /// <summary>
        /// Sets or gets the <see cref="SoldierLocation"/> instance this viewmodel is linked with.
        /// </summary>
        public SoldierLocation? Location
        {
            set
            {
                if (ReferenceEquals(value, _location))
                {
                    return;
                }

                if (_location is not null)
                {
                    _location.PropertyChanged -= Location_PropertyChanged;
                }

                _location = value;

                if (value is not null)
                {
                    value.PropertyChanged += Location_PropertyChanged;
                }

                OnPropertyChanged();

                // Notify of the other property changes
                OnPropertyChanged(nameof(LocationName));
            }
            get => _location;
        }

        /// <summary>
        /// Gets the name of this instance's <see cref="SoldierLocation"/> instance.
        /// </summary>
        public string LocationName
        {
            set
            {
                if (Location is null)
                {
                    return;
                }

                Location.Name = value;
            }
            get => Location?.Name ?? string.Empty;
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance that is used to make the current location the default location.
        /// </summary>
        public ICommand MakeDefaultCommand => _makeDefaultCommand;

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance that is used for renaming a Location.
        /// </summary>
        public ICommand RenameLocationCommand => _renameLocationCommand;

        /// <summary>
        /// Sets or gets the <see cref="XamlRoot"/> instance of the view this viewmodel is assigned to.
        /// </summary>
        public XamlRoot? XamlRoot { set; get; } = null;

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="SoldierLocationViewModel"/> class.
        /// </summary>
        public SoldierLocationViewModel()
        {
            _deleteLocationCommand = new RelayCommand(DeleteLocation);
            _makeDefaultCommand = new RelayCommand(MakeDefault);
            _renameLocationCommand = new RelayCommand(RenameLocation);
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to trigger the deletion of the <see cref="SoldierLocation"/> instance.
        /// </summary>
        private async void DeleteLocation()
        {
            if (Location is null)
            {
                return;
            }

            ContentDialog dialog = new ContentDialog()
            {

            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Callback method for the <see cref="SoldierLocation.PropertyChanged"/> event of this instance's <see cref="Location"/> instance.
        /// Manages event forwarding.
        /// </summary>
        private void Location_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(SoldierLocation.Name):
                    OnPropertyChanged(nameof(LocationName));
                    break;
            }
        }

        /// <summary>
        /// Method used to make this <see cref="SoldierLocation"/> the default location.
        /// </summary>
        private async void MakeDefault()
        {
            if (Location is null)
            {
                return;
            }

            if (ConfigManager.UserData.DefaultLocationId == Location.Id)
            {
                return;
            }

            ContentDialog dialog = new ContentDialog()
            {
                Title = ResourceManager.GetString("SoldierLocationView/MakeDefaultDialog/Title"),
                Content = ResourceManager.GetString("SoldierLocationView/MakeDefaultDialog/Content"),
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonText = ResourceManager.GetString("SoldierLocationView/MakeDefaultDialog/AcceptButtonText"),
                PrimaryButtonCommand = new RelayCommand(() => ConfigManager.UserData.DefaultLocationId = Location.Id),
                SecondaryButtonText = ResourceManager.GetString("SoldierLocationView/MakeDefaultDialog/RejectButtonText"),
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Method used to trigger the renaming of the <see cref="SoldierLocation"/> instance.
        /// </summary>
        private async void RenameLocation()
        {
            if (Location is null)
            {
                return;
            }

            TextBox nameTextBox = new TextBox()
            {
                Header = ResourceManager.GetString("LocationsPage/LocationNameTextBox/HeaderText"),
                PlaceholderText = ResourceManager.GetString("LocationsPage/LocationNameTextBox/PlaceholderText"),
                Text = LocationName
            };

            ContentDialog dialog = new ContentDialog()
            {
                Title = ResourceManager.GetString("SoldierLocationView/RenameDialog/Title"),
                Content = nameTextBox,
                DefaultButton = ContentDialogButton.Primary,
                IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(LocationName),
                PrimaryButtonText = ResourceManager.GetString("SoldierLocationView/RenameDialog/AcceptButtonText"),
                PrimaryButtonCommand = new RelayCommand(() => LocationName = nameTextBox.Text),
                SecondaryButtonText = ResourceManager.GetString("SoldierLocationView/RenameDialog/RejectButtonText"),
                XamlRoot = XamlRoot
            };

            nameTextBox.TextChanged += (s, e) => dialog.IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(nameTextBox.Text);

            await dialog.ShowAsync();
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
    }
}
