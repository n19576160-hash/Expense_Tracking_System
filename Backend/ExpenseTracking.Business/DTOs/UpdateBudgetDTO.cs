namespace ExpenseTracking.Business.DTOs
{
    public class UpdateBudgetDTO
    {
        public int UserId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal? WarningThreshold { get; set; }
        public decimal? CriticalThreshold { get; set; }
        public bool EnforceHardLimit { get; set; }
    }
}