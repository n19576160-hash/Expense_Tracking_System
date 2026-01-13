using ExpenseTracking.Domain.Entities;

namespace ExpenseTracking.Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByMobileAsync(string mobile);
        Task<bool> EmailExistsAsync(string email);
        Task<User> AuthenticateAsync(string emailOrMobile, string passwordHash);
    }
}
