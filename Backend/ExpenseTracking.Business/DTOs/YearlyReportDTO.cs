namespace ExpenseTracking.Business.DTOs
{
    public class YearlyReportDTO
    {
        public int Year { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal MonthlyAverage { get; set; }
        public int TotalTransactions { get; set; }
        public List<MonthlyBreakdownDTO> MonthlyBreakdown { get; set; }
        public List<CategoryBreakdownDTO> CategoryBreakdown { get; set; }
    }
    
    public class MonthlyBreakdownDTO
    {
        public int Month { get; set; }
        public string MonthName { get; set; }
        public decimal Amount { get; set; }
        public int TransactionCount { get; set; }
    }
}