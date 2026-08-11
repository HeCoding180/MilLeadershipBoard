using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MilLeadershipBoard.UI.Pages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Networking.Connectivity;
using Windows.UI.ApplicationSettings;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MilLeadershipBoard.UI.Windows
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        //   ---   Constructors   ---

        public MainWindow()
        {
            InitializeComponent();

            this.ExtendsContentIntoTitleBar = true;
            this.SetTitleBar(MainWindowTitleBar);

            ContentFrame.Navigate(typeof(DashboardPage));
        }

        //   ---   Private Methods   ---

        private void MainWindowTitleBar_PaneToggleRequested(TitleBar sender, object args)
        {
            // Toggle the MainNavigationView visibility
            MainNavigationView.Visibility = MainNavigationView.Visibility switch
            {
                Visibility.Collapsed => Visibility.Visible,
                _ => Visibility.Collapsed,
            };
        }

        private void MainNavigationView_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
        {
            if (args.IsSettingsInvoked)
            {
                
            }
            else if (args.InvokedItemContainer is NavigationViewItem item)
            {
                switch (item.Tag.ToString())
                {
                    case "Dashboard":
                        ContentFrame.Navigate(typeof(DashboardPage));
                        break;
                    case "Soldiers":
                        ContentFrame.Navigate(typeof(SoldiersPage));
                        break;
                }
            }

            MainNavigationView.Visibility = Visibility.Collapsed;
        }
    }
}
