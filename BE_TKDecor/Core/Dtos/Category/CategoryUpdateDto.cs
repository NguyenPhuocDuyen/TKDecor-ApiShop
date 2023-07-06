using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Category
{
    public class CategoryUpdateDto
    {
        public long CategoryId { get; set; }

        [MaxLength(255)]
        public string Name { get; set; } = null!;

        [MaxLength(255)]
        public string Thumbnail { get; set; } = null!;
    }
}
