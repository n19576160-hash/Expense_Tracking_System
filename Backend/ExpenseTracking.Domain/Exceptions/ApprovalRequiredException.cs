namespace ExpenseTracking.Domain.Exceptions
{
    public class ApprovalRequiredException : BaseException
    {
        public int ExpenseId { get; private set; }
        public decimal Amount { get; private set; }
        public string RequiredDocument { get; private set; }
        
        public ApprovalRequiredException(int expenseId, decimal amount, string requiredDocument) 
            : base($"Expense {expenseId} with amount {amount} requires approval. Please provide: {requiredDocument}", "APPROVAL_REQUIRED")
        {
            ExpenseId = expenseId;
            Amount = amount;
            RequiredDocument = requiredDocument;
        }
        
        public ApprovalRequiredException(string message) 
            : base(message, "APPROVAL_REQUIRED")
        {
        }
    }
}