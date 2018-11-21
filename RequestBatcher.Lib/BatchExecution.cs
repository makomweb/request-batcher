using System;
using System.Threading.Tasks;

namespace RequestBatcher.Lib
{
    public class BatchExecution
    {
        public BatchExecution(Task<BatchResponse> task)
        {
            Task = task;
        }

        public Task<BatchResponse> Task { get; private set; }

        public bool IsCompleted => Task.IsCompleted;

        public TaskStatus Status => Task.Status;

        public BatchResponse Result
        {
            get
            {
                if (Task.IsFaulted)
                {
                    throw new Exception("Has faulted!", Task.Exception);
                }

                if (Task.IsCanceled)
                {
                    throw new Exception("Was canceled!");
                }

                if (Task.IsCompleted)
                {
                    return Task.Result;
                }

                throw new Exception($"Status is '{Task.Status}!'");
            }
        }
    }
}
