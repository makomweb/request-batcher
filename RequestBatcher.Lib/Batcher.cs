using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RequestBatcher.Lib
{
    public class BatchIsFullException : Exception
    {
        public BatchIsFullException(Guid batchId)
            : base($"The batch '{batchId}' is full!") { }
    }
    }

    public abstract class Batch<T>
    {
        private readonly List<T> _items = new List<T>();

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

        public abstract bool IsFull { get; }
    }

    public class SizedBatch<T> : Batch<T>
    {
        private readonly int _maxSize;

        public SizedBatch(int maxSize)
        {
            _maxSize = maxSize;
        }

        public override bool IsFull => _maxSize == Items.Count();
    }

    public class TimeWindowedBatch<T> : Batch<T>
    {
        private readonly DateTime _expires;

        public TimeWindowedBatch(TimeSpan timeWindow)
        {
            _expires = DateTime.Now.Add(timeWindow);
        }

        public override bool IsFull => _expires <= DateTime.Now;
    }

    public abstract class Batcher<T>
    {
        private readonly BatchProcessor<T> _processor;
        private Batch<T> _batch;

        public Batcher(Func<BatchRequest<T>, BatchResponse> callback, int maxItemsPerBatch = 2)
        {
            _processor = new BatchProcessor<T>(callback);
        }

        /// <summary>
        /// Put an item onto the batch in order to process it.
        /// </summary>
        /// <param name="item">the item</param>
        /// <returns>ID which is used to query process status.</returns>
        public Guid Add(T item)
        {
            if (_batch == null)
            {
                _batch = CreateNewBatch();
            }

            if (_batch.IsFull)
            {
                _processor.Enqueue(_batch);
                _batch = CreateNewBatch();
            }

            _batch.Add(item);
            return _batch.Id;
        }

        /// <summary>
        /// Get the execution object for a given batch ID.
        /// </summary>
        /// <param name="batchId">the id</param>
        /// <returns>the execution object</returns>
        public BatchExecution Query(Guid batchId)
        {
            var task = _processor.Query(batchId);
            return new BatchExecution(task);
        }

        protected abstract Batch<T> CreateNewBatch();
    }

    public class SizedBatcher<T> : Batcher<T>
    {
        private readonly int _maxItemsPerBatch;

        public SizedBatcher(Func<BatchRequest<T>, BatchResponse> callback, int maxItemsPerBatch) : base(callback)
        {
            _maxItemsPerBatch = maxItemsPerBatch;

        }

        protected override Batch<T> CreateNewBatch()
        {
            return new SizedBatch<T>(_maxItemsPerBatch);
        }
    }

    public class TimeWindowedBatcher<T> : Batcher<T>
    {
        private readonly TimeSpan _timeWindow;

        public TimeWindowedBatcher(Func<BatchRequest<T>, BatchResponse> callback, TimeSpan timeWindow) : base(callback)
        {
            _timeWindow = timeWindow;
        }

        protected override Batch<T> CreateNewBatch()
        {
            return new TimeWindowedBatch<T>(_timeWindow);
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

    public abstract class BatchResponse { }

    public class BatchResponse<T> : BatchResponse
    {
        public BatchResponse(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }

    public class BatchExecution
    {
        public BatchExecution(Task<BatchResponse> task)
        {
            Task = task;
        }

        public Task<BatchResponse> Task { get; private set; }

        public bool IsCompleted => Task.IsCompleted;

        public TaskStatus Status => Task.Status;

        public BatchResponse Result
        {
            get
            {
                if (Task.IsFaulted)
                {
                    throw new Exception("Has faulted!", Task.Exception);
                }

                if (Task.IsCanceled)
                {
                    throw new Exception("Was canceled.");
                }

                if (Task.IsCompleted)
                {
                    return Task.Result;
                }

                throw new Exception($"Status is '{Task.Status}'.");
            }
        }
    }

    public class BatchProcessor<T>
    {
        private readonly Dictionary<BatchRequest<T>, Task<BatchResponse>> _tasks = new Dictionary<BatchRequest<T>, Task<BatchResponse>>();
        private readonly Func<BatchRequest<T>, BatchResponse> _callback;

        public BatchProcessor(Func<BatchRequest<T>, BatchResponse> callback)
        {
            _callback = callback;
        }

        public void Enqueue(Batch<T> batch)
        {
            var request = new BatchRequest<T>(batch);
            var task = ProcessAsync(request);
            _tasks.Add(request, task);
        }

        private async Task<BatchResponse> ProcessAsync(BatchRequest<T> request)
        {
            await Task.Delay(2000); // ms
            return await Task.Run(() => _callback(request));
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
