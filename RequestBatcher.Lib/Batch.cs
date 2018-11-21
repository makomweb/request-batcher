using System;
using System.Collections.Generic;

namespace RequestBatcher.Lib
{
    public abstract class Batch<T>
    {
        private readonly List<T> _items = new List<T>();

        public IEnumerable<T> Items => _items;

        public Guid Id { get; private set; } = Guid.NewGuid();

        public abstract Guid Add(T item);

        public abstract bool IsFull { get; }

        public event EventHandler IsReady;

        protected void RaiseIsReady()
        {
            IsReady?.Invoke(this, EventArgs.Empty);
        }

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
