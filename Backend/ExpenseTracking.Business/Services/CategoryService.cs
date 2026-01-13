using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Domain.Exceptions;
using ExpenseTracking.Data.Repositories;
using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepo;
        
        public CategoryService(ICategoryRepository categoryRepo)
        {
            _categoryRepo = categoryRepo ?? throw new ArgumentNullException(nameof(categoryRepo));
        }
        
        public async Task<IEnumerable<Category>> GetActiveCategoriesAsync()
        {
            return await _categoryRepo.GetActiveCategoriesAsync();
        }
        
        public async Task<Category> CreateCategoryAsync(CreateCategoryDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            
            if (await _categoryRepo.NameExistsAsync(dto.CategoryName))
                throw new CategoryNameExistsException(dto.CategoryName);
            
            var category = new Category(dto.CategoryName, dto.Description, false, dto.CreatedByUserId);
            await _categoryRepo.AddAsync(category);
            
            return category;
        }
        
        public async Task<Category> UpdateCategoryAsync(int categoryId, UpdateCategoryDTO dto)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));
            
            var category = await _categoryRepo.GetByIdAsync(categoryId);
            if (category == null)
                throw new EntityNotFoundException("Category", categoryId);
            
            if (await _categoryRepo.NameExistsAsync(dto.CategoryName, categoryId))
                throw new CategoryNameExistsException(dto.CategoryName);
            
            category.UpdateDetails(dto.CategoryName, dto.Description);
            await _categoryRepo.UpdateAsync(category);
            
            return category;
        }
        
        public async Task DeactivateCategoryAsync(int categoryId)
        {
            var category = await _categoryRepo.GetByIdAsync(categoryId);
            if (category == null)
                throw new EntityNotFoundException("Category", categoryId);
            
            var expenseCount = await _categoryRepo.GetExpenseCountAsync(categoryId);
            if (expenseCount > 0)
            {
                category.Deactivate();
                await _categoryRepo.UpdateAsync(category);
            }
            else
            {
                await _categoryRepo.DeleteAsync(category);
            }
        }


        public async Task ActivateCategoryAsync(int categoryId)
        {
            var category = await _categoryRepo.GetByIdAsync(categoryId);
            if (category == null)
                throw new EntityNotFoundException("Category", categoryId);
            
            category.Activate();
            await _categoryRepo.UpdateAsync(category);
        }
                
        public async Task DeleteCategoryAsync(int categoryId)
        {
            var category = await _categoryRepo.GetByIdAsync(categoryId);
            if (category == null)
                throw new EntityNotFoundException("Category", categoryId);
            
            var expenseCount = await _categoryRepo.GetExpenseCountAsync(categoryId);
            if (expenseCount > 0)
            {
                throw new CategoryDeletionException(
                    categoryId, 
                    category.CategoryName, 
                    expenseCount);
            }
            
            await _categoryRepo.DeleteAsync(category);
        }
    }
}
