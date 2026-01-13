namespace ExpenseTracking.Business.DTOs
{
    public class NotificationPreferenceDTO
    {
        public int UserId { get; set; }
        public bool EnableInAppNotifications { get; set; }
        public bool EnableEmailNotifications { get; set; }
        public bool EnableSMSNotifications { get; set; }
        public decimal NotifyAtThreshold50 { get; set; }
        public decimal NotifyAtThreshold80 { get; set; }
        public decimal NotifyAtThreshold100 { get; set; }
    }
}
