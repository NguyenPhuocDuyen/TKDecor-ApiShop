namespace BE_TKDecor.Core.Dtos.Category
{
    public class CategoryUpdateDto
    {
        public int CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string ImageUrl { get; set; } = null!;
    }
}
