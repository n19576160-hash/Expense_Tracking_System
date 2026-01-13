namespace ExpenseTracking.Business.DTOs
{
    public class ExpenseSummaryDTO
    {

        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }

        public DateTime? Date { get; set; }  // For daily summary
        public int? Year { get; set; }
        public int? Month { get; set; }
      
        public List<CategoryBreakdownDTO> CategoryBreakdown { get; set; }
        public string TopCategory { get; set; }
        public List<CategoryBreakdownDTO> OverBudgetCategories { get; set; }
    }
    
    public class CategoryBreakdownDTO
    {
        public string CategoryName { get; set; }
        public decimal Amount { get; set; }
        public int Count { get; set; }
        public decimal? BudgetAmount { get; set; }
        public decimal? RemainingBudget { get; set; }
        public bool IsOverBudget { get; set; }
        public DateTime? Date { get; set; }
    }
}
