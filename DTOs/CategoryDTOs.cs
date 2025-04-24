namespace Server.DTOs
{
    public class CreateCategoryRequest
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class UpdateCategoryRequest
    {
        public string Name { get; set; }
        public string Color { get; set; }
    }

    public class CategoryResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Color { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
} 