using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Util
{
    /// <summary>
    /// Class used to manage the <see cref="UIElement.Tapped"/> event and the <see cref="UIElement.DoubleTapped"/> event to create a new event for single clicks.
    /// </summary>
    internal class ClickEventWrapper : IDisposable
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// Gets the <see cref="TappedRoutedEventArgs"/> of the last <see cref="UIElement.Tapped"/> event
        /// or <see langword="null"/> if there's no active <see cref="UIElement.Tapped"/> event that is delayed.
        /// </summary>
        private TappedRoutedEventArgs? _lastTappedEventArgs = null;

        /// <summary>
        /// Field containing the <see cref="DispatcherQueueTimer"/> used to detect single taps.
        /// </summary>
        private readonly DispatcherQueueTimer _singleTappedDelayTimer;

        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the <see cref="UIElement"/> that is wrapped by this <see cref="ClickEventWrapper"/> instance.
        /// </summary>
        public UIElement EncapsulatingElement { get; }

        //   ---   Public Events   ---

        /// <inheritdoc cref="UIElement.Tapped"/>
        public event TappedEventHandler? Tapped;

        /// <inheritdoc cref="UIElement.DoubleTapped"/>
        public event DoubleTappedEventHandler? DoubleTapped;

        /// <summary>
        /// Occurs when a <see cref="UIElement.Tapped"/> event occurs that is not followed by a <see cref="DoubleTapped"/> event.
        /// </summary>
        public event TappedEventHandler? SingleTapped;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="ClickEventWrapper"/> class with a specified <see cref="UIElement"/> instance it wraps.
        /// </summary>
        /// <param name="encapsulatingElement">The <see cref="UIElement"/> instance whose events should be wrapped.</param>
        public ClickEventWrapper(UIElement encapsulatingElement)
        {
            EncapsulatingElement = encapsulatingElement;

            EncapsulatingElement.Tapped += EncapsulatingElement_Tapped;
            EncapsulatingElement.DoubleTapped += EncapsulatingElement_DoubleTapped;

            _singleTappedDelayTimer = EncapsulatingElement.DispatcherQueue.CreateTimer();
            _singleTappedDelayTimer.Interval = TimeSpan.FromMilliseconds(Util.DoubleClickTime);
            _singleTappedDelayTimer.IsRepeating = false;
            _singleTappedDelayTimer.Tick += SingleTappedDelayTimer_Tick;
        }

        //   ---   Private Methods   ---

        private void EncapsulatingElement_Tapped(object sender, TappedRoutedEventArgs e)
        {
            // Raise the Tapped event
            Tapped?.Invoke(this, e);

            lock (_singleTappedDelayTimer)
            {
                if (_singleTappedDelayTimer.IsRunning || _lastTappedEventArgs is not null)
                {
                    _singleTappedDelayTimer.Stop();
                    _lastTappedEventArgs = null;
                    return;
                }

                _lastTappedEventArgs = e;
                _singleTappedDelayTimer.Start();
            }
        }

        private void EncapsulatingElement_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            // Raise the DoubleTapped event
            DoubleTapped?.Invoke(this, e);

            _singleTappedDelayTimer.Stop();

            lock (_singleTappedDelayTimer)
            {
                _lastTappedEventArgs = null;
            }
        }

        /// <summary>
        /// Method used to raise the <see cref="SingleTapped"/> event.
        /// </summary>
        private void OnSingleTapped()
        {
            lock (_singleTappedDelayTimer)
            {
                if (_lastTappedEventArgs is null)
                {
                    return;
                }

                SingleTapped?.Invoke(this, _lastTappedEventArgs);

                _lastTappedEventArgs = null;
            }
        }

        /// <summary>
        /// Callback method used for the <see cref="DispatcherQueueTimer.Tick"/> event of the <see cref="DispatcherQueueTimer"/> instance
        /// created from the <see cref="DispatcherQueue"/> instance of the <see cref="EncapsulatingElement"/>.
        /// </summary>
        private void SingleTappedDelayTimer_Tick(DispatcherQueueTimer sender, object args)
        {
            OnSingleTapped();
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to ensure that the contained classes are correctly disposed of and that they are 
        /// </summary>
        public void Dispose()
        {
            _singleTappedDelayTimer.Stop();
        }
    }
}
