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

namespace MilLeadershipBoard.UI.UserControls
{
    public sealed partial class ExceptionMessageView : UserControl
    {
        //   ---   Public Properties   ---

        /// <inheritdoc cref="ExceptionMessageViewModel.ExceptionTextBlockText"/>.
        public string ExceptionMessageContent
        {
            set => ViewModel.ExceptionTextBlockText = value;
            get => ViewModel.ExceptionTextBlockText;
        }

        /// <summary>
        /// Gets the <see cref="ExceptionMessageViewModel"/> ViewModel instance of this view.
        /// </summary>
        public ExceptionMessageViewModel ViewModel { get; }

        //   ---   Constructors   ---

        /// <summary>
        /// Creats a new instance of the <see cref="ExceptionMessageView"/> class.
        /// </summary>
        public ExceptionMessageView()
        {
            // Initialize the ViewModel
            ViewModel = new ExceptionMessageViewModel();

            InitializeComponent();

            // Link events to the ViewModel
            ExpandExceptionDetailsHyperlinkButton.Tapped += ViewModel.ExpandExceptionDetailsHyperlinkButton_Tapped;
        }
    }
}
