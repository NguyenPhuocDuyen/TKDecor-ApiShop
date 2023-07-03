namespace BusinessObject;

public partial class Article : BaseEntity
{
    public long ArticleId { get; set; }

    public long UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public string Slug { get; set; } = null!;

    public bool IsPublish { get; set; } = false;
     
    public virtual User User { get; set; } = null!;
}
