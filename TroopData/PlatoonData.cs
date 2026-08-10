using MilLeadershipBoard.TroopData.Location;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MilLeadershipBoard.TroopData
{
    internal class PlatoonData : IEquatable<PlatoonData>, INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="Name"/> property.
        /// </summary>
        private string _name;

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the name of the platoon.
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
        /// Gets the <see cref="Guid"/> identifier of this platoon.
        /// </summary>
        [JsonPropertyName("Id")]
        public Guid Id { get; }

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="PlatoonData"/> class.
        /// </summary>
        /// <param name="name">Name of the platoon.</param>
        public PlatoonData(string name)
        {
            _name = name;
        }

        /// <summary>
        /// Constructor used for deserialization.
        /// </summary>
        /// <param name="name">Name of the platoon.</param>
        [JsonConstructor]
        public PlatoonData(Guid id, string name)
        {
            _name = name;
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
        bool IEquatable<PlatoonData>.Equals(PlatoonData? other)
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
