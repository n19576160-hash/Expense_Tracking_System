namespace ExpenseTracking.Business.DTOs
{
    public class BudgetCheckRequestDTO
    {
        public int CategoryId { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public int? GroupId { get; set; }
    }
}