using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.Data.Repositories
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        public CategoryRepository(ExpenseTrackingDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            try
            {
                return await _dbSet
                    .Where(c => c.IsActive)
                    .OrderBy(c => c.CategoryName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetActiveCategories", ex.Message, ex);
            }
        }
        
        public async Task<Category> GetByNameAsync(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException(nameof(name), "Category name cannot be empty");
            
            try
            {
                return await _dbSet.FirstOrDefaultAsync(c => c.CategoryName == name);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetByName", ex.Message, ex);
            }
        }
        
        public async Task<bool> NameExistsAsync(string name, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ValidationException(nameof(name), "Category name cannot be empty");
            
            try
            {
                var query = _dbSet.Where(c => c.CategoryName == name);
                
                if (excludeId.HasValue)
                    query = query.Where(c => c.CategoryId != excludeId.Value);
                
                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("NameExists", ex.Message, ex);
            }
        }
        
        public async Task<int> GetExpenseCountAsync(int categoryId)
        {
            try
            {
                return await _context.Expenses.CountAsync(e => e.CategoryId == categoryId);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetExpenseCount", ex.Message, ex);
            }
        }
    }
}