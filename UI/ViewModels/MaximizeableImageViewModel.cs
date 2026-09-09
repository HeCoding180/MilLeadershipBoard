using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml;
using MilLeadershipBoard.UI.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;

namespace MilLeadershipBoard.UI.ViewModels
{
    /// <summary>
    /// ViewModel class for the <see cref="UserControls.MaximizeableImageView"/> view class.
    /// </summary>
    class MaximizeableImageViewModel : INotifyPropertyChanged, IDisposable
    {
        //   ---   Private Fields (static)   ---

        /// <summary>
        /// Field containing a <see cref="Point"/> with the x and y value of zero.
        /// </summary>
        private static readonly Point ZeroPoint = new Point(0, 0);

        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the unmasked <see cref="Visibility"/> state of the close button.
        /// </summary>

        private Visibility _closeButtonVisible = Visibility.Collapsed;

        /// <summary>
        /// Field containing the <see cref="RelayCommand"/> instance used by the <see cref="CloseCommand"/> property.
        /// </summary>
        private RelayCommand _closeCommand;

        /// <summary>
        /// Field containing the value of the <see cref="IsMaximizedViewOpen"/> property.
        /// </summary>
        private bool _isMaximizedViewOpen = false;

        //   ---   Private Properties (static)   ---

        /// <summary>
        /// Gets the <see cref="UIElement"/> that is used as the reference for dimensions.
        /// </summary>
        private static UIElement RootUiElement => ((App)App.Current).MainWindowInstance!.MainContentElement;

        //   ---   Private Properties   ---

        /// <summary>
        /// Sets or gets the <see cref="Point"/> containing the origin of the <see cref="RootUiElement"/>.
        /// </summary>
        private Point Origin { set; get; }

        /// <summary>
        /// Gets the instance of the view this ViewModel is assigned to.
        /// </summary>
        private MaximizeableImageView View { get; }

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="Visibility"/> of the close button.
        /// </summary>
        public Visibility CloseButtonVisibility
        {
            get
            {
                if (_closeButtonVisible == Visibility.Visible &&
                    View.CloseButtonVisibility == Visibility.Visible)
                {
                    return Visibility.Visible;
                }

                return Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Gets the <see cref="ICommand"/> instance that can be used to close the maximized view of the image.
        /// </summary>
        public ICommand CloseCommand => _closeCommand;

        /// <summary>
        /// Gets the horizontal offset of the popup used for the maximized image view.
        /// </summary>
        public double HorizontalPopupOffset => -Origin.X;

        /// <summary>
        /// Gets of the maximized view should be open.
        /// </summary>
        public bool IsMaximizedViewOpen
        {
            private set
            {
                if (value == _isMaximizedViewOpen)
                {
                    return;
                }

                _isMaximizedViewOpen = value;

                OnPropertyChanged();
            }
            get => _isMaximizedViewOpen;
        }

        /// <summary>
        /// Gets the width of the popup used for the maximized image view.
        /// </summary>
        public double PopupHeight => RootUiElement.ActualSize.Y;

        /// <summary>
        /// Gets the width of the popup used for the maximized image view.
        /// </summary>
        public double PopupWidth => RootUiElement.ActualSize.X;

        /// <summary>
        /// Gets the vertical offset of the popup used for the maximized image view.
        /// </summary>
        public double VerticalPopupOffset => -Origin.Y;

        //   ---   Public Events   ---

        /// <summary>
        /// Raised when the maximized view is opened.
        /// </summary>
        public event Action? MaximizedViewOpened;

        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="MaximizeableImageViewModel"/> class.
        /// </summary>
        /// <param name="view">The view instance this ViewModel is assigned to.</param>
        public MaximizeableImageViewModel(MaximizeableImageView view)
        {
            View = view;

            // Assign a dummy origin
            Origin = ZeroPoint;

            // Create the close command
            _closeCommand = new RelayCommand(CloseMaximizedView);

            if (View.IsLoaded)
            {
                OnViewLoaded();
            }
            else
            {
                View.Loaded += (s, e) => OnViewLoaded();
            }
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Method that is called when the view is loaded.
        /// </summary>
        private void OnViewLoaded()
        {
            // Refresh the visual properties
            RefreshVisualProperties();

            // Subscribe to the PropertyChanged event of the root UI element
            if (RootUiElement is INotifyPropertyChanged pc)
            {
                pc.PropertyChanged += RootUiElement_PropertyChanged;
            }
        }

        /// <summary>
        /// Callback method for the <see cref="INotifyPropertyChanged.PropertyChanged"/> event of the <see cref="RootUiElement"/>.
        /// </summary>
        private void RootUiElement_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(UIElement.ActualOffset) ||
                e.PropertyName == nameof(UIElement.ActualSize))
            {
                RefreshVisualProperties();
            }
        }

        //   ---   Protected Methods   ---

        /// <summary>
        /// Method used to raise the <see cref="MaximizedViewOpened"/> event.
        /// </summary>
        protected void OnMaximizedViewOpened()
        {
            MaximizedViewOpened?.Invoke();
        }

        /// <summary>
        /// Method used to raise the <see cref="PropertyChanged"/> event with automatic property name assignment.
        /// </summary>
        /// <param name="propertyName">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to close the maximized view of the image.
        /// </summary>
        public void CloseMaximizedView()
        {
            IsMaximizedViewOpen = false;
        }

        public void Dispose()
        {
            if (RootUiElement is INotifyPropertyChanged pc)
            {
                pc.PropertyChanged -= RootUiElement_PropertyChanged;
            }
        }

        public void OnMaximizedImageViewPointerEntered()
        {
            _closeButtonVisible = Visibility.Visible;
        }

        public void OnMaximizedImageViewPointerExited()
        {
            _closeButtonVisible = Visibility.Collapsed;
        }

        /// <summary>
        /// Method used to open the maximized view of the image.
        /// </summary>
        public void OpenMaximizedView()
        {
            IsMaximizedViewOpen = true;
        }

        /// <summary>
        /// Method used to refresh the visual properties and notify that the visual properties have changed.
        /// </summary>
        public void RefreshVisualProperties()
        {
            Origin = View.TransformToVisual(RootUiElement).TransformPoint(ZeroPoint);

            OnPropertyChanged(nameof(HorizontalPopupOffset));
            OnPropertyChanged(nameof(VerticalPopupOffset));
            OnPropertyChanged(nameof(PopupHeight));
            OnPropertyChanged(nameof(PopupWidth));
        }
    }
}
