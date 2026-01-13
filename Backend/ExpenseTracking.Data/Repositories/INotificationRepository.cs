
using ExpenseTracking.Domain.Entities;

namespace ExpenseTracking.Data.Repositories
{
    public interface INotificationRepository
    {
        Task<NotificationPreference> GetPreferencesAsync(int userId);
        Task SavePreferencesAsync(NotificationPreference preferences);
        Task AddAlertAsync(BudgetAlert alert);
        Task<List<BudgetAlert>> GetUnreadAlertsAsync(int userId);
        Task<List<BudgetAlert>> GetAlertsByUserAsync(int userId, int pageSize = 50);
        Task MarkAsReadAsync(int alertId);
    }
}