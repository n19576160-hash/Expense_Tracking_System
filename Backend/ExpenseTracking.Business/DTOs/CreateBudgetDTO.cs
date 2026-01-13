namespace ExpenseTracking.Business.DTOs
{
    public class CreateBudgetDTO
    {
        public int UserId { get; set; }
        public int? CategoryId { get; set; }
        public int Year { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal? WarningThreshold { get; set; }
        public decimal? CriticalThreshold { get; set; }
        public bool EnforceHardLimit { get; set; }
        public int? GroupId { get; set; }
    }
}