using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Application.Test
{
    [TestClass]
    public class TypeCacheTest
    {
        [TestMethod]
        public void CacheCanFindString()
        {
            TypeCache target = new TypeCache();

            Type result = target.FindType("System.String");

            Assert.AreEqual(typeof(System.String), result);
        }

        [TestMethod]
        public void CacheCanFindItsOwnType()
        {
            TypeCache target = new TypeCache();

            Type result = target.FindType(target.GetType().FullName);

            Assert.AreEqual(target.GetType(), result);
        }

        [TestMethod]
        public void CacheCanOnlyFindFullNames()
        {
            TypeCache target = new TypeCache();

            Type result = target.FindType("Int32");

            Assert.AreEqual(null, result);
        }

        [TestMethod]
        public void CacheCanFind_AccountOpened_InOtherAssembly()
        {
            TypeCache target = new TypeCache();

            Type result = target.FindType("DDD.Core.Application.Test.AccountOpened");

            Assert.AreEqual(typeof(AccountOpened), result);
        }

        [TestMethod]
        public void CacheCanFindThisTestClassFromCache()
        {
            TypeCache target = new TypeCache();
            target.FindType("DDD.Core.Application.Test.AccountOpened");

            Type result = target.FindType("DDD.Core.Application.Test.AccountOpened");

            Assert.AreEqual(typeof(AccountOpened), result);
        }
    }
}
