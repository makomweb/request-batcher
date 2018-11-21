using System;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Incarnation of a batcher which processes time-framed batches.
    /// </summary>
    /// <typeparam name="T">Type of work item.</typeparam>
    public class TimeWindowedBatcher<T> : Batcher<T>
    {
        private readonly TimeSpan _timeWindow;

        /// <summary>
        /// Initialze an instance of this class.
        /// </summary>
        /// <param name="callback">Callback for turning a BatchRequest of work items of type T into a BatchResponse. This is the part of code which is executed against the regular server.</param>
        /// <param name="timeWindow">the time frame how long this batch accepts new work items.</param>
        public TimeWindowedBatcher(Func<BatchRequest<T>, BatchResponse> callback, TimeSpan timeWindow) : base(callback)
        {
            _timeWindow = timeWindow;
        }

        /// <summary>
        /// Factory method to create a new (empty) batch container.
        /// </summary>
        /// <returns>The batch.</returns>
        protected override Batch<T> CreateNewBatch()
        {
            return new TimeWindowedBatch<T>(_timeWindow);
        }
    }
}
