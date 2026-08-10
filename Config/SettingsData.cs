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
    internal class SettingsData : INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        //   ---   Public Properties   ---

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
