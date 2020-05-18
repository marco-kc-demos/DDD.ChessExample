using DDD.Core.BusinessRules;
using System;
using System.Collections.Generic;
using System.Text;

namespace DDD.Core.Test.BusinessRules
{
    internal class RuleThatNeverHasAViolation : BusinessRule
    {
        public override IEnumerable<BusinessRuleViolation> CheckRule()
        {
            yield break;
        }
    }
}
