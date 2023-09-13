using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackendCSharp.Data;

[Serializable]
public partial class FilterValue
{
    public int Id { get; set; }

    public int FilterId { get; set; }

    public string Text { get; set; } = null!;

    [JsonIgnore]
    public virtual Filter Filter { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<StudentFilter> StudentFilters { get; set; } = new List<StudentFilter>();
}
