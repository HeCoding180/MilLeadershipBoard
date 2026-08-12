using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace MilLeadershipBoard.UI.Controls
{
    /// <summary>
    /// A layout panel that arranges its children into sequential, top-to-bottom filled
    /// columns, similar to a "masonry"/newspaper-style layout.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Each child is stretched to use the full width of its column. All columns share
    /// the same width (<c>availableWidth / columnCount</c>). The panel starts with a
    /// single column and, if the content does not fit within the available height,
    /// increases the column count and re-measures until every item fits — or until
    /// every child occupies its own column as a last-resort fallback.
    /// </para>
    /// <para>
    /// Because column width depends on <see cref="_columnCount"/>, and item height can
    /// depend on column width (e.g. text wrapping), children are re-measured for every
    /// candidate column count rather than simply re-flowing cached sizes.
    /// </para>
    /// <para>
    /// This panel is intended to be used as the <see cref="ItemsControl.ItemsPanel"/> of
    /// a <see cref="ListView"/>. It does not virtualize, so reordering via
    /// <c>CanReorderItems</c>/<c>CanDragItems</c> continues to work normally, but very
    /// large item counts may impact measure performance (see <see cref="MeasureOverride"/>).
    /// </para>
    /// </remarks>
    public class SequentialColumnPanel : Panel, IInsertionPanel
    {
        /// <summary>
        /// The number of columns used in the most recent successful layout pass.
        /// </summary>
        private int _columnCount = 1;

        /// <summary>
        /// The width, in pixels, of each column in the most recent successful layout pass.
        /// </summary>
        private double _columnWidth;

        /// <summary>
        /// Maps each child index (matching <see cref="Panel.Children"/> order) to the
        /// zero-based column index it was assigned to during the last measure pass.
        /// <see langword="null"/> if no valid assignment exists yet (e.g. before the
        /// first measure, or when the available width is infinite).
        /// </summary>
        private List<int>? _assignment;

        /// <summary>
        /// The desired height of each child (matching <see cref="Panel.Children"/> order)
        /// as measured against <see cref="_columnWidth"/> during the last measure pass.
        /// <see langword="null"/> under the same conditions as <see cref="_assignment"/>.
        /// </summary>
        private List<double>? _heights;

        //   ---   Protected Methods (overrides)   ---

        /// <inheritdoc/>
        /// <remarks>
        /// Determines the smallest column count (starting at 1) for which all children
        /// can be stacked into columns without any column's accumulated height exceeding
        /// <paramref name="availableSize"/>.Height. For each candidate column count, every
        /// child is re-measured at the corresponding column width, since column width
        /// changes can change a child's desired height (e.g. wrapping text).
        /// <para>
        /// If no column count up to <c>Children.Count</c> allows every item to fit
        /// (e.g. a single item is taller than the available height on its own), the
        /// panel falls back to giving every child its own column.
        /// </para>
        /// <para>
        /// Complexity is roughly O(n * columnsTried), since every candidate column count
        /// re-measures every child. For very large item collections, consider capping
        /// the search (e.g. via a minimum sensible column width) rather than trying up
        /// to <c>Children.Count</c> columns.
        /// </para>
        /// </remarks>
        protected override Size MeasureOverride(Size availableSize)
        {
            double availableWidth = availableSize.Width;
            double availableHeight = double.IsInfinity(availableSize.Height)
                ? double.MaxValue : availableSize.Height;

            // Without a finite width we can't compute a column width, so just measure
            // children at their natural size and skip column assignment entirely.
            if (Children.Count == 0 || double.IsInfinity(availableWidth))
            {
                foreach (var child in Children)
                    child.Measure(availableSize);
                _assignment = null;
                _heights = null;
                return new Size(0, 0);
            }

            int maxColumns = Children.Count;
            List<int>? assignment = null;
            List<double> heights = new(Children.Count);
            double usedColumnWidth = availableWidth;
            int usedColumnCount = 1;

            // Try increasing column counts until every child fits within availableHeight.
            for (int columnCount = 1; columnCount <= maxColumns; columnCount++)
            {
                double columnWidth = availableWidth / columnCount;
                heights = new List<double>(Children.Count);

                // Re-measure every child at this candidate column width, since desired
                // height may depend on the width available (e.g. wrapping text).
                foreach (var child in Children)
                {
                    child.Measure(new Size(columnWidth, double.PositiveInfinity));
                    heights.Add(child.DesiredSize.Height);
                }

                var colHeights = new double[columnCount];
                var trial = new int[Children.Count];
                int currentCol = 0;
                bool fits = true;

                // Greedily fill columns top-to-bottom, left-to-right: keep adding items
                // to the current column until the next item would overflow the available
                // height, then advance to the next column.
                for (int i = 0; i < Children.Count; i++)
                {
                    double h = heights[i];
                    if (colHeights[currentCol] > 0 && colHeights[currentCol] + h > availableHeight)
                    {
                        currentCol++;
                        if (currentCol >= columnCount) { fits = false; break; }
                    }
                    trial[i] = currentCol;
                    colHeights[currentCol] += h;
                }

                if (fits)
                {
                    assignment = trial.ToList();
                    usedColumnWidth = columnWidth;
                    usedColumnCount = columnCount;
                    break;
                }
            }

            // Fallback: even with one column per item, at least one item is taller than
            // the available height on its own. Give every child its own column so
            // nothing is lost, accepting that content may overflow vertically.
            if (assignment is null)
            {
                usedColumnCount = Children.Count;
                usedColumnWidth = availableWidth / usedColumnCount;
                assignment = Enumerable.Range(0, Children.Count).ToList();
                heights = new List<double>();
                foreach (var child in Children)
                {
                    child.Measure(new Size(usedColumnWidth, double.PositiveInfinity));
                    heights.Add(child.DesiredSize.Height);
                }
            }

            _columnCount = usedColumnCount;
            _columnWidth = usedColumnWidth;
            _assignment = assignment;
            _heights = heights;

            var finalColHeights = new double[usedColumnCount];
            for (int i = 0; i < Children.Count; i++)
                finalColHeights[assignment[i]] += heights[i];

            double desiredHeight = finalColHeights.Length > 0 ? finalColHeights.Max() : 0;
            return new Size(availableWidth, Math.Min(desiredHeight, availableHeight));
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Positions each child according to the column assignment and per-child heights
        /// computed in <see cref="MeasureOverride"/>. Children in the same column are
        /// stacked vertically in their original order; each column occupies a horizontal
        /// slice of width <see cref="_columnWidth"/> starting at <c>column * _columnWidth</c>.
        /// <para>
        /// If <see cref="_assignment"/> (or <see cref="_heights"/>) is <see langword="null"/>
        /// — e.g. measure was skipped because the available width was infinite — children
        /// are arranged in a simple single-column, top-to-bottom stack instead.
        /// </para>
        /// </remarks>
        protected override Size ArrangeOverride(Size finalSize)
        {
            // No column assignment available (e.g. infinite-width measure pass) — fall
            // back to a plain vertical stack using each child's own desired size.
            if (_assignment is null || _heights is null)
            {
                double y = 0;
                foreach (var child in Children)
                {
                    child.Arrange(new Rect(0, y, finalSize.Width, child.DesiredSize.Height));
                    y += child.DesiredSize.Height;
                }
                return finalSize;
            }

            var colY = new double[_columnCount];
            for (int i = 0; i < Children.Count; i++)
            {
                int col = _assignment[i];
                Children[i].Arrange(new Rect(col * _columnWidth, colY[col], _columnWidth, _heights[i]));
                colY[col] += _heights[i];
            }
            return finalSize;
        }

        //   ---   Public Methods   ---

        /// <summary>
        /// Returns the indices of the items that the specified point falls between, for
        /// use during a drag-and-drop reorder operation.
        /// </summary>
        /// <param name="position">
        /// The pointer position, in this panel's coordinate space, of the item currently
        /// being dragged.
        /// </param>
        /// <param name="first">
        /// When this method returns, contains the index of the item immediately before
        /// <paramref name="position"/>, or <c>-1</c> if <paramref name="position"/> is
        /// before the first item in its column.
        /// </param>
        /// <param name="second">
        /// When this method returns, contains the index of the item immediately after
        /// <paramref name="position"/>, or <c>-1</c> if <paramref name="position"/> is
        /// after the last item in its column.
        /// </param>
        /// <remarks>
        /// Finds the column containing <paramref name="position"/>.X (clamping to the
        /// nearest valid column if the pointer is outside the panel's horizontal bounds),
        /// then walks that column's items top-to-bottom comparing against each item's
        /// vertical midpoint to determine which pair of items <paramref name="position"/>
        /// falls between.
        /// </remarks>
        public void GetInsertionIndexes(Point position, out int first, out int second)
        {
            first = -1;
            second = -1;

            if (_assignment is null || _heights is null || Children.Count == 0)
                return;

            // Determine which column the pointer is over.
            int targetColumn = _columnWidth > 0
                ? (int)(position.X / _columnWidth)
                : 0;
            targetColumn = Math.Clamp(targetColumn, 0, _columnCount - 1);

            // Walk the children in that column top-to-bottom, tracking cumulative Y
            // and the previous item's index within the column.
            double y = 0;
            int previousIndex = -1;

            for (int i = 0; i < Children.Count; i++)
            {
                if (_assignment[i] != targetColumn)
                    continue;

                double height = _heights[i];
                double midpoint = y + height / 2;

                if (position.Y < midpoint)
                {
                    first = previousIndex;
                    second = i;
                    return;
                }

                previousIndex = i;
                y += height;
            }

            // Pointer is below the last item in this column.
            first = previousIndex;
            second = -1;
        }
    }
}
