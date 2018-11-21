using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestBatcher.Lib
{
    public class BatchProcessor<T>
    {
        private readonly Dictionary<BatchRequest<T>, Task<BatchResponse>> _tasks = new Dictionary<BatchRequest<T>, Task<BatchResponse>>();
        private readonly Func<BatchRequest<T>, BatchResponse> _callback;

        public BatchProcessor(Func<BatchRequest<T>, BatchResponse> callback)
        {
            _callback = callback;
        }

        public void StartProcessing(Batch<T> batch)
        {
            var request = new BatchRequest<T>(batch);
            var task = ProcessAsync(request); // Note: Completion of the task is not awaited here!
            _tasks.Add(request, task);
        }

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

        public Task<BatchResponse> Query(Guid batchId)
        {
            var request = Request(batchId);
            return Query(request);
        }

        private Task<BatchResponse> Query(BatchRequest<T> request)
        {
            return _tasks[request];
        }

        private BatchRequest<T> Request(Guid batchId)
        {
            return _tasks.Keys.First(request => request.BatchId == batchId);
        }
    }
}
