using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Util
{
    internal static class Util
    {
        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the system double click time in milliseconds set for this user.
        /// </summary>
        public static uint DoubleClickTime => GetDoubleClickTime();

        //   ---   Private Methods (extern)   ---

        /// <summary>
        /// Gets the set time for a doubleclick in milliseconds.
        /// </summary>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern uint GetDoubleClickTime();

        //   ---   Public Methods   ---

        /// <summary>
        /// Method used to get the <see cref="DateOnly"/> of the monday of the <paramref name="date"/>'s week.
        /// </summary>
        /// <param name="date">The reference date.</param>
        /// <returns>The date of the last monday before the reference date.</returns>
        public static DateOnly GetMondayOfWeek(DateOnly date)
        {
            int diff = (7 + (int)date.DayOfWeek - (int)DayOfWeek.Monday) % 7;
            return date.AddDays(-diff);
        }

        /// <summary>
        /// Method used to convert a <see cref="DateTimeOffset"/> to a <see cref="DateOnly"/> struct.
        /// </summary>
        /// <param name="dateTimeOffset">The <see cref="DateTimeOffset"/> that is to be converted to a <see cref="DateOnly"/> struct.</param>
        /// <returns>A <see cref="DateOnly"/> representing the specified <paramref name="dateTimeOffset"/>.</returns>
        public static DateOnly DateTimeOffsetToDateOnly(DateTimeOffset dateTimeOffset)
            => DateOnly.FromDateTime(dateTimeOffset.DateTime);

        /// <summary>
        /// Method used to convert a <see cref="DateOnly"/> to a <see cref="DateTimeOffset"/> struct.
        /// </summary>
        /// <param name="date">The <see cref="DateOnly"/> that is to be converted to a <see cref="DateTimeOffset"/> struct.</param>
        /// <returns>A <see cref="DateTimeOffset"/> representing the specified <paramref name="date"/>.</returns>
        public static DateTimeOffset DateOnlyToDateTimeOffset(DateOnly date)
            => new DateTimeOffset(date.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero);
    }
}
