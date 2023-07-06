namespace BE_TKDecor.Core.Dtos.Category
{
    public class CategoryGetDto
    {
        public long CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string Thumbnail { get; set; } = null!;

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool? IsDelete { get; set; } = false;
    }
}
