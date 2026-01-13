namespace ExpenseTracking.Business.DTOs
{
    public class ExportRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Format { get; set; } // "PDF" or "CSV"
        public string ReportType { get; set; } // "Yearly", "Category", "BudgetPerformance"
        public int? CategoryId { get; set; }
    }
}
