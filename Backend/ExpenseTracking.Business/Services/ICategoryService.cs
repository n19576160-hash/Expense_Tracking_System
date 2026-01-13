using ExpenseTracking.Domain.Entities;
using ExpenseTracking.Business.DTOs;

namespace ExpenseTracking.Business.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetActiveCategoriesAsync();
        Task<Category> CreateCategoryAsync(CreateCategoryDTO dto);
        Task<Category> UpdateCategoryAsync(int categoryId, UpdateCategoryDTO dto);
        Task DeactivateCategoryAsync(int categoryId);
        Task ActivateCategoryAsync(int categoryId);
        Task DeleteCategoryAsync(int categoryId);
    }
}