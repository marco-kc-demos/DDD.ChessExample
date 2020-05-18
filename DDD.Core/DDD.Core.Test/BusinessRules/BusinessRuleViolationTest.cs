using DDD.Core.BusinessRules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Test.BusinessRules
{
    [TestClass]
    public class BusinessRuleViolationTest
    {
        [TestMethod]
        public void BusinessRuleViolationHasMessage()
        {
            BusinessRuleViolation target = new BusinessRuleViolation("Violation!");

            Assert.AreEqual("Violation!", target.ViolationMessage);
        }
    }
}
