using System.Net;
using System.Text.Json;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.API
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;
        
        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }
        
        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            // Log the exception (not using Console.WriteLine in middleware)
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);
            
            context.Response.ContentType = "application/json";
            
            var errorResponse = new ErrorResponse
            {
                Timestamp = DateTime.Now,
                Path = context.Request.Path
            };
            
            switch (exception)
            {
                case ValidationException valEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorCode = valEx.ErrorCode;
                    errorResponse.Message = valEx.Message;
                    errorResponse.ValidationErrors = valEx.ValidationErrors;
                    break;
                
                case EntityNotFoundException notFoundEx:
                    context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                    errorResponse.ErrorCode = notFoundEx.ErrorCode;
                    errorResponse.Message = notFoundEx.Message;
                    errorResponse.Details = new Dictionary<string, object>
                    {
                        { "EntityType", notFoundEx.EntityType },
                        { "EntityId", notFoundEx.EntityId }
                    };
                    break;
                
                case DuplicateEntityException dupEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Conflict;
                    errorResponse.ErrorCode = dupEx.ErrorCode;
                    errorResponse.Message = dupEx.Message;
                    errorResponse.Details = new Dictionary<string, object>
                    {
                        { "EntityType", dupEx.EntityType },
                        { "DuplicateField", dupEx.DuplicateField },
                        { "DuplicateValue", dupEx.DuplicateValue }
                    };
                    break;
                
                case ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException authEx:
                    context.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                    errorResponse.ErrorCode = authEx.ErrorCode;
                    errorResponse.Message = authEx.Message;
                    break;
                
                case BudgetExceededException budgetEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorCode = budgetEx.ErrorCode;
                    errorResponse.Message = budgetEx.Message;
                    errorResponse.Details = new Dictionary<string, object>
                    {
                        { "BudgetAmount", budgetEx.BudgetAmount },
                        { "CurrentSpent", budgetEx.CurrentSpent },
                        { "AttemptedAmount", budgetEx.AttemptedAmount },
                        { "OverageAmount", budgetEx.OverageAmount }
                    };
                    break;
                
                case BusinessRuleViolationException ruleEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorCode = ruleEx.ErrorCode;
                    errorResponse.Message = ruleEx.Message;
                    break;
                
                case DatabaseException dbEx:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.ErrorCode = dbEx.ErrorCode;
                    errorResponse.Message = "A database error occurred";
                    errorResponse.Details = new Dictionary<string, object>
                    {
                        { "Operation", dbEx.Operation }
                    };
                    break;
                
                case BaseException baseEx:
                    context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                    errorResponse.ErrorCode = baseEx.ErrorCode;
                    errorResponse.Message = baseEx.Message;
                    break;
                
                default:
                    context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    errorResponse.ErrorCode = "INTERNAL_SERVER_ERROR";
                    errorResponse.Message = "An unexpected error occurred";
                    break;
            }
            
            var jsonResponse = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });
            
            await context.Response.WriteAsync(jsonResponse);
        }
    }
    
    public class ErrorResponse
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
        public string Path { get; set; }
        public Dictionary<string, string> ValidationErrors { get; set; }
        public Dictionary<string, object> Details { get; set; }
    }
}