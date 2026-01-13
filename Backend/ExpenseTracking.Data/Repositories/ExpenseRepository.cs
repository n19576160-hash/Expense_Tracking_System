using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.Data.Repositories
{
    public class ExpenseRepository : Repository<Expense>, IExpenseRepository
    {
        public ExpenseRepository(ExpenseTrackingDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<Expense>> GetByUserAsync(int userId)
        {
            try
            {
                return await _dbSet
                    .Include(e => e.Category)
                    .Include(e => e.User)
                    .Where(e => e.UserId == userId)
                    .OrderByDescending(e => e.ExpenseDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetByUser", ex.Message, ex);
            }
        }
        
        public async Task<IEnumerable<Expense>> GetByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new InvalidDateRangeException(startDate, endDate);
            
            try
            {
                return await _dbSet
                    .Include(e => e.Category)
                    .Where(e => e.UserId == userId 
                        && e.ExpenseDate >= startDate 
                        && e.ExpenseDate <= endDate)
                    .OrderByDescending(e => e.ExpenseDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetByDateRange", ex.Message, ex);
            }
        }
        
        public async Task<IEnumerable<Expense>> GetByCategoryAsync(int userId, int categoryId, int year)
        {
            try
            {
                return await _dbSet
                    .Include(e => e.Category)
                    .Where(e => e.UserId == userId 
                        && e.CategoryId == categoryId 
                        && e.ExpenseDate.Year == year)
                    .OrderByDescending(e => e.ExpenseDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetByCategory", ex.Message, ex);
            }
        }
        
        public async Task<IEnumerable<Expense>> GetByMonthAsync(int userId, int year, int month)
        {
            if (month < 1 || month > 12)
                throw new ValidationException(nameof(month), "Month must be between 1 and 12");
            
            try
            {
                return await _dbSet
                    .Include(e => e.Category)
                    .Where(e => e.UserId == userId 
                        && e.ExpenseDate.Year == year 
                        && e.ExpenseDate.Month == month)
                    .OrderByDescending(e => e.ExpenseDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetByMonth", ex.Message, ex);
            }
        }
        public async Task<IEnumerable<Expense>> GetByDateAsync(int userId, DateTime date)
        {
            try
            {
                return await _dbSet
                    .Include(e => e.Category)
                    .Include(e => e.User)
                    .Where(e => e.UserId == userId && e.ExpenseDate.Date == date.Date)
                    .OrderByDescending(e => e.CreatedDate)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetByDate", ex.Message, ex);
            }
        }
        public async Task<decimal> GetTotalSpentAsync(int userId, int? categoryId, int year, int? groupId)
        {
            try
            {
                var query = _dbSet.Where(e => e.UserId == userId && e.ExpenseDate.Year == year);
                
                if (categoryId.HasValue)
                    query = query.Where(e => e.CategoryId == categoryId.Value);
                
                if (groupId.HasValue)
                    query = query.Where(e => e.GroupId == groupId);
                else
                    query = query.Where(e => e.GroupId == null);
                
                return await query.SumAsync(e => (decimal?)e.Amount) ?? 0;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetTotalSpent", ex.Message, ex);
            }
        }
        
        public async Task<bool> CheckDuplicateAsync(int userId, DateTime date, decimal amount, int categoryId, string description)
        {
            try
            {
                return await _dbSet.AnyAsync(e => 
                    e.UserId == userId 
                    && e.ExpenseDate == date 
                    && e.Amount == amount 
                    && e.CategoryId == categoryId 
                    && e.Description == description);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("CheckDuplicate", ex.Message, ex);
            }
        }
    }
}