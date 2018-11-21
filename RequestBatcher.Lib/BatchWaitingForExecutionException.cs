using System;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Exception which is thrown when batch information is queried but the batch was not executed yet.
    /// </summary>
    public class BatchWaitingForExecutionException : Exception
    {
        /// <summary>
        /// Initialize an instance of this class.
        /// </summary>
        /// <param name="batchId">the batch ID.</param>
        public BatchWaitingForExecutionException(Guid batchId)
            : base($"Batch '{batchId}' is waiting to be executed!") { }
    }
}
