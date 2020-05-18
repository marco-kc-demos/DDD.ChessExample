using DDD.Core.BusinessRules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDD.Core.Test.BusinessRules
{
    [TestClass]
    public class BusinessRuleTest
    {
        [TestMethod]
        public void CheckRuleYieldsViolation()
        {
            BusinessRule target = new RuleThatAlwaysHasOneViolation();

            IEnumerable<BusinessRuleViolation> result = target.CheckRule();

            Assert.AreEqual(1, result.Count());
            Assert.IsTrue(result.Any(v => v.ViolationMessage == "Always one violation"));
        }

        [TestMethod]
        public void CheckRuleYieldsAllViolations()
        {
            BusinessRule target = new RuleThatAlwaysHasTwoViolations();

            IEnumerable<BusinessRuleViolation> result = target.CheckRule();

            Assert.AreEqual(2, result.Count());
            Assert.IsTrue(result.Any(v => v.ViolationMessage == "First violation"));
            Assert.IsTrue(result.Any(v => v.ViolationMessage == "Second violation"));
        }

        [TestMethod]
        public void ThrowIfNotSatisfiedThrowsBusinessRuleViolationException()
        {
            BusinessRule target = new RuleThatAlwaysHasOneViolation();

            Action act = () =>
            {
                target.ThrowIfNotSatisfied();
            };

            var ex = Assert.ThrowsException<BusinessRuleViolationException>(act);
            Assert.AreEqual("Rule Violations: 1 violations have been detected.", ex.Message);
            Assert.AreEqual(1, ex.Violations.Count());
            Assert.IsTrue(ex.Violations.Any(v => v.ViolationMessage == "Always one violation"));
        }

        [TestMethod]
        public void ThrowIfNotSatisfiedThrowsBusinessRuleViolationExceptionWithAllViolations()
        {
            BusinessRule target = new RuleThatAlwaysHasTwoViolations();

            Action act = () =>
            {
                target.ThrowIfNotSatisfied();
            };

            var ex = Assert.ThrowsException<BusinessRuleViolationException>(act);
            Assert.AreEqual("Rule Violations: 2 violations have been detected.", ex.Message);
            Assert.AreEqual(2, ex.Violations.Count());
            Assert.IsTrue(ex.Violations.Any(v => v.ViolationMessage == "First violation"));
            Assert.IsTrue(ex.Violations.Any(v => v.ViolationMessage == "Second violation"));
        }

        [TestMethod]
        public void ThrowIfNotSatisfiedThrowsNoExceptionWhenThereAreNoViolations()
        {
            BusinessRule target = new RuleThatNeverHasAViolation();

            target.ThrowIfNotSatisfied();

            // Assert that no exception has been thrown
        }


        [TestMethod]
        public void StaticThrowIfNotSatisfiedThrowsBusinessRuleViolationException()
        {
            BusinessRule target = new RuleThatAlwaysHasOneViolation();

            Action act = () =>
            {
                BusinessRule.ThrowIfNotSatisfied(target);
            };

            var ex = Assert.ThrowsException<BusinessRuleViolationException>(act);
            Assert.AreEqual("Rule Violations: 1 violations have been detected.", ex.Message);
            Assert.AreEqual(1, ex.Violations.Count());
            Assert.IsTrue(ex.Violations.Any(v => v.ViolationMessage == "Always one violation"));
        }

        [TestMethod]
        public void StaticThrowIfNotSatisfiedThrowsBusinessRuleViolationExceptionWithAllViolations()
        {
            BusinessRule target = new RuleThatAlwaysHasTwoViolations();

            Action act = () =>
            {
                BusinessRule.ThrowIfNotSatisfied(target);
            };

            var ex = Assert.ThrowsException<BusinessRuleViolationException>(act);
            Assert.AreEqual("Rule Violations: 2 violations have been detected.", ex.Message);
            Assert.AreEqual(2, ex.Violations.Count());
            Assert.IsTrue(ex.Violations.Any(v => v.ViolationMessage == "First violation"));
            Assert.IsTrue(ex.Violations.Any(v => v.ViolationMessage == "Second violation"));
        }

        [TestMethod]
        public void StaticThrowIfNotSatisfiedThrowsNoExceptionWhenThereAreNoViolations()
        {
            BusinessRule target = new RuleThatNeverHasAViolation();

            BusinessRule.ThrowIfNotSatisfied(target);

            // Assert that no exception has been thrown
        }

        [TestMethod]
        public void And_CombinesRules_AndResultsInViolationListThatIncludeBoth()
        {
            BusinessRule rule1 = new RuleThatAlwaysHasOneViolation();
            BusinessRule rule2 = new RuleThatAlwaysHasTwoViolations();

            BusinessRule result = rule1.And(rule2);

            IEnumerable<BusinessRuleViolation> violations = result.CheckRule();
            Assert.AreEqual(3, violations.Count());
            Assert.IsTrue(violations.Any(v => v.ViolationMessage == "Always one violation"));
            Assert.IsTrue(violations.Any(v => v.ViolationMessage == "First violation"));
            Assert.IsTrue(violations.Any(v => v.ViolationMessage == "Second violation"));
        }

        [TestMethod]
        public void AndOperator_CombinesRules_AndResultsInViolationListThatIncludeBoth()
        {
            BusinessRule rule1 = new RuleThatAlwaysHasOneViolation();
            BusinessRule rule2 = new RuleThatAlwaysHasTwoViolations();

            BusinessRule result = rule1 & rule2;

            IEnumerable<BusinessRuleViolation> violations = result.CheckRule();
            Assert.AreEqual(3, violations.Count());
            Assert.IsTrue(violations.Any(v => v.ViolationMessage == "Always one violation"));
            Assert.IsTrue(violations.Any(v => v.ViolationMessage == "First violation"));
            Assert.IsTrue(violations.Any(v => v.ViolationMessage == "Second violation"));
        }

        [TestMethod]
        public void AndOperator_CombinesRules_ViolationAreNotDistinct()
        {
            BusinessRule rule1 = new RuleThatAlwaysHasOneViolation();
            BusinessRule rule2 = new RuleThatAlwaysHasOneViolation();

            BusinessRule result = rule1 & rule2;

            IEnumerable<BusinessRuleViolation> violations = result.CheckRule();
            Assert.AreEqual(2, violations.Count());
            Assert.IsTrue(violations.All(v => v.ViolationMessage == "Always one violation"));
        }

        [TestMethod]
        public void AndOperator_CombinesRules_CanBeWithoutAnyViolation()
        {
            BusinessRule rule1 = new RuleThatNeverHasAViolation();
            BusinessRule rule2 = new RuleThatNeverHasAViolation();

            BusinessRule result = rule1 & rule2;

            IEnumerable<BusinessRuleViolation> violations = result.CheckRule();
            Assert.AreEqual(0, violations.Count());
        }
    }
}
