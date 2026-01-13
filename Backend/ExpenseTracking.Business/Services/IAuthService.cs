using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public interface IAuthService
    {
        Task<User> RegisterAsync(RegisterRequestDTO request);
        Task<LoginResultDTO> LoginAsync(LoginRequestDTO request);
        Task SendPasswordResetAsync(string email);
        Task ResetPasswordAsync(ResetPasswordRequestDTO request);
    }
    
    public class LoginResultDTO
    {
        public string Token { get; set; }
        public User User { get; set; }
    }
}