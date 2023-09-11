using System;
using System.Collections.Generic;

namespace BackendCSharp.Data;

public partial class Student
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public virtual ICollection<StudentFilter> StudentFilters { get; set; } = new List<StudentFilter>();

    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();

    public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
}
