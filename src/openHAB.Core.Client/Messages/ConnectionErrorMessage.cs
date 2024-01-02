namespace openHAB.Core.Client.Messages
{
    /// <summary>
    /// Triggers a visual error on a connection issue.
    /// </summary>
    public class ConnectionErrorMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConnectionErrorMessage"/> class.
        /// </summary>
        /// <param name="errorMessage">The error message.</param>
        public ConnectionErrorMessage(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        /// <summary>
        /// Gets or sets the error text.
        /// </summary>
        /// <value>The text.</value>
        public string ErrorMessage
        {
            get; set;
        }
    }
}
