namespace ExpenseTracking.Domain.Exceptions
{
    public class CategoryDeletionException : BaseException
    {
        public int CategoryId { get; private set; }
        public string CategoryName { get; private set; }
        public int ExpenseCount { get; private set; }
        
        public CategoryDeletionException(int categoryId, string categoryName, int expenseCount) 
            : base($"Cannot delete category '{categoryName}' with {expenseCount} existing expenses. Please deactivate instead.", "CATEGORY_DELETION_RESTRICTED")
        {
            CategoryId = categoryId;
            CategoryName = categoryName;
            ExpenseCount = expenseCount;
        }
    }
}