using System;
using System.Collections.Generic;
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

        //   ---   Public Events   ---

        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to raise the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name">Name of the property whose value has changed.</param>
        private void OnPropertyChanged([CallerMemberName] string name = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
