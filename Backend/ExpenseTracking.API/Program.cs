using Microsoft.EntityFrameworkCore;
using ExpenseTracking.Data;
using ExpenseTracking.Data.Repositories;
using ExpenseTracking.Business.Services;
using ExpenseTracking.Business.DTOs;
using ExpenseTracking.Domain.Exceptions;
using Scalar.AspNetCore; // without this ,MapScalarApiReference will not work
using Microsoft.AspNetCore.Builder;
// using Microsoft.OpenApi.Models;

namespace ExpenseTracking.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("=== Expense Tracking & Budget Control System ===");
            Console.WriteLine("Starting application...\n");
            
            try
            {
                // Build the application
                var builder = WebApplication.CreateBuilder(args);
                
                // Configure services
                ConfigureServices(builder.Services, builder.Configuration);
                
                var app = builder.Build();
                
                // Configure middleware
                ConfigureMiddleware(app);
                
                Console.WriteLine("Application started successfully!");
                Console.WriteLine($"API running at: https://localhost:5275");
                Console.WriteLine($"Scalar UI at: http://localhost:5275/scalar/v1\n");
                
                // Run the application
                await app.RunAsync();
            }
            catch (DatabaseException dbEx)
            {
                Console.WriteLine($"\n[DATABASE ERROR] {dbEx.ErrorCode}");
                Console.WriteLine($"Operation: {dbEx.Operation}");
                Console.WriteLine($"Message: {dbEx.Message}");
                Console.WriteLine($"Timestamp: {dbEx.Timestamp}");
                if (dbEx.InnerException != null)
                    Console.WriteLine($"Inner Exception: {dbEx.InnerException.Message}");
            }
            catch (ValidationException valEx)
            {
                Console.WriteLine($"\n[VALIDATION ERROR] {valEx.ErrorCode}");
                Console.WriteLine($"Message: {valEx.Message}");
                Console.WriteLine($"Timestamp: {valEx.Timestamp}");
                
                if (valEx.ValidationErrors.Any())
                {
                    Console.WriteLine("Validation Errors:");
                    foreach (var error in valEx.ValidationErrors)
                    {
                        Console.WriteLine($"  - {error.Key}: {error.Value}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n[FATAL ERROR]");
                Console.WriteLine($"Message: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            }
            finally
            {
                Console.WriteLine("\nApplication shutdown.");
            }
        }
        
        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            try
            {
                Console.WriteLine("Configuring services...");
                
                // Database Context
                services.AddDbContext<ExpenseTrackingDbContext>(options =>
                {
                    var connectionString = configuration.GetConnectionString("DefaultConnection");
                    options.UseSqlServer(connectionString, sqlOptions =>
                    {
                        sqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(30),
                            errorNumbersToAdd: null);
                    });
                });
                
                // Repository Registration
                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IExpenseRepository, ExpenseRepository>();
                services.AddScoped<IBudgetRepository, BudgetRepository>();
                services.AddScoped<ICategoryRepository, CategoryRepository>();
                
                // Service Registration
                services.AddScoped<IExpenseService, ExpenseService>();
                services.AddScoped<IBudgetService, BudgetService>();
                services.AddScoped<ICategoryService, CategoryService>();
                
                // CORS
                services.AddCors(options =>
                {
                    options.AddPolicy("AllowFrontend", policy =>
                    {
                        policy.WithOrigins("http://localhost:3000", "http://localhost:4200")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    });
                });
                
                // Controllers
                services.AddControllers();
                
                // scaler configuration
                services.AddOpenApi();
                
                Console.WriteLine("Services configured successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to configure services: {ex.Message}", ex);
            }
        }
        
        private static void ConfigureMiddleware(WebApplication app)
        {
            try
            {
                Console.WriteLine("Configuring middleware...");
                
                // Development middleware
                if (app.Environment.IsDevelopment())
                { 
                    app.MapOpenApi(); // create OpenAPI endpoint
                    app.MapScalarApiReference(); // show Scalar interface
                    Console.WriteLine("Scalar enabled for development.");
                }
                
                // Global exception handling middleware
                app.UseMiddleware<ExceptionHandlingMiddleware>();
                
                app.UseHttpsRedirection();
                app.UseCors("AllowFrontend");
                app.UseAuthorization();
                app.MapControllers();
                
                Console.WriteLine("Middleware configured successfully.");
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to configure middleware: {ex.Message}", ex);
            }
        }
        
        // Demo method to show exception handling
        public static async Task DemoExpenseOperations()
        {
            Console.WriteLine("\n=== DEMO: Expense Operations ===\n");
            
            try
            {
                // This would normally come from dependency injection
                Console.WriteLine("1. Attempting to add valid expense...");
                // await expenseService.AddExpenseAsync(validDto);
                Console.WriteLine("   ✓ Expense added successfully\n");
            }
            catch (ValidationException ex)
            {
                Console.WriteLine($"   ✗ Validation failed: {ex.Message}");
                foreach (var error in ex.ValidationErrors)
                {
                    Console.WriteLine($"     - {error.Key}: {error.Value}");
                }
            }
            catch (BudgetExceededException ex)
            {
                Console.WriteLine($"   ✗ Budget exceeded!");
                Console.WriteLine($"     Budget Amount: ${ex.BudgetAmount}");
                Console.WriteLine($"     Current Spent: ${ex.CurrentSpent}");
                Console.WriteLine($"     Attempted: ${ex.AttemptedAmount}");
                Console.WriteLine($"     Overage: ${ex.OverageAmount}");
            }
            catch (DuplicateEntityException ex)
            {
                Console.WriteLine($"   ✗ Duplicate detected: {ex.Message}");
                Console.WriteLine($"     Entity: {ex.EntityType}");
                Console.WriteLine($"     Field: {ex.DuplicateField}");
            }
            catch (BaseException ex)
            {
                Console.WriteLine($"   ✗ Error: {ex.Message}");
                Console.WriteLine($"     Error Code: {ex.ErrorCode}");
                Console.WriteLine($"     Timestamp: {ex.Timestamp}");
            }
            
            try
            {
                Console.WriteLine("\n2. Attempting to update expense with invalid date...");
                // This will throw ExpenseEditRestrictionException
                throw new ExpenseEditRestrictionException(
                    123, 
                    new DateTime(2024, 1, 15), 
                    "Cannot edit current year expenses without approval");
            }
            catch (ExpenseEditRestrictionException ex)
            {
                Console.WriteLine($"   ✗ Edit restricted: {ex.Message}");
                Console.WriteLine($"     Expense ID: {ex.ExpenseId}");
                Console.WriteLine($"     Expense Date: {ex.ExpenseDate:yyyy-MM-dd}");
                Console.WriteLine($"     Restriction: {ex.Restriction}");
            }
            
            try
            {
                Console.WriteLine("\n3. Attempting to delete category with expenses...");
                throw new CategoryDeletionException(5, "Office Supplies", 47);
            }
            catch (CategoryDeletionException ex)
            {
                Console.WriteLine($"   ✗ Deletion failed: {ex.Message}");
                Console.WriteLine($"     Category: {ex.CategoryName} (ID: {ex.CategoryId})");
                Console.WriteLine($"     Expense Count: {ex.ExpenseCount}");
            }
            
            try
            {
                Console.WriteLine("\n4. Attempting unauthorized access...");
                throw new ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException(10, "Budget", "delete");
            }
            catch (ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException ex)
            {
                Console.WriteLine($"   ✗ Access denied: {ex.Message}");
                Console.WriteLine($"     User ID: {ex.UserId}");
                Console.WriteLine($"     Resource: {ex.Resource}");
                Console.WriteLine($"     Action: {ex.Action}");
            }
            
            try
            {
                Console.WriteLine("\n5. Attempting expense requiring approval...");
                throw new ApprovalRequiredException(456, 5000.00m, "Manager approval document");
            }
            catch (ApprovalRequiredException ex)
            {
                Console.WriteLine($"   ✗ Approval required: {ex.Message}");
                Console.WriteLine($"     Expense ID: {ex.ExpenseId}");
                Console.WriteLine($"     Amount: ${ex.Amount}");
                Console.WriteLine($"     Required: {ex.RequiredDocument}");
            }
            
            try
            {
                Console.WriteLine("\n6. Attempting with invalid date range...");
                var endDate = new DateTime(2024, 1, 1);
                var startDate = new DateTime(2024, 12, 31);
                throw new InvalidDateRangeException(startDate, endDate);
            }
            catch (InvalidDateRangeException ex)
            {
                Console.WriteLine($"   ✗ Invalid range: {ex.Message}");
                Console.WriteLine($"     Start: {ex.StartDate:yyyy-MM-dd}");
                Console.WriteLine($"     End: {ex.EndDate:yyyy-MM-dd}");
            }
            
            Console.WriteLine("\n=== DEMO Complete ===\n");
        }
    }
}
