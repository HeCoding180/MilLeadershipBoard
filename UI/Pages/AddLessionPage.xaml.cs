using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Input;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MilLeadershipBoard.UI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddLessionPage : Page
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="AddLessionCommand"/> command.
        /// </summary>
        private RelayCommand _addLessionCommand;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="ICommand"/> used to add a new lession.
        /// </summary>
        public ICommand AddLessionCommand => _addLessionCommand;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="AddLessionPage"/> class.
        /// </summary>
        public AddLessionPage()
        {
            _addLessionCommand = new RelayCommand(AddLession, CanAddLession);

            InitializeComponent();
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to add a lession.
        /// </summary>
        private void AddLession()
        {

        }

        /// <summary>
        /// Method used to check if a lession can be added.
        /// </summary>
        private bool CanAddLession()
        {

        }
    }
}
