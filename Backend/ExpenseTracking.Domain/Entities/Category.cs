using System.ComponentModel.DataAnnotations;

namespace ExpenseTracking.Domain.Entities
{
    public class Category : BaseEntity
    {
        [Key]
        public int CategoryId { get; private set; }
        
        [Required, MaxLength(50)]
        public string CategoryName { get; private set; }
        
        [MaxLength(200)]
        public string Description { get; private set; }
        
        public bool IsDefault { get; private set; }
        
        public bool IsActive { get; private set; }
        
        public int? CreatedByUserId { get; private set; }
        
        // Navigation Properties
        public virtual User CreatedBy { get; private set; }
        public virtual ICollection<Expense> Expenses { get; private set; }
        public virtual ICollection<Budget> Budgets { get; private set; }
        
        private Category()
        {
            Expenses = new List<Expense>();
            Budgets = new List<Budget>();
        }
        
        public Category(string categoryName, string description = null, bool isDefault = false, int? createdByUserId = null) : this()
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be empty", nameof(categoryName));
            
            CategoryName = categoryName;
            Description = description;
            IsDefault = isDefault;
            IsActive = true;
            CreatedByUserId = createdByUserId;
        }
        
        public void UpdateDetails(string categoryName, string description)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
                throw new ArgumentException("Category name cannot be empty", nameof(categoryName));
            
            CategoryName = categoryName;
            Description = description;
            Update();
        }
        
        public void Deactivate()
        {
            if (Expenses.Any())
                throw new InvalidOperationException("Cannot deactivate category with existing expenses");
            
            IsActive = false;
            Update();
        }
        
        public void Activate()
        {
            IsActive = true;
            Update();
        }
        
        public bool HasExpenses()
        {
            return Expenses != null && Expenses.Any();
        }
    }
}
