﻿using NUnit.Framework;
using RequestBatcher.Lib;

namespace RequestBatcher.Tests
{
    public class BatcherTests
    {
        [Test]
        public void Two_subsequent_requests_should_have_the_same_id()
        {
            var batcher = new Batcher<string>();
            var g1 = batcher.Add("one");
            var g2 = batcher.Add("two");

            Assert.AreEqual(g1, g2);

            var g3 = batcher.Add("three");
            var g4 = batcher.Add("four");
        
            Assert.AreEqual(g3, g4);
            Assert.AreNotEqual(g1, g3);
        }
        
        [Test]
        public void If_batch_is_full_it_should_be_processed()
        {
            var batcher = new Batcher<string>();
            var g1 = batcher.Add("one");
            var g2 = batcher.Add("two");
            var g3 = batcher.Add("three");

            var r1 = batcher.Query(g1);
            Assert.NotNull(r1);
        }
    }
}
