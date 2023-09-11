using System;
using System.Collections.Generic;

namespace BackendCSharp.Data;

public partial class StudentSubject
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int SubjectId { get; set; }

    public virtual Student Student { get; set; } = null!;

    public virtual Subject Subject { get; set; } = null!;
}
