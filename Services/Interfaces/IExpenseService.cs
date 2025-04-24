using Server.DTOs;
using Server.Models;

namespace Server.Services.Interfaces
{
    public interface IExpenseService
    {
        Task<ExpenseResponse> CreateExpenseAsync(string userId, CreateExpenseRequest request);
        Task<IEnumerable<ExpenseResponse>> GetExpensesAsync(string userId, ExpenseFilterRequest filter);
        Task<ExpenseResponse> GetExpenseAsync(string userId, string id);
        Task<ExpenseResponse> UpdateExpenseAsync(string userId, string id, UpdateExpenseRequest request);
        Task<bool> DeleteExpenseAsync(string userId, string id);
    }
} 