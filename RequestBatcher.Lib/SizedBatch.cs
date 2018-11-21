using System;
using System.Linq;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Incarnation of the batch class which is used when a batch should contain a predefined number of items before it is marked as full.
    /// </summary>
    /// <typeparam name="T">The type of work items.</typeparam>
    public class SizedBatch<T> : Batch<T>
    {
        private readonly int _maxSize;

        /// <summary>
        /// Initialized an instance of this class.
        /// </summary>
        /// <param name="maxSize">The maximum amount of work items for a single batch.</param>
        public SizedBatch(int maxSize)
        {
            _maxSize = maxSize;
        }

        /// <summary>
        /// Indicate that this batch is full.
        /// </summary>
        public override bool IsFull => _maxSize == Items.Count();

        /// <summary>
        /// Add a new work item to the batch.
        /// </summary>
        /// <param name="item">the Work item.</param>
        /// <returns>The ID of the batch the work item is part of.</returns>
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
