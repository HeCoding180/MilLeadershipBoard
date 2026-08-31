using MilLeadershipBoard.Config;
using MilLeadershipBoard.Models.TroopData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Models
{
    /// <summary>
    /// Model class containing lession information.
    /// </summary>
    public class LessionData : INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the custom full name of the lession chief. Contains <see cref="string.Empty"/> if no custom name is set.
        /// </summary>
        private string _lessionChiefCustomFullName = string.Empty;

        /// <summary>
        /// Field containing the value for the <see cref="LessionChiefId"/> property.
        /// </summary>
        private Guid _lessionChiefId = Guid.Empty;

        /// <summary>
        /// Field containing the value for the <see cref="LessionName"/> property.
        /// </summary>
        private string _lessionName = string.Empty;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="Guid"/> identifier of this lession.
        /// </summary>
        [JsonPropertyName("Id")]
        public Guid Id { get; }

        /// <summary>
        /// Sets or gets the <see cref="SoldierData"/> instance of the lession chief.
        /// </summary>
        [JsonIgnore]
        public SoldierData? LessionChief
        {
            set
            {
                LessionChiefId = value?.Id ?? Guid.Empty;
            }
            get
            {
                if (LessionChiefId == Guid.Empty)
                {
                    return null;
                }

                return ConfigManager.Config.Soldiers.FirstOrDefault(s => s?.Id == LessionChiefId);
            }
        }

        /// <summary>
        /// Sets or gets the custom full name of the lession chief.
        /// </summary>
        [JsonPropertyName("LessionChiefCustomFullName")]
        public string LessionChiefCustomFullName
        {
            set
            {
                if (value == _lessionChiefCustomFullName)
                {
                    return;
                }

                _lessionChiefCustomFullName = value;

                OnPropertyChanged();
            }
            get => _lessionChiefCustomFullName;
        }

        /// <summary>
        /// Sets or gets the full name of the lession chief. If the set value doesn't match the full name of a registerd <see cref="SoldierData"/> a custom name will be saved.
        /// Otherwise the <see cref="LessionChiefId"/> will be set accordingly.
        /// </summary>
        [JsonIgnore]
        public string LessionChiefFullName
        {
            set
            {
                if (value == string.Empty)
                {
                    // Empty name set, remove lession chief and clear custom name
                    LessionChiefId = Guid.Empty;
                    LessionChiefCustomFullName = string.Empty;
                }

                if ((LessionChief is not null) && (value == LessionChief.GetFullName()))
                {
                    // Full name matches the full name of the lession chief SoldierData instance, clear the custom name
                    LessionChiefCustomFullName = string.Empty;
                    return;
                }

                // Search for a SoldierData instance with a matching full name
                SoldierData? match = ConfigManager.Config.Soldiers.FirstOrDefault(s => s.GetFullName() == value);

                if (match is null)
                {
                    // No soldier that has a matching full name was found
                    // Use custom lession chief full name
                    LessionChiefCustomFullName = value;

                    // Remove lession chief
                    LessionChiefId = Guid.Empty;
                }
                else
                {
                    // Soldier instance with matching name found
                    // Clear custom name
                    LessionChiefCustomFullName = string.Empty;

                    // Set lession chief ID
                    LessionChiefId = match.Id;
                }
            }
            get => LessionChief?.GetFullName() ?? LessionChiefCustomFullName;
        }

        /// <summary>
        /// Sets or gets the <see cref="Guid"/> identifier of the lession chief soldier.
        /// </summary>
        [JsonPropertyName("LessionChiefId")]
        public Guid LessionChiefId
        {
            set
            {
                if (value == _lessionChiefId)
                {
                    return;
                }

                _lessionChiefId = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(LessionChiefFullName));
            }
            get => _lessionChiefId;
        }

        /// <summary>
        /// Sets or gets the name of the lession.
        /// </summary>
        [JsonPropertyName("LessionName")]
        public string LessionName
        {
            set
            {
                if (value == _lessionName)
                {
                    return;
                }

                _lessionName = value;

                OnPropertyChanged();
            }
            get => _lessionName;
        }

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="LessionData"/> class and generates a new <see cref="Guid"/> identifier for it.
        /// </summary>
        public LessionData() : this(Guid.NewGuid())
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="LessionData"/> class with a specified <see cref="Guid"/> identifier.
        /// </summary>
        /// <param name="id">The <see cref="Guid"/> identifier of this lession.</param>
        [JsonConstructor]
        public LessionData(Guid id)
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
    }
}
