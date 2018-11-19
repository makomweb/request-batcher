using System;
using System.Collections.Generic;

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
        private readonly Dictionary<Guid, Batch<T>> _batches = new Dictionary<Guid, Batch<T>>();
        private readonly int _maxItemsPerBatch;
        private Batch<T> _batch;

        public Batcher(int maxItemsPerBatch = 2)
        {
            _maxItemsPerBatch = maxItemsPerBatch;
            _batch = new Batch<T>(_maxItemsPerBatch);
        }

        public Guid Add(T item)
        {
            if (IsBatchFull)
            {
                _batches.Add(_batch.Id, _batch);
                _batch = new Batch<T>(_maxItemsPerBatch);
            }

            _batch.Add(item);
            return _batch.Id;
        }

        private bool IsBatchFull => _batch.IsFull;
    }

    public class BatchRequest
    {

    }

    public class BatchResponse
    {

    }

    public class BatchProcessor
    {
        public BatchResponse Process(BatchRequest request)
        {
            return new BatchResponse();
        }
    }
}
