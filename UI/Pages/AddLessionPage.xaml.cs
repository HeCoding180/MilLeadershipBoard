using CommunityToolkit.Mvvm.Input;
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

        /// <summary>
        /// Gets the action that is to be executed when the state of the <see cref="AddLessionsPageViewModel.CanAddLession"/> property changes.
        /// </summary>
        public Action<bool>? CanAddLessionChangedAction { set; get; }

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="AddLessionPage"/> class.
        /// </summary>
        public AddLessionPage()
        {
            _addLessionCommand = new RelayCommand(AddLession);

            InitializeComponent();
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Callback method for the <see cref="FrameworkElement.Loaded"/> event of this page.
        /// </summary>
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext is AddLessionsPageViewModel vm)
            {
                vm.PropertyChanged += Vm_PropertyChanged;
            }
        }

        /// <summary>
        /// Callback method for the <see cref="INotifyPropertyChanged.PropertyChanged"/> event of the DataContext ViewModel.
        /// </summary>
        private void Vm_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (CanAddLessionChangedAction is null)
            {
                return;
            }

            if (e.PropertyName == nameof(AddLessionsPageViewModel.CanAddLession))
            {
                CanAddLessionChangedAction(((AddLessionsPageViewModel)sender!).CanAddLession);
            }
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to add a lession.
        /// </summary>
        public void AddLession()
        {
            if (DataContext is AddLessionsPageViewModel vm)
            {
                vm.AddLession();
            }
        }
    }
}
