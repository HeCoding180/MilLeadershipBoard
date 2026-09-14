using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using MilLeadershipBoard.Resources;
using MilLeadershipBoard.UI.UserControls;
using MilLeadershipBoard.UI.Windows;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Messaging
{
    /// <summary>
    /// Class used for dispatching messages that are to be displayed on the root window.
    /// </summary>
    internal static class MessageDispatcher
    {
        //   ---   Private Properties   ---

        /// <summary>
        /// Sets or gets the Host used to set the messaging
        /// </summary>
        private static IMessageHostWindow? Host { set; get; }

        /// <summary>
        /// Gets the <see cref="Host"/> as a <see cref="Window"/>.
        /// </summary>
        private static Window HostWindow
        {
            get
            {
                EnsureMessagingHostSet();
                return (Window)Host;
            }
        }

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets whether a message host window is set.
        /// </summary>
        public static bool IsMessagingHostSet => Host is not null;

        //   ---   Private Methods   ---

        /// <summary>
        /// Method used to ensure that the <see cref="Host"/> is set to an object instance.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="Host"/> is null.</exception>
        [MemberNotNull(nameof(Host))]
        private static void EnsureMessagingHostSet()
        {
            if (!IsMessagingHostSet)
            {
                throw new InvalidOperationException("Cannot show message, no message host is set.");
            }
        }

        private static void MessageBar_Closed(InfoBar sender, InfoBarClosedEventArgs args)
        {
            EnsureMessagingHostSet();

            // Remove the InfoBar
            Host.MessageStackPanel.Children.Remove(sender);
            sender.Closed -= MessageBar_Closed;
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to set the <see cref="IMessageHostWindow"/> that is used to display the messages dispatched from this class.
        /// </summary>
        /// <param name="host">The <see cref="IMessageHostWindow"/> instance where messages are to be displayed.</param>
        /// <exception cref="ArgumentException">Thrown if the passed <paramref name="host"/> is not a <see cref="UIElement"/> instance.</exception>
        public static void SetMessageHost(IMessageHostWindow host)
        {
            if (host is not Window)
            {
                throw new ArgumentException("The passed host is not a Window instance.", nameof(host));
            }

            Host = host;
        }

        /// <summary>
        /// Method used to dispatch 
        /// </summary>
        /// <param name="messageBar">The <see cref="InfoBar"/> instance containing the message.</param>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="Host"/> is null.</exception>
        public static void ShowMessage(InfoBar messageBar)
        {
            EnsureMessagingHostSet();

            messageBar.IsOpen = true;
            messageBar.Visibility = Visibility.Visible;
            messageBar.Closed += MessageBar_Closed;

            HostWindow.DispatcherQueue.TryEnqueue(() =>
            {
                Host.MessageStackPanel.Children.Add(messageBar);
                Host.MessagePaneVisibility = Visibility.Visible;
            });
        }

        /// <summary>
        /// Method used to dispatch a message with a simple content and title.
        /// </summary>
        /// <param name="title">The title of the <see cref="InfoBar"/>.</param>
        /// <param name="message">The content text of the <see cref="InfoBar"/>.</param>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="Host"/> is null.</exception>
        public static void ShowMessage(string title, string message, InfoBarSeverity severity)
        {
            InfoBar messageBar = new InfoBar()
            {
                Title = title,
                Content = message,
                Severity = severity
            };

            ShowMessage(messageBar);
        }

        /// <summary>
        /// Method used to dispatch a message for an exception.
        /// </summary>
        /// <param name="title">A text summarizing the excepton or what failed. This text is added to the title of the <see cref="InfoBar"/>.</param>
        /// <param name="exception">The exception containing details about what failed.</param>
        /// <param name="severity">The <see cref="InfoBarSeverity"/> of the message that defines the look of the <see cref="InfoBar"/> that is used to display the message.</param>
        /// <exception cref="InvalidOperationException">Thrown if the <see cref="Host"/> is null.</exception>
        public static void ShowExceptionMessage(string title, Exception exception, InfoBarSeverity severity = InfoBarSeverity.Error)
        {
            EnsureMessagingHostSet();

            HostWindow.DispatcherQueue.TryEnqueue(() =>
            {
                ExceptionMessageView messageView = new ExceptionMessageView();
                messageView.ExceptionMessageContent = exception.ToString();
                messageView.Margin = new Thickness(0.0, 0.0, 0.0, 16.0);

                InfoBar messageBar = new InfoBar()
                {
                    Title = title,
                    Content = messageView,
                    Severity = severity
                };

                ShowMessage(messageBar);
            });
        }
    }
}
