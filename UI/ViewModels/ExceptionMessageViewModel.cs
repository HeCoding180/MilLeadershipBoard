using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using MilLeadershipBoard.Resources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.UI.ViewModels
{
    /// <summary>
    /// ViewModel class for the <see cref="UserControls.ExceptionMessageView"/> view class.
    /// </summary>
    public class ExceptionMessageViewModel : INotifyPropertyChanged
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Field containing the value of the <see cref="ExceptionTextBlockText"/> property.
        /// </summary>
        private string _exceptionTextBlockText = string.Empty;

        /// <summary>
        /// Field containing the value of the <see cref="ExceptionTextBlockVisibility"/> property.
        /// </summary>
        private Visibility _exceptionTextBlockVisibility = Visibility.Collapsed;

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the text that is to be shown in the expandable text section.
        /// </summary>
        public string ExceptionTextBlockText
        {
            set
            {
                if (value == _exceptionTextBlockText)
                {
                    return;
                }

                _exceptionTextBlockText = value;

                OnPropertyChanged();
            }
            get => _exceptionTextBlockText;
        }

        /// <summary>
        /// Gets the <see cref="Visibility"/> of the <see cref="Microsoft.UI.Xaml.Controls.TextBlock"/> that is to be used to display the exception details.
        /// </summary>
        public Visibility ExceptionTextBlockVisibility
        {
            private set
            {
                if (value == _exceptionTextBlockVisibility)
                {
                    return;
                }

                _exceptionTextBlockVisibility = value;

                OnPropertyChanged();
                OnPropertyChanged(nameof(ExpandExceptionDetailsHyperlinkButtonText));
            }
            get => _exceptionTextBlockVisibility;
        }

        /// <summary>
        /// Gets the text that is to be displayed for the expand <see cref="Microsoft.UI.Xaml.Controls.HyperlinkButton"/>.
        /// </summary>
        public string ExpandExceptionDetailsHyperlinkButtonText
        {
            get
            {
                return ExceptionTextBlockVisibility switch
                {
                    Visibility.Collapsed => ResourceManager.GetString("MessageDispatcher/ShowErrorDetailsTextBlock/Expand"),
                    Visibility.Visible => ResourceManager.GetString("MessageDispatcher/ShowErrorDetailsTextBlock/Contract"),
                    _ => throw new InvalidEnumArgumentException("The value" + ExceptionTextBlockVisibility + " is an invalid enum value for the property " +
                                                                nameof(ExceptionTextBlockVisibility) + " which is of type " + typeof(Visibility).FullName)
                };
            }
        }

        //   ---   Public Events   ---

        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="ExceptionMessageViewModel"/> class.
        /// </summary>
        public ExceptionMessageViewModel()
        {

        }

        //   ---   Protected Methods   ---

        /// <summary>
        /// Method used to raise the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Callback Method for the <see cref="UIElement.Tapped"/>
        /// </summary>
        public void ExpandExceptionDetailsHyperlinkButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Toggle the visibility.
            ExceptionTextBlockVisibility = ExceptionTextBlockVisibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
