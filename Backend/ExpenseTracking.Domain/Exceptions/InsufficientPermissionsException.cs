namespace ExpenseTracking.Domain.Exceptions
{
    public class InsufficientPermissionsException : BaseException
    {
        public int UserId { get; private set; }
        public string RequiredRole { get; private set; }
        public string CurrentRole { get; private set; }
        
        public InsufficientPermissionsException(int userId, string requiredRole, string currentRole) 
            : base($"User {userId} with role '{currentRole}' does not have permission. Required role: '{requiredRole}'", "INSUFFICIENT_PERMISSIONS")
        {
            UserId = userId;
            RequiredRole = requiredRole;
            CurrentRole = currentRole;
        }
    }
}