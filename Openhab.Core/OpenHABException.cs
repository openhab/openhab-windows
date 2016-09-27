using System;

namespace OpenHAB.Core
{
    /// <summary>
    /// An Exception class used to throw unexpected errors
    /// </summary>
    public class OpenHABException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABException"/> class.
        /// </summary>
        /// <param name="message">The error message</param>
        public OpenHABException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OpenHABException"/> class.
        /// </summary>
        /// <param name="message">The error message</param>
        /// <param name="exception">The original exception</param>
        public OpenHABException(string message, Exception exception)
            : base(message, exception)
        {
        }
    }
}
