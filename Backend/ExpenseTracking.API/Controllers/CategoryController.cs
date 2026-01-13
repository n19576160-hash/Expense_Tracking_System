using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ExpenseTracking.Business.Services;
using ExpenseTracking.Business.DTOs;
using ExpenseTracking.Domain.Exceptions;

namespace ExpenseTracking.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        
        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService ?? throw new ArgumentNullException(nameof(categoryService));
        }
        
        /// <summary>
        /// Get all active categories
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllCategories()
        {
            var categories = await _categoryService.GetActiveCategoriesAsync();
            
            return Ok(categories.Select(c => new
            {
                c.CategoryId,
                c.CategoryName,
                c.Description,
                c.IsDefault,
                c.IsActive
            }));
        }
        
        /// <summary>
        /// Create a new category
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid category data");
            
            var userId = GetCurrentUserId();
            dto.CreatedByUserId = userId;
            
            var category = await _categoryService.CreateCategoryAsync(dto);
            
            return Ok(new
            {
                success = true,
                message = "Category created successfully",
                category = new
                {
                    category.CategoryId,
                    category.CategoryName,
                    category.Description,
                    category.IsActive
                }
            });
        }
        
        /// <summary>
        /// Update an existing category
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] UpdateCategoryDTO dto)
        {
            if (!ModelState.IsValid)
                throw new ValidationException("Invalid category data");
            
            var category = await _categoryService.UpdateCategoryAsync(id, dto);
            
            return Ok(new
            {
                success = true,
                message = "Category updated successfully",
                category = new
                {
                    category.CategoryId,
                    category.CategoryName,
                    category.Description
                }
            });
        }
        
        /// <summary>
        /// Delete a category (only if no expenses exist)
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            await _categoryService.DeleteCategoryAsync(id);
            
            return Ok(new
            {
                success = true,
                message = "Category deleted successfully"
            });
        }
        
        /// <summary>
        /// Deactivate a category (preserve historical data)
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<IActionResult> DeactivateCategory(int id)
        {
            await _categoryService.DeactivateCategoryAsync(id);
            
            return Ok(new
            {
                success = true,
                message = "Category deactivated successfully"
            });
        }
        
        /// <summary>
        /// Activate a category
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<IActionResult> ActivateCategory(int id)
        {
            await _categoryService.ActivateCategoryAsync(id);
            
            return Ok(new
            {
                success = true,
                message = "Category activated successfully"
            });
        }
        
        private int GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
                throw new ExpenseTracking.Domain.Exceptions.UnauthorizedAccessException("User ID not found in token");
            
            return int.Parse(userIdClaim);
        }
    }
}
