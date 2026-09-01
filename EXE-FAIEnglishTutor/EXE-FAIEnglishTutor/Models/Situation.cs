using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace EXE_FAIEnglishTutor.Models;

public partial class Situation
{
    public int SituatuonId { get; set; }

    public string SituationName { get; set; } = null!;

    public string? Description { get; set; }

    public string? ImageUrl { get; set; }

    public string? RoleAi { get; set; }

    public string? RoleUser { get; set; }

    public string? LearningObjectives { get; set; }

    [NotMapped]
    public List<string> LearningObjectivesList
    {
        get
        {
            if (string.IsNullOrEmpty(LearningObjectives)) return new List<string>();
            try
            {
                return JsonSerializer.Deserialize<List<string>>(LearningObjectives) ?? new List<string>();
            }
            catch
            {
                return new List<string>();
            }
        }
    }

    public DateTime? CreatedAt { get; set; }

    public int? TypeId { get; set; }

    public int? LevelId { get; set; }

    public virtual Level? Level { get; set; }

    public virtual Type? Type { get; set; }
}
