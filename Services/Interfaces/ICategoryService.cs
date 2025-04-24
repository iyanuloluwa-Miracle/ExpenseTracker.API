using Server.DTOs;
using Server.Models;

namespace Server.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateCategoryAsync(string userId, CreateCategoryRequest request);
        Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(string userId);
        Task<CategoryResponse> UpdateCategoryAsync(string userId, string id, UpdateCategoryRequest request);
        Task<bool> DeleteCategoryAsync(string userId, string id);
    }
} 