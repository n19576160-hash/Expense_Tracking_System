using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExpenseTracking.Domain.Entities
{
    public class NotificationPreference : BaseEntity
    {
        [Key]
        public int PreferenceId { get; private set; }
        
        [Required]
        public int UserId { get; private set; }
        
        public bool EnableInAppNotifications { get; private set; }
        
        public bool EnableEmailNotifications { get; private set; }
        
        public bool EnableSMSNotifications { get; private set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal NotifyAtThreshold50 { get; private set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal NotifyAtThreshold80 { get; private set; }
        
        [Column(TypeName = "decimal(5,2)")]
        public decimal NotifyAtThreshold100 { get; private set; }
        
        // Navigation Property (one-to-one)
        public virtual User User { get; private set; }
        
        private NotificationPreference() { }
        
        public NotificationPreference(int userId)
        {
            UserId = userId;
            EnableInAppNotifications = true;
            EnableEmailNotifications = false;
            EnableSMSNotifications = false;
            NotifyAtThreshold50 = 50.00m;
            NotifyAtThreshold80 = 80.00m;
            NotifyAtThreshold100 = 100.00m;
        }
        
        public void UpdatePreferences(bool inApp, bool email, bool sms)
        {
            EnableInAppNotifications = inApp;
            EnableEmailNotifications = email;
            EnableSMSNotifications = sms;
            Update();
        }
        
        public void UpdateThresholds(decimal threshold50, decimal threshold80, decimal threshold100)
        {
            if (threshold50 <= 0 || threshold50 > 100)
                throw new ArgumentException("Threshold must be between 0 and 100", nameof(threshold50));
            
            if (threshold80 <= 0 || threshold80 > 100)
                throw new ArgumentException("Threshold must be between 0 and 100", nameof(threshold80));
            
            if (threshold100 <= 0 || threshold100 > 150)
                throw new ArgumentException("Threshold must be between 0 and 150", nameof(threshold100));
            
            NotifyAtThreshold50 = threshold50;
            NotifyAtThreshold80 = threshold80;
            NotifyAtThreshold100 = threshold100;
            Update();
        }
        
        public bool ShouldNotifyAt(decimal percentage)
        {
            return percentage >= NotifyAtThreshold50 || 
                   percentage >= NotifyAtThreshold80 || 
                   percentage >= NotifyAtThreshold100;
        }
    }
}