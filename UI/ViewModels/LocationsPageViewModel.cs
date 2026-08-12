using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.TroopData.Location;
using MilLeadershipBoard.UI.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;

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
                Header = ResourceManager.GetString("LocationsPage/LocationNameTextBox/HeaderText"),
                PlaceholderText = ResourceManager.GetString("LocationsPage/LocationNameTextBox/PlaceholderText")
            };

            ContentDialog dialog = new ContentDialog()
            {
                Title = ResourceManager.GetString("LocationsPage/AddLocationDialog/Title"),
                IsPrimaryButtonEnabled = false,
                PrimaryButtonText = ResourceManager.GetString("LocationsPage/AddLocationDialog/PrimaryButtonText"),
                PrimaryButtonCommand = new RelayCommand(() => CreateLocation(nameTextBox.Text)),
                SecondaryButtonText = ResourceManager.GetString("LocationsPage/AddLocationDialog/SecondaryButtonText"),
                Content = nameTextBox,
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = XamlRoot
            };

            nameTextBox.TextChanged += (sender, args) => dialog.IsPrimaryButtonEnabled = !string.IsNullOrWhiteSpace(nameTextBox.Text);

            await dialog.ShowAsync();
        }

        /// <summary>
        /// Method used to create a new <see cref="SoldierLocation"/> based on the set <paramref name="name"/>.
        /// </summary>
        /// <param name="name">The name of the location.</param>
        private void CreateLocation(string name)
        {
            SoldierLocation location = new SoldierLocation(name);
            ConfigManager.Config.Locations.Add(location);
        }

        //   ---   Public Methods   ---

        /// <inheritdoc cref="IDisposable.Dispose"/>
        void IDisposable.Dispose()
        {
            
        }
    }
}
