using System.ComponentModel.DataAnnotations;

namespace BE_TKDecor.Core.Dtos.Article
{
    public class ArticleCreateDto
    {
        [MinLength(5)]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        [MinLength(50)]
        public string Content { get; set; } = null!;

        public string Thumbnail { get; set; } = null!;

        public bool IsPublish { get; set; } = false;
    }
}
