using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackendCSharp.Data;

[Serializable]
public partial class Subject
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();

    [JsonIgnore]
    public virtual ICollection<StudentSubject> StudentSubjects { get; set; } = new List<StudentSubject>();
}
