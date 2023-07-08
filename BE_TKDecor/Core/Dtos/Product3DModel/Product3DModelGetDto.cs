namespace BE_TKDecor.Core.Dtos.Product3DModel
{
    public class Product3DModelGetDto
    {
        public Guid Product3DModelId { get; set; }

        public string ModelName { get; set; } = null!;

        public string VideoUrl { get; set; } = null!;

        public string ModelUrl { get; set; } = null!;

        public string ThumbnailUrl { get; set; } = null!;

        public string ProductName { get; set; } = null!;
    }
}
