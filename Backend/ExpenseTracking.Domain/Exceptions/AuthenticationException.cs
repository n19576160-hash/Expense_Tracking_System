namespace ExpenseTracking.Domain.Exceptions
{
    public class AuthenticationException : BaseException
    {
        public string AttemptedEmail { get; private set; }
        
        public AuthenticationException(string message) 
            : base(message, "AUTHENTICATION_FAILED")
        {
        }
        
        public AuthenticationException(string attemptedEmail, string message) 
            : base(message, "AUTHENTICATION_FAILED")
        {
            AttemptedEmail = attemptedEmail;
        }
    }
}
