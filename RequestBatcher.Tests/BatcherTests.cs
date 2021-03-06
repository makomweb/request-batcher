﻿using NUnit.Framework;
using RequestBatcher.Lib;
using System;
using System.Threading.Tasks;

namespace RequestBatcher.Tests
{
    public class BatcherTests
    {
        [Test]
        public void Two_subsequent_requests_should_have_the_same_id()
        {
            const int MAX_BATCH_CAPACITY = 2;
            var batcher = new SizedBatcher<string>(_ => new SuccessBatchResponse<bool>(true), MAX_BATCH_CAPACITY);
            var one = batcher.Add("one");
            var two = batcher.Add("two");

            Assert.AreEqual(one, two, "Both requests should be handled by the same batch (identified by it's ID)!");

            var three = batcher.Add("three");
            var four = batcher.Add("four");
        
            Assert.AreEqual(three, four, "Both requests should be handled by the same batch (identified by it's ID)!");
            Assert.AreNotEqual(one, three, "There should be 2 different batches!");
        }
        
        [Test]
        public async Task If_batch_is_full_it_should_be_processed()
        {
            const int MAX_BATCH_CAPACITY = 2;
            var batcher = new SizedBatcher<string>(batch =>
            {
                var j = string.Join(" ", batch.Items);
                return new SuccessBatchResponse<string>(j);
            }, MAX_BATCH_CAPACITY);

            var one = batcher.Add("one");
            var two = batcher.Add("two");

            var batchExecution = batcher.Query(one);

            await batchExecution.WaitForCompletion();
            Assert.IsTrue(batchExecution.IsCompleted, "Execution should be completed!");
            Assert.IsInstanceOf<SuccessBatchResponse<string>>(batchExecution.Result, "Result should be of type 'SuccessBatchResponse<string>'!");

            var result = batchExecution.Result as SuccessBatchResponse<string>;
            Assert.AreEqual("one two", result.Value);
        }

        [Test]
        public void If_batch_is_NOT_full_quering_the_status_should_throw()
        {
            const int MAX_BATCH_CAPACITY = 3;
            var batcher = new SizedBatcher<string>(batch =>
            {
                var j = string.Join(" ", batch.Items);
                return new SuccessBatchResponse<string>(j);
            }, MAX_BATCH_CAPACITY);

            var one = batcher.Add("one");
            var two = batcher.Add("two");

            try
            {
                var batchExecution = batcher.Query(one);
                Assert.Fail("Quering the batch-execution should have thrown before!");
            }
            catch (BatchWaitingForExecutionException)
            {
                // Just an exception of this type is expected.
            }
        }

        [Test]
        public async Task If_time_window_is_NOT_expired_quering_the_status_should_throw()
        {
            TimeSpan MAX_BATCH_TIME_WINDOW = TimeSpan.FromSeconds(5);
            var batcher = new TimeWindowedBatcher<string>(batch =>
            {
                var j = string.Join(" ", batch.Items);
                return new SuccessBatchResponse<string>(j);
            }, MAX_BATCH_TIME_WINDOW);

            var one = batcher.Add("one");
            var two = batcher.Add("two");

            await Task.Delay(300); // ms

            try
            {
                var batchExecution = batcher.Query(one);
                Assert.Fail("Quering the batch-execution should have thrown before!");
            }
            catch (BatchWaitingForExecutionException)
            {
                // Just an exception of this type is expected.
            }
        }

        [Test]
        public async Task If_time_window_is_expired_quering_the_status_should_succeed()
        {
            TimeSpan MAX_BATCH_TIME_WINDOW = TimeSpan.FromMilliseconds(300);
            var batcher = new TimeWindowedBatcher<string>(batch =>
            {
                var j = string.Join(" ", batch.Items);
                return new SuccessBatchResponse<string>(j);
            }, MAX_BATCH_TIME_WINDOW);

            var one = batcher.Add("one");
            var two = batcher.Add("two");

            await Task.Delay(400); // ms

            var batchExecution = batcher.Query(one);
            Assert.IsTrue(batchExecution.IsCompleted, "Execution should be completed!");
        }

        [Test]
        public void If_batch_execution_fails_an_error_response_is_expected()
        {
            const int MAX_BATCH_CAPACITY = 2;
            var batcher = new SizedBatcher<string>(_ =>
            {
                throw new Exception("Execution has failed!");
            }, MAX_BATCH_CAPACITY);

            var one = batcher.Add("one");
            var two = batcher.Add("two");
            
            var batchExecution = batcher.Query(one);
            Assert.IsTrue(batchExecution.IsCompleted, "Execution should be completed!");
            Assert.IsInstanceOf<ExceptionBatchResponse>(batchExecution.Result, "Result should be of type 'ExceptionBatchResponse'!");

            var result = batchExecution.Result as ExceptionBatchResponse;
            Assert.AreEqual("Execution has failed!", result.InnerException.Message);
        }
    }
}
