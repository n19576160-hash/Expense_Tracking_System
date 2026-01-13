namespace ExpenseTracking.Domain.Exceptions
{
    public abstract class BaseException : Exception
    {
        public string ErrorCode { get; protected set; }
        public DateTime Timestamp { get; private set; }
        
        protected BaseException(string message, string errorCode) : base(message)
        {
            ErrorCode = errorCode;
            Timestamp = DateTime.Now;
        }
        
        protected BaseException(string message, string errorCode, Exception innerException) 
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            Timestamp = DateTime.Now;
        }
    }
}