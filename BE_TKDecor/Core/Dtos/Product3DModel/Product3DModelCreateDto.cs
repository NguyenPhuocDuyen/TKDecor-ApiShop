namespace BE_TKDecor.Core.Dtos.Product3DModel
{
    public class Product3DModelCreateDto
    {
        public string ModelName { get; set; } = null!;

        public string VideoUrl { get; set; } = null!;

        public string ModelUrl { get; set; } = null!;

        public string ThumbnailUrl { get; set; } = null!;
    }
}
