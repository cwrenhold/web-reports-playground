using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackendCSharp.Data;

[Serializable]
public partial class StudentFilter
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int FilterId { get; set; }

    public int FilterValueId { get; set; }

    [JsonIgnore]
    public virtual Filter Filter { get; set; } = null!;

    [JsonIgnore]
    public virtual FilterValue FilterValue { get; set; } = null!;

    [JsonIgnore]
    public virtual Student Student { get; set; } = null!;
}
