using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.Data.Repositories
{
    public class BudgetRepository : Repository<Budget>, IBudgetRepository
    {
        public BudgetRepository(ExpenseTrackingDbContext context) : base(context)
        {
        }
        
        public async Task<Budget> GetBudgetAsync(int userId, int? categoryId, int year, int? groupId)
        {
            try
            {
                var query = _dbSet
                    .Include(b => b.Category)
                    .Where(b => b.UserId == userId && b.Year == year);
                
                if (categoryId.HasValue)
                    query = query.Where(b => b.CategoryId == categoryId);
                else
                    query = query.Where(b => b.CategoryId == null);
                
                if (groupId.HasValue)
                    query = query.Where(b => b.GroupId == groupId);
                else
                    query = query.Where(b => b.GroupId == null);
                
                return await query.FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetBudget", ex.Message, ex);
            }
        }
        
        public async Task<IEnumerable<Budget>> GetBudgetsByYearAsync(int userId, int year)
        {
            try
            {
                return await _dbSet
                    .Include(b => b.Category)
                    .Where(b => b.UserId == userId && b.Year == year)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetBudgetsByYear", ex.Message, ex);
            }
        }
        
        public async Task<BudgetAlert> GetLastAlertAsync(int budgetId)
        {
            try
            {
                return await _context.BudgetAlerts
                    .Where(a => a.BudgetId == budgetId)
                    .OrderByDescending(a => a.AlertDate)
                    .FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetLastAlert", ex.Message, ex);
            }
        }
        
        public async Task<bool> BudgetExistsAsync(int userId, int? categoryId, int year, int? groupId)
        {
            try
            {
                var query = _dbSet.Where(b => b.UserId == userId && b.Year == year);
                
                if (categoryId.HasValue)
                    query = query.Where(b => b.CategoryId == categoryId);
                else
                    query = query.Where(b => b.CategoryId == null);
                
                if (groupId.HasValue)
                    query = query.Where(b => b.GroupId == groupId);
                else
                    query = query.Where(b => b.GroupId == null);
                
                return await query.AnyAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("BudgetExists", ex.Message, ex);
            }
        }
    }
}