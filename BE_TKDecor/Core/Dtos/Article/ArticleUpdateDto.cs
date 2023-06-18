namespace BE_TKDecor.Core.Dtos.Article
{
    public class ArticleUpdateDto
    {
        public int ArticleId { get; set; }

        public string Title { get; set; } = null!;

        public string Content { get; set; } = null!;

        public string Thumbnail { get; set; } = null!;
    }
}
