namespace BackendCSharp.Models;

public class ReportDataItem
{
    public string? FilterValue { get; set; }

    public int GradeId { get; set; }

    public string Label { get; set; } = string.Empty;

    public decimal? Points { get; set; }

    public int Count { get; set; }
}
