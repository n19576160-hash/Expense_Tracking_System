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
    public class BudgetController : ControllerBase
    {
        private readonly IBudgetService _budgetService;
        
        public BudgetController(IBudgetService budgetService)
        {
            _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
        }
        
        /// <summary>
        /// Create a new budget
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateBudget([FromBody] CreateBudgetDTO dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid budget data");
            
            var userId = GetCurrentUserId();
            dto.UserId = userId;
            
            var budget = await _budgetService.CreateBudgetAsync(dto);
            
            return Ok(new
            {
                success = true,
                message = "Budget created successfully",
                budget = new
                {
                    budget.BudgetId,
                    budget.Year,
                    budget.BudgetAmount,
                    budget.WarningThreshold,
                    budget.CriticalThreshold,
                    budget.EnforceHardLimit,
                    CategoryName = budget.Category?.CategoryName
                }
            });
        }
        
        /// <summary>
        /// Update an existing budget
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBudget(int id, [FromBody] UpdateBudgetDTO dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid budget data");
            
            var userId = GetCurrentUserId();
            dto.UserId = userId;
            
            var budget = await _budgetService.UpdateBudgetAsync(id, dto);
            
            return Ok(new
            {
                success = true,
                message = "Budget updated successfully",
                budget = new
                {
                    budget.BudgetId,
                    budget.BudgetAmount,
                    budget.WarningThreshold,
                    budget.CriticalThreshold,
                    budget.EnforceHardLimit
                }
            });
        }
        
        /// <summary>
        /// Get all budgets for a specific year
        /// </summary>
        [HttpGet("year/{year}")]
        public async Task<IActionResult> GetBudgetsByYear(int year)
        {
            if (year < 2000 || year > 2100)
                throw new ValidationException(nameof(year), "Invalid year");
            
            var userId = GetCurrentUserId();
            var budgets = await _budgetService.GetBudgetsByYearAsync(userId, year);
            
            return Ok(budgets.Select(b => new
            {
                b.BudgetId,
                b.Year,
                b.CategoryId,
                CategoryName = b.Category?.CategoryName ?? "Overall Budget",
                b.BudgetAmount,
                b.WarningThreshold,
                b.CriticalThreshold,
                b.EnforceHardLimit
            }));
        }
        
        /// <summary>
        /// Get budget performance details
        /// </summary>
        [HttpGet("{id}/performance")]
        public async Task<IActionResult> GetBudgetPerformance(int id)
        {
            var performance = await _budgetService.GetBudgetPerformanceAsync(id);
            
            if (performance == null)
                throw new EntityNotFoundException("Budget", id);
            
            return Ok(performance);
        }
        
        /// <summary>
        /// Check budget before adding expense
        /// </summary>
        [HttpPost("check-before-expense")]
        public async Task<IActionResult> CheckBudgetBeforeExpense([FromBody] BudgetCheckRequestDTO request)
        {
            var userId = GetCurrentUserId();
            
            var result = await _budgetService.CheckBudgetBeforeExpenseAsync(
                userId, 
                request.CategoryId, 
                request.Amount, 
                request.ExpenseDate, 
                request.GroupId);
            
            return Ok(result);
        }
        
        /// <summary>
        /// Delete a budget
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBudget(int id)
        {
            var userId = GetCurrentUserId();
            await _budgetService.DeleteBudgetAsync(id, userId);
            
            return Ok(new
            {
                success = true,
                message = "Budget deleted successfully"
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