using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracking.Domain.Entities
{
    public class User : BaseEntity
    {
        [Key]
        public int UserId { get; private set; }
        
        [Required, MaxLength(100)]
        public string Name { get; private set; }
        
        [Required, MaxLength(100)]
        public string Email { get; private set; }
        
        [MaxLength(20)]
        public string MobileNumber { get; private set; }
        
        [Required]
        public string PasswordHash { get; private set; }
        
        public DateTime? LastLoginDate { get; private set; }
        
        public bool IsActive { get; private set; }
        
        // Navigation Properties (one-to-many)
        public virtual ICollection<Expense> Expenses { get; private set; }
        public virtual ICollection<Budget> Budgets { get; private set; }
        public virtual ICollection<UserGroup> UserGroups { get; private set; }
        public virtual NotificationPreference NotificationPreference { get; private set; }
        
        // Private constructor for EF
        private User() 
        {
            Expenses = new List<Expense>();
            Budgets = new List<Budget>();
            UserGroups = new List<UserGroup>();
        }
        
        public User(string name, string email, string passwordHash, string mobileNumber = null) : this()
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));
            
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            MobileNumber = mobileNumber;
            IsActive = true;
        }
        
        public void UpdateProfile(string name, string mobileNumber)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            Name = name;
            MobileNumber = mobileNumber;
            Update();
        }
        
        public void ChangePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));
            
            PasswordHash = newPasswordHash;
            Update();
        }
        
        public void UpdateLastLogin()
        {
            LastLoginDate = DateTime.Now;
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