namespace openHAB.Core.Messages
{
    /// <summary>
    /// Represents the state of a data operation.
    /// </summary>
    public enum OperationState
    {
        /// <summary>
        /// Represents no operation state.
        /// </summary>
        None,

        /// <summary>
        /// Represents the start of a data operation.
        /// </summary>
        Started,

        /// <summary>
        /// Represents the completion of a data operation.
        /// </summary>
        Completed
    }

    /// <summary>
    /// Represents a data operation.
    /// </summary>
    public class DataOperation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataOperation"/> class with the specified state.
        /// </summary>
        /// <param name="state">The state of the data operation.</param>
        public DataOperation(OperationState state = OperationState.None)
        {
            State = state;
        }

        /// <summary>
        /// Gets or sets the state of the data operation.
        /// </summary>
        public OperationState State { get; set; }
    }
}