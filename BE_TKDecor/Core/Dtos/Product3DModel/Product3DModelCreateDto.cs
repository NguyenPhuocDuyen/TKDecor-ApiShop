using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Product3DModel
{
    public class Product3DModelCreateDto
    {
        [MinLength(2)]
        [MaxLength(100)]
        public string ModelName { get; set; } = null!;

        public string VideoUrl { get; set; } = null!;

        public string ModelUrl { get; set; } = null!;

        public string ThumbnailUrl { get; set; } = null!;
    }
}
