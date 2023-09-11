using System;
using System.Collections.Generic;

namespace BackendCSharp.Data;

public partial class FilterValue
{
    public int Id { get; set; }

    public int FilterId { get; set; }

    public string Text { get; set; } = null!;

    public virtual Filter Filter { get; set; } = null!;

    public virtual ICollection<StudentFilter> StudentFilters { get; set; } = new List<StudentFilter>();
}
