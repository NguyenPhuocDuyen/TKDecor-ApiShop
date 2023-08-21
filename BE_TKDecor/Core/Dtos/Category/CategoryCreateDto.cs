using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Category
{
    public class CategoryCreateDto
    {
        [MinLength(2)]
        [MaxLength(100)]
        public string Name { get; set; } = null!;

        public string Thumbnail { get; set; } = null!;
    }
}
