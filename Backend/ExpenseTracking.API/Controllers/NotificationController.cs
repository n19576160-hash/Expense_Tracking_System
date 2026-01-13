using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracking.Business.Services;
using ExpenseTracking.Business.DTOs;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService ?? throw new ArgumentNullException(nameof(notificationService));
        }
        
        /// <summary>
        /// Get all unread alerts
        /// </summary>
        [HttpGet("unread")]
        public async Task<IActionResult> GetUnreadAlerts()
        {
            var userId = GetCurrentUserId();
            var alerts = await _notificationService.GetUnreadAlertsAsync(userId);
            
            return Ok(alerts.Select(a => new
            {
                a.AlertId,
                a.Message,
                a.ThresholdLevel,
                a.AlertDate,
                a.IsRead,
                BudgetInfo = new
                {
                    a.Budget?.BudgetAmount,
                    CategoryName = a.Budget?.Category?.CategoryName
                }
            }));
        }
        
        /// <summary>
        /// Mark an alert as read
        /// </summary>
        [HttpPost("{alertId}/mark-read")]
        public async Task<IActionResult> MarkAsRead(int alertId)
        {
            await _notificationService.MarkAlertAsReadAsync(alertId);
            
            return Ok(new
            {
                success = true,
                message = "Alert marked as read"
            });
        }
        
        /// <summary>
        /// Get notification preferences
        /// </summary>
        [HttpGet("preferences")]
        public async Task<IActionResult> GetPreferences()
        {
            var userId = GetCurrentUserId();
            var preferences = await _notificationService.GetPreferencesAsync(userId);
            
            if (preferences == null)
            {
                // Return default preferences
                return Ok(new
                {
                    enableInAppNotifications = true,
                    enableEmailNotifications = false,
                    enableSMSNotifications = false,
                    notifyAtThreshold50 = 50.00m,
                    notifyAtThreshold80 = 80.00m,
                    notifyAtThreshold100 = 100.00m
                });
            }
            
            return Ok(new
            {
                preferences.EnableInAppNotifications,
                preferences.EnableEmailNotifications,
                preferences.EnableSMSNotifications,
                preferences.NotifyAtThreshold50,
                preferences.NotifyAtThreshold80,
                preferences.NotifyAtThreshold100
            });
        }
        
        /// <summary>
        /// Save notification preferences
        /// </summary>
        [HttpPost("preferences")]
        public async Task<IActionResult> SavePreferences([FromBody] NotificationPreferenceDTO dto)
        {
            var userId = GetCurrentUserId();
            dto.UserId = userId;
            
            await _notificationService.SavePreferencesAsync(dto);
            
            return Ok(new
            {
                success = true,
                message = "Preferences saved successfully"
            });
        }
        
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException("User ID not found in token");
            
            return int.Parse(userIdClaim);
        }
    }
}