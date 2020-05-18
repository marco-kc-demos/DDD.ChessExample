using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DDD.Core.BusinessRules
{
    /// <summary>
    /// Represents a business rule in the domain (DDD).
    /// </summary>
    public abstract class BusinessRule
    {
        /// <summary>
        /// Checks the business rule
        /// </summary>
        /// <returns>all violations against the rule</returns>
        public abstract IEnumerable<BusinessRuleViolation> CheckRule();

        /// <summary>
        /// Checks the business rule and throws a BusinessRuleViolationException if it is not satified
        /// </summary>
        /// <exception cref="BusinessRuleViolationException">if the rule is not satified</exception>
        public void ThrowIfNotSatisfied()
        {
            IEnumerable<BusinessRuleViolation> violations = CheckRule();

            if (violations != null && violations.Any())
            {
                throw new BusinessRuleViolationException(violations);
            }
        }

        /// <summary>
        /// Checks the business rule and throws a BusinessRuleViolationException if it is not satified
        /// </summary>
        /// <exception cref="BusinessRuleViolationException">if the rule is not satified</exception>
        public static void ThrowIfNotSatisfied(BusinessRule rule)
        {
            rule.ThrowIfNotSatisfied();
        }

        /// <summary>
        /// Combines this rule with another rule. Both rules must be satified to satisfy the combined rule.
        /// </summary>
        /// <param name="rule">another rule</param>
        /// <returns>a combined rule</returns>
        public BusinessRule And(BusinessRule rule)
        {
            return new AndBusinessRule(this, rule);
        }

        /// <summary>
        /// Combines two rules. Both rules must be satified to satisfy the combined rule.
        /// </summary>
        /// <param name="firstRule">first rule</param>
        /// <param name="secondRule">second rule</param>
        /// <returns>the combined rule</returns>
        public static BusinessRule operator & (BusinessRule firstRule, BusinessRule secondRule)
        {
            return new AndBusinessRule(firstRule, secondRule);
        }
    }
}
