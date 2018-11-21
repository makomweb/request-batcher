using System;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// Exception which is used to inform about failed server requests.
    /// E.g. when a server responds with a 404 during processing a request of batched work items.
    /// </summary>
    public class ExceptionBatchResponse : BatchResponse
    {
        /// <summary>
        /// Initialized an instance of this class.
        /// </summary>
        /// <param name="innerException"></param>
        public ExceptionBatchResponse(Exception innerException)
        {
            InnerException = innerException;
        }

        /// <summary>
        /// Get access to the inner exception.
        /// </summary>
        public Exception InnerException { get; }
    }
}
