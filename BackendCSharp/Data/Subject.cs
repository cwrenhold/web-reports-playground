using System;
using System.Collections.Generic;

namespace BackendCSharp.Data;

public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();

    public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
}
