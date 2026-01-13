using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public interface IExpenseService
    {
        Task<Expense> AddExpenseAsync(CreateExpenseDTO dto);
        Task<Expense> UpdateExpenseAsync(int expenseId, UpdateExpenseDTO dto);
        Task DeleteExpenseAsync(int expenseId, int userId);
        Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(int userId, int categoryId, int year);
        Task<ExpenseSummaryDTO> GetMonthlySummaryAsync(int userId, int year, int month);
        Task<bool> CheckDuplicateExpenseAsync(CreateExpenseDTO dto);
        Task<Expense> ApproveExpenseAsync(int expenseId, string approvalNote, string documentPath);
        Task<ExpenseSummaryDTO> GetDailySummaryAsync(int userId, DateTime date);
    }
}
