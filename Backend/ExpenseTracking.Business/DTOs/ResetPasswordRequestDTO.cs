namespace ExpenseTracking.Business.DTOs
{
    public class ResetPasswordRequestDTO
    {
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}
