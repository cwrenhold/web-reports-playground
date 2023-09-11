using System;
using System.Collections.Generic;

namespace BackendCSharp.Data;

public partial class Filter
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<FilterValue> FilterValues { get; set; } = new List<FilterValue>();

    public virtual ICollection<StudentFilter> StudentFilters { get; set; } = new List<StudentFilter>();
}
