namespace ExpenseTracking.Domain.Exceptions
{
    public class EntityNotFoundException : BaseException
    {
        public string EntityType { get; private set; }
        public int EntityId { get; private set; }
        
        public EntityNotFoundException(string entityType, int entityId) 
            : base($"{entityType} with ID {entityId} was not found", "ENTITY_NOT_FOUND")
        {
            EntityType = entityType;
            EntityId = entityId;
        }
        
        public EntityNotFoundException(string entityType, int entityId, string additionalInfo) 
            : base($"{entityType} with ID {entityId} was not found. {additionalInfo}", "ENTITY_NOT_FOUND")
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }
}