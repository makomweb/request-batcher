using System;
using System.Collections.Generic;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Abstract base class for all incarnations of a batch.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Batch<T>
    {
        private readonly List<T> _items = new List<T>();

        /// <summary>
        /// Get the work items from this batch object.
        /// </summary>
        public IEnumerable<T> Items => _items;

        /// <summary>
        /// Get the ID of this batch. To lookup status of its execution.
        /// </summary>
        public Guid Id { get; private set; } = Guid.NewGuid();

        /// <summary>
        /// Add a new work item to the batch.
        /// </summary>
        /// <param name="item">the work item</param>
        /// <returns>The ID of this batch.</returns>
        public abstract Guid Add(T item);

        /// <summary>
        /// Return if this batch is full - ready to be executed.
        /// </summary>
        public abstract bool IsFull { get; }

        /// <summary>
        /// Event which is raised when this batch is ready to be executed.
        /// </summary>
        public event EventHandler IsReady;

        /// <summary>
        /// Raise the 'IsReady' event.
        /// </summary>
        protected void RaiseIsReady()
        {
            IsReady?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Method for handling adding a new work item to the batch.
        /// </summary>
        /// <param name="item">The work item.</param>
        /// <returns>The ID of this batch.</returns>
        protected Guid DoAdd(T item)
        {
            if (IsFull)
            {
                throw new BatchIsFullException(Id);
            }

            _items.Add(item);

            return Id;
        }
    }
}
