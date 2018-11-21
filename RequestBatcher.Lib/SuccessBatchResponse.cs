namespace RequestBatcher.Lib
{
    /// <summary>
    /// An instance of this class is returned when a batch request was executed successfully.
    /// </summary>
    /// <typeparam name="T">The type of work items within this batch.</typeparam>
    public class SuccessBatchResponse<T> : BatchResponse
    {
        /// <summary>
        /// Initialize an instance of this class.
        /// </summary>
        /// <param name="value"></param>
        public SuccessBatchResponse(T value)
        {
            Value = value;
        }

        /// <summary>
        /// Get access to the response value.
        /// </summary>
        public T Value { get; private set; }
    }
}
