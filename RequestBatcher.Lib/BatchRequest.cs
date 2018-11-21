using System;
using System.Collections.Generic;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Class which wraps a batch of work items in order to process it with the batch-processor.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BatchRequest<T>
    {
        private Batch<T> _batch;

        /// <summary>
        /// Initialize an instance of this class.
        /// </summary>
        /// <param name="batch">the batch of work items.</param>
        public BatchRequest(Batch<T> batch)
        {
            _batch = batch;
        }

        /// <summary>
        /// Get the enumeration of work items.
        /// </summary>
        public IEnumerable<T> Items => _batch.Items;

        /// <summary>
        /// Get the ID of this batch.
        /// </summary>
        public Guid BatchId => _batch.Id;
    }
}
