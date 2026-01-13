using System.ComponentModel.DataAnnotations;

namespace ExpenseTracking.Domain.Entities
{
    public class UserGroup : BaseEntity
    {
        [Key]
        public int UserGroupId { get; private set; }
        
        [Required]
        public int UserId { get; private set; }
        
        [Required]
        public int GroupId { get; private set; }
        
        [Required, MaxLength(20)]
        public string Role { get; private set; }
        
        public DateTime JoinedDate { get; private set; }
        
        public bool IsActive { get; private set; }
        
        // Navigation Properties (many-to-one)
        public virtual User User { get; private set; }
        public virtual ExpenseGroup Group { get; private set; }
        
        private UserGroup() { }
        
        public UserGroup(int userId, int groupId, string role)
        {
            if (role != "Owner" && role != "Member")
                throw new ArgumentException("Role must be either 'Owner' or 'Member'", nameof(role));
            
            UserId = userId;
            GroupId = groupId;
            Role = role;
            JoinedDate = DateTime.Now;
            IsActive = true;
        }
        
        public void ChangeRole(string newRole)
        {
            if (newRole != "Owner" && newRole != "Member")
                throw new ArgumentException("Role must be either 'Owner' or 'Member'", nameof(newRole));
            
            Role = newRole;
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
        
        public bool IsOwner()
        {
            return Role == "Owner";
        }
        
        public bool IsMember()
        {
            return Role == "Member";
        }
    }
}
