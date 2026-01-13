namespace ExpenseTracking.Business.DTOs
{
    public class CategoryReportDTO
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public int Year { get; set; }
        public decimal TotalAmount { get; set; }
        public int TransactionCount { get; set; }
        public decimal AverageAmount { get; set; }
        public List<MonthlyBreakdownDTO> MonthlyData { get; set; }
    }
}