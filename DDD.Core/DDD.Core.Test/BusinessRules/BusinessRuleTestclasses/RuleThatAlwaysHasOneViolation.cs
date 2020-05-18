using DDD.Core.BusinessRules;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Test.BusinessRules
{
    internal class RuleThatAlwaysHasOneViolation : BusinessRule
    {
        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            yield return new BusinessRuleViolation("Always one violation");
        }
    }
}
