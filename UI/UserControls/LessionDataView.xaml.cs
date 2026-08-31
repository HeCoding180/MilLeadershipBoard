using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MilLeadershipBoard.Models;
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
    public sealed partial class LessionDataView : UserControl
    {
        //   ---   Public Properties (static)   ---

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="Location"/> property.
        /// </summary>
        public static readonly DependencyProperty LessionProperty =
        DependencyProperty.Register(
            nameof(Lession),
            typeof(LessionData),
            typeof(LessionDataView),
            new PropertyMetadata(null, OnModelChanged));

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the <see cref="LessionData"/> instance.
        /// </summary>
        public LessionData Lession
        {
            set => SetValue(LessionProperty, value);
            get => (LessionData)GetValue(LessionProperty);
        }

        /// <summary>
        /// Creates a new instance of the <see cref="LessionDataView"/> class
        /// </summary>
        public LessionDataView()
        {
            InitializeComponent();
        }

        //   ---   Private Methods (static)   ---

        /// <summary>
        /// Callback method for when the value of the <see cref="LessionProperty"/> <see cref="DependencyProperty"/> changes.
        /// </summary>
        private static void OnModelChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is not LessionDataView view)
            {
                return;
            }

            if (view.MainGrid.IsLoaded &&
                view.MainGrid.DataContext is LessionDataViewModel vm)
            {
                vm.Model = e.NewValue as LessionData;
            }
        }

        //   ---   Private Methods   ---

        private void MainGrid_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainGrid.DataContext is LessionDataViewModel vm)
            {
                vm.Model = Lession;
                vm.DispatcherQueue = DispatcherQueue;
            }
        }

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            if (MainGrid.DataContext is LessionDataViewModel vm)
            {
                vm.Dispose();
            }
        }
    }
}
