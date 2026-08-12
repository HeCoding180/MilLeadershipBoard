using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MilLeadershipBoard.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MilLeadershipBoard.UI.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SoldiersPage : Page
    {
        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="SoldiersPage"/> class.
        /// </summary>
        public SoldiersPage()
        {
            InitializeComponent();
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method called when the page is loaded.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is SoldiersPageViewModel vm)
            {
                vm.SoldierCreated += ViewModel_SoldierCreated;
            }
        }

        /// <summary>
        /// Method called when a new soldier has been created by the ViewModel.
        /// </summary>
        private void ViewModel_SoldierCreated(SoldiersPageViewModel sender, EventArgs args)
        {
            // Focus the RankTextBox
            RankTextBox.Focus(FocusState.Programmatic);
        }
    }
}
