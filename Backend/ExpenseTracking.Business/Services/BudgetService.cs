using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;
using ExpenseTracking.Data.Repositories;
using ExpenseTracking.Data;
using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public class BudgetService : IBudgetService
    {
        private readonly IBudgetRepository _budgetRepo;
        private readonly IExpenseRepository _expenseRepo;
        private readonly ExpenseTrackingDbContext _context;
        
        public BudgetService(
            IBudgetRepository budgetRepo, 
            IExpenseRepository expenseRepo,
            ExpenseTrackingDbContext context)
        {
            _budgetRepo = budgetRepo ?? throw new ArgumentNullException(nameof(budgetRepo));
            _expenseRepo = expenseRepo ?? throw new ArgumentNullException(nameof(expenseRepo));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        
        public async Task<Budget> CreateBudgetAsync(CreateBudgetDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            
            // Check if budget already exists
            if (await _budgetRepo.BudgetExistsAsync(dto.UserId, dto.CategoryId, dto.Year, dto.GroupId))
                throw new BudgetAlreadyExistsException(dto.Year, dto.CategoryId);
            
            var budget = new Budget(dto.UserId, dto.Year, dto.BudgetAmount, dto.CategoryId, dto.GroupId);
            
            if (dto.WarningThreshold.HasValue && dto.CriticalThreshold.HasValue)
                budget.SetThresholds(dto.WarningThreshold.Value, dto.CriticalThreshold.Value);
            
            if (dto.EnforceHardLimit)
                budget.EnableHardLimit();
            
            await _budgetRepo.AddAsync(budget);
            return budget;
        }
        
        public async Task<Budget> UpdateBudgetAsync(int budgetId, UpdateBudgetDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            
            var budget = await _budgetRepo.GetByIdAsync(budgetId);
            if (budget == null)
                throw new EntityNotFoundException("Budget", budgetId);
            
            if (budget.UserId != dto.UserId)
                throw new ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException(dto.UserId, "Budget", "update");
            
            budget.UpdateAmount(dto.BudgetAmount);
            
            if (dto.WarningThreshold.HasValue && dto.CriticalThreshold.HasValue)
                budget.SetThresholds(dto.WarningThreshold.Value, dto.CriticalThreshold.Value);
            
            if (dto.EnforceHardLimit)
                budget.EnableHardLimit();
            else
                budget.DisableHardLimit();
            
            await _budgetRepo.UpdateAsync(budget);
            return budget;
        }
        
        public async Task<IEnumerable<Budget>> GetBudgetsByYearAsync(int userId, int year)
        {
            return await _budgetRepo.GetBudgetsByYearAsync(userId, year);
        }
        
        public async Task<BudgetCheckResultDTO> CheckBudgetBeforeExpenseAsync(
            int userId, int categoryId, decimal amount, DateTime expenseDate, int? groupId)
        {
            var year = expenseDate.Year;
            var budget = await _budgetRepo.GetBudgetAsync(userId, categoryId, year, groupId);
            
            if (budget == null)
            {
                return new BudgetCheckResultDTO
                {
                    HasBudget = false,
                    IsOverBudget = false,
                    RequiresApproval = false,
                    ShouldTriggerAlert = false
                };
            }
            
            var currentSpent = await _expenseRepo.GetTotalSpentAsync(userId, categoryId, year, groupId);
            var projectedSpent = currentSpent + amount;
            
            budget.IsOverThreshold(projectedSpent, out decimal percentageUsed);
            
            var result = new BudgetCheckResultDTO
            {
                HasBudget = true,
                BudgetId = budget.BudgetId,
                BudgetAmount = budget.BudgetAmount,
                CurrentSpent = currentSpent,
                ProjectedSpent = projectedSpent,
                PercentageUsed = percentageUsed,
                IsOverBudget = projectedSpent > budget.BudgetAmount,
                RequiresApproval = budget.EnforceHardLimit && projectedSpent > budget.BudgetAmount
            };
            
            // Check if should trigger alert
            var lastAlert = await _budgetRepo.GetLastAlertAsync(budget.BudgetId);
            
            if (percentageUsed >= budget.CriticalThreshold && 
                (lastAlert == null || lastAlert.ThresholdLevel < budget.CriticalThreshold))
            {
                result.ShouldTriggerAlert = true;
            }
            else if (percentageUsed >= budget.WarningThreshold && 
                     (lastAlert == null || lastAlert.ThresholdLevel < budget.WarningThreshold))
            {
                result.ShouldTriggerAlert = true;
            }
            
            return result;
        }
        
        public async Task CreateBudgetAlertAsync(int budgetId, int userId, decimal percentageUsed)
        {
            var message = $"Budget alert: You have used {percentageUsed:F2}% of your budget.";
            var alert = new BudgetAlert(budgetId, userId, percentageUsed, message);
            
            await _context.BudgetAlerts.AddAsync(alert);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteBudgetAsync(int budgetId, int userId)
        {
            var budget = await _budgetRepo.GetByIdAsync(budgetId);
            if (budget == null)
                throw new EntityNotFoundException("Budget", budgetId);

            if (budget.UserId != userId)
                throw new ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException(userId, "Budget", "delete");

            await _budgetRepo.DeleteAsync(budget);
        }

        public async Task<BudgetPerformanceDTO> GetBudgetPerformanceAsync(int budgetId)
        {
            var budget = await _budgetRepo.GetByIdAsync(budgetId);
            if (budget == null)
                throw new EntityNotFoundException("Budget", budgetId);

            var spent = await _expenseRepo.GetTotalSpentAsync(
                budget.UserId, 
                budget.CategoryId, 
                budget.Year, 
                budget.GroupId);

            budget.IsOverThreshold(spent, out decimal percentageUsed);

            return new BudgetPerformanceDTO
            {
                BudgetId = budgetId,
                BudgetAmount = budget.BudgetAmount,
                SpentAmount = spent,
                RemainingAmount = budget.BudgetAmount - spent,
                PercentageUsed = percentageUsed,
                Status = GetBudgetStatus(percentageUsed, budget),
                WarningThreshold = budget.WarningThreshold,
                CriticalThreshold = budget.CriticalThreshold
            };
        }

        private string GetBudgetStatus(decimal percentageUsed, Budget budget)
        {
            if (percentageUsed >= budget.CriticalThreshold)
                return "Critical";
            if (percentageUsed >= budget.WarningThreshold)
                return "Warning";
            return "Safe";
        }
    }
}

        