using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MilLeadershipBoard.TroopData.Location;
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

        //   ---   Public Methods   ---

        private static void OnItemChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SoldierLocationView view = (SoldierLocationView)d;
        }
    }
}
