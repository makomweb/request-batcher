using NUnit.Framework;
using RequestBatcher.Lib;

namespace RequestBatcher.Tests
{
    public class BatcherTests
    {
        [Test]
        public void Two_subsequent_requests_should_have_the_same_id()
        {
            var batcher = new Batcher();
            var g1 = batcher.Add(new Request());
            var g2 = batcher.Add(new Request());

            Assert.AreEqual(g1, g2);

            var g3 = batcher.Add(new Request());
            var g4 = batcher.Add(new Request());
        
            Assert.AreEqual(g3, g4);
            Assert.AreNotEqual(g1, g3);
        }
        
    }
}
