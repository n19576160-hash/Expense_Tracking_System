using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public interface IReportService
    {
        Task<YearlyReportDTO> GenerateYearlyReportAsync(int userId, int year);
        Task<CategoryReportDTO> GenerateCategoryReportAsync(int userId, int categoryId, int year);
        Task<BudgetPerformanceReportDTO> GenerateBudgetPerformanceReportAsync(int userId, int year);
        Task<byte[]> ExportReportAsync(int userId, ExportRequestDTO request);
    }
}