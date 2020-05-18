using DDD.Core.BusinessRules;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Test.BusinessRules
{
    internal class RuleThatAlwaysHasTwoViolations : BusinessRule
    {
        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            yield return new BusinessRuleViolation("First violation");
            yield return new BusinessRuleViolation("Second violation");
        }
    }
}
