using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestBatcher.Lib
{
    public class BatchIsFullException : Exception
    {
        public BatchIsFullException(Guid id)
            : base($"The batch '{id}' batch is full") { }
    }

    public class Batch<T>
    {
        private readonly List<T> _items = new List<T>();
        private readonly int _maxSize;

        public Batch(int maxSize)
        {
            _maxSize = maxSize;
        }

        public IEnumerable<T> Items => _items;

        public Guid Id { get; private set; } = Guid.NewGuid();

        public void Add(T item)
        {
            if (IsFull)
            {
                throw new BatchIsFullException(Id);
            }

            _items.Add(item);
        }

        public bool IsFull => _maxSize == _items.Count;
    }

    public class Batcher<T>
    {
        private readonly int _maxItemsPerBatch;
        private BatchProcessor<T> _processor = new BatchProcessor<T>();
        private Batch<T> _batch;

        public Batcher(int maxItemsPerBatch = 2)
        {
            _maxItemsPerBatch = maxItemsPerBatch;
            _batch = new Batch<T>(_maxItemsPerBatch);
        }

        public Guid Add(T item)
        {
            if (_batch.IsFull)
            {
                _processor.Enqueue(_batch);
                _batch = new Batch<T>(_maxItemsPerBatch);
            }

            _batch.Add(item);
            return _batch.Id;
        }

        public BatchProcessResult Query(Guid batchId)
        {
            var t = _processor.Query(batchId);
            return new BatchProcessResult(t);
        }
    }

    public class BatchRequest<T>
    {
        private Batch<T> _batch;

        public BatchRequest(Batch<T> batch)
        {
            _batch = batch;
        }

        public IEnumerable<T> Items => _batch.Items;

        public Guid BatchId => _batch.Id;
    }

    public class BatchResponse { }

    public class BatchProcessResult
    {
        private readonly Task<BatchResponse> _task;

        public BatchProcessResult(Task<BatchResponse> task)
        {
            _task = task;
        }

        public bool IsCompleted => _task.IsCompleted;

        public TaskStatus Status => _task.Status;

        public BatchResponse Value
        {
            get
            {
                if (_task.IsFaulted)
                {
                    throw new Exception("Has faulted!", _task.Exception);
                }

                if (_task.IsCanceled)
                {
                    throw new Exception("Was canceled.");
                }

                if (_task.IsCompleted)
                {
                    return _task.Result;
                }

                throw new Exception($"Status is '{_task.Status}'.");
            }
        }

        public Task<BatchResponse> GetValueAsync()
        {
            return _task;
        }
    }

    public class BatchProcessor<T>
    {
        private readonly Dictionary<BatchRequest<T>, Task<BatchResponse>> _tasks = new Dictionary<BatchRequest<T>, Task<BatchResponse>>();

        public void Enqueue(Batch<T> batch)
        {
            var r = new BatchRequest<T>(batch);
            var t = ProcessAsync(r);
            _tasks.Add(r, t);
        }

        private async Task<BatchResponse> ProcessAsync(BatchRequest<T> request)
        {
#if false
            // TODO implement async processing!
            return Task.FromResult(new BatchResponse());
            return Task.Run(() => new BatchResponse());
#else
            await Task.Delay(3000); // ms
            return new BatchResponse();
#endif
        }

        public Task<BatchResponse> Query(Guid batchId)
        {
            var r = Request(batchId);
            return Query(r);
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
