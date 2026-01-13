namespace ExpenseTracking.Domain.Exceptions
{
    public class BudgetAlreadyExistsException : DuplicateEntityException
    {
        public int Year { get; private set; }
        public int? CategoryId { get; private set; }
        
        public BudgetAlreadyExistsException(int year, int? categoryId) 
            : base("Budget", "Year-Category", $"{year}-{categoryId?.ToString() ?? "Overall"}")
        {
            Year = year;
            CategoryId = categoryId;
        }
    }
}