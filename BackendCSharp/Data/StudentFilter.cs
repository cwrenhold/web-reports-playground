using System;
using System.Collections.Generic;

namespace BackendCSharp.Data;

public partial class StudentFilter
{
    public int Id { get; set; }

    public int StudentId { get; set; }

    public int FilterId { get; set; }

    public int FilterValueId { get; set; }

    public virtual Filter Filter { get; set; } = null!;

    public virtual FilterValue FilterValue { get; set; } = null!;

    public virtual Student Student { get; set; } = null!;
}
