using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.TroopData.Location;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MilLeadershipBoard.UI.ViewModels
{
    internal class LocationsPageViewModel : IDisposable
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="AddLocationCommand"/> property.
        /// </summary>
        private RelayCommand _addLocationCommand;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance that is used for adding a Location.
        /// </summary>
        public ICommand AddLocationCommand => _addLocationCommand;

        /// <summary>
        /// Sets or gets the <see cref="XamlRoot"/> instance of the view this viewmodel is assigned to.
        /// </summary>
        public XamlRoot? XamlRoot { set; get; } = null;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="LocationsPageViewModel"/> class.
        /// </summary>
        public LocationsPageViewModel()
        {
            _addLocationCommand = new RelayCommand(AddLocation);
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to add a new location.
        /// </summary>
        private async void AddLocation()
        {
            if (XamlRoot is null)
            {
                return;
            }

            TextBox nameTextBox = new TextBox()
            {
                Header = ResourceManager.GetString("LocationsPage/AddLocationDialog/NameTextBox/HeaderText"),
                PlaceholderText = ResourceManager.GetString("LocationsPage/AddLocationDialog/NameTextBox/PlaceholderText")
            };

            RelayCommand createCommand = new RelayCommand(() => CreateLocation(nameTextBox.Text), () => !string.IsNullOrEmpty(nameTextBox.Text));

            nameTextBox.TextChanged += (sender, args) => createCommand.NotifyCanExecuteChanged();

            ContentDialog dialog = new ContentDialog()
            {
                Title = ResourceManager.GetString("LocationsPage/AddLocationDialog/Title"),
                PrimaryButtonText = ResourceManager.GetString("LocationsPage/AddLocationDialog/PrimaryButtonText"),
                SecondaryButtonText = ResourceManager.GetString("LocationsPage/AddLocationDialog/SecondaryButtonText"),
                Content = nameTextBox,
                DefaultButton = ContentDialogButton.Primary,
                PrimaryButtonCommand = createCommand,
                XamlRoot = XamlRoot
            };

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Method used to create a new <see cref="SoldierLocation"/> based on the set <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the location.</param>
        private void CreateLocation(string name)
        {
            SoldierLocation location = new SoldierLocation(name);
            ConfigManager.UserData.Locations.Add(location);
        }

        //   ---   Public Methods   ---

        /// <inheritdoc cref="IDisposable.Dispose"/>
        void IDisposable.Dispose()
        {
            
        }
    }
}
