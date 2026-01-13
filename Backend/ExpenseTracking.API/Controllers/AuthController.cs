using Microsoft.AspNetCore.Mvc;
using ExpenseTracking.Business.Services;
using ExpenseTracking.Business.DTOs;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        
        public AuthController(IAuthService authService)
        {
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));
        }
        
        /// <summary>
        /// Register a new user
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid registration data");
            
            var user = await _authService.RegisterAsync(request);
            
            return Ok(new
            {
                success = true,
                message = "Registration successful. Please login.",
                user = new
                {
                    user.UserId,
                    user.Name,
                    user.Email
                }
            });
        }
        
        /// <summary>
        /// Login user
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid login data");
            
            var result = await _authService.LoginAsync(request);
            
            return Ok(new
            {
                success = true,
                message = "Login successful",
                token = result.Token,
                user = new
                {
                    result.User.UserId,
                    result.User.Name,
                    result.User.Email
                }
            });
        }
        
        /// <summary>
        /// Request password reset
        /// </summary>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            await _authService.SendPasswordResetAsync(request.Email);
            
            return Ok(new
            {
                success = true,
                message = "If the email exists, a reset link has been sent."
            });
        }
        
        /// <summary>
        /// Reset password with token
        /// </summary>
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            await _authService.ResetPasswordAsync(request);
            
            return Ok(new
            {
                success = true,
                message = "Password reset successful. Please login."
            });
        }
    }
}
