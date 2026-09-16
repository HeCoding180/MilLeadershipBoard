using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Resources
{
    public class DatedResourceNotExistingException : Exception
    {
        //   ---   Public Properties   ---

        /// <summary>
        /// Gets the name of the dated resource that doesn't exist.
        /// </summary>
        public string? ResourceName { init; get; }

        /// <summary>
        /// Gets the date of the dated resource that doesn't exist.
        /// </summary>
        public DateOnly? ResourceDate { init; get; }

        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="DatedResourceNotExistingException"/> class.
        /// </summary>
        public DatedResourceNotExistingException() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="DatedResourceNotExistingException"/> class with a set exception message.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public DatedResourceNotExistingException(string message) : base(message)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="DatedResourceNotExistingException"/> class with a set exception message and inner exception.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">The inner exception of this exception.</param>
        public DatedResourceNotExistingException(string message, Exception? innerException) : base(message, innerException)
        {

        }
    }
}
