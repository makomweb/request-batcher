using System;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Exception which is thrown when there is no capacity in a batch or a batch has expired to accept new work items.
    /// </summary>
    public class BatchIsFullException : Exception
    {
        /// <summary>
        /// Initialize an instance of this class.
        /// </summary>
        /// <param name="batchId">The ID of the batch.</param>
        public BatchIsFullException(Guid batchId)
            : base($"The batch '{batchId}' is full!") { }
    }
}
