using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Util
{
    internal static class Util
    {
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
    }
}
