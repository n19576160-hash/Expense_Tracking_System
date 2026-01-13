using System.ComponentModel.DataAnnotations;

namespace ExpenseTracking.Domain.Entities
{
    public class AuditLog
    {
        [Key]
        public int AuditId { get; private set; }
        
        [Required]
        public int UserId { get; private set; }
        
        [Required, MaxLength(50)]
        public string Action { get; private set; }
        
        [Required, MaxLength(50)]
        public string EntityType { get; private set; }
        
        public int EntityId { get; private set; }
        
        [MaxLength(2000)]
        public string Changes { get; private set; }
        
        public DateTime ActionDate { get; private set; }
        
        [MaxLength(50)]
        public string IPAddress { get; private set; }
        
        // Navigation Property
        public virtual User User { get; private set; }
        
        private AuditLog() { }
        
        public AuditLog(int userId, string action, string entityType, int entityId, string changes = null, string ipAddress = null)
        {
            if (string.IsNullOrWhiteSpace(action))
                throw new ArgumentException("Action cannot be empty", nameof(action));
            
            if (string.IsNullOrWhiteSpace(entityType))
                throw new ArgumentException("Entity type cannot be empty", nameof(entityType));
            
            UserId = userId;
            Action = action;
            EntityType = entityType;
            EntityId = entityId;
            Changes = changes;
            IPAddress = ipAddress;
            ActionDate = DateTime.Now;
        }
    }
}