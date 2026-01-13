namespace ExpenseTracking.Domain.Exceptions
{
    public class UnauthorizedAccessException : BaseException
    {
        public int UserId { get; private set; }
        public string Resource { get; private set; }
        public string Action { get; private set; }
        
        public UnauthorizedAccessException(int userId, string resource, string action) 
            : base($"User {userId} is not authorized to {action} {resource}", "UNAUTHORIZED_ACCESS")
        {
            UserId = userId;
            Resource = resource;
            Action = action;
        }
        
        public UnauthorizedAccessException(string message) 
            : base(message, "UNAUTHORIZED_ACCESS")
        {
        }
    }
}