using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class Level
{
    public int LevelId { get; set; }

    public string? LevelName { get; set; }

    public string? LevelScore { get; set; }

    public virtual ICollection<Situation> Situations { get; set; } = new List<Situation>();
}
