﻿using NUnit.Framework;
using RequestBatcher.Lib;
using System.Threading.Tasks;

namespace RequestBatcher.Tests
{
    public class BatcherTests
    {
        [Test]
        public void Two_subsequent_requests_should_have_the_same_id()
        {
            var batcher = new SizedBatcher<string>(_ => new BatchResponse<bool>(true), 2);
            var one = batcher.Add("one");
            var two = batcher.Add("two");

            Assert.AreEqual(one, two);

            var three = batcher.Add("three");
            var four = batcher.Add("four");
        
            Assert.AreEqual(three, four);
            Assert.AreNotEqual(one, three);
        }
        
        [Test]
        public async Task If_batch_is_full_it_should_be_processed()
        {
            var batcher = new SizedBatcher<string>(batch =>
            {
                var j = string.Join(" ", batch.Items);
                return new BatchResponse<string>(j);
            }, 2);

            var one = batcher.Add("one");
            var two = batcher.Add("two");
            var three = batcher.Add("three");

            var batchExecution = batcher.Query(one);
            Assert.IsFalse(batchExecution.IsCompleted, "Execution should not be completed!");

            await batchExecution.Task;
            Assert.IsTrue(batchExecution.IsCompleted, "Execution should be completed!");
            Assert.IsInstanceOf<BatchResponse<string>>(batchExecution.Result, "Result should be of type 'BatchResponse<string>'!");

            var result = batchExecution.Result as BatchResponse<string>;
            Assert.AreEqual("one two", result.Value);
        }

        //[Test]
        //public async Task If_time_window_expires_batch_should_be_processed()
        //{
        //    var batcher = new Batcher<string>(batch =>
        //    {
        //        var j = string.Join(" ", batch.Items);
        //        return new BatchResponse<string>(j);
        //    });

        //    var one = batcher.Add("one");
        //    var two = batcher.Add("two");

        //    await Task.Delay(2000); // ms

        //    await
        //}
    }
}
