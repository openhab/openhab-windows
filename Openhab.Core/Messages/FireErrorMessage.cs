using OpenHAB.Core.Common;

namespace OpenHAB.Core.Messages
{
    /// <summary>
    /// Triggers a visual error message.
    /// </summary>
    public class FireErrorMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FireErrorMessage"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public FireErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FireErrorMessage"/> class.
        /// </summary>
        /// <param name="errorType"></param>
        /// <param name="errorMessage">The error message.</param>
        public FireErrorMessage(ErrorTypes errorType, string errorMessage)
        {
            ErrorMessage = errorMessage;
            ErrorType = errorType;
        }

        /// <summary>
        /// Gets or sets the error text.
        /// </summary>
        /// <value>The text.</value>
        public string ErrorMessage
        {
            get; set;
        }

        /// <summary>Gets the type of the error.</summary>
        /// <value>The type of the error.</value>
        public ErrorTypes ErrorType
        {
            get;
        }
    }
}
