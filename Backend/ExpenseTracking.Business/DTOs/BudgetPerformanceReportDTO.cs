namespace ExpenseTracking.Business.DTOs
{
    public class BudgetPerformanceReportDTO
    {
        public int Year { get; set; }
        public List<BudgetPerformanceDTO> BudgetPerformances { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal TotalRemaining { get; set; }
        public decimal OverallPercentageUsed { get; set; }
    }
}