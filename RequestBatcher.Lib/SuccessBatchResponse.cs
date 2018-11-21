namespace RequestBatcher.Lib
{
    public class SuccessBatchResponse<T> : BatchResponse
    {
        public SuccessBatchResponse(T value)
        {
            Value = value;
        }

        public T Value { get; private set; }
    }
}
