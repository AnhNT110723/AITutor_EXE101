using System;
using System.Collections.Generic;

namespace EXE_FAIEnglishTutor.Models;

public partial class Type
{
    public int TypeId { get; set; }

    public string? TypeName { get; set; }

    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
}
