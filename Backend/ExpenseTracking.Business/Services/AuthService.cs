using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;
using ExpenseTracking.Data.Repositories;
using ExpenseTracking.Business.DTOs;
using System.Security.Cryptography;
using System.Text;

namespace ExpenseTracking.Business.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepo;
        private readonly IJwtTokenService _jwtService;
        
        public AuthService(IUserRepository userRepo, IJwtTokenService jwtService)
        {
            _userRepo = userRepo ?? throw new ArgumentNullException(nameof(userRepo));
            _jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
        }
        
        public async Task<User> RegisterAsync(RegisterRequestDTO request)
        {
            // Validate
            if (string.IsNullOrWhiteSpace(request.Name))
                throw new ValidationException(nameof(request.Name), "Name is required");
            
            if (string.IsNullOrWhiteSpace(request.Email))
                throw new ValidationException(nameof(request.Email), "Email is required");
            
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException(nameof(request.Password), "Password is required");
            
            if (request.Password.Length < 8)
                throw new ValidationException(nameof(request.Password), "Password must be at least 8 characters");
            
            // Check if email exists
            if (await _userRepo.EmailExistsAsync(request.Email))
                throw new EmailAlreadyExistsException(request.Email);
            
            // Hash password
            var passwordHash = HashPassword(request.Password);
            
            // Create user
            var user = new User(request.Name, request.Email, passwordHash, request.MobileNumber);
            await _userRepo.AddAsync(user);
            
            // Create default notification preferences
            var preferences = new NotificationPreference(user.UserId);
            // Save preferences through repository
            
            return user;
        }
        
        public async Task<LoginResultDTO> LoginAsync(LoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.EmailOrMobile))
                throw new ValidationException(nameof(request.EmailOrMobile), "Email or mobile is required");
            
            if (string.IsNullOrWhiteSpace(request.Password))
                throw new ValidationException(nameof(request.Password), "Password is required");
            
            var passwordHash = HashPassword(request.Password);
            var user = await _userRepo.AuthenticateAsync(request.EmailOrMobile, passwordHash);
            
            if (user == null)
                throw new AuthenticationException(request.EmailOrMobile, "Invalid email/mobile or password");
            
            user.UpdateLastLogin();
            await _userRepo.UpdateAsync(user);
            
            var token = _jwtService.GenerateToken(user);
            
            return new LoginResultDTO
            {
                Token = token,
                User = user
            };
        }
        
        public async Task SendPasswordResetAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            if (user == null)
                return; // Don't reveal if email exists
            
            // Generate reset token and send email
            // Implementation depends on your email service
            // For now, just a placeholder
        }
        
        public async Task ResetPasswordAsync(ResetPasswordRequestDTO request)
        {
            // Validate token and reset password
            // Implementation depends on your token storage
            throw new NotImplementedException("Password reset not yet implemented");
        }
        
        private string HashPassword(string password)
        {
            // Use SHA256 for now (In production, use BCrypt)
            using var sha256 = SHA256.Create();
            var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
