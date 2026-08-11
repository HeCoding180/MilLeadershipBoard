using MilLeadershipBoard.TroopData;
using MilLeadershipBoard.TroopData.Location;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Config
{
    internal class UserData : INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="DefaultLocation"/> property.
        /// </summary>
        private Guid _defaultLocationId = Guid.Empty;

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the <see cref="Guid"/> of the default <see cref="SoldierLocation"/> for new <see cref="SoldierData"/> instances
        /// or when a <see cref="SoldierLocation"/> instance is deleted to move its <see cref="SoldierData"/> instance to.
        /// </summary>
        [JsonPropertyName("DefaultLocationId")]
        public Guid DefaultLocationId
        {
            set
            {
                if (value == _defaultLocationId)
                {
                    return;
                }

                _defaultLocationId = value;

                OnPropertyChanged();
            }
            get => _defaultLocationId;
        }

        [JsonIgnore]
        public SoldierLocation? DefaultLocation
        {
            set => DefaultLocationId = value?.Id ?? Guid.Empty;
            get
            {
                Guid id = DefaultLocationId;

                if (id == Guid.Empty)
                {
                    return null;
                }

                return Locations.FirstOrDefault(l => l.Id == id);
            }
        }

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
