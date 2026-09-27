using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MilLeadershipBoard.Util.Exceptions
{
    /// <summary>
    /// Exception class used when the DataContext type is not supported.
    /// </summary>
    class DataContextNotSupportedException : ApplicationException
    {
        //   ---   Constructors   ---

        /// <summary>
        /// Creates a new instance of the <see cref="DataContextNotSupportedException"/> class.
        /// </summary>
        public DataContextNotSupportedException() : base()
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="DataContextNotSupportedException"/> class with an exception message.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        public DataContextNotSupportedException(string message) : base(message)
        {

        }

        /// <summary>
        /// Creates a new instance of the <see cref="DataContextNotSupportedException"/> class with an exception message and an inner <see cref="Exception"/>.
        /// </summary>
        /// <param name="message">Message of the exception.</param>
        /// <param name="innerException">Inner <see cref="Exception"/> of this <see cref="Exception"/>.</param>
        public DataContextNotSupportedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
