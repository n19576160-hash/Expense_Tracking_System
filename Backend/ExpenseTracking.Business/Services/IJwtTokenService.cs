using ExpenseTracking.Domain.Entities;

namespace ExpenseTracking.Business.Services
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
        int? ValidateToken(string token);
    }
}