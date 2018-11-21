using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// The batch processor is responsible for doing the book keeping about batch requests, for executing requests to the server, 
    /// and for providing status information about batch jobs.
    /// </summary>
    /// <typeparam name="T">The type of work item which is batched.</typeparam>
    public class BatchProcessor<T>
    {
        private readonly Dictionary<BatchRequest<T>, Task<BatchResponse>> _tasks = new Dictionary<BatchRequest<T>, Task<BatchResponse>>();
        private readonly Func<BatchRequest<T>, BatchResponse> _callback;

        /// <summary>
        /// Initialize an instance of this class.
        /// </summary>
        /// <param name="callback">Callback for turning a BatchRequest of work items of type T into a BatchResponse. This is the part of code which is executed against the regular server.</param>
        public BatchProcessor(Func<BatchRequest<T>, BatchResponse> callback)
        {
            _callback = callback;
        }

        /// <summary>
        /// Start processing a batch.
        /// </summary>
        /// <param name="batch">the batch of work items of type T.</param>
        public void StartProcessing(Batch<T> batch)
        {
            var request = new BatchRequest<T>(batch);
            var task = ProcessAsync(request); // Note: Completion of the task is not awaited here!
            _tasks.Add(request, task);
        }

        /// <summary>
        /// Execute the callback in an asynchronous way.
        /// </summary>
        /// <param name="request">The batch request contains the work items of type T.</param>
        /// <returns>A batch response, which can be queried for processing results.</returns>
        private async Task<BatchResponse> ProcessAsync(BatchRequest<T> request)
        {
            try
            {
                return await Task.Run(() => _callback(request));
            }
            catch (Exception ex)
            {
                return new ExceptionBatchResponse(ex);
            }
        }

        /// <summary>
        /// Query for a batch response.
        /// </summary>
        /// <param name="batchId">the Batch ID.</param>
        /// <returns>Task which executes processing the batch job.</returns>
        public Task<BatchResponse> Query(Guid batchId)
        {
            var request = Request(batchId);
            return Query(request);
        }

        /// <summary>
        /// Query for a batch response.
        /// </summary>
        /// <param name="request">the Batch request identified by it's ID.</param>
        /// <returns>Task which executes processing the batch job.</returns>
        private Task<BatchResponse> Query(BatchRequest<T> request)
        {
            return _tasks[request];
        }

        /// <summary>
        /// Get the batch request identified by the batch ID.
        /// </summary>
        /// <param name="batchId">the Batch ID.</param>
        /// <returns>The Batch request.</returns>
        private BatchRequest<T> Request(Guid batchId)
        {
            return _tasks.Keys.First(request => request.BatchId == batchId);
        }
    }
}
