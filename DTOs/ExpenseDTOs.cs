namespace Server.DTOs
{
    public class CreateExpenseRequest
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public DateTime Date { get; set; }
    }

    public class UpdateExpenseRequest
    {
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public DateTime Date { get; set; }
    }

    public class ExpenseResponse
    {
        public string Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public DateTime Date { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ExpenseFilterRequest
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal? MinAmount { get; set; }
        public decimal? MaxAmount { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public string SortBy { get; set; }
    }
} 