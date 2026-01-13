namespace ExpenseTracking.Domain.Exceptions
{
    public class ExpenseEditRestrictionException : BaseException
    {
        public int ExpenseId { get; private set; }
        public DateTime ExpenseDate { get; private set; }
        public string Restriction { get; private set; }
        
        public ExpenseEditRestrictionException(int expenseId, DateTime expenseDate, string restriction) 
            : base($"Cannot edit expense {expenseId} from {expenseDate:yyyy-MM-dd}. Reason: {restriction}", "EXPENSE_EDIT_RESTRICTED")
        {
            ExpenseId = expenseId;
            ExpenseDate = expenseDate;
            Restriction = restriction;
        }
    }
}
