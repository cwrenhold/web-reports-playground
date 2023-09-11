using System;
using System.Collections.Generic;

namespace BackendCSharp.Data;

public partial class StudentGrade
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int SubjectId { get; set; }

    public int GradeId { get; set; }

    public virtual Grade Grade { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
