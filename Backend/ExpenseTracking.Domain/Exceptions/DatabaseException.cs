namespace ExpenseTracking.Domain.Exceptions
{
    public class DatabaseException : BaseException
    {
        public string Operation { get; private set; }
        
        public DatabaseException(string operation, string message) 
            : base($"Database error during {operation}: {message}", "DATABASE_ERROR")
        {
            Operation = operation;
        }
        
        public DatabaseException(string operation, string message, Exception innerException) 
            : base($"Database error during {operation}: {message}", "DATABASE_ERROR", innerException)
        {
            Operation = operation;
        }
    }
}