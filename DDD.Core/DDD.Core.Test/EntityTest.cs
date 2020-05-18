using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Test
{
    [TestClass]
    public class EntityTest
    {
        [TestMethod]
        public void EntityHasId()
        {
            Person a = new Person(17);

            Assert.AreEqual(17, a.Id);
        }
    }

    public class Person : Entity<long>
    {
        public Person(long id) : base(id)
        {
        }
    }
}
