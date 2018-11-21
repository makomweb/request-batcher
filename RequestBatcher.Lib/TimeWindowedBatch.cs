using System;
using System.Threading.Tasks;

namespace RequestBatcher.Lib
{
    public class TimeWindowedBatch<T> : Batch<T>
    {
        private readonly DateTime _expires;

        public TimeWindowedBatch(TimeSpan timeWindow)
        {
            _expires = DateTime.Now.Add(timeWindow);
            Initialize(timeWindow);
        }

        public override bool IsFull => _expires <= DateTime.Now;

        public override Guid Add(T item)
        {
            return DoAdd(item);
        }

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
