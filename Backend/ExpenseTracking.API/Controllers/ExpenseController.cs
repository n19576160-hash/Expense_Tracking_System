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
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;
        
        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService ?? throw new ArgumentNullException(nameof(expenseService));
        }
        
        /// <summary>
        /// Add a new expense
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AddExpense([FromBody] CreateExpenseDTO dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid expense data");
            
            var userId = GetCurrentUserId();
            dto.UserId = userId;
            
            var expense = await _expenseService.AddExpenseAsync(dto);
            
            return Ok(new
            {
                success = true,
                message = expense.IsOverBudget 
                    ? "Expense added but budget exceeded!" 
                    : "Expense added successfully",
                expense = new
                {
                    expense.ExpenseId,
                    expense.Amount,
                    expense.ExpenseDate,
                    expense.Description,
                    expense.IsOverBudget,
                    expense.RequiresApproval,
                    CategoryName = expense.Category?.CategoryName
                },
                isWarning = expense.IsOverBudget
            });
        }
        
        /// <summary>
        /// Update an existing expense
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(int id, [FromBody] UpdateExpenseDTO dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid expense data");
            
            var userId = GetCurrentUserId();
            dto.UserId = userId;
            
            var expense = await _expenseService.UpdateExpenseAsync(id, dto);
            
            return Ok(new
            {
                success = true,
                message = "Expense updated successfully",
                expense = new
                {
                    expense.ExpenseId,
                    expense.Amount,
                    expense.ExpenseDate,
                    expense.Description,
                    CategoryName = expense.Category?.CategoryName
                }
            });
        }
        
        /// <summary>
        /// Delete an expense
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(int id)
        {
            var userId = GetCurrentUserId();
            await _expenseService.DeleteExpenseAsync(id, userId);
            
            return Ok(new
            {
                success = true,
                message = "Expense deleted successfully"
            });
        }
        
        /// <summary>
        /// Get expenses by date range
        /// </summary>
        [HttpGet("date-range")]
        public async Task<IActionResult> GetExpensesByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate)
        {
            var userId = GetCurrentUserId();
            var expenses = await _expenseService.GetExpensesByDateRangeAsync(userId, startDate, endDate);
            
            return Ok(expenses.Select(e => new
            {
                e.ExpenseId,
                e.Amount,
                e.ExpenseDate,
                e.Description,
                e.IsOverBudget,
                CategoryName = e.Category?.CategoryName,
                e.CreatedDate
            }));
        }
        
        /// <summary>
        /// Get expenses by category and year
        /// </summary>
        [HttpGet("category/{categoryId}/year/{year}")]
        public async Task<IActionResult> GetExpensesByCategory(int categoryId, int year)
        {
            var userId = GetCurrentUserId();
            var expenses = await _expenseService.GetExpensesByCategoryAsync(userId, categoryId, year);
            
            return Ok(expenses.Select(e => new
            {
                e.ExpenseId,
                e.Amount,
                e.ExpenseDate,
                e.Description,
                e.IsOverBudget,
                CategoryName = e.Category?.CategoryName
            }));
        }
        
        /// <summary>
        /// Get daily expense summary
        /// </summary>
        [HttpGet("summary/daily")]
        public async Task<IActionResult> GetDailySummary([FromQuery] DateTime date)
        {
            var userId = GetCurrentUserId();
            var summary = await _expenseService.GetDailySummaryAsync(userId, date);
            
            return Ok(summary);
        }
        
        /// <summary>
        /// Get monthly expense summary
        /// </summary>
        [HttpGet("summary/monthly")]
        public async Task<IActionResult> GetMonthlySummary([FromQuery] int year, [FromQuery] int month)
        {
            if (month < 1 || month > 12)
                throw new ValidationException(nameof(month), "Month must be between 1 and 12");
            
            var userId = GetCurrentUserId();
            var summary = await _expenseService.GetMonthlySummaryAsync(userId, year, month);
            
            return Ok(summary);
        }
        
        /// <summary>
        /// Check for duplicate expense
        /// </summary>
        [HttpPost("check-duplicate")]
        public async Task<IActionResult> CheckDuplicate([FromBody] CreateExpenseDTO dto)
        {
            var userId = GetCurrentUserId();
            dto.UserId = userId;
            
            var isDuplicate = await _expenseService.CheckDuplicateExpenseAsync(dto);
            
            return Ok(new
            {
                isDuplicate,
                message = isDuplicate 
                    ? "A similar expense already exists" 
                    : "No duplicate found"
            });
        }
        
        /// <summary>
        /// Approve an expense that requires approval
        /// </summary>
        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveExpense(
            int id, 
            [FromBody] ApproveExpenseDTO dto)
        {
            var expense = await _expenseService.ApproveExpenseAsync(id, dto.ApprovalNote, dto.DocumentPath);
            
            return Ok(new
            {
                success = true,
                message = "Expense approved successfully",
                expense
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