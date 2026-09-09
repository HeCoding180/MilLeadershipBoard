using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MilLeadershipBoard.UI.ViewModels;
using MilLeadershipBoard.Util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MilLeadershipBoard.UI.UserControls
{
    /// <summary>
    /// Image control that allows displaying the set image in a maximized view.
    /// </summary>
    public sealed partial class MaximizeableImageView : UserControl
    {
        //   ---   Public Fields (static)   ---

        /// <summary>
        /// Field containing the <see cref="DependencyProperty"/> for the <see cref="CloseButtonVisibility"/> property.
        /// </summary>
        public static readonly DependencyProperty CloseButtonVisibilityProperty = DependencyProperty.Register(nameof(CloseButtonVisibility),
                                                                                               typeof(Visibility),
                                                                                               typeof(MaximizeableImageView),
                                                                                               new PropertyMetadata(Visibility.Visible));

        /// <summary>
        /// Field containing the <see cref="DependencyProperty"/> for the <see cref="Source"/> property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source),
                                                                                               typeof(ImageSource),
                                                                                               typeof(MaximizeableImageView),
                                                                                               new PropertyMetadata(null));

        /// <summary>
        /// Field containing the <see cref="DependencyProperty"/> for the <see cref="Stretch"/> property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch),
                                                                                                typeof(Stretch),
                                                                                                typeof(MaximizeableImageView),
                                                                                                new PropertyMetadata(Stretch.Uniform));

        //   ---   Private Properties   ---

        /// <summary>
        /// Field containing the ViewModel of this View.
        /// </summary>
        private readonly MaximizeableImageViewModel ViewModel;

        /// <summary>
        /// Field containing the <see cref="ClickEventWrapper"/> instance used to detect single clicks.
        /// </summary>
        private readonly ClickEventWrapper BaseImageControlClickEventWrapper;

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the <see cref="Visibility"/> of the close button in the maximized view of the image.
        /// </summary>
        public Visibility CloseButtonVisibility
        {
            set => SetValue(CloseButtonVisibilityProperty, value);
            get => (Visibility)GetValue(CloseButtonVisibilityProperty);
        }

        /// <inheritdoc cref="Image.Source"/>
        public ImageSource Source
        {
            set => SetValue(SourceProperty, value);
            get => (ImageSource)GetValue(SourceProperty);
        }

        /// <inheritdoc cref="Image.Stretch"/>
        public Stretch Stretch
        {
            set => SetValue(StretchProperty, value);
            get => (Stretch)GetValue(StretchProperty);
        }

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="MaximizeableImageView"/> class.
        /// </summary>
        public MaximizeableImageView()
        {
            // Create the date context
            ViewModel = new MaximizeableImageViewModel(this);

            ViewModel.MaximizedViewOpened += OnMaximizedViewOpened;

            InitializeComponent();

            // Initialize the ClickEventWrapper
            BaseImageControlClickEventWrapper = new ClickEventWrapper(BaseImageControl);
            BaseImageControlClickEventWrapper.SingleTapped += BaseImageControlClickEventWrapper_SingleTapped;

            // Listen for changes to the attached ToolTipService.ToolTip property on THIS control
            RegisterPropertyChangedCallback(ToolTipService.ToolTipProperty, OnToolTipChanged);
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Callback method for the <see cref="ClickEventWrapper.SingleTapped"/> event of the <see cref="BaseImageControl"/>.
        /// </summary>
        private void BaseImageControlClickEventWrapper_SingleTapped(object sender, TappedRoutedEventArgs e)
        {
            ViewModel.OpenMaximizedView();
        }

        /// <summary>
        /// Callback method for the <see cref="UIElement.PointerEntered"/> event.
        /// </summary>
        private void Border_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.OnMaximizedImageViewPointerEntered();
            CloseButtonBorder.Visibility = CloseButtonVisibility;
        }

        /// <summary>
        /// Callback method for the <see cref="UIElement.PointerExited"/> event.
        /// </summary>
        private void Border_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            ViewModel.OnMaximizedImageViewPointerExited();
            CloseButtonBorder.Visibility = Visibility.Collapsed;
        }

        /// <summary>
        /// Callback method for the <see cref="MaximizeableImageViewModel.MaximizedViewOpened"/> event of the ViewModel.
        /// </summary>
        private void OnMaximizedViewOpened()
        {
            // Focus the popup image to allow keyboard input capture
            PopupImageControl.Focus(FocusState.Programmatic);
        }

        private void OnToolTipChanged(DependencyObject sender, DependencyProperty dp)
        {
            var tooltip = ToolTipService.GetToolTip(this);

            // Move it off the UserControl and onto the inner Image
            ToolTipService.SetToolTip(this, null);
            ToolTipService.SetToolTip(BaseImageControl, tooltip);
        }

        private void PopupBorder_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Close the popup (border was tapped -> light dismiss behavior)
            ViewModel.CloseMaximizedView();
        }

        private void PopupImageControl_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Escape)
            {
                ViewModel.CloseMaximizedView();
            }
        }

        /// <summary>
        /// Callback method for the <see cref="FrameworkElement.Loaded"/> event.
        /// </summary>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            ViewModel.RefreshVisualProperties();
        }

        /// <summary>
        /// Callback method for the <see cref="FrameworkElement.Unloaded"/> event.
        /// </summary>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.Dispose();
        }

        /// <summary>
        /// Callback method for the <see cref="FrameworkElement.SizeChanged"/> event.
        /// </summary>
        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewModel.RefreshVisualProperties();
        }
    }
}
