using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Server.DTOs;
using Server.Services.Interfaces;
using System.Security.Claims;

namespace Server.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ExpenseController : ControllerBase
    {
        private readonly IExpenseService _expenseService;

        public ExpenseController(IExpenseService expenseService)
        {
            _expenseService = expenseService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateExpense([FromBody] CreateExpenseRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _expenseService.CreateExpenseAsync(userId, request);
            return Ok(response);
        }

        [HttpGet]
        public async Task<IActionResult> GetExpenses([FromQuery] ExpenseFilterRequest filter)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _expenseService.GetExpensesAsync(userId, filter);
            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetExpense(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _expenseService.GetExpenseAsync(userId, id);
            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateExpense(string id, [FromBody] UpdateExpenseRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var response = await _expenseService.UpdateExpenseAsync(userId, id, request);
            if (response == null)
                return NotFound();

            return Ok(response);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExpense(string id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var success = await _expenseService.DeleteExpenseAsync(userId, id);
            if (!success)
                return NotFound();

            return Ok(new MessageResponse { Message = "Expense deleted successfully" });
        }
    }
} 