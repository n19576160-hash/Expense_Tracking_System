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
    public class ReportController : ControllerBase
    {
        private readonly IReportService _reportService;
        
        public ReportController(IReportService reportService)
        {
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
        }
        
        /// <summary>
        /// Get yearly expense report
        /// </summary>
        [HttpGet("yearly/{year}")]
        public async Task<IActionResult> GetYearlyReport(int year)
        {
            if (year < 2000 || year > 2100)
                throw new ValidationException(nameof(year), "Invalid year");
            
            var userId = GetCurrentUserId();
            var report = await _reportService.GenerateYearlyReportAsync(userId, year);
            
            return Ok(report);
        }
        
        /// <summary>
        /// Get category-wise expense report
        /// </summary>
        [HttpGet("category/{categoryId}/year/{year}")]
        public async Task<IActionResult> GetCategoryReport(int categoryId, int year)
        {
            var userId = GetCurrentUserId();
            var report = await _reportService.GenerateCategoryReportAsync(userId, categoryId, year);
            
            return Ok(report);
        }
        
        /// <summary>
        /// Get budget performance report
        /// </summary>
        [HttpGet("budget-performance/year/{year}")]
        public async Task<IActionResult> GetBudgetPerformanceReport(int year)
        {
            var userId = GetCurrentUserId();
            var report = await _reportService.GenerateBudgetPerformanceReportAsync(userId, year);
            
            return Ok(report);
        }
        
        /// <summary>
        /// Export report to PDF or CSV
        /// </summary>
        [HttpPost("export")]
        public async Task<IActionResult> ExportReport([FromBody] ExportRequestDTO request)
        {
            var userId = GetCurrentUserId();
            var fileBytes = await _reportService.ExportReportAsync(userId, request);
            
            var contentType = request.Format.ToUpper() == "PDF" 
                ? "application/pdf" 
                : "text/csv";
            
            var fileName = $"expense_report_{DateTime.Now:yyyyMMdd}.{request.Format.ToLower()}";
            
            return File(fileBytes, contentType, fileName);
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