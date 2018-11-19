using NUnit.Framework;
using RequestBatcher.Lib;

namespace RequestBatcher.Tests
{
    public class BatcherTests
    {
        [Test]
        public void Two_subsequent_requests_should_have_the_same_id()
        {
            var batcher = new Batcher<string>();
            var one = batcher.Add("one");
            var two = batcher.Add("two");

            Assert.AreEqual(one, two);

            var three = batcher.Add("three");
            var four = batcher.Add("four");
        
            Assert.AreEqual(three, four);
            Assert.AreNotEqual(one, three);
        }
        
        [Test]
        public void If_batch_is_full_it_should_be_processed()
        {
            var batcher = new Batcher<string>();
            var one = batcher.Add("one");
            var two = batcher.Add("two");
            var three = batcher.Add("three");

            var result = batcher.Query(one);
            Assert.NotNull(result);
        }
    }
}
