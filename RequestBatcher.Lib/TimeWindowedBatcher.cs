using System;

namespace RequestBatcher.Lib
{
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
}
