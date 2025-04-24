using Server.DTOs;
using Server.Models;
using Server.Services.Interfaces;
using MongoDB.Driver;

namespace Server.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMongoDbService _mongoDb;

        public CategoryService(IMongoDbService mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<CategoryResponse> CreateCategoryAsync(string userId, CreateCategoryRequest request)
        {
            var category = new Category
            {
                UserId = userId,
                Name = request.Name,
                Color = request.Color,
                CreatedAt = DateTime.UtcNow
            };

            await _mongoDb.Categories.InsertOneAsync(category);
            return new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                Color = category.Color,
                CreatedAt = category.CreatedAt
            };
        }

        public async Task<IEnumerable<CategoryResponse>> GetCategoriesAsync(string userId)
        {
            var categories = await _mongoDb.Categories.Find(c => c.UserId == userId).ToListAsync();
            return categories.Select(c => new CategoryResponse
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                CreatedAt = c.CreatedAt,
                UpdatedAt = c.UpdatedAt
            });
        }

        public async Task<CategoryResponse?> UpdateCategoryAsync(string userId, string id, UpdateCategoryRequest request)
        {
            var category = await _mongoDb.Categories.Find(c => c.Id == id && c.UserId == userId).FirstOrDefaultAsync();
            if (category == null)
                return null;

            var update = Builders<Category>.Update
                .Set(c => c.Name, request.Name)
                .Set(c => c.Color, request.Color)
                .Set(c => c.UpdatedAt, DateTime.UtcNow);

            await _mongoDb.Categories.UpdateOneAsync(c => c.Id == id, update);

            return new CategoryResponse
            {
                Id = category.Id,
                Name = request.Name,
                Color = request.Color,
                CreatedAt = category.CreatedAt,
                UpdatedAt = DateTime.UtcNow
            };
        }

        public async Task<bool> DeleteCategoryAsync(string userId, string id)
        {
            // Check if category is being used by any expenses
            var expenseCount = await _mongoDb.Expenses.CountDocumentsAsync(e => e.CategoryId == id);
            if (expenseCount > 0)
                return false;

            var result = await _mongoDb.Categories.DeleteOneAsync(c => c.Id == id && c.UserId == userId);
            return result.DeletedCount > 0;
        }
    }
} 