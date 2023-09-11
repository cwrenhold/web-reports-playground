using System;
using System.Collections.Generic;

namespace BackendCSharp.Data;

public partial class Grade
{
    public int Id { get; set; }

    public decimal? Points { get; set; }

    public string Text { get; set; } = null!;

    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
}
