using System;
using System.Threading.Tasks;

namespace RequestBatcher.Lib
{
    /// <summary>
    /// An instance of this class carries execution information about a batch request.
    /// </summary>
    public class BatchExecution
    {
        private Task<BatchResponse> _task;

        /// <summary>
        /// Initialized an instance of this class.
        /// </summary>
        /// <param name="task">Task object which is responsible for performing the asynchronous request-response-work.</param>
        public BatchExecution(Task<BatchResponse> task)
        {
            _task = task;
        }

        /// <summary>
        /// Return the task itself in order to enable awaiting completion.
        /// </summary>
        /// <returns>The async task</returns>
        public Task WaitForCompletion()
        {
            return _task;
        }

        /// <summary>
        /// Check if the execution was completed.
        /// </summary>
        public bool IsCompleted => _task.IsCompleted;

        /// <summary>
        /// Get the status of execution.
        /// </summary>
        public TaskStatus Status => _task.Status;

        /// <summary>
        /// Get the result of this execution.
        /// </summary>
        public BatchResponse Result
        {
            get
            {
                if (_task.IsFaulted)
                {
                    throw new Exception("Has faulted!", _task.Exception);
                }

                if (_task.IsCanceled)
                {
                    throw new Exception("Was canceled!");
                }

                if (_task.IsCompleted)
                {
                    return _task.Result;
                }

                throw new Exception($"Status is '{_task.Status}!'");
            }
        }
    }
}
