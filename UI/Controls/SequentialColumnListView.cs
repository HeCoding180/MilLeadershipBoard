using System;
using System.Collections;
using System.Linq;
using Windows.Foundation;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Windows.ApplicationModel.DataTransfer;

namespace MilLeadershipBoard.UI.Controls
{
    /// <summary>
    /// A <see cref="ListView"/> that lays its items out using <see cref="SequentialColumnPanel"/>
    /// (sequential top-to-bottom column filling, full-width items) and supports drag-to-reorder
    /// via manual handling, since WinUI 3's built-in <c>CanReorderItems</c> reordering does not
    /// function with a custom <c>ItemsPanel</c> (see
    /// <see href="https://github.com/microsoft/microsoft-ui-xaml/issues/9275"/>).
    /// </summary>
    /// <remarks>
    /// <para>
    /// Reordering is implemented by moving the dragged item within the source collection live,
    /// during <see cref="OnDragOver"/> — not only on <see cref="OnDrop"/> — so the layout opens
    /// a gap at the drop target as the user drags, matching the visual behavior of native reorder.
    /// </para>
    /// <para>
    /// Requires <see cref="ItemsControl.ItemsSource"/> to be (or implement) <see cref="IList"/>
    /// so items can be located and moved by index. <see cref="System.Collections.ObjectModel.ObservableCollection{T}"/>
    /// satisfies this out of the box; a custom collection type must implement <see cref="IList"/>
    /// (or you must adapt <see cref="IndexOfItem"/>/<see cref="MoveItem"/> to its API).
    /// </para>
    /// <para>
    /// <see cref="OnDragEnter"/>, <see cref="OnDragOver"/>, and <see cref="OnDrop"/> are overridden
    /// directly, since <see cref="UIElement"/> exposes them as protected virtual methods. The
    /// <see cref="ListViewBase.DragItemsStarting"/> and <see cref="ListViewBase.DragItemsCompleted"/>
    /// events have no protected virtual equivalent in the public API, so those two are still
    /// subscribed to internally in the constructor — this is the closest to override-only behavior
    /// achievable while still capturing which item is being dragged.
    /// </para>
    /// </remarks>
    public class SequentialColumnListView : ListView
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// The item currently being dragged, or <see langword="null"/> when no drag is in progress.
        /// </summary>
        private object? _draggedItem;

        //   ---   Constructors   ---

        /// <summary>
        /// Initializes a new <see cref="SequentialColumnListView"/>, configuring the item panel
        /// and the drag-item-tracking events that have no overridable equivalent.
        /// </summary>
        public SequentialColumnListView()
        {
            DefaultStyleKey = typeof(ListView);

            CanDragItems = true;
            // Native reorder is disabled deliberately: with a custom ItemsPanel it does not
            // work correctly (shows a "not allowed" cursor) and would otherwise conflict with
            // the manual handling below.
            CanReorderItems = false;
            AllowDrop = true;

            // No protected virtual "OnDragItemsStarting"/"OnDragItemsCompleted" exists on
            // ListViewBase, so these two remain as event subscriptions.
            DragItemsStarting += OnDragItemsStarting;
            DragItemsCompleted += OnDragItemsCompleted;
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Locates the index of <paramref name="item"/> within <see cref="ItemsControl.ItemsSource"/>.
        /// </summary>
        private int IndexOfItem(object item)
        {
            if (ItemsSource is IList list)
                return list.IndexOf(item);
            return Items.IndexOf(item);
        }

        /// <summary>
        /// Moves the item at <paramref name="oldIndex"/> to <paramref name="newIndex"/> within
        /// <see cref="ItemsControl.ItemsSource"/>.
        /// </summary>
        /// <remarks>
        /// Uses remove-then-insert via <see cref="IList"/>. If your source collection exposes a
        /// dedicated <c>Move(oldIndex, newIndex)</c> method, prefer calling that instead — it
        /// raises a single collection-changed notification instead of two, which can avoid a
        /// visible flicker.
        /// </remarks>
        private void MoveItem(int oldIndex, int newIndex)
        {
            if (ItemsSource is not IList list)
                return;

            var item = list[oldIndex];
            list.RemoveAt(oldIndex);
            list.Insert(newIndex, item);
        }

        /// <summary>
        /// Captures the data item being dragged when a drag gesture starts.
        /// </summary>
        private void OnDragItemsStarting(object sender, DragItemsStartingEventArgs e)
        {
            // Single-item drag assumed; extend to handle e.Items as a batch for multi-select drag.
            _draggedItem = e.Items.FirstOrDefault();
        }

        /// <summary>
        /// Fallback cleanup in case a drag ends without <see cref="OnDrop"/> firing (e.g. cancelled).
        /// </summary>
        private void OnDragItemsCompleted(ListViewBase sender, DragItemsCompletedEventArgs args)
        {
            _draggedItem = null;
        }

        //   ---   Protected Methods (overrides)   ---

        /// <inheritdoc/>
        /// <remarks>
        /// Resets drag bookkeeping state as the pointer enters the list's drop-target area.
        /// </remarks>
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);
            e.AcceptedOperation = DataPackageOperation.Move;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// As the drag moves over the list, computes the insertion point under the pointer via
        /// <see cref="SequentialColumnPanel.GetInsertionIndexes"/> and, if it differs from the
        /// dragged item's current position, moves the item there immediately — causing the panel
        /// to re-lay-out and open a gap at the target position while dragging continues.
        /// </remarks>
        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.IsGlyphVisible = false;

            if (_draggedItem is null)
                return;

            if (ItemsPanelRoot is not SequentialColumnPanel panel)
                return;

            Point position = e.GetPosition(panel);
            panel.GetInsertionIndexes(position, out int first, out int second);

            int currentIndex = IndexOfItem(_draggedItem);
            if (currentIndex < 0)
                return;

            int targetIndex = second != -1
                ? second
                : (first != -1 ? first + 1 : Items.Count);

            // Removing the item before its target shifts everything after it left by one.
            if (targetIndex > currentIndex)
                targetIndex--;

            if (targetIndex != currentIndex)
                MoveItem(currentIndex, targetIndex);
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Finalizes the drop. The move already happened live during <see cref="OnDragOver"/>,
        /// so this only needs to clear drag state.
        /// </remarks>
        protected override void OnDrop(DragEventArgs e)
        {
            base.OnDrop(e);
            _draggedItem = null;
        }
    }
}
