using System;

namespace RequestBatcher.Lib
{
    public class ExceptionBatchResponse : BatchResponse
    {
        public ExceptionBatchResponse(Exception innerException)
        {
            InnerException = innerException;
        }

        public Exception InnerException { get; }
    }
}
