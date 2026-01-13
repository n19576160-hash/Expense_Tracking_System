using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracking.Domain.Entities
{
    public class Budget : BaseEntity
    {
        [Key]
        public int BudgetId { get; private set; }
        
        [Required]
        public int UserId { get; private set; }
        
        public int? CategoryId { get; private set; }
        
        [Required]
        public int Year { get; private set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BudgetAmount { get; private set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal WarningThreshold { get; private set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal CriticalThreshold { get; private set; }
        
        public bool EnforceHardLimit { get; private set; }
        
        public int? GroupId { get; private set; }
        
        // Navigation Properties
        public virtual User User { get; private set; }
        public virtual Category Category { get; private set; }
        public virtual ExpenseGroup Group { get; private set; }
        public virtual ICollection<BudgetAlert> BudgetAlerts { get; private set; }
        
        private Budget()
        {
            BudgetAlerts = new List<BudgetAlert>();
        }
        
        public Budget(int userId, int year, decimal budgetAmount, int? categoryId = null, int? groupId = null) : this()
        {
            if (budgetAmount <= 0)
                throw new ArgumentException("Budget amount must be positive", nameof(budgetAmount));
            
            if (year < 2000 || year > 2100)
                throw new ArgumentException("Invalid year", nameof(year));
            
            UserId = userId;
            Year = year;
            BudgetAmount = budgetAmount;
            CategoryId = categoryId;
            GroupId = groupId;
            WarningThreshold = 80.00m;
            CriticalThreshold = 100.00m;
            EnforceHardLimit = false;
        }
        
        public void UpdateAmount(decimal newAmount)
        {
            if (newAmount <= 0)
                throw new ArgumentException("Budget amount must be positive", nameof(newAmount));
            
            BudgetAmount = newAmount;
            Update();
        }
        
        public void SetThresholds(decimal warningThreshold, decimal criticalThreshold)
        {
            if (warningThreshold <= 0 || warningThreshold > 100)
                throw new ArgumentException("Warning threshold must be between 0 and 100", nameof(warningThreshold));
            
            if (criticalThreshold <= 0 || criticalThreshold > 150)
                throw new ArgumentException("Critical threshold must be between 0 and 150", nameof(criticalThreshold));
            
            if (warningThreshold >= criticalThreshold)
                throw new ArgumentException("Warning threshold must be less than critical threshold");
            
            WarningThreshold = warningThreshold;
            CriticalThreshold = criticalThreshold;
            Update();
        }
        
        public void EnableHardLimit()
        {
            EnforceHardLimit = true;
            Update();
        }
        
        public void DisableHardLimit()
        {
            EnforceHardLimit = false;
            Update();
        }
        
        public bool IsOverThreshold(decimal spentAmount, out decimal percentageUsed)
        {
            percentageUsed = (spentAmount / BudgetAmount) * 100;
            return percentageUsed >= CriticalThreshold;
        }
        
        public bool IsInWarningZone(decimal spentAmount)
        {
            decimal percentageUsed = (spentAmount / BudgetAmount) * 100;
            return percentageUsed >= WarningThreshold && percentageUsed < CriticalThreshold;
        }
    }
}
