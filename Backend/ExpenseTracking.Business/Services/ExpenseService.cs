using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;
using ExpenseTracking.Data.Repositories;
using ExpenseTracking.Data;
using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IExpenseRepository _expenseRepo;
        private readonly IBudgetService _budgetService;
        private readonly ICategoryRepository _categoryRepo;
        
        public ExpenseService(
            IExpenseRepository expenseRepo, 
            IBudgetService budgetService,
            ICategoryRepository categoryRepo)
        {
            _expenseRepo = expenseRepo ?? throw new ArgumentNullException(nameof(expenseRepo));
            _budgetService = budgetService ?? throw new ArgumentNullException(nameof(budgetService));
            _categoryRepo = categoryRepo ?? throw new ArgumentNullException(nameof(categoryRepo));
        }
        
        public async Task<Expense> AddExpenseAsync(CreateExpenseDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            
            // Validate category exists
            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null)
                throw new EntityNotFoundException("Category", dto.CategoryId);
            
            if (!category.IsActive)
                throw new BusinessRuleViolationException("InactiveCategory", "Cannot add expenses to inactive categories");
            
            // Check duplicate
            if (await CheckDuplicateExpenseAsync(dto))
                throw new DuplicateEntityException("Expense", "Date-Amount-Category", $"{dto.ExpenseDate}-{dto.Amount}-{dto.CategoryId}");
            
            // Create expense entity
            var expense = new Expense(
                dto.UserId, 
                dto.CategoryId, 
                dto.Amount, 
                dto.ExpenseDate, 
                dto.Description, 
                dto.GroupId);
            
            // Check budget before adding
            var budgetCheck = await _budgetService.CheckBudgetBeforeExpenseAsync(
                dto.UserId, dto.CategoryId, dto.Amount, dto.ExpenseDate, dto.GroupId);
            
            if (budgetCheck.IsOverBudget)
            {
                expense.MarkAsOverBudget();
                
                if (budgetCheck.RequiresApproval)
                {
                    if (string.IsNullOrWhiteSpace(dto.ApprovalDocumentPath))
                        throw new ApprovalRequiredException(0, dto.Amount, "Approval document and note");
                    
                    expense.SetRequiresApproval(true);
                }
            }
            
            // Save expense
            await _expenseRepo.AddAsync(expense);
            
            // Trigger alert if needed
            if (budgetCheck.ShouldTriggerAlert)
            {
                await _budgetService.CreateBudgetAlertAsync(
                    budgetCheck.BudgetId.Value, 
                    dto.UserId, 
                    budgetCheck.PercentageUsed);
            }
            
            return expense;
        }
        
        public async Task<Expense> UpdateExpenseAsync(int expenseId, UpdateExpenseDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            
            var expense = await _expenseRepo.GetByIdAsync(expenseId);
            if (expense == null)
                throw new EntityNotFoundException("Expense", expenseId);
            
            if (expense.UserId != dto.UserId)
                throw new ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException(dto.UserId, "Expense", "update");
            
            //  Validate category
            var category = await _categoryRepo.GetByIdAsync(dto.CategoryId);
            if (category == null)
                throw new EntityNotFoundException("Category", dto.CategoryId);
            
            // Update expense
            expense.UpdateDetails(dto.Amount, dto.CategoryId, dto.ExpenseDate, dto.Description);
            await _expenseRepo.UpdateAsync(expense);
            
            return expense;
        }
        
        public async Task DeleteExpenseAsync(int expenseId, int userId)
        {
            var expense = await _expenseRepo.GetByIdAsync(expenseId);
            if (expense == null)
                throw new EntityNotFoundException("Expense", expenseId);
            
            if (expense.UserId != userId)
                throw new ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException(userId, "Expense", "delete");
            
            if (!expense.CanBeDeleted())
                throw new ExpenseEditRestrictionException(
                    expenseId, 
                    expense.ExpenseDate, 
                    "Current year expenses cannot be deleted without approval");
            
            await _expenseRepo.DeleteAsync(expense);
        }
        
        public async Task<IEnumerable<Expense>> GetExpensesByDateRangeAsync(int userId, DateTime startDate, DateTime endDate)
        {
            return await _expenseRepo.GetByDateRangeAsync(userId, startDate, endDate);
        }
        
        public async Task<IEnumerable<Expense>> GetExpensesByCategoryAsync(int userId, int categoryId, int year)
        {
            return await _expenseRepo.GetByCategoryAsync(userId, categoryId, year);
        }
        
        public async Task<ExpenseSummaryDTO> GetMonthlySummaryAsync(int userId, int year, int month)
        {
            var expenses = await _expenseRepo.GetByMonthAsync(userId, year, month);
            var budgets = await _budgetService.GetBudgetsByYearAsync(userId, year);
            
            var summary = new ExpenseSummaryDTO
            {
                Year = year,
                Month = month,
                TotalAmount = expenses.Sum(e => e.Amount),
                TransactionCount = expenses.Count(),
                CategoryBreakdown = new List<CategoryBreakdownDTO>()
            };
            
            var grouped = expenses.GroupBy(e => new { e.CategoryId, e.Category.CategoryName });
            
            foreach (var group in grouped)
            {
                var budget = budgets.FirstOrDefault(b => b.CategoryId == group.Key.CategoryId);
                var spent = group.Sum(e => e.Amount);
                
                summary.CategoryBreakdown.Add(new CategoryBreakdownDTO
                {
                    CategoryName = group.Key.CategoryName,
                    Amount = spent,
                    Count = group.Count(),
                    BudgetAmount = budget?.BudgetAmount,
                    RemainingBudget = budget != null ? budget.BudgetAmount - spent : null,
                    IsOverBudget = budget != null && spent > budget.BudgetAmount
                });
            }
            
            return summary;
        }
        
        public async Task<bool> CheckDuplicateExpenseAsync(CreateExpenseDTO dto)
        {
            return await _expenseRepo.CheckDuplicateAsync(
                dto.UserId, 
                dto.ExpenseDate, 
                dto.Amount, 
                dto.CategoryId, 
                dto.Description);
        }

        public async Task<Expense> ApproveExpenseAsync(int expenseId, string approvalNote, string documentPath)
        {
            var expense = await _expenseRepo.GetByIdAsync(expenseId);
            if (expense == null)
                throw new EntityNotFoundException("Expense", expenseId);

            expense.Approve(approvalNote, documentPath);
            await _expenseRepo.UpdateAsync(expense);

            return expense;
        }
        public async Task<ExpenseSummaryDTO> GetDailySummaryAsync(int userId, DateTime date)
        {
            var expenses = await _expenseRepo.GetByDateAsync(userId, date);
            
            var summary = new ExpenseSummaryDTO
            {
                Date = date,
                Year = date.Year,
                Month = date.Month,
                TotalAmount = expenses.Sum(e => e.Amount),
                TransactionCount = expenses.Count(),
                CategoryBreakdown = expenses
                    .GroupBy(e => new { e.CategoryId, e.Category.CategoryName })
                    .Select(g => new CategoryBreakdownDTO
                    {
                        CategoryName = g.Key.CategoryName,
                        Amount = g.Sum(e => e.Amount),
                        Count = g.Count()
                    })
                    .ToList()
            };
            
            return summary;
        }
    
    }
}
