using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class Podcast
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Content { get; set; }

    public string? ImageUrl { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? UserId { get; set; }

    public string? Topic { get; set; }

    public string? AudioUrl { get; set; }
}
