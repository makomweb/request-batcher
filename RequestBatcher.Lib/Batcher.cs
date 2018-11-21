using System;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Abstract base class for all Batcher<typeparamref name="T"/> incarnations.
    /// </summary>
    /// <typeparam name="T">The type of request which will be batched.</typeparam>
    public abstract class Batcher<T>
    {
        private readonly BatchProcessor<T> _processor;
        private Batch<T> _batch;

        /// <summary>
        /// Initialized an instance of this class.
        /// </summary>
        /// <param name="callback">Callback for turning a BatchRequest of work items of type T into a BatchResponse. This is the part of code which is executed against the regular server.</param>
        /// <param name="maxItemsPerBatch">The maximum number of work items per batch.</param>
        public Batcher(Func<BatchRequest<T>, BatchResponse> callback, int maxItemsPerBatch = 2)
        {
            _processor = new BatchProcessor<T>(callback);
        }

        /// <summary>
        /// Put a work item onto the batch in order to process it.
        /// </summary>
        /// <param name="item">the work item</param>
        /// <returns>ID which is used to query execution status.</returns>
        public Guid Add(T item)
        {
            if (_batch == null)
            {
                _batch = CreateNewBatch();
                _batch.IsReady += OnBatchIsReady;
            }

            return _batch.Add(item);
        }

        /// <summary>
        /// Event handler which is called when a batch is ready to be processed (aka executed).
        /// </summary>
        /// <param name="sender">The sender is the batch itself.</param>
        /// <param name="e">There are currently no event arguments.</param>
        private void OnBatchIsReady(object sender, EventArgs e)
        {
            _batch.IsReady -= OnBatchIsReady;
            _batch = null;

            var batch = sender as Batch<T>;
            _processor.StartProcessing(batch);
        }

        /// <summary>
        /// Get the execution object for a given batch ID.
        /// </summary>
        /// <param name="batchId">the id</param>
        /// <returns>the execution object</returns>
        public BatchExecution Query(Guid batchId)
        {
            if (_batch != null && _batch.Id == batchId)
            {
                throw new BatchWaitingForExecutionException(_batch.Id);
            }

            var task = _processor.Query(batchId);
            return new BatchExecution(task);
        }

        /// <summary>
        /// Factory method for creating a new unique batch.
        /// </summary>
        /// <returns>The batch object.</returns>
        protected abstract Batch<T> CreateNewBatch();
    }
}
