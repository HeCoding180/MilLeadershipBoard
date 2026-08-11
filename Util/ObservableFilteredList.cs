using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Util
{
    /// <summary>
    /// A read-only, live-filtered view over an <see cref="ObservableCollection{T}"/>.
    /// Automatically stays in sync with the encapsulated collection and forwards
    /// (index-adjusted) <see cref="CollectionChanged"/> events for items that match
    /// the supplied filter predicate.
    /// Since there is no absolute index of its own (items are a subset/projection
    /// of the source collection), this class does not support write operations
    /// such as Add, Remove, or item re-ordering directly.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class ObservableFilteredList<T> : IReadOnlyList<T>, INotifyCollectionChanged, INotifyPropertyChanged, IDisposable
    {
        //   ---   Private Fields   ---

        /// <summary>
        /// The encapsulated collection this instance is a filtered, read-only
        /// view of. Never modified directly by this class.
        /// </summary>
        private readonly ObservableCollection<T> _sourceCollection;

        /// <summary>
        /// The predicate used to decide whether an item from the source
        /// collection is included in the filtered view.
        /// </summary>
        private readonly Func<T, bool> _filterFunc;

        /// <summary>
        /// The current, materialized snapshot of items from
        /// <see cref="_sourceCollection"/> that pass <see cref="_filterFunc"/>,
        /// kept in the same relative order as the source collection. This is
        /// the backing store for the public indexer/enumeration.
        /// </summary>
        private readonly List<T> _filteredItems;

        /// <summary>
        /// Set to <see langword="true"/> once <see cref="Dispose"/> has run, to guard
        /// against double-unsubscription from the source collection's events.
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// The name of the property to watch for changes on each filtered
        /// item, as passed to the constructor. When a matching
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> notification is
        /// received for a filtered item, that item is automatically
        /// re-evaluated via <see cref="ReevaluateItem"/>. <c>null</c> or
        /// whitespace disables this behavior entirely.
        /// </summary>
        private readonly string? _observingPropertyName;

        /// <summary>
        /// Resolved once in the constructor: <c>true</c> only if
        /// <see cref="_observingPropertyName"/> was supplied AND <typeparamref name="T"/>
        /// implements <see cref="INotifyPropertyChanged"/>. Guards all
        /// subscribe/unsubscribe/callback logic so items are only ever wired up
        /// for property-change observation when it's actually meaningful.
        /// </summary>
        private readonly bool _isObservingItemPropertyChanges;

        //   ---   Public Properties   ---

        /// <summary>
        /// Number of items in the filtered view.
        /// </summary>
        public int Count => _filteredItems.Count;

        /// <summary>
        /// Gets the item at the given index within the filtered view.
        /// Note: this index has no relation to the index of the item in the
        /// encapsulated source collection.
        /// </summary>
        public T this[int index] => _filteredItems[index];

        //   ---   Public Events   ---

        /// <summary>
        /// Raised whenever the filtered view changes as a result of a change in
        /// the source collection (an item entering or leaving the filtered set,
        /// a visible item being replaced, or a visible item moving). Indices in
        /// the raised <see cref="NotifyCollectionChangedEventArgs"/> refer to
        /// positions within this filtered view, not the source collection.
        /// </summary>
        public event NotifyCollectionChangedEventHandler? CollectionChanged;

        /// <summary>
        /// Raised after <see cref="CollectionChanged"/> to notify bindings that
        /// <see cref="Count"/> and/or the indexer ("Item[]") have changed, matching
        /// the behavior of <see cref="ObservableCollection{T}"/>.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="ObservableFilteredList"/> class.
        /// </summary>
        /// <param name="encapsulatingCollection">The source collection to filter.</param>
        /// <param name="filterFunc">Predicate deciding which items are included.</param>
        /// <param name="observingPropertyName">
        /// Optional. When neither <c>null</c> nor whitespace, and <typeparamref name="T"/>
        /// implements <see cref="INotifyPropertyChanged"/>, every item currently
        /// (and subsequently) in the filtered view is subscribed to, and a
        /// <see cref="INotifyPropertyChanged.PropertyChanged"/> notification with
        /// this property name triggers an automatic <see cref="ReevaluateItem"/>
        /// call for that item. Leave as <c>null</c> to disable this behavior.
        /// </param>
        public ObservableFilteredList(ObservableCollection<T> encapsulatingCollection, Func<T, bool> filterFunc, string? observingPropertyName = null)
        {
            _sourceCollection = encapsulatingCollection ?? throw new ArgumentNullException(nameof(encapsulatingCollection));
            _filterFunc = filterFunc ?? throw new ArgumentNullException(nameof(filterFunc));
            _observingPropertyName = observingPropertyName;

            _isObservingItemPropertyChanges =
                !string.IsNullOrWhiteSpace(observingPropertyName) &&
                typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T));

            // Build the initial snapshot of the filtered view, subscribing to
            // each included item's PropertyChanged event as we go (if enabled).
            _filteredItems = new List<T>();

            foreach (T item in _sourceCollection.Where(_filterFunc))
            {
                _filteredItems.Add(item);
                SubscribeToItem(item);
            }

            _sourceCollection.CollectionChanged += OnSourceCollectionChanged;
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>An enumerator that can be used to iterate through the collection.</returns>
        public IEnumerator<T> GetEnumerator() => _filteredItems.GetEnumerator();

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="IEnumerator"/> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        /// <summary>
        /// Re-evaluates <paramref name="item"/> against the filter predicate and
        /// updates the filtered view accordingly. Use this when a property of an
        /// item that the filter depends on has changed in place (e.g. via
        /// INotifyPropertyChanged), since such in-place changes don't raise
        /// <see cref="ObservableCollection{T}.CollectionChanged"/> on the source
        /// collection and would otherwise leave the filtered view stale.
        ///
        /// The given item does not need to currently be part of the
        /// encapsulated source collection:
        /// - If it's currently included in the filtered view but no longer
        ///   passes the filter, it is removed and a forwarded Remove event is raised.
        /// - If it's not currently included, now passes the filter, and is found
        ///   in the source collection, it is inserted at the correct position and
        ///   a forwarded Add event is raised.
        /// - If it's not currently included, now passes the filter, but is NOT
        ///   found in the source collection, it is ignored (there is no valid
        ///   position to place it at, since this view only ever mirrors items
        ///   that exist in the source collection).
        /// - Otherwise (state already consistent), nothing happens and no event
        ///   is raised.
        ///
        /// This method itself does not subscribe/unsubscribe item property-change
        /// handlers directly; it delegates to <see cref="InsertFilteredItem"/> /
        /// <see cref="RemoveFilteredItemAt"/>, which keep that bookkeeping
        /// consistent for every code path that mutates <see cref="_filteredItems"/>.
        /// </summary>
        /// <param name="item">The item to re-evaluate.</param>
        public void ReevaluateItem(T item)
        {
            bool matchesNow = _filterFunc(item);
            int filteredIndex = _filteredItems.IndexOf(item);
            bool isCurrentlyIncluded = filteredIndex >= 0;

            if (isCurrentlyIncluded && !matchesNow)
            {
                // Item no longer passes the filter: drop it from the view.
                RemoveFilteredItemAt(filteredIndex);
                RaiseRemove(item, filteredIndex);
            }
            else if (!isCurrentlyIncluded && matchesNow)
            {
                int sourceIndex = _sourceCollection.IndexOf(item);

                if (sourceIndex < 0)
                    return; // Not part of the source collection; nothing to insert.

                int insertIndex = GetFilteredIndexForSourceIndex(sourceIndex);
                InsertFilteredItem(insertIndex, item);
                RaiseAdd(item, insertIndex);
            }
            // else: already in the correct state, nothing to do.
        }

        /// <summary>
        /// Unsubscribes from the source collection's events, and from every
        /// currently-included item's <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// event (if item property-change observation is enabled). Call this
        /// when the ObservableFilteredList instance is no longer needed, to
        /// avoid leaking the subscriptions (and thus this instance) via the
        /// source collection and its items.
        /// </summary>
        public void Dispose()
        {
            if (_isDisposed)
                return;

            _sourceCollection.CollectionChanged -= OnSourceCollectionChanged;

            if (_isObservingItemPropertyChanges)
            {
                foreach (T item in _filteredItems)
                    UnsubscribeFromItem(item);
            }

            _isDisposed = true;
        }

        //   ---   Private Methods   ---

        /// <summary>
        /// Entry point for all changes on <see cref="_sourceCollection"/>.
        /// Dispatches to the matching handler based on the action type so the
        /// filtered snapshot and forwarded events stay in sync with the source.
        /// </summary>
        private void OnSourceCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    HandleAdd(e);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    HandleRemove(e);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    HandleReplace(e);
                    break;

                case NotifyCollectionChangedAction.Move:
                    HandleMove(e);
                    break;

                case NotifyCollectionChangedAction.Reset:
                    HandleReset();
                    break;
            }
        }

        /// <summary>
        /// Handles items added to the source collection: any added item that
        /// passes the filter is inserted into <see cref="_filteredItems"/> at
        /// its correct relative position (subscribing to it, if enabled), and a
        /// forwarded Add event is raised. Items that fail the filter are
        /// skipped without raising any event.
        /// </summary>
        private void HandleAdd(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;

            // At this point the item(s) already exist in _sourceCollection, so we
            // can use the current source contents to compute the correct filtered
            // insertion index.
            int sourceIndex = e.NewStartingIndex;

            for (int i = 0; i < e.NewItems.Count; i++)
            {
                T item = (T)e.NewItems[i]!;

                if (!_filterFunc(item))
                {
                    sourceIndex++;
                    continue;
                }

                int filteredIndex = GetFilteredIndexForSourceIndex(sourceIndex);
                InsertFilteredItem(filteredIndex, item);

                RaiseAdd(item, filteredIndex);

                sourceIndex++;
            }
        }

        /// <summary>
        /// Handles items removed from the source collection: any removed item
        /// that is currently present in <see cref="_filteredItems"/> is removed
        /// from the filtered view as well (unsubscribing from it, if enabled),
        /// and a forwarded Remove event is raised. Items that never matched the
        /// filter are ignored.
        /// </summary>
        private void HandleRemove(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null)
                return;

            // The item(s) are already gone from _sourceCollection by the time this
            // event fires, so we locate them by identity within our own filtered
            // snapshot instead of relying on source indices.
            foreach (T item in e.OldItems)
            {
                int filteredIndex = _filteredItems.IndexOf(item);

                if (filteredIndex < 0)
                    continue; // Item never matched the filter, nothing to do.

                RemoveFilteredItemAt(filteredIndex);

                RaiseRemove(item, filteredIndex);
            }
        }

        /// <summary>
        /// Handles item replacements in the source collection. Depending on
        /// whether the old and new values pass the filter, this results in a
        /// forwarded Replace (both match: unsubscribes the old item, subscribes
        /// the new one), Remove (only the old value matched: unsubscribes it),
        /// Add (only the new value matches: subscribes it), or no event/subscription
        /// change at all (neither matches).
        /// </summary>
        private void HandleReplace(NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems == null || e.NewItems == null)
                return;

            for (int i = 0; i < e.OldItems.Count; i++)
            {
                T oldItem = (T)e.OldItems[i]!;
                T newItem = (T)e.NewItems[i]!;

                bool oldMatches = _filterFunc(oldItem);
                bool newMatches = _filterFunc(newItem);

                if (oldMatches && newMatches)
                {
                    // Still visible: swap value in place.
                    int filteredIndex = _filteredItems.IndexOf(oldItem);
                    if (filteredIndex < 0)
                        continue;

                    UnsubscribeFromItem(oldItem);
                    _filteredItems[filteredIndex] = newItem;
                    SubscribeToItem(newItem);

                    RaiseReplace(oldItem, newItem, filteredIndex);
                }
                else if (oldMatches && !newMatches)
                {
                    // Item leaves the filtered view.
                    int filteredIndex = _filteredItems.IndexOf(oldItem);
                    if (filteredIndex < 0)
                        continue;

                    RemoveFilteredItemAt(filteredIndex);
                    RaiseRemove(oldItem, filteredIndex);
                }
                else if (!oldMatches && newMatches)
                {
                    // Item enters the filtered view.
                    int sourceIndex = e.NewStartingIndex + i;
                    int filteredIndex = GetFilteredIndexForSourceIndex(sourceIndex);

                    InsertFilteredItem(filteredIndex, newItem);
                    RaiseAdd(newItem, filteredIndex);
                }
                // else: neither old nor new match -> no-op for the filtered view.
            }
        }

        /// <summary>
        /// Handles items moved within the source collection. Items that don't
        /// pass the filter are ignored. Items that do pass the filter are
        /// removed from and reinserted into <see cref="_filteredItems"/> at the
        /// position matching their new location in the source collection; a
        /// forwarded Move event is only raised if the filtered position actually
        /// changed. Since the same item instance stays in the view either way,
        /// its PropertyChanged subscription (if any) is left untouched.
        /// </summary>
        private void HandleMove(NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems == null)
                return;

            foreach (T item in e.NewItems)
            {
                if (!_filterFunc(item))
                    continue; // Item was never part of the filtered view.

                int oldFilteredIndex = _filteredItems.IndexOf(item);
                if (oldFilteredIndex < 0)
                    continue;

                _filteredItems.RemoveAt(oldFilteredIndex);

                // Note: if T contains duplicate values, IndexOf may resolve to the
                // wrong occurrence. Use reference-equal or uniquely-keyed items
                // where this matters.
                int sourceIndex = _sourceCollection.IndexOf(item);
                int newFilteredIndex = GetFilteredIndexForSourceIndex(sourceIndex);

                _filteredItems.Insert(newFilteredIndex, item);

                if (oldFilteredIndex != newFilteredIndex)
                {
                    RaiseMove(item, oldFilteredIndex, newFilteredIndex);
                }
            }
        }

        /// <summary>
        /// Handles a Reset on the source collection (e.g. Clear() or a bulk
        /// replace) by unsubscribing from every currently-included item,
        /// fully rebuilding the filtered snapshot from scratch (subscribing to
        /// each newly-included item), and forwarding a Reset event, since
        /// individual index-based diffs are not meaningful for this action.
        /// </summary>
        private void HandleReset()
        {
            if (_isObservingItemPropertyChanges)
            {
                foreach (T item in _filteredItems)
                    UnsubscribeFromItem(item);
            }

            _filteredItems.Clear();

            foreach (T item in _sourceCollection.Where(_filterFunc))
            {
                _filteredItems.Add(item);
                SubscribeToItem(item);
            }

            RaiseReset();
        }

        /// <summary>
        /// Counts how many items in the source collection before (and excluding)
        /// <paramref name="sourceIndex"/> pass the filter. This equals the index
        /// the item at <paramref name="sourceIndex"/> should occupy within the
        /// filtered view, assuming relative ordering is preserved.
        /// </summary>
        private int GetFilteredIndexForSourceIndex(int sourceIndex)
        {
            int filteredIndex = 0;

            for (int i = 0; i < sourceIndex; i++)
            {
                if (_filterFunc(_sourceCollection[i]))
                    filteredIndex++;
            }

            return filteredIndex;
        }

        /// <summary>
        /// Single choke point for inserting an item into <see cref="_filteredItems"/>.
        /// Keeps the list mutation and the item's PropertyChanged subscription
        /// (if enabled) atomic, so every caller that adds an item to the
        /// filtered view automatically stays consistent.
        /// </summary>
        private void InsertFilteredItem(int index, T item)
        {
            _filteredItems.Insert(index, item);
            SubscribeToItem(item);
        }

        /// <summary>
        /// Single choke point for removing an item from <see cref="_filteredItems"/>.
        /// Keeps the list mutation and the item's PropertyChanged unsubscription
        /// (if enabled) atomic, so every caller that removes an item from the
        /// filtered view automatically stays consistent.
        /// </summary>
        private void RemoveFilteredItemAt(int index)
        {
            T item = _filteredItems[index];
            _filteredItems.RemoveAt(index);
            UnsubscribeFromItem(item);
        }

        /// <summary>
        /// Subscribes to <paramref name="item"/>'s <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// event, but only if item property-change observation is enabled
        /// (<see cref="_isObservingItemPropertyChanges"/>) and the item actually
        /// implements <see cref="INotifyPropertyChanged"/>. Safe to call
        /// unconditionally from any code path that adds an item to the filtered view.
        /// </summary>
        private void SubscribeToItem(T item)
        {
            if (_isObservingItemPropertyChanges && item is INotifyPropertyChanged notifyingItem)
                notifyingItem.PropertyChanged += OnItemPropertyChanged;
        }

        /// <summary>
        /// Unsubscribes from <paramref name="item"/>'s <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// event, mirroring <see cref="SubscribeToItem"/>. Safe to call
        /// unconditionally from any code path that removes an item from the filtered view.
        /// </summary>
        private void UnsubscribeFromItem(T item)
        {
            if (_isObservingItemPropertyChanges && item is INotifyPropertyChanged notifyingItem)
                notifyingItem.PropertyChanged -= OnItemPropertyChanged;
        }

        /// <summary>
        /// Callback for filtered items' <see cref="INotifyPropertyChanged.PropertyChanged"/>
        /// event. Ignores changes to any property other than <see cref="_observingPropertyName"/>;
        /// otherwise re-evaluates the changed item via <see cref="ReevaluateItem"/>
        /// so the filtered view reflects the item's new state.
        /// </summary>
        private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName != _observingPropertyName)
                return;

            if (sender is T item)
                ReevaluateItem(item);
        }

        /// <summary>
        /// Raises a forwarded Add <see cref="CollectionChanged"/> event for
        /// <paramref name="item"/> at the given filtered-view index, and
        /// updates dependent property-changed notifications.
        /// </summary>
        private void RaiseAdd(T item, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            RaiseCountAndIndexerChanged();
        }

        /// <summary>
        /// Raises a forwarded Remove <see cref="CollectionChanged"/> event for
        /// <paramref name="item"/> at the given filtered-view index, and
        /// updates dependent property-changed notifications.
        /// </summary>
        private void RaiseRemove(T item, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
            RaiseCountAndIndexerChanged();
        }

        /// <summary>
        /// Raises a forwarded Replace <see cref="CollectionChanged"/> event
        /// (same filtered-view index, old value swapped for new value). Count
        /// does not change on a replace, so only the indexer notification is
        /// raised.
        /// </summary>
        private void RaiseReplace(T oldItem, T newItem, int index)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newItem, oldItem, index));
            RaiseIndexerChanged();
        }

        /// <summary>
        /// Raises a forwarded Move <see cref="CollectionChanged"/> event
        /// reflecting the item's old and new position within the filtered
        /// view. Count does not change on a move, so only the indexer
        /// notification is raised.
        /// </summary>
        private void RaiseMove(T item, int oldIndex, int newIndex)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, item, newIndex, oldIndex));
            RaiseIndexerChanged();
        }

        /// <summary>
        /// Raises a forwarded Reset <see cref="CollectionChanged"/> event after
        /// the filtered snapshot has been fully rebuilt, and updates dependent
        /// property-changed notifications.
        /// </summary>
        private void RaiseReset()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
            RaiseCountAndIndexerChanged();
        }

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> for both <see cref="Count"/> and
        /// the indexer, used whenever the number of items in the filtered view
        /// changes (Add, Remove, Reset).
        /// </summary>
        private void RaiseCountAndIndexerChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
            RaiseIndexerChanged();
        }

        /// <summary>
        /// Raises <see cref="PropertyChanged"/> for the indexer ("Item[]"),
        /// matching the special property name WPF/UWP bindings listen for to
        /// know that item values at existing indices may have changed.
        /// </summary>
        private void RaiseIndexerChanged()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("Item[]"));
        }
    }
}