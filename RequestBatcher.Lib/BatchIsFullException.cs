using System;

namespace RequestBatcher.Lib
{
    public class BatchIsFullException : Exception
    {
        public BatchIsFullException(Guid batchId)
            : base($"The batch '{batchId}' is full!") { }
    }
}
