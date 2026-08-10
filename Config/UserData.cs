using MilLeadershipBoard.TroopData;
using MilLeadershipBoard.TroopData.Location;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Config
{
    internal class UserData : INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets an <see cref="ObservableCollection{T}"/> containing all <see cref="SoldierData"/> instances.
        /// </summary>
        public ObservableCollection<SoldierData> Soldiers { set; get; } = new ObservableCollection<SoldierData>();

        /// <summary>
        /// Sets or gets an <see cref="ObservableCollection{T}"/> containing all <see cref="SoldierLocation"/> instances.
        /// </summary>
        public ObservableCollection<SoldierLocation> Locations { set; get; } = new ObservableCollection<SoldierLocation>();

        //   ---   Public Events   ---

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to raise the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property whose value has changed.</param>
        private void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
