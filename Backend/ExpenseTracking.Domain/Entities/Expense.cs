using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracking.Domain.Entities
{
    public class Expense : BaseEntity
    {
        [Key]
        public int ExpenseId { get; private set; }
        
        [Required]
        public int UserId { get; private set; }
        
        [Required]
        public int CategoryId { get; private set; }
        
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; private set; }
        
        [Required]
        public DateTime ExpenseDate { get; private set; }
        
        [MaxLength(500)]
        public string Description { get; private set; }
        
        public bool IsOverBudget { get; private set; }
        
        public bool RequiresApproval { get; private set; }
        
        public bool IsApproved { get; private set; }
        
        [MaxLength(1000)]
        public string ApprovalNote { get; private set; }
        
        [MaxLength(500)]
        public string ApprovalDocumentPath { get; private set; }
        
        public int? GroupId { get; private set; }
        
        // Navigation Properties (many-to-one)
        public virtual User User { get; private set; }
        public virtual Category Category { get; private set; }
        public virtual ExpenseGroup Group { get; private set; }
        
        private Expense() { }
        
        public Expense(int userId, int categoryId, decimal amount, DateTime expenseDate, string description = null, int? groupId = null)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));
            
            if (expenseDate > DateTime.Today)
                throw new ArgumentException("Expense date cannot be in the future", nameof(expenseDate));
            
            UserId = userId;
            CategoryId = categoryId;
            Amount = amount;
            ExpenseDate = expenseDate;
            Description = description;
            GroupId = groupId;
            IsApproved = true; // Default approved
        }
        
        public void UpdateDetails(decimal amount, int categoryId, DateTime expenseDate, string description)
        {
            if (amount <= 0)
                throw new ArgumentException("Amount must be positive", nameof(amount));
            
            if (expenseDate > DateTime.Today)
                throw new ArgumentException("Expense date cannot be in the future", nameof(expenseDate));
            
            if (expenseDate.Year == DateTime.Now.Year)
                throw new InvalidOperationException("Cannot edit current year expenses without approval");
            
            Amount = amount;
            CategoryId = categoryId;
            ExpenseDate = expenseDate;
            Description = description;
            Update();
        }
        
        public void MarkAsOverBudget()
        {
            IsOverBudget = true;
            Update();
        }
        
        public void SetRequiresApproval(bool requires)
        {
            RequiresApproval = requires;
            if (requires)
                IsApproved = false;
            Update();
        }
        
        public void Approve(string approvalNote, string documentPath)
        {
            if (!RequiresApproval)
                throw new InvalidOperationException("This expense does not require approval");
            
            if (string.IsNullOrWhiteSpace(approvalNote))
                throw new ArgumentException("Approval note is required", nameof(approvalNote));
            
            IsApproved = true;
            ApprovalNote = approvalNote;
            ApprovalDocumentPath = documentPath;
            Update();
        }
        
        public void Reject(string rejectionNote)
        {
            if (!RequiresApproval)
                throw new InvalidOperationException("This expense does not require approval");
            
            IsApproved = false;
            ApprovalNote = rejectionNote;
            Update();
        }
        
        public bool CanBeDeleted()
        {
            return ExpenseDate.Year != DateTime.Now.Year;
        }
    }
}
