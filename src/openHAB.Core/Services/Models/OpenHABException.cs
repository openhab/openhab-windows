using System;

namespace openHAB.Core.Client.Models
{
    /// <summary>
    /// An Exception class used to throw unexpected errors.
    /// </summary>
    public class ServiceException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public ServiceException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="exception">The original exception.</param>
        public ServiceException(string message, Exception exception)
            : base(message, exception)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABException"/> class.
        /// </summary>
        public ServiceException()
        {
        }
    }
}
