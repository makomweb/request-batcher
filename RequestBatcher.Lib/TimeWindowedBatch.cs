using System;
using System.Threading.Tasks;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Incarnation of the batch class which is used when a batch is executed after accepting new work items within a given time frame.
    /// </summary>
    /// <typeparam name="T">The type of work items.</typeparam>
    public class TimeWindowedBatch<T> : Batch<T>
    {
        private readonly DateTime _expires;

        /// <summary>
        /// Initialize an instance of this class.
        /// </summary>
        /// <param name="timeWindow">The time window for accepting new work items.</param>
        public TimeWindowedBatch(TimeSpan timeWindow)
        {
            _expires = DateTime.Now.Add(timeWindow);
            Initialize(timeWindow);
        }

        /// <summary>
        /// Show if this batch is ready to be executed.
        /// </summary>
        public override bool IsFull => _expires <= DateTime.Now;

        /// <summary>
        /// Add a new work item.
        /// </summary>
        /// <param name="item">the work item</param>
        /// <returns>The ID of the batch this work item is part of.</returns>
        public override Guid Add(T item)
        {
            return DoAdd(item);
        }

        /// <summary>
        /// Initialize expiration logic.
        /// </summary>
        /// <param name="timeWindow">The time window until this batch expires.</param>
        private async void Initialize(TimeSpan timeWindow)
        {
            try
            {
                await Task.Delay(timeWindow);
                RaiseIsReady();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }
}
