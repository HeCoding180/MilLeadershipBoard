using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MilLeadershipBoard.UI.ViewModels
{
    internal class LocationsPageViewModel
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
        private void AddLocation()
        {

        }
    }
}
