using Server.DTOs;
using Server.Models;
using Server.Services.Interfaces;
using MongoDB.Driver;

namespace Server.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IMongoDbService _mongoDb;

        public DashboardService(IMongoDbService mongoDb)
        {
            _mongoDb = mongoDb;
        }

        public async Task<DashboardSummaryResponse> GetSummaryAsync(string userId)
        {
            var expenses = await _mongoDb.Expenses.Find(e => e.UserId == userId).ToListAsync();
            if (!expenses.Any())
                return new DashboardSummaryResponse();

            var totalExpenses = expenses.Sum(e => e.Amount);
            var averageSpending = totalExpenses / expenses.Count;
            var highestExpense = expenses.Max(e => e.Amount);
            var lowestExpense = expenses.Min(e => e.Amount);

            return new DashboardSummaryResponse
            {
                TotalExpenses = totalExpenses,
                AverageSpending = averageSpending,
                HighestExpense = highestExpense,
                LowestExpense = lowestExpense,
                TotalTransactions = expenses.Count
            };
        }

        public async Task<IEnumerable<ExpenseResponse>> GetRecentTransactionsAsync(string userId)
        {
            var expenses = await _mongoDb.Expenses.Find(e => e.UserId == userId)
                .Sort(Builders<Expense>.Sort.Descending(e => e.Date))
                .Limit(10)
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

        public async Task<IEnumerable<CategorySummaryResponse>> GetCategorySummaryAsync(string userId)
        {
            var expenses = await _mongoDb.Expenses.Find(e => e.UserId == userId).ToListAsync();
            var categories = await _mongoDb.Categories.Find(c => c.UserId == userId).ToListAsync();

            var totalAmount = expenses.Sum(e => e.Amount);
            var categorySummaries = expenses
                .GroupBy(e => e.CategoryId)
                .Select(g => new CategorySummaryResponse
                {
                    CategoryId = g.Key,
                    CategoryName = categories.FirstOrDefault(c => c.Id == g.Key)?.Name ?? "Unknown",
                    Color = categories.FirstOrDefault(c => c.Id == g.Key)?.Color ?? "#000000",
                    Total = g.Sum(e => e.Amount),
                    Percentage = totalAmount > 0 ? (g.Sum(e => e.Amount) / totalAmount) * 100 : 0
                })
                .OrderByDescending(c => c.Total)
                .ToList();

            return categorySummaries;
        }

        public async Task<IEnumerable<MonthlySummaryResponse>> GetMonthlySummaryAsync(string userId)
        {
            var expenses = await _mongoDb.Expenses.Find(e => e.UserId == userId).ToListAsync();
            var monthlySummaries = expenses
                .GroupBy(e => new { e.Date.Year, e.Date.Month })
                .Select(g => new MonthlySummaryResponse
                {
                    Month = $"{g.Key.Year}-{g.Key.Month:D2}",
                    Total = g.Sum(e => e.Amount)
                })
                .OrderBy(m => m.Month)
                .ToList();

            return monthlySummaries;
        }
    }
} 