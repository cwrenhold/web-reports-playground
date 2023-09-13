using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackendCSharp.Data;

[Serializable]
public partial class Grade
{
    public int Id { get; set; }

    public decimal? Points { get; set; }

    public string Text { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<StudentGrade> StudentGrades { get; set; } = new List<StudentGrade>();
}
