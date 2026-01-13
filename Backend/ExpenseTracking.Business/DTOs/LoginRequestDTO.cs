namespace ExpenseTracking.Business.DTOs
{
    public class LoginRequestDTO
    {
        public string EmailOrMobile { get; set; }
        public string Password { get; set; }
    }
}