using ExpenseTracking.Domain.Entities;

namespace ExpenseTracking.Data.Repositories
{
    public interface IBudgetRepository : IRepository<Budget>
    {
        Task<Budget> GetBudgetAsync(int userId, int? categoryId, int year, int? groupId);
        Task<IEnumerable<Budget>> GetBudgetsByYearAsync(int userId, int year);
        Task<BudgetAlert> GetLastAlertAsync(int budgetId);
        Task<bool> BudgetExistsAsync(int userId, int? categoryId, int year, int? groupId);
    }
}