using MilLeadershipBoard.Config;
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

namespace MilLeadershipBoard.TroopData
{
    /// <summary>
    /// Class used to store data about an individual solder.
    /// </summary>
    public class SoldierData : IEquatable<SoldierData>, INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="FirstName"/> property.
        /// </summary>
        private string _firstName;

        /// <summary>
        /// Field containing the value of the <see cref="LastName"/> property.
        /// </summary>
        private string _lastName;

        /// <summary>
        /// Field containing the value of the <see cref="LocationId"/> property.
        /// </summary>
        private Guid _locationId = Guid.Empty;

        /// <summary>
        /// Field containing the value of the <see cref="PlatoonId"/> property.
        /// </summary>
        private Guid _platoonId = Guid.Empty;

        /// <summary>
        /// Field containing the value of the <see cref="Rank"/> property.
        /// </summary>
        private string _rank = string.Empty;

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the first name of the solder.
        /// </summary>
        [JsonPropertyName("FirstName")]
        public string FirstName
        {
            set
            {
                if (value == _firstName)
                {
                    return;
                }

                _firstName = value;

                OnPropertyChanged();
            }
            get => _firstName;
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> identifier of this platoon.
        /// </summary>
        [JsonPropertyName("Id")]
        public Guid Id { get; }

        /// <summary>
        /// Sets or gets the last name of the solder.
        /// </summary>
        [JsonPropertyName("LastName")]
        public string LastName
        {
            set
            {
                if (value == _lastName)
                {
                    return;
                }

                _lastName = value;

                OnPropertyChanged();
            }
            get => _lastName;
        }

        /// <summary>
        /// Sets or gets the <see cref="SoldierLocation"/> of this soldier.
        /// </summary>
        [JsonIgnore]
        public SoldierLocation? Location
        {
            set
            {
                LocationId = value?.Id ?? Guid.Empty;
            }
            get => ConfigManager.Config.Locations.FirstOrDefault(l => l.Id == LocationId);
        }

        /// <summary>
        /// Sets or gets the <see cref="Guid"/> of the location this soldier is currently at.
        /// </summary>
        [JsonPropertyName("LocationId")]
        public Guid LocationId
        {
            set
            {
                if (value == _locationId)
                {
                    return;
                }

                _locationId = value;

                Location?.Soldiers.ReevaluateItem(this);
                
                OnPropertyChanged();
                OnPropertyChanged(nameof(Location));
                OnPropertyChanged(nameof(LocationName));
            }
            get => _locationId;
        }

        /// <summary>
        /// Gets the name of the <see cref="SoldierLocation"/> this instance is assigned to.
        /// </summary>
        [JsonIgnore]
        public string LocationName => Location?.Name ?? "Unassigned";

        /// <summary>
        /// Gets an <see cref="ObservableCollection{T}"/> containing all <see cref="SoldierLocation"/> instances.
        /// </summary>
        public ObservableCollection<SoldierLocation> Locations => ConfigManager.Config.Locations;

        /// <summary>
        /// Sets or gets the <see cref="Guid"/> of the platoon this soldier is part of.
        /// </summary>
        [JsonPropertyName("PlatoonId")]
        public Guid PlatoonId
        {
            set
            {
                if (value == _platoonId)
                {
                    return;
                }

                _platoonId = value;

                OnPropertyChanged();
            }
            get => _platoonId;
        }

        /// <summary>
        /// Sets or gets the military rank of this soldier.
        /// </summary>
        public string Rank
        {
            set
            {
                if (value == _rank)
                {
                    return;
                }

                _rank = value;

                OnPropertyChanged();
            }
            get => _rank;
        }

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="SoldierData"/> class.
        /// </summary>
        /// <param name="firstName">First name of the soldier.</param>
        /// <param name="lastName">Last name of the soldier.</param>
        public SoldierData(string firstName, string lastName)
        {
            _firstName = firstName;
            _lastName = lastName;

            Id = Guid.NewGuid();
        }

        /// <summary>
        /// Constructor used for deserialization.
        /// </summary>
        /// <param name="id">Identifier of the soldier.</param>
        /// <param name="firstName">First name of the soldier.</param>
        /// <param name="lastName">Last name of the soldier.</param>
        [JsonConstructor]
        public SoldierData(Guid id, string firstName, string lastName)
        {
            Id = id;
            _firstName = firstName;
            _lastName = lastName;
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
        bool IEquatable<SoldierData>.Equals(SoldierData? other)
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
