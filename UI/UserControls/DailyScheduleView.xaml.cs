using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using MilLeadershipBoard.Models;
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
    public sealed partial class DailyScheduleView : UserControl
    {
        //   ---   Public Fields (static)   ---

        /// <summary>
        /// Field containing the <see cref="DependencyProperty"/> for the <see cref="Model"/> property.
        /// </summary>
        public static readonly DependencyProperty ModelProperty = DependencyProperty.Register(nameof(Model),
                                                                                              typeof(DailySchedule),
                                                                                              typeof(DailyScheduleView),
                                                                                              new PropertyMetadata(null));

        //   ---   Public Properties   ---

        /// <summary>
        /// Sets or gets the model instance for this <see cref="DailyScheduleView"/> instance.
        /// </summary>
        public DailySchedule? Model
        {
            set => SetValue(ModelProperty, value);
            get => (DailySchedule?)GetValue(ModelProperty);
        }

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="DailyScheduleView"/> class.
        /// </summary>
        public DailyScheduleView()
        {
            InitializeComponent();
        }
    }
}
