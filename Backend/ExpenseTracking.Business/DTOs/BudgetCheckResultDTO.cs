namespace ExpenseTracking.Business.DTOs
{
    public class BudgetCheckResultDTO
    {
        public bool HasBudget { get; set; }
        public int? BudgetId { get; set; }
        public decimal BudgetAmount { get; set; }
        public decimal CurrentSpent { get; set; }
        public decimal ProjectedSpent { get; set; }
        public decimal PercentageUsed { get; set; }
        public bool IsOverBudget { get; set; }
        public bool RequiresApproval { get; set; }
        public bool ShouldTriggerAlert { get; set; }
    }
}
