using CommunityToolkit.Mvvm.Input;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Models;
using MilLeadershipBoard.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.UI.ViewModels
{
    /// <summary>
    /// ViewModel class for the <see cref="MilLeadershipBoard.UI.Pages.AddLessionPage"/> class.
    /// </summary>
    internal class AddLessionsPageViewModel : INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="CanAddLession"/> property.
        /// </summary>
        private bool _canAddLession = false;

        /// <summary>
        /// Field containing the value of the <see cref="LessionChief"/> property.
        /// </summary>
        private string? _lessionChief = null;

        /// <summary>
        /// Field containing the value of the <see cref="LessionName"/> property.
        /// </summary>
        private string _lessionName = string.Empty;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets if a lession can be created from this 
        /// </summary>
        public bool CanAddLession
        {
            private set
            {
                if (value == _canAddLession)
                {
                    return;
                }

                _canAddLession = value;

                OnPropertyChanged();
            }
            get => _canAddLession;
        }

        /// <summary>
        /// Sets or gets the name of the lession.
        /// </summary>
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
                UpdateCanCreateLessionState();
            }
            get => _lessionName;
        }

        /// <summary>
        /// Sets or gets the name of the lession.
        /// </summary>
        public string? LessionChief
        {
            set
            {
                if (value == _lessionChief)
                {
                    return;
                }

                _lessionChief = value;

                OnPropertyChanged();
                UpdateCanCreateLessionState();
            }
            get => _lessionChief;
        }

        /// <summary>
        /// Gets an array of nullable <see langword="string"/>s that contains all soldiers.
        /// </summary>
        public string?[] LessionChiefsItemsSource => [.. ((string?[])[null]).Concat(ConfigManager.Config.Soldiers.Select(s => $"{s.Rank} {s.LastName} {s.FirstName}"))];

        /// <summary>
        /// Gets the target null value for the lession chief ComboBox.
        /// </summary>
        public string LessionChiefTargetNullValue => ResourceManager.GetString("AddLessionsPage/LessionChiefTargetNullValue");

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="AddLessionsPageViewModel"/> class.
        /// </summary>
        public AddLessionsPageViewModel()
        {

        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Evaluates and updates the state for the <see cref="CanAddLession"/> property.
        /// </summary>
        /// <returns>The state for the <see cref="CanAddLession"/> property.</returns>
        private void UpdateCanCreateLessionState()
        {
            CanAddLession = !string.IsNullOrWhiteSpace(LessionName) &&
                            !string.IsNullOrWhiteSpace(LessionChief);
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

        /// <summary>
        /// Method used to add a lession.
        /// </summary>
        public void AddLession()
        {
            if (!CanAddLession)
            {
                return;
            }

            LessionData lession = new LessionData()
            {
                LessionChiefFullName = LessionChief ?? string.Empty,
                LessionName = LessionName
            };

            ConfigManager.Config.Lessions.Add(lession);
        }
    }
}
