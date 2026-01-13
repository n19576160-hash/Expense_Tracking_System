using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public interface IBudgetService
    {
        Task<Budget> CreateBudgetAsync(CreateBudgetDTO dto);
        Task<Budget> UpdateBudgetAsync(int budgetId, UpdateBudgetDTO dto);
        Task<IEnumerable<Budget>> GetBudgetsByYearAsync(int userId, int year);
        Task<BudgetCheckResultDTO> CheckBudgetBeforeExpenseAsync(int userId, int categoryId, decimal amount, DateTime expenseDate, int? groupId);
        Task CreateBudgetAlertAsync(int budgetId, int userId, decimal percentageUsed);
        Task DeleteBudgetAsync(int budgetId, int userId);
        Task<BudgetPerformanceDTO> GetBudgetPerformanceAsync(int budgetId);
    }
}

