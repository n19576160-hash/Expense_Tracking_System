using System.ComponentModel.DataAnnotations;

namespace ExpenseTracking.Domain.Entities
{
    public class ExpenseGroup : BaseEntity
    {
        [Key]
        public int GroupId { get; private set; }
        
        [Required, MaxLength(100)]
        public string GroupName { get; private set; }
        
        [MaxLength(500)]
        public string Description { get; private set; }
        
        [Required]
        public int OwnerId { get; private set; }
        
        public bool IsActive { get; private set; }
        
        public bool RequireOwnerApprovalForOverBudget { get; private set; }
        
        // Navigation Properties (one-to-many)
        public virtual User Owner { get; private set; }
        public virtual ICollection<UserGroup> UserGroups { get; private set; }
        public virtual ICollection<Expense> Expenses { get; private set; }
        public virtual ICollection<Budget> Budgets { get; private set; }
        
        private ExpenseGroup()
        {
            UserGroups = new List<UserGroup>();
            Expenses = new List<Expense>();
            Budgets = new List<Budget>();
        }
        
        public ExpenseGroup(string groupName, int ownerId, string description = null) : this()
        {
            if (string.IsNullOrWhiteSpace(groupName))
                throw new ArgumentException("Group name cannot be empty", nameof(groupName));
            
            GroupName = groupName;
            OwnerId = ownerId;
            Description = description;
            IsActive = true;
            RequireOwnerApprovalForOverBudget = true;
        }
        
        public void UpdateDetails(string groupName, string description)
        {
            if (string.IsNullOrWhiteSpace(groupName))
                throw new ArgumentException("Group name cannot be empty", nameof(groupName));
            
            GroupName = groupName;
            Description = description;
            Update();
        }
        
        public void EnableApprovalRequirement()
        {
            RequireOwnerApprovalForOverBudget = true;
            Update();
        }
        
        public void DisableApprovalRequirement()
        {
            RequireOwnerApprovalForOverBudget = false;
            Update();
        }
        
        public void Deactivate()
        {
            IsActive = false;
            Update();
        }
        
        public void Activate()
        {
            IsActive = true;
            Update();
        }
    }
}