using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
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
    /// <summary>
    /// Image control that allows displaying the set image in a maximized view.
    /// </summary>
    public sealed partial class MaximizeableImageView : UserControl
    {
        //   ---   Public Fields (static)   ---

        /// <summary>
        /// Field containing the <see cref="DependencyProperty"/> for the <see cref="Stretch"/> property.
        /// </summary>
        public static readonly DependencyProperty StretchProperty = DependencyProperty.Register(nameof(Stretch),
                                                                                                typeof(Stretch),
                                                                                                typeof(MaximizeableImageView),
                                                                                                new PropertyMetadata(Stretch.Uniform));

        /// <summary>
        /// Field containing the <see cref="DependencyProperty"/> for the <see cref="Source"/> property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.Register(nameof(Source),
                                                                                               typeof(ImageSource),
                                                                                               typeof(MaximizeableImageView),
                                                                                               new PropertyMetadata(null));

        //   ---   Public Properties   ---

        /// <inheritdoc cref="Image.Stretch"/>
        public Stretch Stretch
        {
            set => SetValue(StretchProperty, value);
            get => (Stretch)GetValue(StretchProperty);
        }

        /// <inheritdoc cref="Image.Source"/>
        public ImageSource Source
        {
            set => SetValue(SourceProperty, value);
            get => (ImageSource)GetValue(SourceProperty);
        }

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="MaximizeableImageView"/> class.
        /// </summary>
        public MaximizeableImageView()
        {
            InitializeComponent();

            // Listen for changes to the attached ToolTipService.ToolTip property on THIS control
            RegisterPropertyChangedCallback(ToolTipService.ToolTipProperty, OnToolTipChanged);
        }

        //   ---   Private Methods   ---

        private void OnToolTipChanged(DependencyObject sender, DependencyProperty dp)
        {
            var tooltip = ToolTipService.GetToolTip(this);

            // Move it off the UserControl and onto the inner Image
            ToolTipService.SetToolTip(this, null);
            ToolTipService.SetToolTip(BaseImageControl, tooltip);
        }

        /// <summary>
        /// Callback method for the <see cref="UIElement.Tapped"/> event of the <see cref="BaseImageControl"/>.
        /// </summary>
        private void BaseImageControl_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Where does this page sit relative to the window's root?
            var transform = this.TransformToVisual(((App)App.Current).MainWindowInstance!.MainContentElement);
            var origin = transform.TransformPoint(new Point(0, 0));

            MaximizedViewPopup.HorizontalOffset = -origin.X;
            MaximizedViewPopup.VerticalOffset = -origin.Y;

            PopupBorder.Width = XamlRoot.Size.Width;
            PopupBorder.Height = XamlRoot.Size.Height;

            MaximizedViewPopup.IsOpen = true;
        }
    }
}
