using System;
using System.Linq;

namespace RequestBatcher.Lib
{
    public class SizedBatch<T> : Batch<T>
    {
        private readonly int _maxSize;

        public SizedBatch(int maxSize)
        {
            _maxSize = maxSize;
        }

        public override bool IsFull => _maxSize == Items.Count();

        public override Guid Add(T item)
        {
            var id = DoAdd(item);

            if (IsFull)
            {
                RaiseIsReady();
            }

            return id;
        }
    }
}
