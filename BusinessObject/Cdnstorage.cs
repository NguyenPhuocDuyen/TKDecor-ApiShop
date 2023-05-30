using System;
using System.Collections.Generic;

namespace BusinessObject;

public partial class Cdnstorage
{
    public int FileId { get; set; }

    public string FileName { get; set; } = null!;

    public string FileSize { get; set; } = null!;

    public string CdnUrl { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
}
