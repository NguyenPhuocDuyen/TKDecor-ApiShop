using BusinessObject;

namespace BE_TKDecor.Core.Dtos.Article
{
    public class ArticleGetDto : BaseEntity
    {
        public Guid ArticleId { get; set; }

        public Guid UserId { get; set; }

        public string UserName { get; set; } = null!;

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Thumbnail { get; set; } = null!;

        public string? Slug { get; set; }

        public bool? IsPublish { get; set; } = false;
    }
}
