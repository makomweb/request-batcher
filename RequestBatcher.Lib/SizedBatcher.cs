using System;

namespace RequestBatcher.Lib
{
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
}
