using System;
using System.Collections.Generic;

namespace RequestBatcher.Lib
{
    public class Request
    {

    }

    public class Batcher
    {
        private List<Request> _batch = new List<Request>();
        private Dictionary<Guid, List<Request>> _batches = new Dictionary<Guid, List<Request>>();
        private Guid _currentBatchId = Guid.NewGuid();
        private int _maxItemsPerBatch;

        public Batcher(int maxItemsPerBatch = 2)
        {
            _maxItemsPerBatch = maxItemsPerBatch;
        }

        public Guid Add(Request request)
        {
            _batch.Add(request);
            return _currentBatchId;
        }

        private Guid NextBatchId => Guid.NewGuid();
    }
}
