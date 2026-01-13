namespace ExpenseTracking.Business.DTOs
{
    public class CreateGroupDTO
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public int OwnerId { get; set; }
        public bool RequireOwnerApprovalForOverBudget { get; set; }
    }
}