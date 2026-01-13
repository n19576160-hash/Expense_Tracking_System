namespace ExpenseTracking.Business.DTOs
{
    public class BudgetPerformanceDTO
    {
        public int BudgetId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal SpentAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal PercentageUsed { get; set; }
        public string Status { get; set; }
        public decimal WarningThreshold { get; set; }
        public decimal CriticalThreshold { get; set; }
    }
}