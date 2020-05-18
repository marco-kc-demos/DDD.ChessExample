using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using System.ComponentModel.DataAnnotations;

namespace Minor.Miffy.InMemoryBus.Test
{
    [TestClass]
    public class AwaitableIEnumerableTest
    {
        [TestMethod]
        public async Task GetAwaiterWaitsUntilAllTasksAreFinished()
        {
            int counter = 0;
            IEnumerable<int> numbers = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };

            var q = from n in numbers
                    select Task.Run(() =>
                    {
                        Thread.Sleep(10 * n);
                        Interlocked.Add(ref counter, n);
                    });

            // act
            await q;

            // Assert
            Assert.AreEqual(10, q.Count());
            Assert.AreEqual(55, counter);
        }
    }
}
