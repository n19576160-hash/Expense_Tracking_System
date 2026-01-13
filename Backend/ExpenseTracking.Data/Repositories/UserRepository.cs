using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ExpenseTrackingDbContext context) : base(context)
        {
        }
        
        public async Task<User> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException(nameof(email), "Email cannot be empty");
            
            try
            {
                return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetByEmail", ex.Message, ex);
            }
        }
        
        public async Task<User> GetByMobileAsync(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                throw new ValidationException(nameof(mobile), "Mobile number cannot be empty");
            
            try
            {
                return await _dbSet.FirstOrDefaultAsync(u => u.MobileNumber == mobile);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("GetByMobile", ex.Message, ex);
            }
        }
        
        public async Task<bool> EmailExistsAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ValidationException(nameof(email), "Email cannot be empty");
            
            try
            {
                return await _dbSet.AnyAsync(u => u.Email == email);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("EmailExists", ex.Message, ex);
            }
        }
        
        public async Task<User> AuthenticateAsync(string emailOrMobile, string passwordHash)
        {
            if (string.IsNullOrWhiteSpace(emailOrMobile))
                throw new ValidationException(nameof(emailOrMobile), "Email or mobile cannot be empty");
            
            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ValidationException(nameof(passwordHash), "Password hash cannot be empty");
            
            try
            {
                return await _dbSet.FirstOrDefaultAsync(u => 
                    (u.Email == emailOrMobile || u.MobileNumber == emailOrMobile) 
                    && u.PasswordHash == passwordHash 
                    && u.IsActive);
            }
            catch (Exception ex)
            {
                throw new DatabaseException("Authenticate", ex.Message, ex);
            }
        }
    }
}
