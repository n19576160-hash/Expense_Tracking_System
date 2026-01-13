namespace ExpenseTracking.Business.DTOs
{
    public class RegisterRequestDTO
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string MobileNumber { get; set; }
        public string Password { get; set; }
    }
}