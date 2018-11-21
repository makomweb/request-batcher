using System;

namespace RequestBatcher.Lib
{
    public abstract class Batcher<T>
    {
        private readonly BatchProcessor<T> _processor;
        private Batch<T> _batch;

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

        protected abstract Batch<T> CreateNewBatch();
    }
}
