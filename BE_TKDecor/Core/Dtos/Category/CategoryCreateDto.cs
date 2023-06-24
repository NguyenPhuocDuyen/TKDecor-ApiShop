using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Category
{
    public class CategoryCreateDto
    {
        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(255)]
        public string ImageUrl { get; set; } = null!;
    }
}
