namespace ExpenseTracking.Business.DTOs
{
    public class UpdateGroupDTO
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public bool RequireOwnerApprovalForOverBudget { get; set; }
    }
}