using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class Situation
{
    public int SituatuonId { get; set; }

    public string SituationName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? RoleAi { get; set; }

    public string? RoleUser { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? TypeId { get; set; }

    public int? LevelId { get; set; }

    public virtual Level? Level { get; set; }

    public virtual Type? Type { get; set; }
}
