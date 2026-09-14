using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using MilLeadershipBoard.Models;
using MilLeadershipBoard.UI.UserControls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.UI.Windows
{
    /// <summary>
    /// Interface for windows that provide a user interface for showing messages provided by the <see cref="Messaging.MessageDispatcher"/>.
    /// </summary>
    public interface IMessageHostWindow
    {
        //   ---   Public Properties (abstract)   ---

        /// <summary>
        /// Sets or gets the <see cref="Visibility"/> of the <see cref="UIElement"/> that contains the messaging UI.
        /// </summary>
        public Visibility MessagePaneVisibility { set; get; }

        /// <summary>
        /// Gets the <see cref="StackPanel"/> that contains the <see cref="InfoBar"/> instances used to display the messages.
        /// </summary>
        public StackPanel MessageStackPanel { get; }
    }
}
