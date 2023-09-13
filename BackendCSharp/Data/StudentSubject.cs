using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackendCSharp.Data;

[Serializable]
public partial class StudentSubject
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int SubjectId { get; set; }

    [JsonIgnore]
    public virtual Student Student { get; set; } = null!;

    [JsonIgnore]
    public virtual Subject Subject { get; set; } = null!;
}
