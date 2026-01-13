namespace ExpenseTracking.Domain.Exceptions
{
    public class BudgetExceededException : BaseException
    {
        public int BudgetId { get; private set; }
        public decimal BudgetAmount { get; private set; }
        public decimal CurrentSpent { get; private set; }
        public decimal AttemptedAmount { get; private set; }
        public decimal OverageAmount { get; private set; }
        
        public BudgetExceededException(int budgetId, decimal budgetAmount, decimal currentSpent, decimal attemptedAmount) 
            : base($"Budget exceeded. Budget: {budgetAmount}, Current: {currentSpent}, Attempted: {attemptedAmount}", "BUDGET_EXCEEDED")
        {
            BudgetId = budgetId;
            BudgetAmount = budgetAmount;
            CurrentSpent = currentSpent;
            AttemptedAmount = attemptedAmount;
            OverageAmount = (currentSpent + attemptedAmount) - budgetAmount;
        }
    }
}