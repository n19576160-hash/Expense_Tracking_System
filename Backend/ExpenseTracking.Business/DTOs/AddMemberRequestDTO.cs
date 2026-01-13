namespace ExpenseTracking.Business.DTOs
{
    public class AddMemberRequestDTO
    {
        public int UserId { get; set; }
        public string Role { get; set; } // "Owner" or "Member"
    }
}
