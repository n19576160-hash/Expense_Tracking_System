namespace ExpenseTracking.Domain.Exceptions
{
    public class ValidationException : BaseException
    {
        public Dictionary<string, string> ValidationErrors { get; private set; }
        
        public ValidationException(string message) 
            : base(message, "VALIDATION_ERROR")
        {
            ValidationErrors = new Dictionary<string, string>();
        }
        
        public ValidationException(string fieldName, string errorMessage) 
            : base($"Validation failed for {fieldName}", "VALIDATION_ERROR")
        {
            ValidationErrors = new Dictionary<string, string>
            {
                { fieldName, errorMessage }
            };
        }
        
        public ValidationException(Dictionary<string, string> errors) 
            : base("Multiple validation errors occurred", "VALIDATION_ERROR")
        {
            ValidationErrors = errors ?? new Dictionary<string, string>();
        }
        
        public void AddError(string fieldName, string errorMessage)
        {
            if (!ValidationErrors.ContainsKey(fieldName))
            {
                ValidationErrors.Add(fieldName, errorMessage);
            }
        }
    }
}