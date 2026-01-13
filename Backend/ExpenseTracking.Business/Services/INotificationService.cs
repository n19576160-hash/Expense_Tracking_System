using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public partial interface INotificationService
    {
        // Send Budget Alert
        Task SendBudgetAlertAsync(int userId, int categoryId, decimal percentageUsed);
        
        // Get Unread Alerts 
        Task<List<BudgetAlert>> GetUnreadAlertsAsync(int userId);
        
        // Mark Alert as Read
        Task MarkAlertAsReadAsync(int alertId);
        Task<NotificationPreference> GetPreferencesAsync(int userId);
        Task SavePreferencesAsync(NotificationPreferenceDTO dto);
    }
}