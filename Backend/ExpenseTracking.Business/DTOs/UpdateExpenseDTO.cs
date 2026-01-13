namespace ExpenseTracking.Business.DTOs
{
    public class UpdateExpenseDTO
    {
        public int UserId { get; set; }
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public string Description { get; set; }
    }
}
