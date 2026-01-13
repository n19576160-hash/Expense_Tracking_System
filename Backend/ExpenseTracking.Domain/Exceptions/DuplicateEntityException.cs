namespace ExpenseTracking.Domain.Exceptions
{
    public class DuplicateEntityException : BaseException
    {
        public string EntityType { get; private set; }
        public string DuplicateField { get; private set; }
        public string DuplicateValue { get; private set; }
        
        public DuplicateEntityException(string entityType, string duplicateField, string duplicateValue) 
            : base($"{entityType} with {duplicateField} '{duplicateValue}' already exists", "DUPLICATE_ENTITY")
        {
            EntityType = entityType;
            DuplicateField = duplicateField;
            DuplicateValue = duplicateValue;
        }
    }
}