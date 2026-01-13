namespace ExpenseTracking.Domain.Exceptions
{
    public class BusinessRuleViolationException : BaseException
    {
        public string RuleName { get; private set; }
        public string RuleDescription { get; private set; }
        
        public BusinessRuleViolationException(string ruleName, string ruleDescription) 
            : base($"Business rule violation: {ruleName}. {ruleDescription}", "BUSINESS_RULE_VIOLATION")
        {
            RuleName = ruleName;
            RuleDescription = ruleDescription;
        }
        
        public BusinessRuleViolationException(string message) 
            : base(message, "BUSINESS_RULE_VIOLATION")
        {
        }
    }
}