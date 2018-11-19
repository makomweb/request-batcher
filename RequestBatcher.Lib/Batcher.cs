using System;
using System.Collections.Generic;

namespace RequestBatcher.Lib
{
    public class Batcher<T>
    {
        private List<T> _batch = new List<T>();
        private Dictionary<Guid, List<T>> _batches = new Dictionary<Guid, List<T>>();
        private Guid _currentBatchId = Guid.NewGuid();
        private int _maxItemsPerBatch;

        public Batcher(int maxItemsPerBatch = 2)
        {
            _maxItemsPerBatch = maxItemsPerBatch;
        }

        public Guid Add(T item)
        {
            if (IsBatchFull)
            {
                _batches.Add(_currentBatchId, _batch);
                _batch.Clear();
                _currentBatchId = NextBatchId;
            }

            _batch.Add(item);
            return _currentBatchId;
        }

        private bool IsBatchFull => _batch.Count == _maxItemsPerBatch;

        private Guid NextBatchId => Guid.NewGuid();
    }
}
