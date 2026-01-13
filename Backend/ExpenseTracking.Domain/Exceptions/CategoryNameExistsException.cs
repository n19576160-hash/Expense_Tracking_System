namespace ExpenseTracking.Domain.Exceptions
{
    public class CategoryNameExistsException : DuplicateEntityException
    {
        public CategoryNameExistsException(string categoryName) 
            : base("Category", "CategoryName", categoryName)
        {
        }
    }
}