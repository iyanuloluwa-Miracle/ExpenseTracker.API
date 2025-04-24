using Server.DTOs;

namespace Server.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardSummaryResponse> GetSummaryAsync(string userId);
        Task<IEnumerable<ExpenseResponse>> GetRecentTransactionsAsync(string userId);
        Task<IEnumerable<CategorySummaryResponse>> GetCategorySummaryAsync(string userId);
        Task<IEnumerable<MonthlySummaryResponse>> GetMonthlySummaryAsync(string userId);
    }
} 