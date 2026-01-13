using ExpenseTracking.Domain.Entities;

namespace ExpenseTracking.Data.Repositories
{
    public interface IExpenseRepository : IRepository<Expense>
    {
        Task<IEnumerable<Expense>> GetByUserAsync(int userId);
        Task<IEnumerable<Expense>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Expense>> GetByCategoryAsync(int userId, int categoryId, int year);
        Task<IEnumerable<Expense>> GetByMonthAsync(int userId, int year, int month);
        Task<IEnumerable<Expense>> GetByDateAsync(int userId, DateTime date); 
        Task<decimal> GetTotalSpentAsync(int userId, int? categoryId, int year, int? groupId);
        Task<bool> CheckDuplicateAsync(int userId, DateTime date, decimal amount, int categoryId, string description);
    }
}