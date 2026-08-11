using MilLeadershipBoard.Config;
using MilLeadershipBoard.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MilLeadershipBoard.TroopData.Location
{
    /// <summary>
    /// Class used to specify a troop location that will be shown under the 
    /// </summary>
    public class SoldierLocation : IEquatable<SoldierLocation>, INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="Name"/> property.
        /// </summary>
        private string _name;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="Guid"/> identifier of this <see cref="SoldierLocation"/> instance.
        /// </summary>
        [JsonPropertyName("Id")]
        public Guid Id { get; }

        /// <summary>
        /// Sets or gets the name of the troop location.
        /// </summary>
        [JsonPropertyName("Name")]
        public string Name
        {
            set
            {
                if (value == _name)
                {
                    return;
                }

                _name = value;

                OnPropertyChanged();
            }
            get => _name;
        }

        /// <summary>
        /// Gets a <see cref="ObservableFilteredList{T}"/> containing all <see cref="SoldierData"/> instances whose location is at this location.
        /// </summary>
        [JsonIgnore]
        public ObservableFilteredList<SoldierData> Soldiers { get; }

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="SoldierLocation"/> class.
        /// </summary>
        /// <param name="name">Name of the troop location.</param>
        public SoldierLocation(string name)
        {
            _name = name;

            // Create the filtered list
            Soldiers = new ObservableFilteredList<SoldierData>(ConfigManager.UserData.Soldiers, (s) => s.LocationId == Id, nameof(SoldierData.LocationId));
        }

        /// <summary>
        /// Constructor used for deserialization.
        /// </summary>
        /// <param name="id">Identifier of the <see cref="SoldierLocation"/>.</param>
        /// <param name="name">Name of the troop location.</param>
        [JsonConstructor]
        public SoldierLocation(Guid id, string name) : this(name)
        {
            Id = id;
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

        /// <inheritdoc cref="IEquatable{T}.Equals"/>
        bool IEquatable<SoldierLocation>.Equals(SoldierLocation? other)
        {
            if (other is null)
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            // If equality cannot be determined by reference, use the ID to compare equality
            return Id.Equals(other.Id);
        }
    }
}
