namespace ExpenseTracking.Business.DTOs
{
    public class GroupDetailsDTO
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public string OwnerName { get; set; }
        public bool RequireOwnerApprovalForOverBudget { get; set; }
        public List<GroupMemberDTO> Members { get; set; }
    }
    
    public class GroupMemberDTO
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime JoinedDate { get; set; }
    }
}