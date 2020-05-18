namespace DDD.Core.BusinessRules
{
    public class BusinessRuleViolation
    {
        public string ViolationMessage { get; }

        public BusinessRuleViolation(string violationMessage)
        {
            ViolationMessage = violationMessage;
        }
    }
}