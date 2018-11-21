using System;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Incarnation of a batcher which processes pre-sized batches.
    /// </summary>
    /// <typeparam name="T">Type of work item.</typeparam>
    public class SizedBatcher<T> : Batcher<T>
    {
        private readonly int _maxItemsPerBatch;

        /// <summary>
        /// Initialze an instance of this class.
        /// </summary>
        /// <param name="callback">Callback for turning a BatchRequest of work items of type T into a BatchResponse. This is the part of code which is executed against the regular server.</param>
        /// <param name="maxItemsPerBatch">the maximum number of work items per batch.</param>
        public SizedBatcher(Func<BatchRequest<T>, BatchResponse> callback, int maxItemsPerBatch) : base(callback)
        {
            _maxItemsPerBatch = maxItemsPerBatch;
        }

        /// <summary>
        /// Factory method to create a new (empty) batch container.
        /// </summary>
        /// <returns>The batch.</returns>
        protected override Batch<T> CreateNewBatch()
        {
            return new SizedBatch<T>(_maxItemsPerBatch);
        }
    }
}
