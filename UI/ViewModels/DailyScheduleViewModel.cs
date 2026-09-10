using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;
using MilLeadershipBoard.Config;
using MilLeadershipBoard.Models;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.UI.Pages;
using MilLeadershipBoard.UI.UserControls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using static System.Net.Mime.MediaTypeNames;

namespace MilLeadershipBoard.UI.ViewModels
{
    public class DailyScheduleViewModel : IDisposable, INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        //   ---   Public Events   ---

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged"/>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="DailyScheduleViewModel"/> class.
        /// </summary>
        /// <param name="view">The view this ViewModel instance is assigned to.</param>
        public DailyScheduleViewModel(DailyScheduleView view)
        {
            
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
        /// Method used to dispose of this <see cref="DailyScheduleViewModel"/> instance.
        /// </summary>
        public void Dispose()
        {
            
        }
    }
}
