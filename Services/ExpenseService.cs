using Server.DTOs;
using Server.Models;
using Server.Services.Interfaces;
using MongoDB.Driver;

namespace Server.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly IMongoDbService _mongoDb;

        public ExpenseService(IMongoDbService mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<ExpenseResponse> CreateExpenseAsync(string userId, CreateExpenseRequest request)
        {
            var expense = new Expense
            {
                UserId = userId,
                Amount = request.Amount,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Date = request.Date,
                CreatedAt = DateTime.UtcNow
            };

            await _mongoDb.Expenses.InsertOneAsync(expense);
            return await GetExpenseAsync(userId, expense.Id) ?? 
                throw new InvalidOperationException("Failed to create expense");
        }

        public async Task<IEnumerable<ExpenseResponse>> GetExpensesAsync(string userId, ExpenseFilterRequest filter)
        {
            var builder = Builders<Expense>.Filter;
            var filterDefinition = builder.Eq(e => e.UserId, userId);

            if (filter.StartDate.HasValue)
                filterDefinition &= builder.Gte(e => e.Date, filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                filterDefinition &= builder.Lte(e => e.Date, filter.EndDate.Value);
            if (filter.MinAmount.HasValue)
                filterDefinition &= builder.Gte(e => e.Amount, filter.MinAmount.Value);
            if (filter.MaxAmount.HasValue)
                filterDefinition &= builder.Lte(e => e.Amount, filter.MaxAmount.Value);
            if (!string.IsNullOrEmpty(filter.Description))
                filterDefinition &= builder.Regex(e => e.Description, new MongoDB.Bson.BsonRegularExpression(filter.Description, "i"));
            if (!string.IsNullOrEmpty(filter.CategoryId))
                filterDefinition &= builder.Eq(e => e.CategoryId, filter.CategoryId);

            var sortDefinition = Builders<Expense>.Sort.Descending(e => e.Date);
            if (!string.IsNullOrEmpty(filter.SortBy))
            {
                switch (filter.SortBy.ToLower())
                {
                    case "amount_asc":
                        sortDefinition = Builders<Expense>.Sort.Ascending(e => e.Amount);
                        break;
                    case "amount_desc":
                        sortDefinition = Builders<Expense>.Sort.Descending(e => e.Amount);
                        break;
                    case "date_asc":
                        sortDefinition = Builders<Expense>.Sort.Ascending(e => e.Date);
                        break;
                    case "category_asc":
                        sortDefinition = Builders<Expense>.Sort.Ascending(e => e.CategoryId);
                        break;
                    case "category_desc":
                        sortDefinition = Builders<Expense>.Sort.Descending(e => e.CategoryId);
                        break;
                }
            }

            var expenses = await _mongoDb.Expenses.Find(filterDefinition)
                .Sort(sortDefinition)
                .ToListAsync();

            var categories = await _mongoDb.Categories.Find(c => c.UserId == userId).ToListAsync();
            var categoryDict = categories.ToDictionary(c => c.Id, c => c.Name);

            return expenses.Select(e => new ExpenseResponse
            {
                Id = e.Id,
                Amount = e.Amount,
                Description = e.Description,
                CategoryId = e.CategoryId,
                CategoryName = categoryDict.GetValueOrDefault(e.CategoryId),
                Date = e.Date,
                CreatedAt = e.CreatedAt,
                UpdatedAt = e.UpdatedAt
            });
        }

        public async Task<ExpenseResponse?> GetExpenseAsync(string userId, string id)
        {
            var expense = await _mongoDb.Expenses.Find(e => e.Id == id && e.UserId == userId).FirstOrDefaultAsync();
            if (expense == null)
                return null;

            var category = await _mongoDb.Categories.Find(c => c.Id == expense.CategoryId).FirstOrDefaultAsync();

            return new ExpenseResponse
            {
                Id = expense.Id,
                Amount = expense.Amount,
                Description = expense.Description,
                CategoryId = expense.CategoryId,
                CategoryName = category?.Name,
                Date = expense.Date,
                CreatedAt = expense.CreatedAt,
                UpdatedAt = expense.UpdatedAt
            };
        }

        public async Task<ExpenseResponse?> UpdateExpenseAsync(string userId, string id, UpdateExpenseRequest request)
        {
            var expense = await _mongoDb.Expenses.Find(e => e.Id == id && e.UserId == userId).FirstOrDefaultAsync();
            if (expense == null)
                return null;

            var update = Builders<Expense>.Update
                .Set(e => e.Amount, request.Amount)
                .Set(e => e.Description, request.Description)
                .Set(e => e.CategoryId, request.CategoryId)
                .Set(e => e.Date, request.Date)
                .Set(e => e.UpdatedAt, DateTime.UtcNow);

            await _mongoDb.Expenses.UpdateOneAsync(e => e.Id == id, update);
            return await GetExpenseAsync(userId, id);
        }

        public async Task<bool> DeleteExpenseAsync(string userId, string id)
        {
            var result = await _mongoDb.Expenses.DeleteOneAsync(e => e.Id == id && e.UserId == userId);
            return result.DeletedCount > 0;
        }
    }
} 