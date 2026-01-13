
using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.Data.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly ExpenseTrackingDbContext _context;
        
        public NotificationRepository(ExpenseTrackingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<NotificationPreference> GetPreferencesAsync(int userId)
        {
            try
            {
                return await _context.NotificationPreferences
                    .FirstOrDefaultAsync(p => p.UserId == userId);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetPreferences", ex.Message, ex);
            }
        }
        
        public async Task SavePreferencesAsync(NotificationPreference preferences)
        {
            if (preferences == null)
                throw new ArgumentNullException(nameof(preferences));
            
            try
            {
                var existing = await GetPreferencesAsync(preferences.UserId);
                
                if (existing == null)
                {
                    await _context.NotificationPreferences.AddAsync(preferences);
                }
                else
                {
                    _context.NotificationPreferences.Update(preferences);
                }
                
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("SavePreferences", ex.Message, ex);
            }
        }
        
        public async Task AddAlertAsync(BudgetAlert alert)
        {
            if (alert == null)
                throw new ArgumentNullException(nameof(alert));
            
            try
            {
                await _context.BudgetAlerts.AddAsync(alert);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("AddAlert", ex.Message, ex);
            }
        }
        
        public async Task<List<BudgetAlert>> GetUnreadAlertsAsync(int userId)
        {
            try
            {
                return await _context.BudgetAlerts
                    .Include(a => a.Budget)
                        .ThenInclude(b => b.Category)
                    .Where(a => a.UserId == userId && !a.IsRead)
                    .OrderByDescending(a => a.AlertDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetUnreadAlerts", ex.Message, ex);
            }
        }
        
        public async Task<List<BudgetAlert>> GetAlertsByUserAsync(int userId, int pageSize = 50)
        {
            try
            {
                return await _context.BudgetAlerts
                    .Include(a => a.Budget)
                        .ThenInclude(b => b.Category)
                    .Where(a => a.UserId == userId)
                    .OrderByDescending(a => a.AlertDate)
                    .Take(pageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetAlertsByUser", ex.Message, ex);
            }
        }
        
        public async Task MarkAsReadAsync(int alertId)
        {
            try
            {
                var alert = await _context.BudgetAlerts.FindAsync(alertId);
                if (alert != null)
                {
                    alert.MarkAsRead();
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                throw new DatabaseException("MarkAsRead", ex.Message, ex);
            }
        }
    }
}
