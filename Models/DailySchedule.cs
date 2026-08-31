using Microsoft.UI.Xaml.Media.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Models
{
    /// <summary>
    /// Class used to describe a daily shedule.
    /// </summary>
    public class DailySchedule
    {
        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the date of the schedule.
        /// </summary>
        public DateOnly Date { get; }

        /// <summary>
        /// Gets the <see cref="ImageSource"/> of the image of the schedule.
        /// </summary>
        public BitmapImage Source { get; }

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="DailySchedule"/> class with a specified <paramref name="date"/>
        /// and <see cref="BitmapImage"/> instance containing the schedule image.
        /// </summary>
        /// <param name="date">The date of the schedule.</param>
        /// <param name="source">The <see cref="BitmapImage"/> of the image of the schedule.</param>
        public DailySchedule(DateOnly date, BitmapImage source)
        {
            Date = date;
            Source = source;
        }
    }
}
