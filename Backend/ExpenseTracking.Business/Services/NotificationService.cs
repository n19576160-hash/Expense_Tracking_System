
using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;
using ExpenseTracking.Data.Repositories;
using ExpenseTracking.Business.DTOs;
using ExpenseTracking.Data;

namespace ExpenseTracking.Business.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IBudgetRepository _budgetRepo;
        private readonly INotificationRepository _notificationRepo;
        private readonly ExpenseTrackingDbContext _context;
        
        public NotificationService(
            IBudgetRepository budgetRepo, 
            INotificationRepository notificationRepo,
            ExpenseTrackingDbContext context)
        {
            _budgetRepo = budgetRepo ?? throw new ArgumentNullException(nameof(budgetRepo));
            _notificationRepo = notificationRepo ?? throw new ArgumentNullException(nameof(notificationRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task SendBudgetAlertAsync(int userId, int categoryId, decimal percentageUsed)
        {
            var preferences = await _notificationRepo.GetPreferencesAsync(userId);
            
            if (preferences == null || !preferences.EnableInAppNotifications)
                return;
            
            var budget = await _budgetRepo.GetBudgetAsync(userId, categoryId, DateTime.Now.Year, null);
            if (budget == null)
                return;
            
            var message = $"Budget alert: You have used {percentageUsed:F2}% of your budget for this category.";
            
            var alert = new BudgetAlert(budget.BudgetId, userId, percentageUsed, message);
            
            await _context.BudgetAlerts.AddAsync(alert);
            await _context.SaveChangesAsync();
            
            // Send email/SMS if enabled
            if (preferences.EnableEmailNotifications)
            {
                // Email sending logic here
                // await _emailService.SendEmailAsync(...);
            }
            
            if (preferences.EnableSMSNotifications)
            {
                // SMS sending logic here
                // await _smsService.SendSMSAsync(...);
            }
        }
        
        public async Task<List<BudgetAlert>> GetUnreadAlertsAsync(int userId)
        {
            return await _notificationRepo.GetUnreadAlertsAsync(userId);
        }
        
        public async Task MarkAlertAsReadAsync(int alertId)
        {
            await _notificationRepo.MarkAsReadAsync(alertId);
        }
        
        public async Task<NotificationPreference> GetPreferencesAsync(int userId)
        {
            return await _notificationRepo.GetPreferencesAsync(userId);
        }
        
        public async Task SavePreferencesAsync(NotificationPreferenceDTO dto)
        {
            var preference = await _notificationRepo.GetPreferencesAsync(dto.UserId);
            
            if (preference == null)
            {
                // Create new preferences
                preference = new NotificationPreference(dto.UserId);
            }
            
            // Update preferences
            preference.UpdatePreferences(
                dto.EnableInAppNotifications,
                dto.EnableEmailNotifications,
                dto.EnableSMSNotifications
            );
            
            preference.UpdateThresholds(
                dto.NotifyAtThreshold50,
                dto.NotifyAtThreshold80,
                dto.NotifyAtThreshold100
            );
            
            await _notificationRepo.SavePreferencesAsync(preference);
        }
    }
}
