namespace ExpenseTracking.Business.DTOs
{
    public class CreateExpenseDTO
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Description { get; set; }
        public int? GroupId { get; set; }
        public string ApprovalDocumentPath { get; set; }
    }
}