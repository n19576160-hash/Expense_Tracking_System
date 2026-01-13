namespace ExpenseTracking.Domain.Exceptions
{
    public class InvalidDateRangeException : BaseException
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }
        
        public InvalidDateRangeException(DateTime startDate, DateTime endDate) 
            : base($"Invalid date range: Start date ({startDate:yyyy-MM-dd}) must be before or equal to end date ({endDate:yyyy-MM-dd})", "INVALID_DATE_RANGE")
        {
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
