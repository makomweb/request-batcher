using NUnit.Framework;
using RequestBatcher.Lib;
using System.Threading.Tasks;

namespace RequestBatcher.Tests
{
    public class BatcherTests
    {
        [Test]
        public void Two_subsequent_requests_should_have_the_same_id()
        {
            var batcher = new Batcher<string>(_ => new BatchResponse<bool>(true));
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
            var batcher = new Batcher<string>(batch =>
            {
                var j = string.Join(" ", batch.Items);
                return new BatchResponse<string>(j);
            });

            var one = batcher.Add("one");
            var two = batcher.Add("two");
            var three = batcher.Add("three");

            var execution = batcher.Query(one);
            Assert.IsFalse(execution.IsCompleted, "Status should be 'not completed'!");

            await execution.Task;
            Assert.IsTrue(execution.IsCompleted, "Status should be 'completed' now!");
            Assert.IsInstanceOf<BatchResponse<string>>(execution.Result, "Value should be of type 'BatchResponse'!");

            var result = execution.Result as BatchResponse<string>;
            Assert.AreEqual("one two", result.Value);
        }
    }
}
