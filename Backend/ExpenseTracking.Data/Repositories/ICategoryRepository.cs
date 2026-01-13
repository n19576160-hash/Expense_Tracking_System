using ExpenseTracking.Domain.Entities;

namespace ExpenseTracking.Data.Repositories
{
    public interface ICategoryRepository : IRepository<Category>
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<Category> GetByNameAsync(string name);
        Task<bool> NameExistsAsync(string name, int? excludeId = null);
        Task<int> GetExpenseCountAsync(int categoryId);
    }
}
