using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BackendCSharp.Data;

[Serializable]
public partial class Filter
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    [JsonIgnore]
    public virtual ICollection<FilterValue> FilterValues { get; set; } = new List<FilterValue>();

    [JsonIgnore]
    public virtual ICollection<StudentFilter> StudentFilters { get; set; } = new List<StudentFilter>();
}
