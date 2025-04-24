namespace Server.DTOs
{
    public class DashboardSummaryResponse
    {
        public decimal TotalExpenses { get; set; }
        public decimal AverageSpending { get; set; }
        public decimal HighestExpense { get; set; }
        public decimal LowestExpense { get; set; }
        public int TotalTransactions { get; set; }
    }

    public class CategorySummaryResponse
    {
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string Color { get; set; }
        public decimal Total { get; set; }
        public decimal Percentage { get; set; }
    }

    public class MonthlySummaryResponse
    {
        public string Month { get; set; }
        public decimal Total { get; set; }
    }
} 