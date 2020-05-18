using System.Collections.Generic;
using System.Linq;

namespace DDD.Core.BusinessRules
{
    internal class AndBusinessRule : BusinessRule
    {
        private readonly BusinessRule _firstRule;
        private readonly BusinessRule _secondRule;

        public AndBusinessRule(BusinessRule firstRule, BusinessRule secondRule)
        {
            _firstRule = firstRule;
            _secondRule = secondRule;
        }

        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            return _firstRule.CheckRule().Concat(
                  _secondRule.CheckRule());
        }
    }
}