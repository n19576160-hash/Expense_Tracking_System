namespace ExpenseTracking.Domain.Exceptions
{
    public class EmailAlreadyExistsException : DuplicateEntityException
    {
        public EmailAlreadyExistsException(string email) 
            : base("User", "Email", email)
        {
        }
    }
}