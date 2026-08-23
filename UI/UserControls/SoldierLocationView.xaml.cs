using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MilLeadershipBoard.Models.TroopData.Location;
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

namespace MilLeadershipBoard.UI.UserControls
{
    internal sealed partial class SoldierLocationView : UserControl
    {
        //   ---   Public Properties (static)   ---

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Location"/> property.
        /// </summary>
        public static readonly DependencyProperty LocationProperty =
        DependencyProperty.Register(
            nameof(Location),
            typeof(SoldierLocation),
            typeof(SoldierLocationView),
            new PropertyMetadata(null, OnItemChanged));

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the <see cref="SoldierLocation"/> instance.
        /// </summary>
        public SoldierLocation Location
        {
            set => SetValue(LocationProperty, value);
            get => (SoldierLocation)GetValue(LocationProperty);
        }

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="SoldierLocationView"/> class.
        /// </summary>
        public SoldierLocationView()
        {
            InitializeComponent();
        }

        //   ---   Private Methods (static)   ---

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not SoldierLocationView view)
            {
                return;
            }
        }

        //   ---   Private Methods   ---

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (RootBorder.DataContext is SoldierLocationViewModel vm)
            {
                vm.XamlRoot = this.XamlRoot;
                vm.Location = this.Location;
            }
        }

        private void ContentListView_Drop(object sender, DragEventArgs e)
        {
            if (RootBorder.DataContext is SoldierLocationViewModel vm)
            {
                vm.ItemDropped(sender, e);
            }
        }
    }
}
