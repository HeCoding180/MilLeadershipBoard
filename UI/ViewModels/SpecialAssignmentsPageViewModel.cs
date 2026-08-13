using MilLeadershipBoard.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.UI.ViewModels
{
    internal class SpecialAssignmentsPageViewModel : IDisposable, INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        //   ---   Public Properties   ---

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="BreaksPageViewModel"/> class.
        /// </summary>
        public SpecialAssignmentsPageViewModel()
        {
            ConfigManager.Config.PropertyChanged += Config_PropertyChanged;
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Callback method for the <see cref="ConfigData.PropertyChanged"/> event of the <see cref="ConfigManager"/>'s <see cref="ConfigData"/> instance.
        /// </summary>
        private void Config_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ConfigData):
                    break;
            }
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
        /// Method used to dispose of this <see cref="BreaksPageViewModel"/> instance.
        /// </summary>
        void IDisposable.Dispose()
        {
            ConfigManager.Config.PropertyChanged -= Config_PropertyChanged;
        }
    }
}
