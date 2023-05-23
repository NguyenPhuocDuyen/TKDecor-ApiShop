using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Article
{
    public int ArticleId { get; set; }

    public int UserId { get; set; }

    public string Title { get; set; } = null!;

    public string Content { get; set; } = null!;

    public string Thumbnail { get; set; } = null!;

    public bool? IsPublish { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool? IsDelete { get; set; }

    public virtual User User { get; set; } = null!;
}
