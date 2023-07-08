using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Article
{
    public class ArticleUpdateDto
    {
        public Guid ArticleId { get; set; }

        [MaxLength(255)]
        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Thumbnail { get; set; } = null!;
    }
}
