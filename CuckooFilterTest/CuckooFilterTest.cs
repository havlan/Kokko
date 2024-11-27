namespace CuckooFilterTest
{
    using CuckooFilter;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    [TestClass]
    public sealed class CuckooFilterTest
    {
        CuckooFilter filter;

        [TestInitialize]
        public void TestInit()
        {
            this.filter = new CuckooFilter(10_000);
        }

        [TestMethod]
        public void TestLookupGuids()
        {
            var guids = new List<Guid>();
            for (var i = 0; i < 10; i++)
            {
                var guid = Guid.NewGuid();
                guids.Add(guid);
                Assert.IsTrue(this.filter.Insert(guid.ToByteArray()));
            }

            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(this.filter.Lookup(guids[i].ToByteArray()));
            }
        }

        [TestMethod]
        public void TestDeleteGuids()
        {
            var guids = new List<Guid>();
            for (var i = 0; i < 10; i++)
            {
                var guid = Guid.NewGuid();
                guids.Add(guid);
                Assert.IsTrue(this.filter.Insert(guid.ToByteArray()));
            }

            for (var i = 0; i < 10; i++)
            {
                Assert.IsTrue(this.filter.Delete(guids[i].ToByteArray()));
            }

            for (var i = 0; i < 10; i++)
            {
                Assert.IsFalse(this.filter.Lookup(guids[i].ToByteArray()));
            }
        }

        [TestMethod]
        public void TestBiggerScaleScenario()
        {
            this.filter = new CuckooFilter(100_000_000);
            var guids = new List<(Guid, Guid)>();
            for (var i = 0; i < 5_000_000; i++)
            {
                var guid = Guid.NewGuid();
                var guid2 = Guid.NewGuid();
                guids.Add((guid, guid2));
                Assert.IsTrue(this.filter.Insert(guid.ToByteArray().Concat(guid2.ToByteArray()).ToArray()));
            }

            for (var i = 0; i < 5_000_000; i++)
            {
                (var g1, var g2) = guids[i];
                var array = g1.ToByteArray().Concat(g2.ToByteArray()).ToArray();
                Assert.IsTrue(this.filter.Lookup(array));
            }
        }
    }
}
