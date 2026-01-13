using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracking.Domain.Entities
{
    public class BudgetAlert : BaseEntity
    {
        [Key]
        public int AlertId { get; private set; }
        
        [Required]
        public int BudgetId { get; private set; }
        
        [Required]
        public int UserId { get; private set; }
        
        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal ThresholdLevel { get; private set; }
        
        [Required]
        public DateTime AlertDate { get; private set; }
        
        public bool IsRead { get; private set; }
        
        [Required, MaxLength(500)]
        public string Message { get; private set; }
        
        // Navigation Properties
        public virtual Budget Budget { get; private set; }
        public virtual User User { get; private set; }
        
        private BudgetAlert() { }
        
        public BudgetAlert(int budgetId, int userId, decimal thresholdLevel, string message)
        {
            if (thresholdLevel <= 0)
                throw new ArgumentException("Threshold level must be positive", nameof(thresholdLevel));
            
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be empty", nameof(message));
            
            BudgetId = budgetId;
            UserId = userId;
            ThresholdLevel = thresholdLevel;
            Message = message;
            AlertDate = DateTime.Now;
            IsRead = false;
        }
        
        public void MarkAsRead()
        {
            IsRead = true;
            Update();
        }
        
        public void MarkAsUnread()
        {
            IsRead = false;
            Update();
        }
    }
}