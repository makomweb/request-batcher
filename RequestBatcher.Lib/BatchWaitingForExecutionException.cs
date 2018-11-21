using System;

namespace RequestBatcher.Lib
{
    public class BatchWaitingForExecutionException : Exception
    {
        public BatchWaitingForExecutionException(Guid batchId)
            : base($"Batch '{batchId}' is waiting to be executed!") { }
    }
}
