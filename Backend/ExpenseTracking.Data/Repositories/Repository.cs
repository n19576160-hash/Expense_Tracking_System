using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.Data.Repositories
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly ExpenseTrackingDbContext _context;
        protected readonly DbSet<T> _dbSet;
        
        protected Repository(ExpenseTrackingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }
        
        public virtual async Task<T> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FindAsync(id);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetById", ex.Message, ex);
            }
        }
        
        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.ToListAsync();
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetAll", ex.Message, ex);
            }
        }
        
        public virtual async Task<T> AddAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            try
            {
                await _dbSet.AddAsync(entity);
                await _context.SaveChangesAsync();
                return entity;
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseException("Add", ex.Message, ex);
            }
        }
        
        public virtual async Task UpdateAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            try
            {
                _dbSet.Update(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseException("Update", ex.Message, ex);
            }
        }
        
        public virtual async Task DeleteAsync(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            
            try
            {
                _dbSet.Remove(entity);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                throw new DatabaseException("Delete", ex.Message, ex);
            }
        }
        
        public virtual async Task<bool> ExistsAsync(int id)
        {
            try
            {
                var entity = await _dbSet.FindAsync(id);
                return entity != null;
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Exists", ex.Message, ex);
            }
        }
    }
}