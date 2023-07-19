using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Category
{
    public class CategoryGetDto : BaseEntity
    {
        public Guid CategoryId { get; set; }

        public string Name { get; set; } = null!;

        public string Thumbnail { get; set; } = null!;
    }
}
